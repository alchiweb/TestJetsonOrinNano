using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow
{
    //
    // Résumé :
    //     Describes the byte ordering for multi-byte words.
    public enum ByteOrder
    {
        //
        // Résumé :
        //     Little-endian byte ordering, in which bytes are handled from the lowest to the
        //     highest
        LittleEndian,
        //
        // Résumé :
        //     Big-endian byte ordering, in which bytes are handled from the highest to the
        //     lowest
        BigEndian
    }
}
