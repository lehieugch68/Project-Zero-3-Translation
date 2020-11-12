using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectZero3_Translation
{
    
    public class VWF
    {
        private static VWF _Instance;
        private VWFUI _VWF;
        private bool _IsCreated = false;
        protected VWF()
        {
            _VWF = new VWFUI();
            _VWF.dataGridView.DefaultCellStyle.Font = new Font("Consolas", 8.5F);
        }
        public static VWF Instance()
        {
            if (_Instance == null)
            {
                _Instance = new VWF();
            }
            return _Instance;
        }

        public void TransferData(List<FontCharacter> font_characters)
        {
            _VWF._FontCharacter = font_characters;
            _VWF.dataGridView.DataSource = null;
            _VWF.dataGridView.Rows.Clear();
            foreach (FontCharacter entry in font_characters)
            {
                _VWF.dataGridView.Rows.Add(
                    entry.Index,
                    entry.Hex,
                    entry.Character,
                    $"{entry.Width}",
                    "24",
                    entry.Replace == null ? "" : entry.Replace);
            }
        }
        public void Show()
        {
            if (!_IsCreated) _IsCreated = true;
            _VWF.Show();
        }

        public bool isCreated()
        {
            return _IsCreated;
        }
    }
}
