using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Dynamic;
using System.Web.Script.Serialization;

namespace ProjectZero3_Translation
{
    public static class ProjectZero3
    {
        public struct FHDInfo
        {
            public int Index;
            public uint DecompressSize;
            public long DecompressedSizeOffset;
            public long CompressedSizeOffset;
            public long FHDOffset;
            public long BINOffset;
        }
        public struct RepackInfo
        {
            public long OldSize;
            public long NewSize;
            public long BINOffset;
        }
        private static FHDInfo GetInfo(Stream stream, string filename)
        {
            BinaryReader reader = new BinaryReader(stream);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            FHDInfo fhd_info = new FHDInfo();
            if (reader.ReadUInt32() == 0x46484400)
            {
                reader.BaseStream.Seek(8, SeekOrigin.Begin);
                uint fhd_size = reader.ReadUInt32();
                uint files = reader.ReadUInt32();
                uint decompressed_offset = reader.ReadUInt32();
                uint type_offset = reader.ReadUInt32();
                uint compressed_offset = reader.ReadUInt32();
                uint name_offset = reader.ReadUInt32();
                uint lba_offset = reader.ReadUInt32();
                reader.BaseStream.Seek(name_offset, SeekOrigin.Begin);
                
                for (int i = 0; i < files; i++)
                {
                    uint dest_name_offset = reader.ReadUInt32();
                    uint file_name_offset = reader.ReadUInt32();
                    long current = reader.BaseStream.Position;
                    uint next_dest_name_offset = reader.ReadUInt32();
                    uint next_file_name_offset = reader.ReadUInt32();
                    if (next_file_name_offset > fhd_size) next_file_name_offset = fhd_size;
                    int str_len = (int)(next_file_name_offset - file_name_offset);
                    reader.BaseStream.Seek(file_name_offset, SeekOrigin.Begin);
                    string file_name = Encoding.UTF8.GetString(reader.ReadBytes(str_len - 1));
                    reader.BaseStream.Seek(current, SeekOrigin.Begin);
                    if (file_name == filename)
                    {
                        long file_compressed_offset = compressed_offset + i * 4;
                        long file_decompressed_offset = decompressed_offset + i * 4;
                        long file_lba_offset = lba_offset + i * 4;
                        reader.BaseStream.Seek(file_lba_offset, SeekOrigin.Begin);
                        uint bin_offset = reader.ReadUInt32() * 2048;
                        reader.BaseStream.Seek(file_decompressed_offset, SeekOrigin.Begin);
                        uint decompressed_size = reader.ReadUInt32();
                        /*reader.BaseStream.Seek(file_compressed_offset, SeekOrigin.Begin);
                        uint compressed_size = reader.ReadUInt32();*/
                        fhd_info.Index = i;
                        fhd_info.BINOffset = bin_offset;
                        fhd_info.DecompressSize = decompressed_size;
                        fhd_info.DecompressedSizeOffset = file_decompressed_offset;
                        fhd_info.CompressedSizeOffset = file_compressed_offset;
                        break;
                    }
                }
                
            }
            else
            {
                throw new Exception("The file is not a Fatal Frame 3 file.");
            }
            reader.Close();
            return fhd_info;
        }

        private static RepackInfo UpdateFHD(string fhd, string filename, long msg_size, long bin_size)
        {
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(fhd));
            BinaryReader reader = new BinaryReader(stream);            
            BinaryWriter writer = new BinaryWriter(stream);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            RepackInfo fhd_info = new RepackInfo();
            if (reader.ReadUInt32() == 0x46484400)
            {
                reader.BaseStream.Seek(8, SeekOrigin.Begin);
                uint fhd_size = reader.ReadUInt32();
                uint files = reader.ReadUInt32();
                uint decompressed_offset = reader.ReadUInt32();
                uint type_offset = reader.ReadUInt32();
                uint compressed_offset = reader.ReadUInt32();
                uint name_offset = reader.ReadUInt32();
                uint lba_offset = reader.ReadUInt32();
                reader.BaseStream.Seek(name_offset, SeekOrigin.Begin);
                /*bool isSizeChanged = false;
                long bytes_changed = 0;*/
                for (int i = 0; i < files; i++)
                {
                    uint dest_name_offset = reader.ReadUInt32();
                    uint file_name_offset = reader.ReadUInt32();
                    long current = reader.BaseStream.Position;
                    uint next_dest_name_offset = reader.ReadUInt32();
                    uint next_file_name_offset = reader.ReadUInt32();
                    if (next_file_name_offset > fhd_size) next_file_name_offset = fhd_size;
                    int str_len = (int)(next_file_name_offset - file_name_offset);
                    reader.BaseStream.Seek(file_name_offset, SeekOrigin.Begin);
                    string file_name = Encoding.UTF8.GetString(reader.ReadBytes(str_len - 1));

                    long file_lba_offset = lba_offset + i * 4;
                    reader.BaseStream.Seek(file_lba_offset, SeekOrigin.Begin);
                    uint bin_offset = reader.ReadUInt32();
                    /*if (bin_offset == 0xFFFFFFFF)
                    {
                        reader.BaseStream.Seek(current, SeekOrigin.Begin);
                        continue;
                    }
                    if (isSizeChanged)
                    {
                        writer.BaseStream.Seek(file_lba_offset, SeekOrigin.Begin);
                        writer.Write((int)(bin_offset + bytes_changed));
                    }*/

                    if (file_name == filename)
                    {
                        long file_compressed_offset = compressed_offset + i * 4;
                        long file_decompressed_offset = decompressed_offset + i * 4;
                        reader.BaseStream.Seek(file_decompressed_offset, SeekOrigin.Begin);
                        uint decompressed_size = reader.ReadUInt32();
                        /*reader.BaseStream.Seek(file_compressed_offset, SeekOrigin.Begin);
                        uint compressed_size = reader.ReadUInt32();*/
                        writer.BaseStream.Seek(file_compressed_offset, SeekOrigin.Begin);
                        writer.Write((uint)(msg_size << 1));
                        writer.BaseStream.Seek(file_decompressed_offset, SeekOrigin.Begin);
                        writer.Write((uint)msg_size);
                        long total_size = decompressed_size % 0x800 == 0 ? decompressed_size : decompressed_size + (0x800 - (long)decompressed_size % 0x800);
                        //fhd_info.BINOffset = bin_offset * 0x800;
                        fhd_info.OldSize = total_size;
                        //fhd_info.NewSize = msg_total_size;
                        fhd_info.BINOffset = bin_offset > AppConfig._TextArchiveOffset ?
                            (bin_size - total_size) / 0x800 : bin_size / 0x800;
                        writer.BaseStream.Position = file_lba_offset;
                        writer.Write((uint)(fhd_info.BINOffset));
                        break;
                        /*if (total_size != msg_total_size)
                        {
                            isSizeChanged = true;
                            bytes_changed = (msg_total_size - total_size) / 2048;
                        }
                        else
                        {
                            break;
                        }*/
                    }
                    reader.BaseStream.Seek(current, SeekOrigin.Begin);
                }
            }
            else
            {
                throw new Exception("The file is not a Fatal Frame 3 file.");
            }
            writer.Close();
            reader.Close();
            File.WriteAllBytes(fhd, stream.ToArray());
            return fhd_info;
        }
        public static List<BlockText> ExtractText(string dir, dynamic config, ProgressBar progressBar)
        {
            List<BlockText> result = new List<BlockText>();
            double percent = 0;
            Operation.ProgressBar(progressBar, (int)percent);
            string fhd = Directory.GetFiles(dir, "*.FHD", SearchOption.AllDirectories).FirstOrDefault();
            string bin = Directory.GetFiles(dir, "*.BIN", SearchOption.AllDirectories).FirstOrDefault();
            if (!File.Exists(fhd) || !File.Exists(bin)) throw new Exception("File game not found.");
            Operation.ProgressBar(progressBar, (int)(percent += 5));
            FHDInfo fhd_info = GetInfo(File.OpenRead(fhd), AppConfig._TextArchive);
            FileStream bin_stream = File.OpenRead(bin);
            BinaryReader bin_reader = new BinaryReader(bin_stream);
            bin_reader.BaseStream.Seek(fhd_info.BINOffset, SeekOrigin.Begin);
            byte[] msg_bytes = bin_reader.ReadBytes((int)fhd_info.DecompressSize);
            bin_reader.Close();
            bin_stream.Close();
            Operation.ProgressBar(progressBar, (int)(percent+=5));
            BinaryReader reader = new BinaryReader(new MemoryStream(msg_bytes));
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            int end_pointer_offset = (int)reader.BaseStream.Length;
            int block_nums = 0;
            while (reader.BaseStream.Position < end_pointer_offset)
            {
                int temp = reader.ReadInt32();
                if (temp < end_pointer_offset) end_pointer_offset = temp;
                block_nums++;
            }
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            List<BlockText> blocks = new List<BlockText>();
            for (int i = 0; i < block_nums; i++)
            {
                uint pointer_offset = reader.ReadUInt32();
                BlockText block = new BlockText(i, pointer_offset);
                blocks.Add(block);
            }
            BlockText[] blocks_sorted = blocks.OrderBy(e => e.PointerOffset).ToArray();
            
            dynamic font_config = File.Exists(AppConfig._ReplaceConfig) ?
                new JavaScriptSerializer().Deserialize<dynamic>(File.ReadAllText(AppConfig._ReplaceConfig)) : null;
            for (int i = 0; i < blocks_sorted.Length; i++)
            {
                List<string> message = new List<string>();
                reader.BaseStream.Seek(blocks_sorted[i].PointerOffset, SeekOrigin.Begin);
                int first_pointer = reader.ReadInt32();
                int pointer_nums = (first_pointer - (int)blocks_sorted[i].PointerOffset) / 4;
                reader.BaseStream.Seek(blocks_sorted[i].PointerOffset, SeekOrigin.Begin);
                for (int x = 0; x < pointer_nums; x++)
                {
                    int pointer = reader.ReadInt32();
                    long pos_temp = reader.BaseStream.Position;
                    int str_len = 0;
                    int next_pointer = 0;
                    next_pointer = x >= pointer_nums - 1 ?
                        (i >= blocks_sorted.Length - 1 ?
                            (int)reader.BaseStream.Length : 
                            (int)blocks_sorted[i + 1].PointerOffset) :
                        reader.ReadInt32();
                    /*if (x >= pointer_nums - 1)
                    {
                        next_pointer = i >= blocks_sorted.Length - 1 ? (int)reader.BaseStream.Length : (int)blocks_sorted[i + 1].PointerOffset;
                    }
                    else
                    {
                        next_pointer = reader.ReadInt32();
                    }*/
                    str_len = next_pointer - pointer;
                    reader.BaseStream.Seek(pointer, SeekOrigin.Begin);
                    byte[] raw = reader.ReadBytes(str_len);

                    string[] str_chars = new string[raw.Length];
                    for (int b = 0; b < raw.Length; b++)
                    {
                        dynamic character = ((IEnumerable<dynamic>)config["encoding"]).Where(e => $"{e[0]}" == $"{raw[b]}").FirstOrDefault();
                        str_chars[b] = character == null ? $"{(char)123}{raw[b].ToString()}{(char)125}" : $"{character[1]}";
                    }
                    string str = String.Join("", str_chars);
                    foreach (Object[] code in config["code"])
                    {
                        str = str.Replace($"{code[0]}", $"{code[1]}");
                    }
                    if (font_config != null)
                    {
                        foreach (Object[] character in font_config)
                        {
                            string char_code = CodeToStr($"{character[0]}", config["encoding"]);
                            str = str.Replace(char_code, $"{character[1]}");
                        }
                    }
                    message.Add(str);
                    reader.BaseStream.Seek(pos_temp, SeekOrigin.Begin);
                }
                blocks_sorted[i].Strings = message;
                percent += 90.0 / blocks_sorted.Length;
                Operation.ProgressBar(progressBar, (int)percent);
            }
            return blocks;
        }

        public static byte[] ReimportText(List<BlockText> blocks, dynamic config, ProgressBar progressBar)
        {
            MemoryStream result = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(result);
            long table_length = blocks.Count * 4;
            writer.Write(new byte[table_length]);
            BlockText[] blocks_sorted = blocks.OrderBy(entry => entry.PointerOffset).ToArray();
            double percent = 0;
            Operation.ProgressBar(progressBar, (int)percent);
            dynamic font_config = File.Exists(AppConfig._ReplaceConfig) ?
                new JavaScriptSerializer().Deserialize<dynamic>(File.ReadAllText(AppConfig._ReplaceConfig)) : null;
            for (int i = 0; i < blocks_sorted.Length; i++)
            {
                string[] strings = blocks_sorted[i].Strings.ToArray();
                List<byte[]> block_text_bytes = new List<byte[]>();
                long text_offset = writer.BaseStream.Position + strings.Length * 4;
                blocks_sorted[i].NewPointerOffset = writer.BaseStream.Position;
                writer.Write((uint)text_offset);
                for (int x = 0; x < strings.Length; x++)
                {
                    foreach (Object[] code in config["code"])
                    {
                        strings[x] = strings[x].Replace($"{code[1]}", $"{code[0]}");
                    }
                    if (font_config != null)
                    {
                        foreach (Object[] character in font_config)
                        {
                            strings[x] = strings[x].Replace($"{character[1]}", $"{character[0]}");
                        }
                    }
                    string[] str_chars = strings[x].ToCharArray().Select(c => c.ToString()).ToArray();
                    List<byte> str_bytes = new List<byte>();
                    for (int y = 0; y < str_chars.Length; y++)
                    {
                        if (str_chars[y] != "{")
                        {
                            foreach (Object[] character in config["encoding"])
                            {
                                if ($"{character[1]}" == str_chars[y])
                                {
                                    str_bytes.Add((byte)(int)character[0]);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            string dec = "";
                            y++;
                            while (str_chars[y] != "}")
                            {
                                dec += str_chars[y++];

                            }
                            str_bytes.Add((byte)int.Parse(dec));
                        }
                    }
                    block_text_bytes.Add(str_bytes.ToArray());
                    text_offset += str_bytes.Count;
                    if (x < strings.Length - 1) writer.Write((uint)text_offset);
                    
                }
                foreach (byte[] text_bytes in block_text_bytes)
                {
                    writer.Write(text_bytes);
                }
                percent += 80.0 / blocks_sorted.Length;
                Operation.ProgressBar(progressBar, (int)percent);
            }
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            foreach (BlockText block in blocks)
            {
                writer.Write((uint)block.NewPointerOffset);
            }
            writer.Close();
            return result.ToArray();
        }

        public static void Repack(string dir, string filename, List<BlockText> blocks, dynamic config, ProgressBar progressBar)
        {
            byte[] msg_raw_data = ReimportText(blocks, config, progressBar);
            double percent = 80;
            byte[] msg_data;
            if (msg_raw_data.Length % 0x800 != 0)
            {
                MemoryStream msg_stream = new MemoryStream();
                BinaryWriter msg_writer = new BinaryWriter(msg_stream);
                msg_writer.Write(msg_raw_data);
                long zeroes_len = (0x800 - msg_raw_data.Length % 0x800);
                msg_writer.Write(new byte[(int)zeroes_len]);
                msg_data = msg_stream.ToArray();
                msg_writer.Close();
            }
            else
            {
                msg_data = msg_raw_data;
            }
            Operation.ProgressBar(progressBar, (int)(percent += 5));
            string fhd = Directory.GetFiles(dir, "*.FHD", SearchOption.AllDirectories).FirstOrDefault();
            string bin = Directory.GetFiles(dir, "*.BIN", SearchOption.AllDirectories).FirstOrDefault();
            if (!File.Exists(fhd) || !File.Exists(bin)) throw new Exception("File game not found.");
            long bin_size = new FileInfo(bin).Length;
            RepackInfo repack_info = UpdateFHD(fhd, filename, msg_raw_data.Length, bin_size);
            Operation.ProgressBar(progressBar, (int)(percent+=5));
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(bin));
            writer.BaseStream.Seek(repack_info.BINOffset * 0x800, SeekOrigin.Begin);
            writer.Write(msg_data);
            writer.Close();
            /*if (repack_info.OldSize == repack_info.NewSize)
            {
                BinaryWriter writer = new BinaryWriter(File.OpenWrite(bin));
                writer.BaseStream.Seek(repack_info.BINOffset, SeekOrigin.Begin);
                writer.Write(msg_data);
                writer.Close();
            }
            else
            {
                int chunk_len = 2048;
                string temp = Path.Combine(dir, $"{Path.GetFileName(bin)}.temp");
                BinaryReader reader = new BinaryReader(File.OpenRead(bin));
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                FileStream temp_file = File.Create(temp);
                BinaryWriter writer = new BinaryWriter(temp_file);
                //long new_size = repack_info.BINOffset + repack_info.NewSize + (writer.BaseStream.Length - repack_info.OldSize);
                while (writer.BaseStream.Length < repack_info.BINOffset)
                {
                    byte[] chunk = reader.ReadBytes(chunk_len);
                    writer.Write(chunk);
                }
                writer.Write(msg_data);
                reader.BaseStream.Position += repack_info.OldSize;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    byte[] chunk = reader.ReadBytes(chunk_len);
                    writer.Write(chunk);
                }
                reader.Close();
                writer.Close();
                temp_file.Close();
                File.Delete(bin);
                File.Move(temp, bin);
            }*/
            Operation.ProgressBar(progressBar, 100);
        }
        public static List<FontCharacter> VariableWidthFont(string dir, ProgressBar progressBar)
        {
            dynamic vwf = new JavaScriptSerializer().Deserialize<dynamic>(Encoding.UTF8.GetString(Properties.Resources.vwf));
            string file = Path.Combine(dir, vwf["file"]);
            List<FontCharacter> result = new List<FontCharacter>();
            FileStream stream = File.OpenRead(file);
            BinaryReader reader = new BinaryReader(stream);
            reader.BaseStream.Seek(int.Parse($"{vwf["offset"]}"), SeekOrigin.Begin);
            int char_nums = int.Parse($"{vwf["size"]}") / 4;
            dynamic font_config = File.Exists(AppConfig._ReplaceConfig) ?
                new JavaScriptSerializer().Deserialize<dynamic>(File.ReadAllText(AppConfig._ReplaceConfig)) : null;
            double percent = 0;
            Operation.ProgressBar(progressBar, (int)percent);
            for (int i = 0; i < char_nums; i++)
            {
                percent += 100.0 / char_nums;
                Operation.ProgressBar(progressBar, (int)percent);
                int width = reader.ReadInt32();
                dynamic info = ((IEnumerable<dynamic>)vwf["characters"]).Where(e => $"{e[0]}" == $"{i}").FirstOrDefault();
                string character = info != null ? $"{info[2]}" : "Unknown";
                string hex = info != null ? ToHex($"{info[1]}") : "Unknown";
                FontCharacter font_character = new FontCharacter(i, width, hex, character);
                font_character.Original = info != null ? $"{info[1]}" : null;
                if (font_config != null && info != null)
                {
                    dynamic entry = ((IEnumerable<dynamic>)font_config).Where(e => $"{e[0]}" == $"{info[1]}").FirstOrDefault();
                    if (entry != null) font_character.Replace = $"{entry[1]}";
                }
                result.Add(font_character);
            }
            reader.Close();
            stream.Close();
            return result;
        }
        public static void VWFRepack(string dir, List<FontCharacter> fontCharacters, ProgressBar progressBar)
        {
            dynamic vwf = new JavaScriptSerializer().Deserialize<dynamic>(Encoding.UTF8.GetString(Properties.Resources.vwf));
            string file = Directory.GetFiles(dir, $"{vwf["file"]}", SearchOption.AllDirectories).FirstOrDefault();
            FileStream stream = File.OpenWrite(file);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.BaseStream.Seek(int.Parse($"{vwf["offset"]}"), SeekOrigin.Begin);
            List<string[]> replace = new List<string[]>(); 
            double percent = 0;
            Operation.ProgressBar(progressBar, (int)percent);
            foreach (FontCharacter entry in fontCharacters)
            {
                percent += 100.0 / fontCharacters.Count;
                Operation.ProgressBar(progressBar, (int)percent);
                writer.Write((int)entry.Width);
                if (!string.IsNullOrEmpty(entry.Replace) && entry.Original != null)
                {
                    replace.Add(new string[] { entry.Original, entry.Replace });
                }
            }
            writer.Close();
            stream.Close();
            string json_replace = new JavaScriptSerializer().Serialize(replace.ToArray());
            if (!File.Exists(AppConfig._ReplaceConfig)) File.Create(AppConfig._ReplaceConfig).Close();
            File.WriteAllText(AppConfig._ReplaceConfig, json_replace);
        }

        private static string ToHex(string input)
        {
            string[] strs = input.ToCharArray().Select(c => c.ToString()).ToArray();
            string result = "";
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] == "{")
                {
                    string dec = "";
                    i++;
                    while(strs[i] != "}")
                    {
                        dec += strs[i++];
                    }
                    result += String.Format("{0:X2}", int.Parse(dec));
                }
            }
            return result;
        }
        private static string CodeToStr(string input, dynamic encoding)
        {
            string[] strs = input.ToCharArray().Select(c => c.ToString()).ToArray();
            string result = "";
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] == "{")
                {
                    string dec = "";
                    i++;
                    while (strs[i] != "}")
                    {
                        dec += strs[i++];
                    }
                    dynamic entry = ((IEnumerable<dynamic>)encoding).Where(e => $"{e[0]}" == dec).FirstOrDefault(); 
                    result += entry == null ? $"{(char)123}{dec}{(char)125}" : $"{entry[1]}";
                }
            }
            return result;
        }
    }
}
