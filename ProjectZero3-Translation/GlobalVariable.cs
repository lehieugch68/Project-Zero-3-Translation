using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectZero3_Translation
{
    public static class GlobalVariable
    {
        public static bool _IsBusy = false;
        public static List<BlockText> _DataMessage = new List<BlockText>();
        public static Dictionary<string, string> _JsonConfig = new Dictionary<string, string>();
    }
}
