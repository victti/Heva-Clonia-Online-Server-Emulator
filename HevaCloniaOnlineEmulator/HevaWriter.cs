using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vUtils.Network;

namespace HCOE
{
    internal class HevaWriter : BufferWriter
    {
        public HevaWriter()
            :base()
        {

        }

        public HevaWriter(Stream stream)
        {
            memoryStream = (MemoryStream)stream;
        }
    }
}
