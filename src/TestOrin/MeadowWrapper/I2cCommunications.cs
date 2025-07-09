using System.Device.I2c;
namespace Meadow.Hardware
{
    public class I2cCommunications : II2cCommunications, IDisposable
    {
        public byte Address => (byte?)(Device?.ConnectionSettings?.DeviceAddress) ?? 0;

        // To detect redundant calls
        private bool _disposedValue;

        public I2cBus? Bus { get; private set; }
        public I2cDevice? Device { get; private set; }

        public I2cCommunications(int busId, int deviceAddress)
        {
            Bus = I2cBus.Create(busId);
            Device = Bus.CreateDevice(deviceAddress);
        }
        public byte ReadRegister(byte address)
        {
            Span<byte> tabBytes = [0];
            Device?.WriteRead([address], tabBytes);
            return tabBytes[0];
        }
        public void ReadRegister(byte address, Span<byte> data)
        {
            Device?.WriteRead([address], data);
        }
        public void WriteRegister(byte address, byte data)
        {
            Device?.Write([address, data]);
        }
        public void WriteRegister(byte address, Span<byte> writeBuffer, ByteOrder order = ByteOrder.LittleEndian)
        {
            Device?.Write([address, .. writeBuffer]);
        }

        public void Read(Span<byte> readBuffer)
        {
            Device?.Read(readBuffer);
        }

        public ushort ReadRegisterAsUShort(byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            throw new NotImplementedException();
        }

        public void Write(byte value)
        {
            Device?.WriteByte(value);
        }

        public void Write(Span<byte> writeBuffer)
        {
            Device?.Write(writeBuffer);
        }

        // public void WriteRegister(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        // {
        //     throw new NotImplementedException();
        // }

        // public void WriteRegister(byte address, uint value, ByteOrder order = ByteOrder.LittleEndian)
        // {
        //     throw new NotImplementedException();
        // }

        // public void WriteRegister(byte address, ulong value, ByteOrder order = ByteOrder.LittleEndian)
        // {
        //     throw new NotImplementedException();
        // }

        // public void Exchange(Span<byte> writeBuffer, Span<byte> readBuffer, DuplexType duplex = DuplexType.Half)
        // {
        //     throw new NotImplementedException();
        // }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Bus?.RemoveDevice(0x68);
                    Device?.Dispose();
                    Device = null;
                    Bus?.Dispose();
                    Bus = null;
                }
                _disposedValue = true;
            }
        }        
    }
}
