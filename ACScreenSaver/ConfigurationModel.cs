using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACScreenSaver
{
    public class ConfigurationModel
    {
        public string ImagesDirectoryPath { get; set; }
        public int IntervalTime { get; set; }
        public bool IsRandom { get; set; }
    }
}
