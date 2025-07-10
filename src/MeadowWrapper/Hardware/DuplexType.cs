﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Hardware
{
    // Résumé :
    //     Describes how read and write buffers are sent. Half-duplex is the most common
    //     and means that the write data is clocked (sent) out, and then the read data is
    //     clocked in only after write has finished. Protocols that only have a single data
    //     line (such as I2C) can only support half-duplex. Full-duplex is supported only
    //     on protocols that use two or more data lines (such as SPI) and means that data
    //     is clocked in at the same time as it is clocked out. Full-duplex peripherals
    //     are much less common than half-duplex.
    public enum DuplexType
    {
        //
        // Résumé :
        //     Half-duplex. Write data is sent, and then read data is read in.
        Half,
        //
        // Résumé :
        //     Full-duplex. Write and read data buffers are sent and received at the same time.
        Full
    }
}
