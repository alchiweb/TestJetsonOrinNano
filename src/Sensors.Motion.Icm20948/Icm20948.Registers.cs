namespace Meadow.Foundation.Sensors.Motion
{
	public partial class Icm20948
	{
		/// <summary>
		/// 
		/// </summary>
		public static class Registers
		{
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_WIA = 0x00;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_WIA = 0xEA;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_USER_CTRL = 0x03;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_BIT_DMP_EN = 0x80;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_BIT_FIFO_EN = 0x40;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_BIT_I2C_MST_EN = 0x20;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_BIT_I2C_IF_DIS = 0x10;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_BIT_DMP_RST = 0x08;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_BIT_DIAMOND_DMP_RST = 0x04;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_PWR_MIGMT_1 = 0x06;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_ALL_RGE_RESET = 0x80;
            /// <summary>
            /// Non low-power mode
            /// </summary>
			public const byte VAL_RUN_MODE = 0x01;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_LP_CONFIG = 0x05;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_PWR_MGMT_1 = 0x06;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_PWR_MGMT_2 = 0x07;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_SENSORS_ON = 0x00;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_DISABLE_GYRO = 0x07;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_DISABLE_ACC = 0x38;


            /// <summary>
            /// Register containing the first byte of the sensor readings.
            /// </summary>
            /// <remarks>
            /// This is used in the calculation of the various sensor readings
            /// in the _sensorReadings member.
            /// </remarks>
            public static readonly byte StartOfSensorData = 0x2D;

            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_XOUT_H = 0x2D;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_XOUT_L = 0x2E;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_YOUT_H = 0x2F;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_YOUT_L = 0x30;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_ZOUT_H = 0x31;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_ZOUT_L = 0x32;

            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_XOUT_H = 0x33;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_XOUT_L = 0x34;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_YOUT_H = 0x35;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_YOUT_L = 0x36;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_ZOUT_H = 0x37;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_ZOUT_L = 0x38;

            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_TEMP_OUT_H = 0x39;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_TEMP_OUT_L = 0x3A;

            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_00 = 0x3B;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_01 = 0x3C;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_02 = 0x3D;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_03 = 0x3E;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_04 = 0x3F;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_05 = 0x40;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_06 = 0x41;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_07 = 0x42;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_EXT_SENS_DATA_08 = 0x43;

            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_FIFO_EN_1 = 0x66;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_FIFO_EN_2 = 0x67;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_FIFO_RST = 0x68;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_FIFO_MODE = 0x69;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_FIFO_COUNTH = 0x70;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_FIFO_COUNTL = 0x71;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_FIFO_R_W = 0x72;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_REG_BANK_SEL = 0x7F;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_REG_BANK_0 = 0x00;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_REG_BANK_1 = 0x10;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_REG_BANK_2 = 0x20;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_REG_BANK_3 = 0x30;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_SMPLRT_DIV = 0x00;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_CONFIG_1 = 0x01;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_GYRO_CONFIG_2 = 0x02;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_XG_OFFS_USRH = 0x03;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_XG_OFFS_USRL = 0x04;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_YG_OFFS_USRH = 0x05;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_YG_OFFS_USRL = 0x06;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ZG_OFFS_USRH = 0x07;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ZG_OFFS_USRL = 0x08;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_XA_OFFS_H = 0x14;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_XA_OFFS_L = 0x15;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_YA_OFFS_H = 0x17;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_YA_OFFS_L = 0x18;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ZA_OFFS_H = 0x1A;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ZA_OFFS_L = 0x1B;
            /// <summary>
            /// bit[5:3]
            /// </summary>
			public const byte VAL_BIT_GYRO_DLPCFG_2 = 0x10;
            /// <summary>
            /// bit[5:3]
            /// </summary>
			public const byte VAL_BIT_GYRO_DLPCFG_4 = 0x20;
            /// <summary>
            /// bit[5:3]
            /// </summary>
			public const byte VAL_BIT_GYRO_DLPCFG_6 = 0x30;
            /// <summary>
            /// bit[2:1]
            /// </summary>
			public const byte VAL_BIT_GYRO_FS_250DPS = 0x00;
            /// <summary>
            /// bit[2:1]
            /// </summary>
			public const byte VAL_BIT_GYRO_FS_500DPS = 0x02;
            /// <summary>
            /// bit[2:1]
            /// </summary>
			public const byte VAL_BIT_GYRO_FS_1000DPS = 0x04;
            /// <summary>
            /// bit[2:1]
            /// </summary>
			public const byte VAL_BIT_GYRO_FS_2000DPS = 0x06;
            /// <summary>
            /// bit[0]
            /// </summary>
			public const byte VAL_BIT_GYRO_DLPF = 0x01;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_SMPLRT_DIV_1 = 0x10;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_SMPLRT_DIV_2 = 0x11;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_CONFIG = 0x14;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_ACCEL_CONFIG_2 = 0x15;
            /// <summary>
            /// bit[5:3]
            /// </summary>
			public const byte VAL_BIT_ACCEL_DLPCFG_2 = 0x10;
            /// <summary>
            /// bit[5:3]
            /// </summary>
			public const byte VAL_BIT_ACCEL_DLPCFG_4 = 0x20;
            /// <summary>
            /// bit[5:3]
            /// </summary>
			public const byte VAL_BIT_ACCEL_DLPCFG_6 = 0x30;
            /// <summary>
            /// bit[2:1]
            /// </summary>
			public const byte VAL_BIT_ACCEL_FS_2g = 0x00;
            /// <summary>
            /// bit[2:1]
            /// </summary>
			public const byte VAL_BIT_ACCEL_FS_4g = 0x02;
            /// <summary>
            /// bit[2:1]
            /// </summary>
			public const byte VAL_BIT_ACCEL_FS_8g = 0x04;
            /// <summary>
            /// bit[2:1]
            /// </summary>
			public const byte VAL_BIT_ACCEL_FS_16g = 0x06;
            /// <summary>
            /// bit[0]
            /// </summary>
			public const byte VAL_BIT_ACCEL_DLPF = 0x01;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_TEMP_CONFIG = 0x53;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_MST_CTRL = 0x01;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_I2C_MST_CTRL_CLK_400KHZ = 0x07;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_SLV0_ADDR = 0x03;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_SLV0_REG = 0x04;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_SLV0_CTRL = 0x05;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_BIT_SLV0_EN = 0x80;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_BIT_MASK_LEN = 0x07;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_SLV0_DO = 0x06;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_SLV1_ADDR = 0x07;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_SLV1_REG = 0x08;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_SLV1_CTRL = 0x09;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_I2C_SLV1_DO = 0x0A;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_MAG_WIA1 = 0x00;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_WIA1 = 0x48;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_MAG_WIA2 = 0x01;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_WIA2 = 0x09;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_MAG_ST1 = 0x10;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_MAG_DATA = 0x11;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_MAG_CNTL2 = 0x31;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_MODE_PD = 0x00;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_MODE_SM = 0x01;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_MODE_10HZ = 0x02;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_MODE_20HZ = 0x04;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_MODE_50HZ = 0x05;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_MODE_100HZ = 0x08;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_MODE_ST = 0x10;
            /// <summary>
            /// 
            /// </summary>
			public const byte ADD_MAG_CNTL3 = 0x32;
            /// <summary>
            /// 
            /// </summary>
			public const byte VAL_MAG_RESET = 0x01;
        }
	}
}