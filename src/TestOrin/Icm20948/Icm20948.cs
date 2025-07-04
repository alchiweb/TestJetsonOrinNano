using System.Device.I2c;
using TestOrin.Extensions;
using TestOrin.Icm20948.Enums;
using TestOrin.Icm20948.Configs;

namespace TestOrin.Icm20948
{

	public class Icm20948 : IDisposable
	{
		/** Current memory bank. */
		private ICM_BANK mCurrentBank;
		/** IMU configuration. */
		public IcmConfig Config { get; private set; }
		/** Scale for gyroscope - depends on configuration. */
		private float mGyroScale;
		/** Scale for accelerometer - depends on configuration. */
		private float mAccScale;
		/** Processed IMU data. */
		private ImuData mData = new ImuData();
		/** Preallocated buffer for raw IMU data. */
		private byte[] mRawDataBuf = new byte[22];
		/** Class for controlling I2C communication. */
		private I2cDevice mI2C;

		/**
		 * Basic constructor, initialises all variables.
		 */
		public Icm20948(I2cDevice device)
		{
            mCurrentBank = ICM_BANK.BANK_UNDEFINED;
            Config = new IcmConfig();
            mGyroScale = 0.0f;
            mAccScale = 0.0f;
            mI2C = device;
		}

		/**
		 * Destructor - closes the serial connection if it was opened.
		 */
		public virtual void Dispose()
		{
		}

		/**
		 * Initialises the IMU with provided configuration data.
		 *  @param config the IMU configuration.
		 */
		public async Task<bool> Initialise(IcmConfig? config = null)
		{
			if (config != null)
			{
                Config = config;
			}
			byte deviceID = 0;

			//if (string.Compare(config.Device, IcmConfig.Device) != 0)
			//{
			//	//			mI2C.closeSerialPort();
			//}

			mData.UpdatePeriod = 1.0f / Config.Framerate;

			//if (mI2C.openSerialPort(IcmConfig.Device))
			{
				SetBank(ICM_BANK.BANK_0);
				deviceID = mI2C.ReadByte(Constants.REG_ADD_WIA);
				if (Constants.REG_VAL_WIA == deviceID)
				{
					/* Reset all IMU configuration. */
					await Reset();

					SetBank(ICM_BANK.BANK_3);
					/* Reset I2C master clock. */
					mI2C.WriteByte(Constants.REG_ADD_I2C_MST_CTRL, 0);

					await ConfigureMasterI2C();

					if (Config.Temp.MustBeEnabled)
					{
						ConfigureTemp();
					}

					if (Config.Gyro.MustBeEnabled)
					{
						ConfigureGyro();
					}
					if (Config.Acc.MustBeEnabled)
					{
						ConfigureAcc();
					}

					if (Config.MagMustBeEnabled)
					{
						Config.MagMustBeEnabled = await ConfigureMag();
					}
				}
			else
			{
				Console.WriteLine($"Connected device is not ICM-20948! ID: {deviceID}");
			}
			}
		//else
		//{
  //              Console.WriteLine($"Failed to open device: {IcmConfig.Device}");
		//}
			return Constants.REG_VAL_WIA == deviceID;
		}

		/**
		 * Pulls the data from the device and processes it.
		 *  @return a reference to the latest IMU data (not thread-safe).
		 */
		public ImuData ImuDataGet()
		{
			bool magEnabled = Config.MagMustBeEnabled;
			short[] s16Gyro = [0, 0, 0];
            short[] s16Accel = [0, 0, 0];
            short[] s16Magn = [0, 0, 0];
            short temperature = 0;

			magEnabled &= ReadAllRawDAta(s16Gyro, s16Accel, s16Magn, ref temperature);
			mData.Gyro[0] = s16Gyro[0] * mGyroScale * Conversion.DEG_TO_RAD;
			mData.Gyro[1] = s16Gyro[1] * mGyroScale * Conversion.DEG_TO_RAD;
            mData.Gyro[2] = s16Gyro[2] * mGyroScale * Conversion.DEG_TO_RAD;

            mData.Acc[0] = s16Accel[0] * mAccScale; // in G's, NOT in m/s/s
            mData.Acc[1] = s16Accel[1] * mAccScale;
            mData.Acc[2] = s16Accel[2] * mAccScale;

            mData.Mag[0] = s16Magn[0] * Conversion.MAG_SCALE;
            mData.Mag[1] = s16Magn[1] * Conversion.MAG_SCALE;
            mData.Mag[2] = s16Magn[2] * Conversion.MAG_SCALE;

            mData.Temp = (temperature - 21) / 333.87f + 21.0f;

			switch (Config.Ahrs)
			{
				case AHRS_ALGORITHM.NONE:
					break;
				case AHRS_ALGORITHM.SIMPLE:
					Conversion.SimpleAHRSupdate(mData);
					break;
				case AHRS_ALGORITHM.MADGWICK:
				/* Fall-through to the default case. */
				default:
                    if (magEnabled)
					{
                        Conversion.MadgwickAHRSupdate(mData);
                    }
                    else
					{
						Conversion.MadgwickAHRSupdateIMU(mData);
					}
					break;
			}
			return mData;
		}

		/**
		 * Calibrates the gyroscope by averaging 1024 samples of gyro and uploading them as a bias to the device.
		 * @note make sure to be stationary!
		 */
		public async Task CalibrateGyro()
		{
			short[] s16G = [0, 0, 0];
			int[] s32G = { 0, 0, 0 };

//#if LOG
		Console.WriteLine("Calibrating IMU offsets, please wait approximately 11 seconds.");
//#endif // LOG

			// Reset the offset so that we perform the fresh calibration
			SetBank(ICM_BANK.BANK_2);
			mI2C.WriteByte(Constants.REG_ADD_XG_OFFS_USRH, 0);
			mI2C.WriteByte(Constants.REG_ADD_XG_OFFS_USRL, 0);
			mI2C.WriteByte(Constants.REG_ADD_YG_OFFS_USRH, 0);
			mI2C.WriteByte(Constants.REG_ADD_YG_OFFS_USRL, 0);
			mI2C.WriteByte(Constants.REG_ADD_ZG_OFFS_USRH, 0);
			mI2C.WriteByte(Constants.REG_ADD_ZG_OFFS_USRL, 0);

			// Read several gyro measurements and average them.
			for (int i = 0; i < 1024; ++i)
			{
				ReadRawGyro(s16G);
				s32G[0] += s16G[0];
				s32G[1] += s16G[1];
				s32G[2] += s16G[2];
				await Task.Delay(TimeSpan.FromMilliseconds(10));
			}

			s16G[0] = (short)-(s32G[0] >> 12 - (short)Config.Gyro.Range);
			s16G[1] = (short)-(s32G[1] >> 12 - (short)Config.Gyro.Range);
			s16G[2] = (short)-(s32G[2] >> 12 - (short)Config.Gyro.Range);

			// Push gyroscope biases to hardware registers
			SetBank(ICM_BANK.BANK_2);
			mI2C.WriteByte(Constants.REG_ADD_XG_OFFS_USRH, (byte)(s16G[0] >> 8 & 0xFF));
			mI2C.WriteByte(Constants.REG_ADD_XG_OFFS_USRL, (byte)(s16G[0] & 0xFF));
			mI2C.WriteByte(Constants.REG_ADD_YG_OFFS_USRH, (byte)(s16G[1] >> 8 & 0xFF));
			mI2C.WriteByte(Constants.REG_ADD_YG_OFFS_USRL, (byte)(s16G[1] & 0xFF));
			mI2C.WriteByte(Constants.REG_ADD_ZG_OFFS_USRH, (byte)(s16G[2] >> 8 & 0xFF));
			mI2C.WriteByte(Constants.REG_ADD_ZG_OFFS_USRL, (byte)(s16G[2] & 0xFF));
        }

		/**
		 * Switches to new memory bank if not already in it.
		 *  @param bank the new memory bank.
		 */
		// C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
		// ORIGINAL LINE: void SetBank(const ICM_BANK bank) const
		private void SetBank(ICM_BANK bank)
		{
			if (bank != mCurrentBank && bank != ICM_BANK.BANK_UNDEFINED)
			{
				mI2C.WriteByte(Constants.REG_ADD_REG_BANK_SEL, (byte)bank);
                mCurrentBank = bank;
			}
		}

		/**
		 * Resets the device and all its settings.
		 */
		private async Task Reset()
		{
			byte sensorsFlag = Constants.REG_VAL_SENSORS_ON;

			await MagI2CWrite(Constants.REG_ADD_MAG_CNTL2, 0x00);
			await Task.Delay(TimeSpan.FromMilliseconds(100));

			SetBank(ICM_BANK.BANK_0);
			/* Reset all settings on master device  */
			mI2C.WriteByte(Constants.REG_ADD_PWR_MGMT_1, Constants.REG_VAL_ALL_RGE_RESET);

			await Task.Delay(TimeSpan.FromMilliseconds(10));
			/* Enable optimal on-board timer and configure temperature sensor */
			mI2C.WriteByte(Constants.REG_ADD_PWR_MGMT_1, (byte)(Constants.REG_VAL_RUN_MODE | (Config.Temp.MustBeEnabled ? 0 : 1) << 3));

			/* Enable both sensors */
			if (!Config.Gyro.MustBeEnabled)
			{
				sensorsFlag |= Constants.REG_VAL_DISABLE_GYRO;
			}
			if (!Config.Acc.MustBeEnabled)
			{
				sensorsFlag |= Constants.REG_VAL_DISABLE_ACC;
			}
			mI2C.WriteByte(Constants.REG_ADD_PWR_MGMT_2, sensorsFlag);
			await Task.Delay(TimeSpan.FromMilliseconds(10));

			/* Reset all settings on magnetometer.
			 * NOTE: this will log error as the value is immediately changed back to 0 by the sensor itself.  */
			await MagI2CWrite(Constants.REG_ADD_MAG_CNTL3, Constants.REG_VAL_MAG_RESET);
			await Task.Delay(TimeSpan.FromMilliseconds(100));
		}

		/**
		 * Configures gyroscope with data from the internal configuration structure.
		 */
		private void ConfigureGyro()
		{
			byte sampleRateDivisor = Config.Gyro.SampleRateDivisor;
			mGyroScale = (float)(((int)Config.Gyro.Range + 1) * 250) / 32768;

			switch (Config.Gyro.Averaging)
			{
				case GyroAveraging.GYRO_AVERAGING_1X:
					sampleRateDivisor = Math.Max(Config.Gyro.SampleRateDivisor, (byte)1);
					break;
				case GyroAveraging.GYRO_AVERAGING_2X:
					sampleRateDivisor = Math.Max(Config.Gyro.SampleRateDivisor, (byte)2);
					break;
				case GyroAveraging.GYRO_AVERAGING_4X:
					sampleRateDivisor = Math.Max(Config.Gyro.SampleRateDivisor, (byte)3);
					break;
				case GyroAveraging.GYRO_AVERAGING_8X:
					sampleRateDivisor = Math.Max(Config.Gyro.SampleRateDivisor, (byte)5);
					break;
				case GyroAveraging.GYRO_AVERAGING_16X:
					sampleRateDivisor = Math.Max(Config.Gyro.SampleRateDivisor, (byte)10);
					break;
				case GyroAveraging.GYRO_AVERAGING_32X:
					sampleRateDivisor = Math.Max(Config.Gyro.SampleRateDivisor, (byte)22);
					break;
				case GyroAveraging.GYRO_AVERAGING_64X:
					sampleRateDivisor = Math.Max(Config.Gyro.SampleRateDivisor, (byte)63);
					break;
				case GyroAveraging.GYRO_AVERAGING_128X:
					sampleRateDivisor = Math.Max(Config.Gyro.SampleRateDivisor, (byte)255);
					break;
				case GyroAveraging.GYRO_AVERAGING_NONE:
					break;
				default:
#if LOG
				printf("ConfigureGyro:: this enum shouldn't be processed: %d !!! \n", (int)Config.mGyro.mAveraging);
#endif // LOG
					break;
			}

			SetBank(ICM_BANK.BANK_2);
			mI2C.WriteByte(Constants.REG_ADD_GYRO_SMPLRT_DIV, (byte)(sampleRateDivisor & 0xFF));
			mI2C.WriteByte(Constants.REG_ADD_GYRO_CONFIG_1, (byte)((int)Config.Gyro.DlpfBandWidth | (int)Config.Gyro.Range << 1));

			if (Config.Gyro.Averaging > GyroAveraging.GYRO_AVERAGING_NONE)
			{
				mI2C.WriteByte(Constants.REG_ADD_GYRO_CONFIG_2, (byte)Config.Gyro.Averaging);
			}
		}

		/**
		 * Configures accelerometer with data from the internal configuration structure.
		 */
		private void ConfigureAcc()
		{
			AccRangeDlpfBandWidth bandwidth = Config.Acc.DlpfBandWidth;
			byte sampleRateDivisor = Config.Acc.SampleRateDivisor;

			mAccScale =(float)Math.Pow(2.0, (float)(Config.Acc.Range + 1)) / 32768;

			if (AccAveraging.ACC_AVERAGING_NONE < Config.Acc.Averaging)
			{
				bandwidth = AccRangeDlpfBandWidth.ACC_DLPF_BANDWIDTH_473HZ;
				switch (Config.Acc.Averaging)
				{
					case AccAveraging.ACC_AVERAGING_4X:
						sampleRateDivisor = Math.Max(Config.Acc.SampleRateDivisor, (byte)3);
						break;
					case AccAveraging.ACC_AVERAGING_8X:
						sampleRateDivisor = Math.Max(Config.Acc.SampleRateDivisor, (byte)5);
						break;
					case AccAveraging.ACC_AVERAGING_16X:
						sampleRateDivisor = Math.Max(Config.Acc.SampleRateDivisor, (byte)7);
						break;
					case AccAveraging.ACC_AVERAGING_32X:
						sampleRateDivisor = Math.Max(Config.Acc.SampleRateDivisor, (byte)10);
						break;
					case AccAveraging.ACC_AVERAGING_NONE:
						break;
					default:
#if LOG
					printf("configureAcc:: this enum shouldn't be processed: %d !!! \n", (int)Config.mAcc.mAveraging);
#endif // LOG
						break;
				}
			}

			SetBank(ICM_BANK.BANK_2);
			mI2C.WriteByte(Constants.REG_ADD_ACCEL_SMPLRT_DIV_1, (byte)(sampleRateDivisor >> 8 & 0x0F));
			mI2C.WriteByte(Constants.REG_ADD_ACCEL_SMPLRT_DIV_2, (byte)(sampleRateDivisor & 0xFF));
			mI2C.WriteByte(Constants.REG_ADD_ACCEL_CONFIG, (byte)((int)bandwidth | (int)Config.Acc.Range << 1));

			if (AccAveraging.ACC_AVERAGING_NONE < Config.Acc.Averaging)
			{
				mI2C.WriteByte(Constants.REG_ADD_ACCEL_CONFIG_2, (byte)Config.Acc.Averaging);
			}
		}

		/**
		 * Configures magnetometer with data from the internal configuration structure.
		 */
		private async Task<bool> ConfigureMag()
		{
			byte[] u8Data = new byte[Conversion.MAG_DATA_LEN];
			int counter = 0;
			bool flag = CheckMag();

			while (!flag && ++counter <= 10)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(100));
				flag = CheckMag();
			}

			if (flag)
			{
#if LOG
			puts("Found Magnetometer!");
#endif // LOG
				await Task.Delay(TimeSpan.FromMilliseconds(1000));
				await MagI2CWrite(Constants.REG_ADD_MAG_CNTL2, Constants.REG_VAL_MAG_MODE_100HZ);

				await Task.Delay(TimeSpan.FromMilliseconds(10));
				MagI2CRead(Constants.REG_ADD_MAG_DATA, (byte)Conversion.MAG_DATA_LEN, u8Data);
			}
#if LOG
		else
		{
			puts("Failed to detect the magnetometer!");
		}
#endif // LOG
			return flag;
		}

		/**
		 * Configures temperature sensor with data from the internal configuration structure.
		 */
		private void ConfigureTemp()
		{
			SetBank(ICM_BANK.BANK_2);
			/* configure temperature sensor */
			mI2C.WriteByte(Constants.REG_ADD_TEMP_CONFIG, (byte)Config.Temp.DlpfBandWidth);
		}

		/**
		 * Configures the ICM device to master on I2C bus so that it may pull updates from magnetometer.
		 */
		private async Task ConfigureMasterI2C()
		{
			byte temp = new byte();
			SetBank(ICM_BANK.BANK_0);
			/* Read the current user control and update it with new configuration to enable I2C master */
			temp = mI2C.ReadByte(Constants.REG_ADD_USER_CTRL);
			mI2C.WriteByte(Constants.REG_ADD_USER_CTRL, (byte)(temp | Constants.REG_VAL_BIT_I2C_MST_EN));

			/* Set I2C master clock to recommended value. */
			SetBank(ICM_BANK.BANK_3);
			mI2C.WriteByte(Constants.REG_ADD_I2C_MST_CTRL, Constants.REG_VAL_I2C_MST_CTRL_CLK_400KHZ);

			await Task.Delay(TimeSpan.FromMilliseconds(10));
		}

		/**
		 * Reads raw gyro data.
		 *  @param[out] gyro the raw gyro data for all three axes.
		 */
		// C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
		// ORIGINAL LINE: void ReadRawGyro(int gyro[3]) const
		private void ReadRawGyro(short[] gyro)
		{
			byte[] u8Buf = new byte[Conversion.GYRO_AND_ACC_DATA_LEN];
			SetBank(ICM_BANK.BANK_0);
			mI2C.Read(Constants.REG_ADD_GYRO_XOUT_H, u8Buf);
			gyro[0] = (short)(u8Buf[0] << 8 | u8Buf[1]);
			gyro[1] = (short)(u8Buf[2] << 8 | u8Buf[3]);
			gyro[2] = (short)(u8Buf[4] << 8 | u8Buf[5]);
		}

		/**
		 * Reads raw acceleration data.
		 *  @param[out] acc the raw acceleration data for all three axes.
		 */
		private void ReadRawAcc(int[] acc)
		{
			byte[] u8Buf = new byte[Conversion.GYRO_AND_ACC_DATA_LEN];
			SetBank(ICM_BANK.BANK_0);
			mI2C.Read(Constants.REG_ADD_ACCEL_XOUT_H, u8Buf);
			acc[0] = u8Buf[0] << 8 | u8Buf[1];
			acc[1] = u8Buf[2] << 8 | u8Buf[3];
			acc[2] = u8Buf[4] << 8 | u8Buf[5];
		}

		/**
		 * Reads raw magnetometer data.
		 *  @param[out] mag the raw magnetometer data for all three axes.
		 *  @return the flag indicating magnetometer overflow.
		 */
		private bool ReadRawMag(int[] mag)
		{
			byte[] u8Buf = new byte[Conversion.MAG_DATA_LEN];
			SetBank(ICM_BANK.BANK_0);
			mI2C.Read(Constants.REG_ADD_EXT_SENS_DATA_00,  u8Buf);
			mag[0] = u8Buf[1] << 8 | u8Buf[0];
			mag[1] = -(u8Buf[3] << 8 | u8Buf[2]);
			mag[2] = -(u8Buf[5] << 8 | u8Buf[4]);
			/* Check if there was an overflow. */
			return (u8Buf[7] & 0x08) == 0;
		}

		/**
		 *  @return the raw temperature data.
		 */
		private int ReadRawTemp()
		{
			byte[] u8Buf = [0, 0];
			SetBank(ICM_BANK.BANK_0);
			mI2C.Read(Constants.REG_ADD_TEMP_OUT_H, u8Buf);
			return u8Buf[0] << 8 | u8Buf[1];
		}

		/**
		 * Reads all data from the device.
		 *  @param[out] gyro the raw gyro data for all three axes.
		 *  @param[out] acc the raw acceleration data for all three axes.
		 *  @param[out] mag the raw magnetometer data for all three axes.
		 *  @param[out] temp the raw temperature data.
		 *  @return the flag indicating magnetometer overflow.
		 */
		private bool ReadAllRawDAta(short[] gyro, short[] acc, short[] mag, ref short temp)
		{
			SetBank(ICM_BANK.BANK_0);
			mI2C.Read(Constants.REG_ADD_ACCEL_XOUT_H, mRawDataBuf);

			/* Parse accelerometer data */
			acc[0] = (short)(mRawDataBuf[0] << 8 | mRawDataBuf[1]);
			acc[1] = (short)(mRawDataBuf[2] << 8 | mRawDataBuf[3]);
			acc[2] = (short)(mRawDataBuf[4] << 8 | mRawDataBuf[5]);

            /* Parse gyroscope data */
            gyro[0] = (short)(mRawDataBuf[6] << 8 | mRawDataBuf[7]);
			gyro[1] = (short)(mRawDataBuf[8] << 8 | mRawDataBuf[9]);
			gyro[2] = (short)(mRawDataBuf[10] << 8 | mRawDataBuf[11]);

            /* Parse temperature data */
            temp = (short)(mRawDataBuf[12] << 8 | mRawDataBuf[13]);

			/* Parse magnetic data */
			mag[0] = (short)(mRawDataBuf[15] << 8 | mRawDataBuf[14]);
			mag[1] = (short)-(mRawDataBuf[17] << 8 | mRawDataBuf[16]);
			mag[2] = (short)-(mRawDataBuf[19] << 8 | mRawDataBuf[18]);

            return (mRawDataBuf[21] & 0x08) == 0;
		}

		/**
		 *  @return true if magnetometer was detected.
		 */
		private bool CheckMag()
		{
			byte[] u8Ret = [0, 0];
			MagI2CRead(Constants.REG_ADD_MAG_WIA1, 2, u8Ret);
#if LOG
		if ((u8Ret[0] != DefineConstants.REG_VAL_MAG_WIA1) || (u8Ret[1] != DefineConstants.REG_VAL_MAG_WIA2))
		{
			printf("Failed to obtain magnetometer address. Expected: %02x%02x, but received: %02x%02x Current bank: %d \n", DefineConstants.REG_VAL_MAG_WIA1, DefineConstants.REG_VAL_MAG_WIA2, u8Ret[0], u8Ret[1], mCurrentBank);
		}
#endif // LOG
			return u8Ret[0] == Constants.REG_VAL_MAG_WIA1 && u8Ret[1] == Constants.REG_VAL_MAG_WIA2;
		}

		/**
		 * Reads data from AK09916 sensor.
		 *  @param regAddr the address of the register from which the read should begin.
		 *  @param length the number of bytes to read (the length of @p data).
		 *  @param[out] data preallocated buffer for read data.
		 */
		private void MagI2CRead(byte regAddr, byte length, byte[] data)
		{
			SetBank(ICM_BANK.BANK_3);
			mI2C.WriteByte(Constants.REG_ADD_I2C_SLV0_ADDR, Constants.I2C_ADD_ICM20948_AK09916 | Constants.I2C_ADD_ICM20948_AK09916_READ);
			mI2C.WriteByte(Constants.REG_ADD_I2C_SLV0_REG, regAddr);
			mI2C.WriteByte(Constants.REG_ADD_I2C_SLV0_CTRL, (byte)(Constants.REG_VAL_BIT_SLV0_EN | length));

			SetBank(ICM_BANK.BANK_0);
			mI2C.Read(Constants.REG_ADD_EXT_SENS_DATA_00, data);
		}

		/**
		 * Writes a single byte data to AK09916 sensor.
		 *  @param regAddr the address of the register from which the read should begin.
		 *  @param value a new data to upload to the device.
		 */
		private async Task MagI2CWrite(byte regAddr, byte value)
		{
			byte[] u8Temp = [0];
			SetBank(ICM_BANK.BANK_3);
			mI2C.WriteByte(Constants.REG_ADD_I2C_SLV0_ADDR, Constants.I2C_ADD_ICM20948_AK09916 | Constants.I2C_ADD_ICM20948_AK09916_WRITE);
			mI2C.WriteByte(Constants.REG_ADD_I2C_SLV0_REG, regAddr);
			mI2C.WriteByte(Constants.REG_ADD_I2C_SLV0_DO, value);
			mI2C.WriteByte(Constants.REG_ADD_I2C_SLV0_CTRL, Constants.REG_VAL_BIT_SLV0_EN | 1);

			await Task.Delay(TimeSpan.FromMilliseconds(100));
			MagI2CRead(regAddr, 1, u8Temp);
#if LOG
		if (value != u8Temp)
		{
			printf("Failed to write %d to magnetometer address: %d. Data received: %d. Current bank: %d \n", value, regAddr, u8Temp, mCurrentBank);
		}
#endif // LOG
		}

	}

}