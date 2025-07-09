using Alchiweb.IoT.Icm20948.Enums;

namespace Alchiweb.IoT.Icm20948.Configs
{
    public class TempConfig
    {
        /** The flag that indicates if the temperature sensor should be enabled. */
        public bool MustBeEnabled { get; set; } = true;
        /** The bandwidth of digital low-pass filter. */
        public TempDlpfBandWidth DlpfBandWidth { get; set; } = TempDlpfBandWidth.TEMP_DLPF_BANDWIDTH_9HZ;
    }
}
