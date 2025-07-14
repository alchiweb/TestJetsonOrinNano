using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace Meadow.Foundation.Sensors.Motion
{
    public partial class Icm20948
    {
        /// <summary>
        /// Valid I2C addresses for the sensor
        /// </summary>
        public enum Addresses : byte
        {
            /// <summary>
            /// Bus address 0x68
            /// </summary>
            Address_0x68 = 0x68,
            /// <summary>
            /// Bus address 0x69
            /// </summary>
            Address_0x69 = 0x69,
            /// <summary>
            /// Bus address 0x68
            /// </summary>
            Default = Address_0x68
        }

        /// <summary>
        /// 
        /// </summary>
        public enum AccAveraging
        {
            /// <summary>
            /// 
            /// </summary>
            ACC_AVERAGING_NONE = -1,
            /// <summary>
            /// 
            /// </summary>
            ACC_AVERAGING_4X = 0,
            /// <summary>
            /// 
            /// </summary>
            ACC_AVERAGING_8X = 1,
            /// <summary>
            /// 
            /// </summary>
            ACC_AVERAGING_16X = 2,
            /// <summary>
            /// 
            /// </summary>
            ACC_AVERAGING_32X = 3
        }
        /// <summary>
        /// 
        /// </summary>
        public enum AccRange
        {
            /// <summary>
            /// 
            /// </summary>
            ACC_RANGE_2G = 0,
            /// <summary>
            /// 
            /// </summary>
            ACC_RANGE_4G = 1,
            /// <summary>
            /// 
            /// </summary>
            ACC_RANGE_8G = 2,
            /// <summary>
            /// 
            /// </summary>
            ACC_RANGE_16G = 3
        }
        /// <summary>
        /// 
        /// </summary>
        public enum AccRangeDlpfBandWidth
        {
            /// <summary>
            /// 
            /// </summary>
            ACC_DLPF_NONE = 0b00000000,
            /// <summary>
            /// 
            /// </summary>
            ACC_DLPF_BANDWIDTH_246HZ = 0b00001001,
            /// <summary>
            /// 
            /// </summary>
            /// <summary>
            /// 
            /// </summary>
            /// <summary>
            /// 
            /// </summary>
            /// <summary>
            /// 
            /// </summary>
            ACC_DLPF_BANDWIDTH_111HZ = 0b00010001,
            /// <summary>
            /// 
            /// </summary>
            ACC_DLPF_BANDWIDTH_50HZ = 0b00011001,
            /// <summary>
            /// 
            /// </summary>
            ACC_DLPF_BANDWIDTH_24HZ = 0b00100001,
            /// <summary>
            /// 
            /// </summary>
            ACC_DLPF_BANDWIDTH_12HZ = 0b00101001,
            /// <summary>
            /// 
            /// </summary>
            ACC_DLPF_BANDWIDTH_6HZ = 0b00110001,
            /// <summary>
            /// 
            /// </summary>
            ACC_DLPF_BANDWIDTH_473HZ = 0b00111001
        }
        /// <summary>
        /// 
        /// </summary>
        public enum AhrsAlgorithm
        {
            /// <summary>
            /// 
            /// </summary>
            NONE = 0,
            /// <summary>
            /// 
            /// </summary>
            SIMPLE = 1,
            /// <summary>
            /// 
            /// </summary>
            MADGWICK = 2
        }
        /// <summary>
        /// 
        /// </summary>
        public enum GyroAveraging
        {
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_NONE = -1,
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_1X = 0,
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_2X = 1,
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_4X = 2,
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_8X = 3,
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_16X = 4,
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_32X = 5,
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_64X = 6,
            /// <summary>
            /// 
            /// </summary>
            GYRO_AVERAGING_128X = 7
        }
        /// <summary>
        /// 
        /// </summary>
        public enum GyroRange
        {
            /// <summary>
            /// 
            /// </summary>
            GYRO_RANGE_250DPS = 0,
            /// <summary>
            /// 
            /// </summary>
            GYRO_RANGE_500DPS = 1,
            /// <summary>
            /// 
            /// </summary>
            GYRO_RANGE_1000DPS = 2,
            /// <summary>
            /// 
            /// </summary>
            GYRO_RANGE_2000DPS = 3
        }
        /// <summary>
        /// 
        /// </summary>
        public enum GyroRangeDlpfBandWidth
        {
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_NONE = 0b00000000,
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_BANDWIDTH_197HZ = 0b00000001,
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_BANDWIDTH_152HZ = 0b00001001,
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_BANDWIDTH_120HZ = 0b00010001,
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_BANDWIDTH_51HZ = 0b00011001,
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_BANDWIDTH_24HZ = 0b00100001,
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_BANDWIDTH_12HZ = 0b00101001,
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_BANDWIDTH_6HZ = 0b00110001,
            /// <summary>
            /// 
            /// </summary>
            GYRO_DLPF_BANDWIDTH_361HZ = 0b00111001
        }
        /// <summary>
        /// 
        /// </summary>
        public enum IcmBank
        {
            /// <summary>
            /// 
            /// </summary>
            BANK_0 = 0x00,
            /// <summary>
            /// 
            /// </summary>
            BANK_1 = 0x10,
            /// <summary>
            /// 
            /// </summary>
            BANK_2 = 0x20,
            /// <summary>
            /// 
            /// </summary>
            BANK_3 = 0x30,
            /// <summary>
            /// 
            /// </summary>
            BANK_UNDEFINED = 0xFF
        }
        /// <summary>
        /// 
        /// </summary>
        public enum TempDlpfBandWidth
        {
            /// <summary>
            /// 
            /// </summary>
            TEMP_DLPF_NONE = 0,
            /// <summary>
            /// 
            /// </summary>
            TEMP_DLPF_BANDWIDTH_218HZ = 1,
            /// <summary>
            /// 
            /// </summary>
            TEMP_DLPF_BANDWIDTH_124HZ = 2,
            /// <summary>
            /// 
            /// </summary>
            TEMP_DLPF_BANDWIDTH_66HZ = 3,
            /// <summary>
            /// 
            /// </summary>
            TEMP_DLPF_BANDWIDTH_34HZ = 4,
            /// <summary>
            /// 
            /// </summary>
            TEMP_DLPF_BANDWIDTH_17HZ = 5,
            /// <summary>
            /// 
            /// </summary>
            TEMP_DLPF_BANDWIDTH_9HZ = 6
        }
    }
}
