namespace Alchiweb.IoT.Icm20948
{
	/**
     * A structure to hold all data from IMU.
     */
	public class ImuData
	{
		/** The period at which IMU data is being pulled from the sensor. */
		public float UpdatePeriod { get; set; } = 0.0f;
        /** The temperature in Celsius degrees. */
        public float Temp { get; set; } = 0.0f;
        /** Raw angular rate from gyroscope in rad/s. */
        public float[] Gyro { get; } = [0.0f, 0.0f, 0.0f];
		/** Raw data from accelerometer in G's. */
		public float[] Acc { get; } = [0.0f, 0.0f, 0.0f];
        /** Raw data from compass in uT. */
        public float[] Mag { get; } = [0.0f, 0.0f, 0.0f];
        /** Quaternion representation of IMU orientation. */
        public float[] Quat { get; } = [1.0f, 0.0f, 0.0f, 0.0f];
        /** Euler angles representation of IMU orientation in degrees. */
        public float[] Angles { get; } = [0.0f, 0.0f, 0.0f];

	}
}