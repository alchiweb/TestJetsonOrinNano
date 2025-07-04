using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOrin.Icm20948.Enums
{
    public enum GyroRangeDlpfBandWidth
    {
        GYRO_DLPF_NONE = 0b00000000,
        GYRO_DLPF_BANDWIDTH_197HZ = 0b00000001,
        GYRO_DLPF_BANDWIDTH_152HZ = 0b00001001,
        GYRO_DLPF_BANDWIDTH_120HZ = 0b00010001,
        GYRO_DLPF_BANDWIDTH_51HZ = 0b00011001,
        GYRO_DLPF_BANDWIDTH_24HZ = 0b00100001,
        GYRO_DLPF_BANDWIDTH_12HZ = 0b00101001,
        GYRO_DLPF_BANDWIDTH_6HZ = 0b00110001,
        GYRO_DLPF_BANDWIDTH_361HZ = 0b00111001
    }
}
