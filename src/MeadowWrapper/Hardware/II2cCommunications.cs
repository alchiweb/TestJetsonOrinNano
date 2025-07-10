using System.Device.I2c;

namespace Meadow.Hardware
{
    public interface II2cCommunications
    {
        byte Address { get; }

        I2cBus? Bus { get; }
        I2cDevice? Device { get; }

    //
    // Résumé :
    //     Reads data from the peripheral.
    //
    // Paramètres :
    //   readBuffer:
    //     The buffer to read from the peripheral into.
    //
    // Remarques :
    //     The number of bytes to be read is determined by the length of readBuffer.
    void Read(Span<byte> readBuffer);

    //
    // Résumé :
    //     Reads data from the peripheral starting at the specified address.
    //
    // Paramètres :
    //   address:
    //
    //   readBuffer:
    //
    // Remarques :
    //     The number of bytes to be read is determined by the length of readBuffer.
    void ReadRegister(byte address, Span<byte> readBuffer);

    //
    // Résumé :
    //     Read a register from the peripheral.
    //
    // Paramètres :
    //   address:
    //     Address of the register to read.
    byte ReadRegister(byte address);

    //
    // Résumé :
    //     Read an unsigned short from a register.
    //
    // Paramètres :
    //   address:
    //     Register address of the low byte (the high byte will follow).
    //
    //   order:
    //     Order of the bytes in the register (little endian is the default).
    //
    // Retourne :
    //     Value read from the register.
    ushort ReadRegisterAsUShort(byte address, ByteOrder order = ByteOrder.LittleEndian);

    //
    // Résumé :
    //     Write a single byte to the peripheral.
    //
    // Paramètres :
    //   value:
    //     Value to be written (8-bits).
    void Write(byte value);

    //
    // Résumé :
    //     Write an array of bytes to the peripheral.
    //
    // Paramètres :
    //   writeBuffer:
    //     A buffer of byte values to be written.
    //
    // Remarques :
    //     The number of bytes to be written is determined by the length of writeBuffer.
    void Write(Span<byte> writeBuffer);

    //
    // Résumé :
    //     Write data to a register in the peripheral.
    //
    // Paramètres :
    //   address:
    //     Address of the register to write to.
    //
    //   value:
    //     Data to write into the register.
    void WriteRegister(byte address, byte value);

    //
    // Résumé :
    //     Write data to a register in the peripheral.
    //
    // Paramètres :
    //   address:
    //     Address of the register to write to.
    //
    //   writeBuffer:
    //     A buffer of byte values to be written.
    //
    //   order:
    //     Indicate if the data should be written as big or little endian.
    //
    // Remarques :
    //     The number of bytes to be written is determined by the length of writeBuffer.
    void WriteRegister(byte address, Span<byte> writeBuffer, ByteOrder order = ByteOrder.LittleEndian);
/*
    //
    // Résumé :
    //     Write an unsigned short to the peripheral.
    //
    // Paramètres :
    //   address:
    //     Address to write the first byte to.
    //
    //   value:
    //     Value to be written (16-bits).
    //
    //   order:
    //     Indicate if the data should be written as big or little endian.
    void WriteRegister(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian);

    //
    // Résumé :
    //     Write an unsigned integer to the peripheral.
    //
    // Paramètres :
    //   address:
    //     Address to write the first byte to.
    //
    //   value:
    //     Value to be written.
    //
    //   order:
    //     Indicate if the data should be written as big or little endian.
    void WriteRegister(byte address, uint value, ByteOrder order = ByteOrder.LittleEndian);

    //
    // Résumé :
    //     Write an unsigned long to the peripheral.
    //
    // Paramètres :
    //   address:
    //     Address to write the first byte to.
    //
    //   value:
    //     Value to be written.
    //
    //   order:
    //     Indicate if the data should be written as big or little endian.
    void WriteRegister(byte address, ulong value, ByteOrder order = ByteOrder.LittleEndian);

    //
    // Résumé :
    //     Write data to followed by read data from the peripheral.
    //
    // Paramètres :
    //   writeBuffer:
    //     Data to write
    //
    //   readBuffer:
    //     Buffer where read data will be written. Number of bytes read is determined by
    //     buffer size.
    //
    //   duplex:
    //     Whether the communication will happen in a half-duplex or full-duplex fashion.
    void Exchange(Span<byte> writeBuffer, Span<byte> readBuffer, DuplexType duplex = DuplexType.Half);
*/

    }
}
