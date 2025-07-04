using System.Device.I2c;

namespace TestOrin.Extensions
{
    public static class I2cDeviceExtensions
    {
        public static byte ReadByte(this I2cDevice device, byte address)
        {
            Span<byte> tabBytes = [0];
            device.WriteRead([address], tabBytes);
            return tabBytes[0];
        }
        public static void Read(this I2cDevice device, byte address, Span<byte> data)
        {
            device.WriteRead([address], data);
        }
        public static void WriteByte(this I2cDevice device, byte address, byte data)
        {
            device.Write([address, data]);
        }
    }
}
