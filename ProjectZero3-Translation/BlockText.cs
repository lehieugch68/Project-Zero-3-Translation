using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectZero3_Translation
{
    public class BlockText
    {
        public int Index { get; set; }
        public long PointerOffset { get; set; }
        public long NewPointerOffset { get; set; }
        public List<string> Strings { get; set; }
        public BlockText(int index, long pointer_offset)
        {
            this.Index = index;
            this.PointerOffset = pointer_offset;
        }
    }
}
