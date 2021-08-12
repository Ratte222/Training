using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Helpers
{
    public class AppSettings
    {
        public string DirectoryForSerializeDeserialize { get; set; }
        public string FileName { get; set; }
        public string DirectoryForFireBaseConfig { get; set; }
        public Dictionary<string, string> FireBase { get; set; }
    }
}
