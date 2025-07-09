using Alchiweb.IoT.Icm20948.Enums;

namespace Alchiweb.IoT.Icm20948.Configs
{
    public class AccConfig
    {
        /** The flag that indicates if the accelerometer should be enabled. */
        public bool MustBeEnabled { get; set; } = true;
        /** The range in which accelerometer should operate. */
        public AccRange Range { get; set; } = AccRange.ACC_RANGE_2G;
        /** The bandwidth of digital low-pass filter. */
        public AccRangeDlpfBandWidth DlpfBandWidth { get; set; } = AccRangeDlpfBandWidth.ACC_DLPF_BANDWIDTH_6HZ;
        /** The averaging of accelerometer output. When set DlpfBandWidth will be set to 7 and SampleRateDivisor may be adjusted.*/
        public AccAveraging Averaging { get; set; } = AccAveraging.ACC_AVERAGING_4X;
        /** Sample rate divisor. Sample rate is calculated as 1.128kHz / (1 + SampleRateDivisor) */
        public byte SampleRateDivisor { get; set; } = 4;
    }
}
