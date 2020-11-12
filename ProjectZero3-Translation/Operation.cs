using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Web.Script.Serialization;

namespace ProjectZero3_Translation
{
    public class Operation
    {
        public static void ProgressBar(ProgressBar progressBar, int percent)
        {
            progressBar.BeginInvoke((MethodInvoker)delegate
            {
                progressBar.Value = percent > 100 ? 100 : percent;
            });

        }
        public static void Export(BlockText data_message)
        {
            byte[] data = Encoding.UTF8.GetBytes(String.Join("\r\n", data_message.Strings.ToArray()));
            DialogManager.SaveFile($"[{data_message.Index}] msg.obj.txt", data, "Text files (*.txt)|*.txt|All files (*.*)|*.*");
        }
        struct ExportJson
        {
            public bool Merge;
            public string Archive;
            public Dictionary<string, string> Files;
        }
        public static void ExportAll(string export_dir, List<BlockText> data_message, bool merge, ProgressBar progressBar)
        {
            double percent = 100.0 / data_message.Count;
            ExportJson json = new ExportJson();
            json.Merge = merge;
            json.Files = new Dictionary<string, string>();
            json.Archive = merge ? "msg.obj.txt" : null;
            if (merge)
            {
                List<string> content = new List<string>();
                foreach (BlockText data in data_message)
                {
                    string data_strings = String.Join("\r\n", data.Strings.ToArray());
                    content.Add(data_strings);
                    json.Files.Add($"{data.Index}", $"{data.Strings.Count}");
                    percent += 100.0 / data_message.Count;
                    ProgressBar(progressBar, (int)percent);
                }
                File.WriteAllText(Path.Combine(export_dir, json.Archive), String.Join("\r\n", content.ToArray()));
            }
            else
            {
                foreach (BlockText data in data_message)
                {
                    string file = Path.Combine(export_dir, $"[{data.Index}] msg.obj.txt");
                    string content = String.Join("\r\n", data.Strings.ToArray());
                    File.WriteAllText(file, content);
                    json.Files.Add($"{data.Index}", Path.GetFileName(file));
                    percent += 100.0 / data_message.Count;
                    ProgressBar(progressBar, (int)percent);
                }
                
            }
            string json_content = new JavaScriptSerializer().Serialize(json);
            File.WriteAllText(Path.Combine(export_dir, "export.json"), json_content);
        }
        public static void ImportAll(string json_file, ProgressBar progressBar)
        {
            dynamic export_config = new JavaScriptSerializer().Deserialize<dynamic>(File.ReadAllText(json_file));
            double percent = 100.0 / export_config["Files"].Count;
            if ((bool)export_config["Merge"] == true)
            {
                string[] strings = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(json_file), export_config["Archive"]));
                int line = 0;
                foreach (KeyValuePair<string, object> entry in export_config["Files"])
                {
                    BlockText data = GlobalVariable._DataMessage.Find(e => e.Index == uint.Parse(entry.Key));
                    int start = line;
                    int index = 0;
                    line += int.Parse($"{entry.Value}");
                    for (int i = start; i < line; i++, index++)
                    {
                        data.Strings[index] = strings[i];
                    }
                    percent += 100.0 / export_config["Files"].Count;
                    ProgressBar(progressBar, (int)percent);
                }
            }
            else
            {
                foreach (KeyValuePair<string, object> entry in export_config["Files"])
                {
                    BlockText data = GlobalVariable._DataMessage.Find(e => e.Index == uint.Parse(entry.Key));
                    percent += 100.0 / export_config["Files"].Count;
                    ProgressBar(progressBar, (int)percent);
                    if (data == null || !File.Exists(Path.Combine(Path.GetDirectoryName(json_file), $"{entry.Value}"))) continue;
                    string[] lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(json_file), $"{entry.Value}"));
                    for (int i = 0; i < data.Strings.Count; i++)
                    {
                        data.Strings[i] = lines[i];
                    }
                }
            }
        }
    }
}
