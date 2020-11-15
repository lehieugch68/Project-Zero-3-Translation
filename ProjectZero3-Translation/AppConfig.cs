using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProjectZero3_Translation
{
    public static class AppConfig
    {
        public static readonly string _TextArchive = "msg.obj";
        public static readonly string _FontArchive = "font_tex.pk4";
        public static readonly int _TextArchiveSize = 344414;
        public static readonly int _TextArchiveOffset = 651;
        public static readonly string _AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string _ConfigDirectory = Path.Combine(_AppDirectory, "Config");
        public static readonly string _ConfigFile = Path.Combine(_ConfigDirectory, "config.json");
        public static readonly string _ReplaceConfig = Path.Combine(AppConfig._ConfigDirectory, "encoding.json");
        public static readonly string _MessageBoxTitle = "Project Zero 3 Translation";
    }
}
