using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniDriveTestApp
{
    public class DriveModel
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int TotalSize { get; set; }
        public int UsedSize { get; set; }
        
        public int FreeSize { get => TotalSize - UsedSize; }
    }
}
