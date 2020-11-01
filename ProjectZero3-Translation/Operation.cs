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
        public static void ExportAll(string export_dir, List<BlockText> data_message, ProgressBar progressBar)
        {
            double percent = 100.0 / data_message.Count;
            Dictionary<string, string> json = new Dictionary<string, string>();
            foreach (BlockText data in data_message)
            {
                string file = Path.Combine(export_dir, $"[{data.Index}] msg.obj.txt");
                string content = String.Join("\r\n", data.Strings.ToArray());
                File.WriteAllText(file, content);
                json.Add($"{data.Index}", Path.GetFileName(file));
                percent += 100.0 / data_message.Count;
                ProgressBar(progressBar, (int)percent);
            }
            string json_content = new JavaScriptSerializer().Serialize(json);
            File.WriteAllText(Path.Combine(export_dir, "export.json"), json_content);
        }
        public static void ImportAll(string json_file, ProgressBar progressBar)
        {
            Dictionary<string, string> dict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(json_file));
            double percent = 100.0 / dict.Count;
            foreach (KeyValuePair<string, string> entry in dict)
            {
                BlockText data = MainUI._DataMessage.Find(e => e.Index == uint.Parse(entry.Key));
                percent += 100.0 / dict.Count;
                ProgressBar(progressBar, (int)percent);
                if (data == null) continue;
                string[] lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(json_file), entry.Value));
                for (int i = 0; i < data.Strings.Count; i++)
                {
                    data.Strings[i] = lines[i];
                }
            }
        }
    }
}
