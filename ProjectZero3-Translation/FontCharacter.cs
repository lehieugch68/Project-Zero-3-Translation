using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectZero3_Translation
{
    public class FontCharacter
    {
        public int Index { get; set; }
        public string Original { get; set; }
        public string Hex { get; set; }
        public string Character { get; set; }
        public int Width { get; set; }
        public string Replace { get; set; }
        public FontCharacter(int index, int width, string hex, string character)
        {
            this.Index = index;
            this.Width = width;
            this.Hex = hex;
            this.Character = character;
        }
    }
}
