using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestOrin.Icm20948.Enums;

namespace TestOrin.Icm20948.Configs
{
    public class GyroConfig
    {
        /** The flag that indicates if the gyroscope should be enabled. */
        public bool MustBeEnabled = true;
        /** The range in which gyroscope should operate. */
        public GyroRange Range { get; set; } = GyroRange.GYRO_RANGE_250DPS;
        /** The bandwidth of digital low-pass filter. */
        public GyroRangeDlpfBandWidth DlpfBandWidth { get; set; } = GyroRangeDlpfBandWidth.GYRO_DLPF_BANDWIDTH_6HZ;
        /** The averaging of gyroscope output. DLPF needs to be enabled for this to be used. SampleRateDivisor may be adjusted. */
        public GyroAveraging Averaging { get; set; } = GyroAveraging.GYRO_AVERAGING_4X;
        /** Sample rate divisor. Sample rate is calculated as 1.128kHz / (1 + SampleRateDivisor) */
        public byte SampleRateDivisor { get; set; } = 4;
    }
}
