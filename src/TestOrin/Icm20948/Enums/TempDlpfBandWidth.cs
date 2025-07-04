using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOrin.Icm20948.Enums
{
    public enum TempDlpfBandWidth
    {
        TEMP_DLPF_NONE = 0,
        TEMP_DLPF_BANDWIDTH_218HZ = 1,
        TEMP_DLPF_BANDWIDTH_124HZ = 2,
        TEMP_DLPF_BANDWIDTH_66HZ = 3,
        TEMP_DLPF_BANDWIDTH_34HZ = 4,
        TEMP_DLPF_BANDWIDTH_17HZ = 5,
        TEMP_DLPF_BANDWIDTH_9HZ = 6
    }
}
