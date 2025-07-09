using System;

namespace Alchiweb.IoT.Icm20948
{
	public static class Conversion
	{
		/** The length of all IMU data that is being read at once in bytes. */
		public static readonly short IMU_DATA_LEN = 22;
		/** The length of individual gyroscope and accelerator data in bytes. */
		public static readonly short GYRO_AND_ACC_DATA_LEN = 6;
		/** The length of individual magnetometer data with status check in bytes. */
		public static readonly short MAG_DATA_LEN = 8;
		/** Constant scale for magnetometer. */
		public static readonly float MAG_SCALE = 0.15f;
		/** Scale to convert angles from degrees to radians. */
		public static readonly float DEG_TO_RAD = (float)Math.PI / 180.0f;


		/**
		 * Fast inverse square root algorithm from http://en.wikipedia.org/wiki/Fast_inverse_square_root
		 *  @param x input data
		 *  @return the result
		 */
		public static float invSqrt(float x)
		{
			float xhalf = 0.5f * x;
			int i = BitConverter.SingleToInt32Bits(x);
			i = 0x5f3759df - (i >> 1);
			x = BitConverter.Int32BitsToSingle(i);
			x = x * (1.5f - xhalf * x * x);
			return x;
		}

		/**
		 * Converts quaternion to Euler angles.
		 *  @param[in/out] data the IMU data structure with quaternion and Euler angles.
		 */
		public static void quatToAngles(ImuData data)
		{
			data.Angles[1] = 2 * (data.Quat[0] * data.Quat[2] - data.Quat[1] * data.Quat[3]);
			if (Math.Abs(data.Angles[1]) >= 1)
			{
				data.Angles[1] = (data.Angles[1] < 0 ? -(float)Math.PI / 2 : (float)Math.PI / 2) * RAD_TO_DEG;
			}
			else
			{
				data.Angles[1] = (float)Math.Asin(data.Angles[1]) * RAD_TO_DEG;
			}
			data.Angles[0] = (float)Math.Atan2(2 * (data.Quat[0] * data.Quat[1] + data.Quat[2] * data.Quat[3]), 1 - 2 * (data.Quat[1] * data.Quat[1] + data.Quat[2] * data.Quat[2])) * RAD_TO_DEG;
			data.Angles[2] = (float)Math.Atan2(2 * (data.Quat[0] * data.Quat[3] + data.Quat[1] * data.Quat[2]), 1 - 2 * (data.Quat[2] * data.Quat[2] + data.Quat[3] * data.Quat[3])) * RAD_TO_DEG;
		}


		/** Scale to convert angles from radians to degrees. */
		internal static float RAD_TO_DEG = 180.0f / (float)Math.PI;


		//---------------------------------------------------------------------------------------------------
		// AHRS algorithm update
		//=====================================================================================================
		//
		// Implementation of Madgwick's IMU and AHRS algorithms.
		// See: http://www.x-io.co.uk/node/8#open_source_ahrs_and_imu_algorithms
		//
		// Date          Author          Notes
		// 29/09/2011    SOH Madgwick    Initial release
		// 02/10/2011    SOH Madgwick    Optimised for reduced CPU load
		// 19/02/2012    SOH Madgwick    Magnetometer measurement is normalised
		//
		//=====================================================================================================
		public static void MadgwickAHRSupdate(ImuData data)
		{
			float recipNorm;
			float s0;
			float s1;
			float s2;
			float s3;
			float qDot1;
			float qDot2;
			float qDot3;
			float qDot4;
			float hx;
			float hy;
			float _2q0mx;
			float _2q0my;
			float _2q0mz;
			float _2q1mx;
			float _2bx;
			float _2bz;
			float _4bx;
			float _4bz;
			float _2q0;
			float _2q1;
			float _2q2;
			float _2q3;
			float _2q0q2;
			float _2q2q3;
			float q0q0;
			float q0q1;
			float q0q2;
			float q0q3;
			float q1q1;
			float q1q2;
			float q1q3;
			float q2q2;
			float q2q3;
			float q3q3;

			// Rate of change of quaternion from gyroscope
			qDot1 = 0.5f * (-data.Quat[1] * data.Gyro[0] - data.Quat[2] * data.Gyro[1] - data.Quat[3] * data.Gyro[2]);
			qDot2 = 0.5f * (data.Quat[0] * data.Gyro[0] + data.Quat[2] * data.Gyro[2] - data.Quat[3] * data.Gyro[1]);
			qDot3 = 0.5f * (data.Quat[0] * data.Gyro[1] - data.Quat[1] * data.Gyro[2] + data.Quat[3] * data.Gyro[0]);
			qDot4 = 0.5f * (data.Quat[0] * data.Gyro[2] + data.Quat[1] * data.Gyro[1] - data.Quat[2] * data.Gyro[0]);

			// Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
			if (!(data.Acc[0] == 0.0f && data.Acc[1] == 0.0f && data.Acc[2] == 0.0f))
			{
				// Normalise accelerometer measurement
				recipNorm = invSqrt(data.Acc[0] * data.Acc[0] + data.Acc[1] * data.Acc[1] + data.Acc[2] * data.Acc[2]);
				data.Acc[0] *= recipNorm;
				data.Acc[1] *= recipNorm;
				data.Acc[2] *= recipNorm;

				// Normalise magnetometer measurement
				recipNorm = invSqrt(data.Mag[0] * data.Mag[0] + data.Mag[1] * data.Mag[1] + data.Mag[2] * data.Mag[2]);
				data.Mag[0] *= recipNorm;
				data.Mag[1] *= recipNorm;
				data.Mag[2] *= recipNorm;

				// Auxiliary variables to avoid repeated arithmetic
				_2q0mx = 2.0f * data.Quat[0] * data.Mag[0];
				_2q0my = 2.0f * data.Quat[0] * data.Mag[1];
				_2q0mz = 2.0f * data.Quat[0] * data.Mag[2];
				_2q1mx = 2.0f * data.Quat[1] * data.Mag[0];
				_2q0 = 2.0f * data.Quat[0];
				_2q1 = 2.0f * data.Quat[1];
				_2q2 = 2.0f * data.Quat[2];
				_2q3 = 2.0f * data.Quat[3];
				_2q0q2 = 2.0f * data.Quat[0] * data.Quat[2];
				_2q2q3 = 2.0f * data.Quat[2] * data.Quat[3];
				q0q0 = data.Quat[0] * data.Quat[0];
				q0q1 = data.Quat[0] * data.Quat[1];
				q0q2 = data.Quat[0] * data.Quat[2];
				q0q3 = data.Quat[0] * data.Quat[3];
				q1q1 = data.Quat[1] * data.Quat[1];
				q1q2 = data.Quat[1] * data.Quat[2];
				q1q3 = data.Quat[1] * data.Quat[3];
				q2q2 = data.Quat[2] * data.Quat[2];
				q2q3 = data.Quat[2] * data.Quat[3];
				q3q3 = data.Quat[3] * data.Quat[3];

				// Reference direction of Earth's magnetic field
				hx = data.Mag[0] * q0q0 - _2q0my * data.Quat[3] + _2q0mz * data.Quat[2] + data.Mag[0] * q1q1 + _2q1 * data.Mag[1] * data.Quat[2] + _2q1 * data.Mag[2] * data.Quat[3] - data.Mag[0] * q2q2 - data.Mag[0] * q3q3;
				hy = _2q0mx * data.Quat[3] + data.Mag[1] * q0q0 - _2q0mz * data.Quat[1] + _2q1mx * data.Quat[2] - data.Mag[1] * q1q1 + data.Mag[1] * q2q2 + _2q2 * data.Mag[2] * data.Quat[3] - data.Mag[1] * q3q3;
				_2bx = (float)Math.Sqrt((float)(hx * hx + hy * hy));
				_2bz = -_2q0mx * data.Quat[2] + _2q0my * data.Quat[1] + data.Mag[2] * q0q0 + _2q1mx * data.Quat[3] - data.Mag[2] * q1q1 + _2q2 * data.Mag[1] * data.Quat[3] - data.Mag[2] * q2q2 + data.Mag[2] * q3q3;
				_4bx = 2.0f * _2bx;
				_4bz = 2.0f * _2bz;

				// Gradient decent algorithm corrective step
				s0 = -_2q2 * (2.0f * q1q3 - _2q0q2 - data.Acc[0]) + _2q1 * (2.0f * q0q1 + _2q2q3 - data.Acc[1]) - _2bz * data.Quat[2] * (_2bx * (0.5f - q2q2 - q3q3) + _2bz * (q1q3 - q0q2) - data.Mag[0]) + (-_2bx * data.Quat[3] + _2bz * data.Quat[1]) * (_2bx * (q1q2 - q0q3) + _2bz * (q0q1 + q2q3) - data.Mag[1]) + _2bx * data.Quat[2] * (_2bx * (q0q2 + q1q3) + _2bz * (0.5f - q1q1 - q2q2) - data.Mag[2]);
				s1 = _2q3 * (2.0f * q1q3 - _2q0q2 - data.Acc[0]) + _2q0 * (2.0f * q0q1 + _2q2q3 - data.Acc[1]) - 4.0f * data.Quat[1] * (1 - 2.0f * q1q1 - 2.0f * q2q2 - data.Acc[2]) + _2bz * data.Quat[3] * (_2bx * (0.5f - q2q2 - q3q3) + _2bz * (q1q3 - q0q2) - data.Mag[0]) + (_2bx * data.Quat[2] + _2bz * data.Quat[0]) * (_2bx * (q1q2 - q0q3) + _2bz * (q0q1 + q2q3) - data.Mag[1]) + (_2bx * data.Quat[3] - _4bz * data.Quat[1]) * (_2bx * (q0q2 + q1q3) + _2bz * (0.5f - q1q1 - q2q2) - data.Mag[2]);
				s2 = -_2q0 * (2.0f * q1q3 - _2q0q2 - data.Acc[0]) + _2q3 * (2.0f * q0q1 + _2q2q3 - data.Acc[1]) - 4.0f * data.Quat[2] * (1 - 2.0f * q1q1 - 2.0f * q2q2 - data.Acc[2]) + (-_4bx * data.Quat[2] - _2bz * data.Quat[0]) * (_2bx * (0.5f - q2q2 - q3q3) + _2bz * (q1q3 - q0q2) - data.Mag[0]) + (_2bx * data.Quat[1] + _2bz * data.Quat[3]) * (_2bx * (q1q2 - q0q3) + _2bz * (q0q1 + q2q3) - data.Mag[1]) + (_2bx * data.Quat[0] - _4bz * data.Quat[2]) * (_2bx * (q0q2 + q1q3) + _2bz * (0.5f - q1q1 - q2q2) - data.Mag[2]);
				s3 = _2q1 * (2.0f * q1q3 - _2q0q2 - data.Acc[0]) + _2q2 * (2.0f * q0q1 + _2q2q3 - data.Acc[1]) + (-_4bx * data.Quat[3] + _2bz * data.Quat[1]) * (_2bx * (0.5f - q2q2 - q3q3) + _2bz * (q1q3 - q0q2) - data.Mag[0]) + (-_2bx * data.Quat[0] + _2bz * data.Quat[2]) * (_2bx * (q1q2 - q0q3) + _2bz * (q0q1 + q2q3) - data.Mag[1]) + _2bx * data.Quat[1] * (_2bx * (q0q2 + q1q3) + _2bz * (0.5f - q1q1 - q2q2) - data.Mag[2]);
				recipNorm = invSqrt(s0 * s0 + s1 * s1 + s2 * s2 + s3 * s3); // normalise step magnitude
				s0 *= recipNorm;
				s1 *= recipNorm;
				s2 *= recipNorm;
				s3 *= recipNorm;

				// Apply feedback step
				qDot1 -= Constants.beta * s0;
				qDot2 -= Constants.beta * s1;
				qDot3 -= Constants.beta * s2;
				qDot4 -= Constants.beta * s3;
			}

			// Integrate rate of change of quaternion to yield quaternion
			data.Quat[0] += qDot1 * data.UpdatePeriod;
			data.Quat[1] += qDot2 * data.UpdatePeriod;
			data.Quat[2] += qDot3 * data.UpdatePeriod;
			data.Quat[3] += qDot4 * data.UpdatePeriod;

			// Normalise quaternion
			recipNorm = invSqrt(data.Quat[0] * data.Quat[0] + data.Quat[1] * data.Quat[1] + data.Quat[2] * data.Quat[2] + data.Quat[3] * data.Quat[3]);
			data.Quat[0] *= recipNorm;
			data.Quat[1] *= recipNorm;
			data.Quat[2] *= recipNorm;
			data.Quat[3] *= recipNorm;

			quatToAngles(data);
		}

		//---------------------------------------------------------------------------------------------------
		// IMU algorithm update


		public static void MadgwickAHRSupdateIMU(ImuData data)
		{
			float recipNorm;
			float s0;
			float s1;
			float s2;
			float s3;
			float qDot1;
			float qDot2;
			float qDot3;
			float qDot4;
			float _2q0;
			float _2q1;
			float _2q2;
			float _2q3;
			float _4q0;
			float _4q1;
			float _4q2;
			float _8q1;
			float _8q2;
			float q0q0;
			float q1q1;
			float q2q2;
			float q3q3;

			// Rate of change of quaternion from gyroscope
			qDot1 = 0.5f * (-data.Quat[1] * data.Gyro[0] - data.Quat[2] * data.Gyro[1] - data.Quat[3] * data.Gyro[2]);
			qDot2 = 0.5f * (data.Quat[0] * data.Gyro[0] + data.Quat[2] * data.Gyro[2] - data.Quat[3] * data.Gyro[1]);
			qDot3 = 0.5f * (data.Quat[0] * data.Gyro[1] - data.Quat[1] * data.Gyro[2] + data.Quat[3] * data.Gyro[0]);
			qDot4 = 0.5f * (data.Quat[0] * data.Gyro[2] + data.Quat[1] * data.Gyro[1] - data.Quat[2] * data.Gyro[0]);

			// Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
			if (!(data.Acc[0] == 0.0f && data.Acc[1] == 0.0f && data.Acc[2] == 0.0f))
			{
				// Normalise accelerometer measurement
				recipNorm = invSqrt(data.Acc[0] * data.Acc[0] + data.Acc[1] * data.Acc[1] + data.Acc[2] * data.Acc[2]);
				data.Acc[0] *= recipNorm;
				data.Acc[1] *= recipNorm;
				data.Acc[2] *= recipNorm;

				// Auxiliary variables to avoid repeated arithmetic
				_2q0 = 2.0f * data.Quat[0];
				_2q1 = 2.0f * data.Quat[1];
				_2q2 = 2.0f * data.Quat[2];
				_2q3 = 2.0f * data.Quat[3];
				_4q0 = 4.0f * data.Quat[0];
				_4q1 = 4.0f * data.Quat[1];
				_4q2 = 4.0f * data.Quat[2];
				_8q1 = 8.0f * data.Quat[1];
				_8q2 = 8.0f * data.Quat[2];
				q0q0 = data.Quat[0] * data.Quat[0];
				q1q1 = data.Quat[1] * data.Quat[1];
				q2q2 = data.Quat[2] * data.Quat[2];
				q3q3 = data.Quat[3] * data.Quat[3];

				// Gradient decent algorithm corrective step
				s0 = _4q0 * q2q2 + _2q2 * data.Acc[0] + _4q0 * q1q1 - _2q1 * data.Acc[1];
				s1 = _4q1 * q3q3 - _2q3 * data.Acc[0] + 4.0f * q0q0 * data.Quat[1] - _2q0 * data.Acc[1] - _4q1 + _8q1 * q1q1 + _8q1 * q2q2 + _4q1 * data.Acc[2];
				s2 = 4.0f * q0q0 * data.Quat[2] + _2q0 * data.Acc[0] + _4q2 * q3q3 - _2q3 * data.Acc[1] - _4q2 + _8q2 * q1q1 + _8q2 * q2q2 + _4q2 * data.Acc[2];
				s3 = 4.0f * q1q1 * data.Quat[3] - _2q1 * data.Acc[0] + 4.0f * q2q2 * data.Quat[3] - _2q2 * data.Acc[1];
				recipNorm = invSqrt(s0 * s0 + s1 * s1 + s2 * s2 + s3 * s3); // normalise step magnitude
				s0 *= recipNorm;
				s1 *= recipNorm;
				s2 *= recipNorm;
				s3 *= recipNorm;

				// Apply feedback step
				qDot1 -= Constants.beta * s0;
				qDot2 -= Constants.beta * s1;
				qDot3 -= Constants.beta * s2;
				qDot4 -= Constants.beta * s3;
			}

			// Integrate rate of change of quaternion to yield quaternion
			data.Quat[0] += qDot1 * data.UpdatePeriod;
			data.Quat[1] += qDot2 * data.UpdatePeriod;
			data.Quat[2] += qDot3 * data.UpdatePeriod;
			data.Quat[3] += qDot4 * data.UpdatePeriod;

			// Normalise quaternion
			recipNorm = invSqrt(data.Quat[0] * data.Quat[0] + data.Quat[1] * data.Quat[1] + data.Quat[2] * data.Quat[2] + data.Quat[3] * data.Quat[3]);
			data.Quat[0] *= recipNorm;
			data.Quat[1] *= recipNorm;
			data.Quat[2] *= recipNorm;
			data.Quat[3] *= recipNorm;

			quatToAngles(data);
		}

		public static void SimpleAHRSupdate(ImuData data)
		{
			float norm;
			float hx;
			float hy;
			float hz;
			float bx;
			float bz;
			float vx;
			float vy;
			float vz;
			float wx;
			float wy;
			float wz;
			float exInt = 0.0F;
			float eyInt = 0.0F;
			float ezInt = 0.0F;
			float ex;
			float ey;
			float ez;
			float halfT = 0.024f;

			float q0q0 = data.Quat[0] * data.Quat[0];
			float q0q1 = data.Quat[0] * data.Quat[1];
			float q0q2 = data.Quat[0] * data.Quat[2];
			float q0q3 = data.Quat[0] * data.Quat[3];
			float q1q1 = data.Quat[1] * data.Quat[1];
			float q1q2 = data.Quat[1] * data.Quat[2];
			float q1q3 = data.Quat[1] * data.Quat[3];
			float q2q2 = data.Quat[2] * data.Quat[2];
			float q2q3 = data.Quat[2] * data.Quat[3];
			float q3q3 = data.Quat[3] * data.Quat[3];

			norm = invSqrt(data.Acc[0] * data.Acc[0] + data.Acc[1] * data.Acc[1] + data.Acc[2] * data.Acc[2]);
			data.Acc[0] = data.Acc[0] * norm;
			data.Acc[1] = data.Acc[1] * norm;
			data.Acc[2] = data.Acc[2] * norm;

			norm = invSqrt(data.Mag[0] * data.Mag[0] + data.Mag[1] * data.Mag[1] + data.Mag[2] * data.Mag[2]);
			data.Mag[0] = data.Mag[0] * norm;
			data.Mag[1] = data.Mag[1] * norm;
			data.Mag[2] = data.Mag[2] * norm;

			// compute reference direction of flux
			hx = 2 * data.Mag[0] * (0.5f - q2q2 - q3q3) + 2 * data.Mag[1] * (q1q2 - q0q3) + 2 * data.Mag[2] * (q1q3 + q0q2);
			hy = 2 * data.Mag[0] * (q1q2 + q0q3) + 2 * data.Mag[1] * (0.5f - q1q1 - q3q3) + 2 * data.Mag[2] * (q2q3 - q0q1);
			hz = 2 * data.Mag[0] * (q1q3 - q0q2) + 2 * data.Mag[1] * (q2q3 + q0q1) + 2 * data.Mag[2] * (0.5f - q1q1 - q2q2);
			bx = (float)Math.Sqrt((float)(hx * hx + hy * hy));
			bz = hz;

			// estimated direction of gravity and flux (v and w)
			vx = 2 * (q1q3 - q0q2);
			vy = 2 * (q0q1 + q2q3);
			vz = q0q0 - q1q1 - q2q2 + q3q3;
			wx = 2 * bx * (0.5f - q2q2 - q3q3) + 2 * bz * (q1q3 - q0q2);
			wy = 2 * bx * (q1q2 - q0q3) + 2 * bz * (q0q1 + q2q3);
			wz = 2 * bx * (q0q2 + q1q3) + 2 * bz * (0.5f - q1q1 - q2q2);

			// error is sum of cross product between reference direction of fields and direction measured by sensors
			ex = data.Acc[1] * vz - data.Acc[2] * vy + (data.Mag[1] * wz - data.Mag[2] * wy);
			ey = data.Acc[2] * vx - data.Acc[0] * vz + (data.Mag[2] * wx - data.Mag[0] * wz);
			ez = data.Acc[0] * vy - data.Acc[1] * vx + (data.Mag[0] * wy - data.Mag[1] * wx);

			if (ex != 0.0f && ey != 0.0f && ez != 0.0f)
			{
				exInt = exInt + ex * Constants.Ki * halfT;
				eyInt = eyInt + ey * Constants.Ki * halfT;
				ezInt = ezInt + ez * Constants.Ki * halfT;

				data.Gyro[0] = data.Gyro[0] + Constants.Kp * ex + exInt;
				data.Gyro[1] = data.Gyro[1] + Constants.Kp * ey + eyInt;
				data.Gyro[2] = data.Gyro[2] + Constants.Kp * ez + ezInt;
			}

			data.Quat[0] = data.Quat[0] + (-data.Quat[1] * data.Gyro[0] - data.Quat[2] * data.Gyro[1] - data.Quat[3] * data.Gyro[2]) * halfT;
			data.Quat[1] = data.Quat[1] + (data.Quat[0] * data.Gyro[0] + data.Quat[2] * data.Gyro[2] - data.Quat[3] * data.Gyro[1]) * halfT;
			data.Quat[2] = data.Quat[2] + (data.Quat[0] * data.Gyro[1] - data.Quat[1] * data.Gyro[2] + data.Quat[3] * data.Gyro[0]) * halfT;
			data.Quat[3] = data.Quat[3] + (data.Quat[0] * data.Gyro[2] + data.Quat[1] * data.Gyro[1] - data.Quat[2] * data.Gyro[0]) * halfT;

			norm = invSqrt(data.Quat[0] * data.Quat[0] + data.Quat[1] * data.Quat[1] + data.Quat[2] * data.Quat[2] + data.Quat[3] * data.Quat[3]);
			data.Quat[0] = data.Quat[0] * norm;
			data.Quat[1] = data.Quat[1] * norm;
			data.Quat[2] = data.Quat[2] * norm;
			data.Quat[3] = data.Quat[3] * norm;

			quatToAngles(data);
		}



	}
}