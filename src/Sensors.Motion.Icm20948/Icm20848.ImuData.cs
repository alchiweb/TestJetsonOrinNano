namespace Meadow.Foundation.Sensors.Motion
{
    public partial class Icm20948
    {
        /// <summary>
        /// A structure to hold all data from IMU.
        /// </summary>
        public class ImuData
        {
            /// <summary>
            /// The period at which IMU data is being pulled from the sensor.
            /// </summary>
            public float UpdatePeriod { get; set; } = 0.0f;
            /// <summary>
            /// The temperature in Celsius degrees.
            /// </summary>
            public float Temp { get; set; } = 0.0f;
            /// <summary>
            /// Raw angular rate from gyroscope in rad/s.
            /// </summary>
            public float[] Gyro { get; } = [0.0f, 0.0f, 0.0f];
            /// <summary>
            /// Raw data from accelerometer in G's.
            /// </summary>
            public float[] Acc { get; } = [0.0f, 0.0f, 0.0f];
            /// <summary>
            /// Raw data from compass in uT.
            /// </summary>
            public float[] Mag { get; } = [0.0f, 0.0f, 0.0f];
            /// <summary>
            /// Quaternion representation of IMU orientation.
            /// </summary>
            public float[] Quat { get; } = [1.0f, 0.0f, 0.0f, 0.0f];
            /// <summary>
            /// Euler angles representation of IMU orientation in degrees.
            /// </summary>
            public float[] Angles { get; } = [0.0f, 0.0f, 0.0f];

        }
    }
}