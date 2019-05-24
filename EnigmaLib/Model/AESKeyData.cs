using System;
using System.Collections.Generic;
using System.Text;

namespace EnigmaLib.Model
{
    internal class AESKeyData
    {
        public byte[] IV { get; set; }
        public byte[] Key { get; set; }
    }
}
