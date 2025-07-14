namespace Meadow.Foundation.Sensors.Motion
{
	public partial class Icm20948
	{
		/// <summary>
		/// 
		/// </summary>
		public static class Constants
		{
            /// <summary>
            /// 
            /// </summary>
			public const byte I2C_ADD_ICM20948 = 0x68;
            /// <summary>
            /// 
            /// </summary>
			public const byte I2C_ADD_ICM20948_AK09916 = 0x0C;
            /// <summary>
            /// 
            /// </summary>
			public const byte I2C_ADD_ICM20948_AK09916_READ = 0x80;
            /// <summary>
            /// 
            /// </summary>
			public const byte I2C_ADD_ICM20948_AK09916_WRITE = 0x00;

            /// <summary>
            /// 2 * proportional gain (Kp)
            /// </summary>
			public const float beta = 0.1f;
            /// <summary>
            /// proportional gain governs rate of convergence to accelerometer/magnetometer
            /// </summary>
			public const float Kp = 4.50f;
            /// <summary>
            /// integral gain governs rate of convergence of gyroscope biases
            /// </summary>
			public const float Ki = 1.0f;
        }
	}
}