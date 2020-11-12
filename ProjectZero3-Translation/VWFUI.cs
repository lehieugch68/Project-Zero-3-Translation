using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ProjectZero3_Translation
{
    public partial class VWFUI : Form
    {
        public List<FontCharacter> _FontCharacter;
        
        public VWFUI()
        {
            InitializeComponent();
        }

        private void VWFUI_FromClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void VWFUI_Load(object sender, EventArgs e)
        {
            
        }
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            FontCharacter font_character = _FontCharacter.Where(entry => entry.Index == e.RowIndex).FirstOrDefault();
            try
            {
                font_character.Width = int.Parse($"{dataGridView.Rows[e.RowIndex].Cells[3].Value}");
                font_character.Replace = $"{dataGridView.Rows[e.RowIndex].Cells[5].Value}";
            }
            catch (Exception err)
            {
                MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!GlobalVariable._JsonConfig.ContainsKey("GameLocation") || GlobalVariable._IsBusy)
            {
                string msg = GlobalVariable._IsBusy ? "Another task is already in progress." : "Please select your game directory.";
                MessageBox.Show(msg, AppConfig._MessageBoxTitle);
            }
            else
            {
                GlobalVariable._IsBusy = true;
                Task.Run(() =>
                {
                    try
                    {
                        ProjectZero3.VWFRepack(GlobalVariable._JsonConfig["GameLocation"], _FontCharacter, this.progressBarFont);
                    }
                    catch (Exception err)
                    {
                        GlobalVariable._IsBusy = false;
                        MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
                    }
                }).GetAwaiter().OnCompleted(() => { GlobalVariable._IsBusy = false; });
            }
        }
    }
}
