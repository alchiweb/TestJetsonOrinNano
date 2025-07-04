using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestOrin.Icm20948.Enums;

namespace TestOrin.Icm20948.Configs
{
    public class IcmConfig
    {
        /** The I2C device address. */
        public string Device { get; set; } = "/dev/i2c-7";
        /** The gyroscope configuration data. */
        public GyroConfig Gyro { get; } = new GyroConfig();
        /** The accelerometer configuration data. */
        public AccConfig Acc { get; } = new AccConfig();
        /** The temperature sensor configuration data. */
        public TempConfig Temp { get; } = new TempConfig();
        /** The flag that indicates if the compass should be enabled. */
        public bool MagMustBeEnabled { get; set; } = true;
        /** The AHRS algorithm to use for sensors fusion. */
        public AHRS_ALGORITHM Ahrs { get; set; } = AHRS_ALGORITHM.MADGWICK;
        /** The frame rate in which data will be processed for AHRS. */
        public float Framerate { get; set; } = 100.0f;
    }

}
