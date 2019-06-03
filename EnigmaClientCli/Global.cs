using System;
using System.Collections.Generic;
using System.Text;
using EnigmaLib.API;

namespace EnigmaClientCli
{
    public static class Global
    {
        public static APIBase APIBase { get; set; } = new APIBase();
    }
}
