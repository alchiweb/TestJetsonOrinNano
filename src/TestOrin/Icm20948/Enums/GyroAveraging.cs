using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOrin.Icm20948.Enums
{
    public enum GyroAveraging
    {
        GYRO_AVERAGING_NONE = -1,
        GYRO_AVERAGING_1X = 0,
        GYRO_AVERAGING_2X = 1,
        GYRO_AVERAGING_4X = 2,
        GYRO_AVERAGING_8X = 3,
        GYRO_AVERAGING_16X = 4,
        GYRO_AVERAGING_32X = 5,
        GYRO_AVERAGING_64X = 6,
        GYRO_AVERAGING_128X = 7
    }
}
