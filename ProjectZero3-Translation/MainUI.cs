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
using System.Web.Script.Serialization;

namespace ProjectZero3_Translation
{
    public partial class MainUI : Form
    {
        Editor _Editor;
        VWF _VWF;
        dynamic _Encoding;
        public MainUI()
        {
            InitializeComponent();
        }

        private void btnSelectGameLocation_Click(object sender, EventArgs e)
        {
            string folderPath = DialogManager.FolderBrowser("Project Zero 3");
            if (!string.IsNullOrEmpty(folderPath))
            {
                txtBoxGameLocation.Text = folderPath;
                if (GlobalVariable._JsonConfig.ContainsKey("GameLocation"))
                {
                    GlobalVariable._JsonConfig["GameLocation"] = folderPath;
                }
                else
                {
                    GlobalVariable._JsonConfig.Add("GameLocation", folderPath);
                }
                string configStr = new JavaScriptSerializer().Serialize(GlobalVariable._JsonConfig);
                try
                {
                    File.WriteAllText(AppConfig._ConfigFile, configStr);
                }
                catch (Exception err)
                {
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
                }
            }
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.GetDirectoryName(AppConfig._ConfigFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(AppConfig._ConfigFile));
            }
            if (!File.Exists(AppConfig._ConfigFile))
            {
                File.WriteAllText(AppConfig._ConfigFile, "{}");
            }
            else
            {
                GlobalVariable._JsonConfig = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(AppConfig._ConfigFile));
                if (GlobalVariable._JsonConfig.ContainsKey("GameLocation") && !string.IsNullOrEmpty(GlobalVariable._JsonConfig["GameLocation"]) && !string.IsNullOrWhiteSpace(GlobalVariable._JsonConfig["GameLocation"]))
                    txtBoxGameLocation.Text = GlobalVariable._JsonConfig["GameLocation"];
            }
            _Encoding = new JavaScriptSerializer().Deserialize<dynamic>(Encoding.GetEncoding(1252).GetString(Properties.Resources.config));
            _Editor = Editor.Instance();
            _VWF = VWF.Instance();
        }

        private void btnOpen_Click(object sender, EventArgs e)
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
                        List<BlockText> data = ProjectZero3.ExtractText(GlobalVariable._JsonConfig["GameLocation"], _Encoding, this.progressBar);
                        if (data.Count > 0)
                        {
                            GlobalVariable._DataMessage = data;
                            this.listFiles.BeginInvoke((MethodInvoker)delegate ()
                            {
                                listFiles.Items.Clear();
                                foreach (BlockText entry in GlobalVariable._DataMessage)
                                {
                                    listFiles.Items.Add($"[{entry.Index}] - msg.obj");
                                }
                            });
                            
                        }
                    }
                    catch (Exception err)
                    {
                        GlobalVariable._IsBusy = false;
                        GlobalVariable._DataMessage.Clear();
                        MessageBox.Show($"An error occurred:\n\n{err}", AppConfig._MessageBoxTitle);
                    }
                }).GetAwaiter().OnCompleted(() => { GlobalVariable._IsBusy = false; });
            }
        }

        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listFiles.SelectedIndex < 0 || GlobalVariable._IsBusy) return;
            int index = listFiles.SelectedIndex;
            try
            {
                BlockText data = GlobalVariable._DataMessage.Where(entry => entry.Index == index).FirstOrDefault();
                EditorUI editor = _Editor.TransferData(data);
                if (!editor.Visible) editor.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
            }
        }

        private void btnReimport_Click(object sender, EventArgs e)
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
                        ProjectZero3.Repack(GlobalVariable._JsonConfig["GameLocation"], AppConfig._TextArchive, GlobalVariable._DataMessage, _Encoding, this.progressBar);
                    }
                    catch (Exception err)
                    {
                        GlobalVariable._IsBusy = false;
                        MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
                    }
                }).GetAwaiter().OnCompleted(() => { GlobalVariable._IsBusy = false; });
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalVariable._DataMessage.Count() < 0 || GlobalVariable._IsBusy || listFiles.SelectedIndex <= -1) return;
            GlobalVariable._IsBusy = true;
            int index = listFiles.SelectedIndex;
            Task.Run(() =>
            {
                try
                {
                    BlockText data = GlobalVariable._DataMessage.Where(entry => entry.Index == index).FirstOrDefault();
                    Operation.Export(data);
                }
                catch (Exception err)
                {
                    GlobalVariable._IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { GlobalVariable._IsBusy = false; });
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalVariable._DataMessage.Count() <= 0 || GlobalVariable._IsBusy || listFiles.SelectedIndex <= -1) return;
            int index = listFiles.SelectedIndex;
            string file_name = $"[${GlobalVariable._DataMessage[index].Index}] msg.obj.txt";
            string file_import = DialogManager.FileBrowser(file_name, "Text files (*.txt)|*.txt|All files (*.*)|*.*");
            if (string.IsNullOrEmpty(file_import)) return;
            GlobalVariable._IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    BlockText data = GlobalVariable._DataMessage.Where(entry => entry.Index == index).FirstOrDefault();
                    double percent = 100.0 / data.Strings.Count;
                    string[] lines = File.ReadAllLines(file_import);
                    for (int i = 0; i < data.Strings.Count; i++)
                    {
                        data.Strings[i] = lines[i];
                        percent += 100.0 / data.Strings.Count;
                        Operation.ProgressBar(this.progressBar, (int)percent);
                    }
                    if (_Editor.isVisible() && _Editor.GetIndex() == data.Index) _Editor.TransferData(data);
                }
                catch (Exception err)
                {
                    GlobalVariable._IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { GlobalVariable._IsBusy = false; });
        }

        private void exportAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportAll(false);
        }

        private void ExportAll(bool merge)
        {
            if (GlobalVariable._DataMessage.Count() <= 0 || GlobalVariable._IsBusy) return;
            string export_dir = DialogManager.FolderBrowser("Export (Directory)");
            if (string.IsNullOrEmpty(export_dir)) return;
            GlobalVariable._IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ExportAll(export_dir, GlobalVariable._DataMessage, merge, this.progressBar);
                }
                catch (Exception err)
                {
                    GlobalVariable._IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { GlobalVariable._IsBusy = false; });
        }

        private void importAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalVariable._DataMessage.Count() <= 0 || GlobalVariable._IsBusy) return;
            string json_file = DialogManager.FileBrowser("export.json", "JSON files (*.json)|*.json");
            if (string.IsNullOrEmpty(json_file)) return;
            GlobalVariable._IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ImportAll(json_file, this.progressBar);
                    if (_Editor.isVisible())
                    {
                        BlockText data = GlobalVariable._DataMessage.Where(entry => entry.Index == _Editor.GetIndex()).FirstOrDefault();
                        _Editor.TransferData(data);
                    }
                }
                catch (Exception err)
                {
                    GlobalVariable._IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err}", AppConfig._MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { GlobalVariable._IsBusy = false; });
        }

        private void btnVWF_Click(object sender, EventArgs e)
        {
            if (!_VWF.isCreated())
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
                            List<FontCharacter> font_chars = ProjectZero3.VariableWidthFont(GlobalVariable._JsonConfig["GameLocation"], this.progressBar);
                            _VWF.TransferData(font_chars);
                        }
                        catch (Exception err)
                        {
                            GlobalVariable._IsBusy = false;
                            MessageBox.Show($"An error occurred:\n\n{err.Message}", AppConfig._MessageBoxTitle);
                        }
                    }).GetAwaiter().OnCompleted(() => { GlobalVariable._IsBusy = false; _VWF.Show(); });
                }
            }
            else
            {
                _VWF.Show();
            }
        }

        private void exportAllOneFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportAll(true);
        }
    }
}
