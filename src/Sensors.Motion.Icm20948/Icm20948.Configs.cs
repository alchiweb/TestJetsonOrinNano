using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Meadow.Foundation.Sensors.Motion
{
    public partial class Icm20948
    {
        /// <summary>
        /// 
        /// </summary>
        public class AccConfig
        {
            /// <summary>
            /// The flag that indicates if the accelerometer should be enabled.
            /// </summary>
            public bool MustBeEnabled { get; set; } = true;
            /// <summary>
            /// The range in which accelerometer should operate.
            /// </summary>
            public AccRange Range { get; set; } = AccRange.ACC_RANGE_2G;
            /// <summary>
            /// The bandwidth of digital low-pass filter.
            /// </summary>
            public AccRangeDlpfBandWidth DlpfBandWidth { get; set; } = AccRangeDlpfBandWidth.ACC_DLPF_BANDWIDTH_6HZ;
            /// <summary>
            /// The averaging of accelerometer output. When set DlpfBandWidth will be set to 7 and SampleRateDivisor may be adjusted.
            /// </summary>
            public AccAveraging Averaging { get; set; } = AccAveraging.ACC_AVERAGING_4X;
            /// <summary>
            /// Sample rate divisor. Sample rate is calculated as 1.128kHz / (1 + SampleRateDivisor)
            /// </summary>
            public byte SampleRateDivisor { get; set; } = 4;
        }
        /// <summary>
        /// 
        /// </summary>
        public class GyroConfig
        {
            /// <summary>
            /// 
            /// </summary>
            /** The flag that indicates if the gyroscope should be enabled. */
            public bool MustBeEnabled = true;
            /// <summary>
            /// The range in which gyroscope should operate.
            /// </summary>
            public GyroRange Range { get; set; } = GyroRange.GYRO_RANGE_250DPS;
            /// <summary>
            /// The bandwidth of digital low-pass filter.
            /// </summary>
            public GyroRangeDlpfBandWidth DlpfBandWidth { get; set; } = GyroRangeDlpfBandWidth.GYRO_DLPF_BANDWIDTH_6HZ;
            /// <summary>
            /// The averaging of gyroscope output. DLPF needs to be enabled for this to be used. SampleRateDivisor may be adjusted.
            /// </summary>
            public GyroAveraging Averaging { get; set; } = GyroAveraging.GYRO_AVERAGING_4X;
            /// <summary>
            /// Sample rate divisor. Sample rate is calculated as 1.128kHz / (1 + SampleRateDivisor)
            /// </summary>
            public byte SampleRateDivisor { get; set; } = 4;
        }
        /// <summary>
        /// 
        /// </summary>
        public class IcmConfig
        {
            /// <summary>
            /// The I2C device address.
            /// </summary>
            public string Device { get; set; } = "/dev/i2c-7";
            /// <summary>
            /// 
            /// </summary>
            /** The gyroscope configuration data. */
            public GyroConfig Gyro { get; } = new GyroConfig();
            /// <summary>
            /// The accelerometer configuration data.
            /// </summary>
            public AccConfig Acc { get; } = new AccConfig();
            /// <summary>
            /// The temperature sensor configuration data.
            /// </summary>
            public TempConfig Temp { get; } = new TempConfig();
            /// <summary>
            /// The flag that indicates if the compass should be enabled.
            /// </summary>
            public bool MagMustBeEnabled { get; set; } = true;
            /// <summary>
            /// The AHRS algorithm to use for sensors fusion.
            /// </summary>
            public AhrsAlgorithm Ahrs { get; set; } = AhrsAlgorithm.MADGWICK;
            /// <summary>
            /// The frame rate in which data will be processed for AHRS.
            /// </summary>
            public float Framerate { get; set; } = 100.0f;
        }
        /// <summary>
        /// 
        /// </summary>
        public class TempConfig
        {
            /// <summary>
            /// The flag that indicates if the temperature sensor should be enabled.
            /// </summary>
            public bool MustBeEnabled { get; set; } = true;
            /// <summary>
            /// The bandwidth of digital low-pass filter.
            /// </summary>
            public TempDlpfBandWidth DlpfBandWidth { get; set; } = TempDlpfBandWidth.TEMP_DLPF_BANDWIDTH_9HZ;
        }
    }
}
