using Meadow.Foundation.Spatial;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Motion;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Foundation.Sensors.Motion
{
    /// <summary>
	/// 
	/// </summary>
	public partial class Icm20948 : ByteCommsSensorBase<(
        Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D,
        MagneticField3D? MagneticField3D, Quaternion? QuaternionOrientation,
        EulerAngles? EulerOrientation, Units.Temperature? Temperature)>,
        IAccelerometer, IGyroscope, ISamplingTemperatureSensor, II2cPeripheral
    {
        /// <summary>
        /// The default I2C address for the peripheral
        /// </summary>
        public byte DefaultI2cAddress => (byte)Addresses.Default;
        /// <summary>
        /// Current memory bank.
        /// </summary>
        private IcmBank mCurrentBank;

        /// <summary>
        /// IMU configuration.
        /// </summary>
        public IcmConfig Config { get; private set; }


        /// <summary>
        /// Current Acceleration
        /// </summary>
        public Acceleration3D? Acceleration3D => Conditions.Acceleration3D;

        /// <summary>
        /// Current Angular Velocity
        /// </summary>
        public AngularVelocity3D? AngularVelocity3D => Conditions.AngularVelocity3D;

        /// <summary>
        /// Current Magnetic Field
        /// </summary>
        public MagneticField3D? MagneticField3D => Conditions.MagneticField3D;

        /// <summary>
        /// Current Quaternion Orientation
        /// </summary>
        public Quaternion? QuaternionOrientation => Conditions.QuaternionOrientation;

        /// <summary>
        /// Current Euler Orientation
        /// </summary>
        public EulerAngles? EulerOrientation => Conditions.EulerOrientation;

        /// <summary>
        /// Current Temperature value
        /// </summary>
        public Units.Temperature? Temperature => Conditions.Temperature;






        /// <summary>
        /// Scale for gyroscope - depends on configuration.
        /// </summary>
        private float mGyroScale;
        /// <summary>
        /// Scale for accelerometer - depends on configuration.
        /// </summary>
        private float mAccScale;
        /// <summary>
        /// Processed IMU data.
        /// </summary>
        private ImuData mData = new ImuData();
        ///// <summary>
        ///// Preallocated buffer for raw IMU data.
        ///// </summary>
        //private byte[] mRawDataBuf = new byte[22];



        /// <summary>
        /// Raised when the magnetic field value changes
        /// </summary>
        public event EventHandler<IChangeResult<MagneticField3D>> MagneticField3DUpdated = default!;

        /// <summary>
        /// Raised when the quaternion orientation value changes
        /// </summary>
        public event EventHandler<IChangeResult<Quaternion>> QuaternionOrientationUpdated = default!;

        /// <summary>
        /// Raised when the euler orientation value changes
        /// </summary>
        public event EventHandler<IChangeResult<EulerAngles>> EulerOrientationUpdated = default!;

        private event EventHandler<IChangeResult<Units.Temperature>> _temperatureHandlers = default!;
        private event EventHandler<IChangeResult<AngularVelocity3D>> _velocityHandlers = default!;
        private event EventHandler<IChangeResult<Acceleration3D>> _accelerationHandlers = default!;

        event EventHandler<IChangeResult<Units.Temperature>> ISamplingSensor<Units.Temperature>.Updated
        {
            add => _temperatureHandlers += value;
            remove => _temperatureHandlers -= value;
        }

        event EventHandler<IChangeResult<AngularVelocity3D>> ISamplingSensor<AngularVelocity3D>.Updated
        {
            add => _velocityHandlers += value;
            remove => _velocityHandlers -= value;
        }

        event EventHandler<IChangeResult<Acceleration3D>> ISamplingSensor<Acceleration3D>.Updated
        {
            add => _accelerationHandlers += value;
            remove => _accelerationHandlers -= value;
        }


        /// <summary>
        /// Create a new Icm20948 object using the default parameters for the component.
        /// </summary>
        /// <param name="i2cBus"></param>
        /// <param name="address"></param>
        public Icm20948(II2cBus i2cBus, byte address = (byte)Addresses.Address_0x68) : base(i2cBus, address, readBufferSize: 22)
        {
            mCurrentBank = IcmBank.BANK_UNDEFINED;
            Config = new IcmConfig();
            mGyroScale = 0.0f;
            mAccScale = 0.0f;
        }


        /// <summary>
        /// Initialises the IMU with provided configuration data.
        /// </summary>
        /// <param name="config">config the IMU configuration.</param>
        /// <returns></returns>
        public async Task<bool> Initialise(IcmConfig? config = null)
        {
            if (config != null)
			{
                Config = config;
			}
			byte deviceID = 0;

			//if (string.Compare(config.Device, IcmConfig.Device) != 0)
			//{
			//	//			BusComms.closeSerialPort();
			//}

			mData.UpdatePeriod = 1.0f / Config.Framerate;

			//if (BusComms.openSerialPort(IcmConfig.Device))
			{
				SetBank(IcmBank.BANK_0);
				deviceID = BusComms.ReadRegister(Registers.ADD_WIA);
				if (Registers.VAL_WIA == deviceID)
				{
					/* Reset all IMU configuration. */
					await Reset();

					SetBank(IcmBank.BANK_3);
					/* Reset I2C master clock. */
					BusComms.WriteRegister(Registers.ADD_I2C_MST_CTRL, 0);

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
			return Registers.VAL_WIA == deviceID;
		}


        /// <summary>
        /// Pulls the data from the device and processes it.
        /// </summary>
        /// <returns>a reference to the latest IMU data (not thread-safe).</returns>
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
				case AhrsAlgorithm.NONE:
					break;
				case AhrsAlgorithm.SIMPLE:
					Conversion.SimpleAHRSupdate(mData);
					break;
				case AhrsAlgorithm.MADGWICK:
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

        /// <summary>
        /// Calibrates the gyroscope by averaging 1024 samples of gyro and uploading them as a bias to the device.
        /// </summary>
		/// <remarks>make sure to be stationary!</remarks>
        /// <returns></returns>
        public async Task CalibrateGyro()
		{
			short[] s16G = [0, 0, 0];
			int[] s32G = { 0, 0, 0 };

//#if LOG
		Console.WriteLine("Calibrating IMU offsets, please wait approximately 11 seconds.");
//#endif // LOG

			// Reset the offset so that we perform the fresh calibration
			SetBank(IcmBank.BANK_2);
			BusComms.WriteRegister(Registers.ADD_XG_OFFS_USRH, 0);
			BusComms.WriteRegister(Registers.ADD_XG_OFFS_USRL, 0);
			BusComms.WriteRegister(Registers.ADD_YG_OFFS_USRH, 0);
			BusComms.WriteRegister(Registers.ADD_YG_OFFS_USRL, 0);
			BusComms.WriteRegister(Registers.ADD_ZG_OFFS_USRH, 0);
			BusComms.WriteRegister(Registers.ADD_ZG_OFFS_USRL, 0);

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
			SetBank(IcmBank.BANK_2);
			BusComms.WriteRegister(Registers.ADD_XG_OFFS_USRH, (byte)(s16G[0] >> 8 & 0xFF));
			BusComms.WriteRegister(Registers.ADD_XG_OFFS_USRL, (byte)(s16G[0] & 0xFF));
			BusComms.WriteRegister(Registers.ADD_YG_OFFS_USRH, (byte)(s16G[1] >> 8 & 0xFF));
			BusComms.WriteRegister(Registers.ADD_YG_OFFS_USRL, (byte)(s16G[1] & 0xFF));
			BusComms.WriteRegister(Registers.ADD_ZG_OFFS_USRH, (byte)(s16G[2] >> 8 & 0xFF));
			BusComms.WriteRegister(Registers.ADD_ZG_OFFS_USRL, (byte)(s16G[2] & 0xFF));
        }

        /// <summary>
        /// Switches to new memory bank if not already in it.
        /// </summary>
        /// <param name="bank">bank the new memory bank.</param>
        private void SetBank(IcmBank bank)
        {
            if (bank != mCurrentBank && bank != IcmBank.BANK_UNDEFINED)
			{
				BusComms.WriteRegister(Registers.ADD_REG_BANK_SEL, (byte)bank);
                mCurrentBank = bank;
			}
		}

        /// <summary>
        /// Resets the device and all its settings.
        /// </summary>
        /// <returns></returns>
        private async Task Reset()
		{
			byte sensorsFlag = Registers.VAL_SENSORS_ON;

			await MagI2CWrite(Registers.ADD_MAG_CNTL2, 0x00);
			await Task.Delay(TimeSpan.FromMilliseconds(100));

			SetBank(IcmBank.BANK_0);
			/* Reset all settings on master device  */
			BusComms.WriteRegister(Registers.ADD_PWR_MGMT_1, Registers.VAL_ALL_RGE_RESET);

			await Task.Delay(TimeSpan.FromMilliseconds(10));
			/* Enable optimal on-board timer and configure temperature sensor */
			BusComms.WriteRegister(Registers.ADD_PWR_MGMT_1, (byte)(Registers.VAL_RUN_MODE | (Config.Temp.MustBeEnabled ? 0 : 1) << 3));

			/* Enable both sensors */
			if (!Config.Gyro.MustBeEnabled)
			{
				sensorsFlag |= Registers.VAL_DISABLE_GYRO;
			}
			if (!Config.Acc.MustBeEnabled)
			{
				sensorsFlag |= Registers.VAL_DISABLE_ACC;
			}
			BusComms.WriteRegister(Registers.ADD_PWR_MGMT_2, sensorsFlag);
			await Task.Delay(TimeSpan.FromMilliseconds(10));

			/* Reset all settings on magnetometer.
			 * NOTE: this will log error as the value is immediately changed back to 0 by the sensor itself.  */
			await MagI2CWrite(Registers.ADD_MAG_CNTL3, Registers.VAL_MAG_RESET);
			await Task.Delay(TimeSpan.FromMilliseconds(100));
		}

        /// <summary>
        /// Configures gyroscope with data from the internal configuration structure.
        /// </summary>
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

			SetBank(IcmBank.BANK_2);
			BusComms.WriteRegister(Registers.ADD_GYRO_SMPLRT_DIV, (byte)(sampleRateDivisor & 0xFF));
			BusComms.WriteRegister(Registers.ADD_GYRO_CONFIG_1, (byte)((int)Config.Gyro.DlpfBandWidth | (int)Config.Gyro.Range << 1));

			if (Config.Gyro.Averaging > GyroAveraging.GYRO_AVERAGING_NONE)
			{
				BusComms.WriteRegister(Registers.ADD_GYRO_CONFIG_2, (byte)Config.Gyro.Averaging);
			}
		}

        /// <summary>
        /// Configures accelerometer with data from the internal configuration structure.
        /// </summary>
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

			SetBank(IcmBank.BANK_2);
			BusComms.WriteRegister(Registers.ADD_ACCEL_SMPLRT_DIV_1, (byte)(sampleRateDivisor >> 8 & 0x0F));
			BusComms.WriteRegister(Registers.ADD_ACCEL_SMPLRT_DIV_2, (byte)(sampleRateDivisor & 0xFF));
			BusComms.WriteRegister(Registers.ADD_ACCEL_CONFIG, (byte)((int)bandwidth | (int)Config.Acc.Range << 1));

			if (AccAveraging.ACC_AVERAGING_NONE < Config.Acc.Averaging)
			{
				BusComms.WriteRegister(Registers.ADD_ACCEL_CONFIG_2, (byte)Config.Acc.Averaging);
			}
		}

        /// <summary>
        /// Configures magnetometer with data from the internal configuration structure.
        /// </summary>
        /// <returns></returns>
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
				await MagI2CWrite(Registers.ADD_MAG_CNTL2, Registers.VAL_MAG_MODE_100HZ);

				await Task.Delay(TimeSpan.FromMilliseconds(10));
				MagI2CRead(Registers.ADD_MAG_DATA, (byte)Conversion.MAG_DATA_LEN, u8Data);
			}
#if LOG
		else
		{
			puts("Failed to detect the magnetometer!");
		}
#endif // LOG
			return flag;
		}

        /// <summary>
        /// Configures temperature sensor with data from the internal configuration structure.
        /// </summary>
        private void ConfigureTemp()
		{
			SetBank(IcmBank.BANK_2);
			/* configure temperature sensor */
			BusComms.WriteRegister(Registers.ADD_TEMP_CONFIG, (byte)Config.Temp.DlpfBandWidth);
		}

        /// <summary>
        /// Configures the ICM device to master on I2C bus so that it may pull updates from magnetometer.
        /// </summary>
        /// <returns></returns>
        private async Task ConfigureMasterI2C()
		{
			byte temp = new byte();
			SetBank(IcmBank.BANK_0);
			/* Read the current user control and update it with new configuration to enable I2C master */
			temp = BusComms.ReadRegister(Registers.ADD_USER_CTRL);
			BusComms.WriteRegister(Registers.ADD_USER_CTRL, (byte)(temp | Registers.VAL_BIT_I2C_MST_EN));

			/* Set I2C master clock to recommended value. */
			SetBank(IcmBank.BANK_3);
			BusComms.WriteRegister(Registers.ADD_I2C_MST_CTRL, Registers.VAL_I2C_MST_CTRL_CLK_400KHZ);

			await Task.Delay(TimeSpan.FromMilliseconds(10));
		}

        /// <summary>
        /// Reads raw gyro data.
        /// </summary>
        /// <param name="gyro">[out] gyro the raw gyro data for all three axes.</param>
        private void ReadRawGyro(short[] gyro)
        {
            byte[] u8Buf = new byte[Conversion.GYRO_AND_ACC_DATA_LEN];
			SetBank(IcmBank.BANK_0);
			BusComms.ReadRegister(Registers.ADD_GYRO_XOUT_H, u8Buf);
			gyro[0] = (short)(u8Buf[0] << 8 | u8Buf[1]);
			gyro[1] = (short)(u8Buf[2] << 8 | u8Buf[3]);
			gyro[2] = (short)(u8Buf[4] << 8 | u8Buf[5]);
		}

        /// <summary>
        /// Reads raw acceleration data.
        /// </summary>
        /// <param name="acc">[out] acc the raw acceleration data for all three axes.</param>
        private void ReadRawAcc(int[] acc)
        {
            byte[] u8Buf = new byte[Conversion.GYRO_AND_ACC_DATA_LEN];
			SetBank(IcmBank.BANK_0);
			BusComms.ReadRegister(Registers.ADD_ACCEL_XOUT_H, u8Buf);
			acc[0] = u8Buf[0] << 8 | u8Buf[1];
			acc[1] = u8Buf[2] << 8 | u8Buf[3];
			acc[2] = u8Buf[4] << 8 | u8Buf[5];
		}

        /// <summary>
        /// Reads raw magnetometer data.
        /// </summary>
        /// <param name="mag">[out] mag the raw magnetometer data for all three axes.</param>
        /// <returns>the flag indicating magnetometer overflow.</returns>
        private bool ReadRawMag(int[] mag)
        {
            byte[] u8Buf = new byte[Conversion.MAG_DATA_LEN];
			SetBank(IcmBank.BANK_0);
			BusComms.ReadRegister(Registers.ADD_EXT_SENS_DATA_00,  u8Buf);
			mag[0] = u8Buf[1] << 8 | u8Buf[0];
			mag[1] = -(u8Buf[3] << 8 | u8Buf[2]);
			mag[2] = -(u8Buf[5] << 8 | u8Buf[4]);
			/* Check if there was an overflow. */
			return (u8Buf[7] & 0x08) == 0;
		}

        /// <summary>
        /// Reads raw temperature data.
        /// </summary>
        /// <returns>the raw temperature data.</returns>
        private int ReadRawTemp()
		{
			byte[] u8Buf = [0, 0];
			SetBank(IcmBank.BANK_0);
			BusComms.ReadRegister(Registers.ADD_TEMP_OUT_H, u8Buf);
			return u8Buf[0] << 8 | u8Buf[1];
		}

        /// <summary>
        /// Reads all data from the device.
        /// </summary>
        /// <param name="gyro">[out] gyro the raw gyro data for all three axes.</param>
        /// <param name="acc">[out] acc the raw acceleration data for all three axes.</param>
        /// <param name="mag">[out] mag the raw magnetometer data for all three axes.</param>
        /// <param name="temp">[out] temp the raw temperature data.</param>
        /// <returns>the flag indicating magnetometer overflow.</returns>
        private bool ReadAllRawDAta(short[] gyro, short[] acc, short[] mag, ref short temp)
        {
            SetBank(IcmBank.BANK_0);
			BusComms.ReadRegister(Registers.ADD_ACCEL_XOUT_H, ReadBuffer.Span);

			/* Parse accelerometer data */
			acc[0] = (short)(ReadBuffer.Span[0] << 8 | ReadBuffer.Span[1]);
			acc[1] = (short)(ReadBuffer.Span[2] << 8 | ReadBuffer.Span[3]);
			acc[2] = (short)(ReadBuffer.Span[4] << 8 | ReadBuffer.Span[5]);

            /* Parse gyroscope data */
            gyro[0] = (short)(ReadBuffer.Span[6] << 8 | ReadBuffer.Span[7]);
			gyro[1] = (short)(ReadBuffer.Span[8] << 8 | ReadBuffer.Span[9]);
			gyro[2] = (short)(ReadBuffer.Span[10] << 8 | ReadBuffer.Span[11]);

            /* Parse temperature data */
            temp = (short)(ReadBuffer.Span[12] << 8 | ReadBuffer.Span[13]);

			/* Parse magnetic data */
			mag[0] = (short)(ReadBuffer.Span[15] << 8 | ReadBuffer.Span[14]);
			mag[1] = (short)-(ReadBuffer.Span[17] << 8 | ReadBuffer.Span[16]);
			mag[2] = (short)-(ReadBuffer.Span[19] << 8 | ReadBuffer.Span[18]);

            return (ReadBuffer.Span[21] & 0x08) == 0;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if magnetometer was detected.</returns>
        private bool CheckMag()
		{
			byte[] u8Ret = [0, 0];
			MagI2CRead(Registers.ADD_MAG_WIA1, 2, u8Ret);
#if LOG
		if ((u8Ret[0] != DefineRegisters.VAL_MAG_WIA1) || (u8Ret[1] != DefineRegisters.VAL_MAG_WIA2))
		{
			printf("Failed to obtain magnetometer address. Expected: %02x%02x, but received: %02x%02x Current bank: %d \n", DefineRegisters.VAL_MAG_WIA1, DefineRegisters.VAL_MAG_WIA2, u8Ret[0], u8Ret[1], mCurrentBank);
		}
#endif // LOG
			return u8Ret[0] == Registers.VAL_MAG_WIA1 && u8Ret[1] == Registers.VAL_MAG_WIA2;
		}
        /// <summary>
        /// Reads data from AK09916 sensor.
        /// </summary>
        /// <param name="regAddr">the address of the register from which the read should begin.</param>
        /// <param name="length">the number of bytes to read (the length of @p data).</param>
        /// <param name="data">[out] data preallocated buffer for read data.</param>
        private void MagI2CRead(byte regAddr, byte length, byte[] data)
        {
            SetBank(IcmBank.BANK_3);
			BusComms.WriteRegister(Registers.ADD_I2C_SLV0_ADDR, Constants.I2C_ADD_ICM20948_AK09916 | Constants.I2C_ADD_ICM20948_AK09916_READ);
			BusComms.WriteRegister(Registers.ADD_I2C_SLV0_REG, regAddr);
			BusComms.WriteRegister(Registers.ADD_I2C_SLV0_CTRL, (byte)(Registers.VAL_BIT_SLV0_EN | length));

			SetBank(IcmBank.BANK_0);
			BusComms.ReadRegister(Registers.ADD_EXT_SENS_DATA_00, data);
		}

        /// <summary>
        /// Writes a single byte data to AK09916 sensor.
        /// </summary>
        /// <param name="regAddr">the address of the register from which the read should begin.</param>
        /// <param name="value">a new data to upload to the device.</param>
        /// <returns></returns>
        private async Task MagI2CWrite(byte regAddr, byte value)
        {
            byte[] u8Temp = [0];
			SetBank(IcmBank.BANK_3);
			BusComms.WriteRegister(Registers.ADD_I2C_SLV0_ADDR, Constants.I2C_ADD_ICM20948_AK09916 | Constants.I2C_ADD_ICM20948_AK09916_WRITE);
			BusComms.WriteRegister(Registers.ADD_I2C_SLV0_REG, regAddr);
			BusComms.WriteRegister(Registers.ADD_I2C_SLV0_DO, value);
			BusComms.WriteRegister(Registers.ADD_I2C_SLV0_CTRL, Registers.VAL_BIT_SLV0_EN | 1);

			await Task.Delay(TimeSpan.FromMilliseconds(100));
			MagI2CRead(regAddr, 1, u8Temp);
#if LOG
		if (value != u8Temp)
		{
			printf("Failed to write %d to magnetometer address: %d. Data received: %d. Current bank: %d \n", value, regAddr, u8Temp, mCurrentBank);
		}
#endif // LOG
		}



        async Task<AngularVelocity3D> ISensor<AngularVelocity3D>.Read()
            => (await Read()).AngularVelocity3D!.Value;

        async Task<Acceleration3D> ISensor<Acceleration3D>.Read()
            => (await Read()).Acceleration3D!.Value;

        async Task<Units.Temperature> ISensor<Units.Temperature>.Read()
            => (await Read()).Temperature!.Value;

        /// <summary>
        /// Reads data from the sensor
        /// </summary>
        /// <returns>The latest sensor reading</returns>
        protected override Task<
            (Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D,
            MagneticField3D? MagneticField3D, Quaternion? QuaternionOrientation,
            EulerAngles? EulerOrientation, Units.Temperature? Temperature)> ReadSensor()
        {
            (Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D,
            MagneticField3D? MagneticField3D, Quaternion? QuaternionOrientation,
            EulerAngles? EulerOrientation, Units.Temperature? Temperature) conditions;

            var imuData = ImuDataGet();
            conditions.Acceleration3D = imuData is null ? null : new Acceleration3D(
                imuData.Acc[0],
                imuData.Acc[1],
                imuData.Acc[2],
                Acceleration.UnitType.Gravity);
            conditions.AngularVelocity3D = imuData is null ? null : new AngularVelocity3D(
                imuData.Gyro[0],
                imuData.Gyro[1],
                imuData.Gyro[2],
                AngularVelocity.UnitType.RadiansPerSecond);
            conditions.MagneticField3D = imuData is null ? null : new MagneticField3D(
                imuData.Mag[0],
                imuData.Mag[1],
                imuData.Mag[2],
                MagneticField.UnitType.Tesla);
            conditions.QuaternionOrientation = imuData is null ? null : new Quaternion(
                imuData.Quat[0],
                imuData.Quat[1],
                imuData.Quat[2],
                imuData.Quat[3]
                );
            conditions.EulerOrientation = imuData is null ? null : new EulerAngles(
                    new Angle(imuData.Angles[0]),
                    new Angle(imuData.Angles[1]),
                    new Angle(imuData.Angles[2])
                    );
            conditions.Temperature = imuData is null ? null : new Units.Temperature(
                imuData.Temp,
                Units.Temperature.UnitType.Celsius);
            return Task.FromResult(conditions);
        }


        /// <summary>
        /// Raise events for subscribers and notify of value changes
        /// </summary>
        /// <param name="changeResult">The updated sensor data</param>
        protected override void RaiseEventsAndNotify(IChangeResult<
            (Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D,
            MagneticField3D? MagneticField3D, Quaternion? QuaternionOrientation,
            EulerAngles? EulerOrientation, Units.Temperature? Temperature)> changeResult)
        {
            if (changeResult.New.Acceleration3D is { } accel)
            {
                _accelerationHandlers?.Invoke(this, new ChangeResult<Acceleration3D>(accel, changeResult.Old?.Acceleration3D));
            }
            if (changeResult.New.AngularVelocity3D is { } angular)
            {
                _velocityHandlers?.Invoke(this, new ChangeResult<AngularVelocity3D>(angular, changeResult.Old?.AngularVelocity3D));
            }
            if (changeResult.New.MagneticField3D is { } magnetic)
            {
                MagneticField3DUpdated?.Invoke(this, new ChangeResult<MagneticField3D>(magnetic, changeResult.Old?.MagneticField3D));
            }
            if (changeResult.New.QuaternionOrientation is { } quaternion)
            {
                QuaternionOrientationUpdated?.Invoke(this, new ChangeResult<Quaternion>(quaternion, changeResult.Old?.QuaternionOrientation));
            }
            if (changeResult.New.EulerOrientation is { } euler)
            {
                EulerOrientationUpdated?.Invoke(this, new ChangeResult<EulerAngles>(euler, changeResult.Old?.EulerOrientation));
            }
            if (changeResult.New.Temperature is { } temp)
            {
                _temperatureHandlers?.Invoke(this, new ChangeResult<Units.Temperature>(temp, changeResult.Old?.Temperature));
            }
            base.RaiseEventsAndNotify(changeResult);
        }
        /// <summary>
        /// Convert a section of the sensor data into a tuple
        /// </summary>
        /// <param name="start">Start of the data in the sensorReadings member variable</param>
        /// <param name="divisor">Divisor</param>
        protected (double X, double Y, double Z) GetReadings(int start, double divisor)
        {
            var x = (short)((ReadBuffer.Span[start + 1] << 8) | ReadBuffer.Span[start]);
            var y = (short)((ReadBuffer.Span[start + 3] << 8) | ReadBuffer.Span[start + 2]);
            var z = (short)((ReadBuffer.Span[start + 5] << 8) | ReadBuffer.Span[start + 4]);

            return (x / divisor, y / divisor, z / divisor);
        }

        /// <summary>
        /// Convert the sensor readings into an orientation in Euler angles
        /// </summary>
        /// <param name="start">First of the sensor readings to convert</param>
        /// <param name="divisor">Divisor to apply to the sensor data</param>
        /// <returns>EulerAngles object containing the orientation information</returns>
        protected EulerAngles ConvertReadingToEulerAngles(int start, double divisor)
        {
            var x = (short)((ReadBuffer.Span[start + 1] << 8) | ReadBuffer.Span[start]);
            var y = (short)((ReadBuffer.Span[start + 3] << 8) | ReadBuffer.Span[start + 2]);
            var z = (short)((ReadBuffer.Span[start + 5] << 8) | ReadBuffer.Span[start + 4]);
            return new EulerAngles(new Angle(x / divisor, Angle.UnitType.Radians), new Angle(y / divisor, Angle.UnitType.Radians), new Angle(z / divisor, Angle.UnitType.Radians));
        }
    }

}