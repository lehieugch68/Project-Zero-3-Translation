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
        string _AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string _ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");
        Editor _Editor = Editor.Instance();
        dynamic _Encoding = new JavaScriptSerializer().Deserialize<dynamic>(Encoding.GetEncoding(1252).GetString(Properties.Resources.encoding));
        string _MessageBoxTitle = "Project Zero 3 Translation";
        bool _IsBusy = false;
        Dictionary<string, string> _JsonConfig = new Dictionary<string, string>();
        public static List<BlockText> _DataMessage = new List<BlockText>();
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
                if (_JsonConfig.ContainsKey("GameLocation"))
                {
                    _JsonConfig["GameLocation"] = folderPath;
                }
                else
                {
                    _JsonConfig.Add("GameLocation", folderPath);
                }
                string configStr = new JavaScriptSerializer().Serialize(_JsonConfig);
                try
                {
                    File.WriteAllText(_ConfigFile, configStr);
                }
                catch (Exception err)
                {
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.GetDirectoryName(_ConfigFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_ConfigFile));
            }
            if (!File.Exists(_ConfigFile))
            {
                File.WriteAllText(_ConfigFile, "{}");
            }
            else
            {
                _JsonConfig = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(_ConfigFile));
                if (_JsonConfig.ContainsKey("GameLocation") && !string.IsNullOrEmpty(_JsonConfig["GameLocation"]) && !string.IsNullOrWhiteSpace(_JsonConfig["GameLocation"]))
                    txtBoxGameLocation.Text = _JsonConfig["GameLocation"];
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!_JsonConfig.ContainsKey("GameLocation") || _IsBusy)
            {
                string msg = _IsBusy ? "Another task is already in progress." : "Please select your game directory.";
                MessageBox.Show(msg, _MessageBoxTitle);
            }
            else
            {
                _IsBusy = true;
                Task.Run(() =>
                {
                    try
                    {
                        List<BlockText> data = ProjectZero3.ExtractText(_JsonConfig["GameLocation"], _Encoding, this.progressBar);
                        if (data.Count > 0)
                        {
                            _DataMessage = data;
                            this.listFiles.BeginInvoke((MethodInvoker)delegate ()
                            {
                                listFiles.Items.Clear();
                                foreach (BlockText entry in _DataMessage)
                                {
                                    listFiles.Items.Add($"[{entry.Index}] - msg.obj");
                                }
                            });
                        }
                    }
                    catch (Exception err)
                    {
                        _IsBusy = false;
                        _DataMessage.Clear();
                        MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                    }
                }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
            }
        }

        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listFiles.SelectedIndex < 0 || _IsBusy) return;
            int index = listFiles.SelectedIndex;
            try
            {
                BlockText data = _DataMessage.Where(entry => entry.Index == index).FirstOrDefault();
                EditorUI editor = _Editor.TransferData(data);
                if (editor.Visible == false) editor.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
            }
        }

        private void btnReimport_Click(object sender, EventArgs e)
        {
            if (!_JsonConfig.ContainsKey("GameLocation") || _IsBusy)
            {
                string msg = _IsBusy ? "Another task is already in progress." : "Please select your game directory.";
                MessageBox.Show(msg, _MessageBoxTitle);
            }
            else
            {
                _IsBusy = true;
                Task.Run(() =>
                {
                    try
                    {
                        ProjectZero3.Repack(_JsonConfig["GameLocation"], _DataMessage, _Encoding, this.progressBar);
                    }
                    catch (Exception err)
                    {
                        _IsBusy = false;
                        MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                    }
                }).GetAwaiter().OnCompleted(() => { _IsBusy = false; MessageBox.Show("Done", _MessageBoxTitle); });
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() < 0 || _IsBusy || listFiles.SelectedIndex <= -1) return;
            _IsBusy = true;
            int index = listFiles.SelectedIndex;
            Task.Run(() =>
            {
                try
                {
                    BlockText data = _DataMessage.Where(entry => entry.Index == index).FirstOrDefault();
                    Operation.Export(data);
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() < 0 || _IsBusy || listFiles.SelectedIndex <= -1) return;
            int index = listFiles.SelectedIndex;
            string file_name = $"[${_DataMessage[index].Index}] msg.obj.txt";
            string file_import = DialogManager.FileBrowser(file_name, "Text files (*.txt)|*.txt|All files (*.*)|*.*");
            if (string.IsNullOrEmpty(file_import)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    BlockText data = _DataMessage.Where(entry => entry.Index == index).FirstOrDefault();
                    double percent = 100.0 / data.Strings.Count;
                    string[] lines = File.ReadAllLines(file_import);
                    for (int i = 0; i < data.Strings.Count; i++)
                    {
                        data.Strings[i] = lines[i];
                        percent += 100.0 / data.Strings.Count;
                        Operation.ProgressBar(this.progressBar, (int)percent);
                    }
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }

        private void exportAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() < 0 || _IsBusy) return;
            string export_dir = DialogManager.FolderBrowser("Export (Directory)");
            if (string.IsNullOrEmpty(export_dir)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ExportAll(export_dir, _DataMessage, this.progressBar);
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }

        private void importAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_DataMessage.Count() < 0 || _IsBusy) return;
            string json_file = DialogManager.FileBrowser("export.json", "JSON files (*.json)|*.json");
            if (string.IsNullOrEmpty(json_file)) return;
            _IsBusy = true;
            Task.Run(() =>
            {
                try
                {
                    Operation.ImportAll(json_file, this.progressBar);
                }
                catch (Exception err)
                {
                    _IsBusy = false;
                    MessageBox.Show($"An error occurred:\n\n{err.Message}", _MessageBoxTitle);
                }
            }).GetAwaiter().OnCompleted(() => { _IsBusy = false; });
        }
    }
}
