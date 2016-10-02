using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLib.Common
{
    public class Paging
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string Sort { get; set; }
        public bool Desc { get; set; }
    }
}
