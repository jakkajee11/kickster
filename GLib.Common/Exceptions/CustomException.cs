using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLib.Common.Exceptions
{
    public class CustomException : Exception
    {
        //public string Source { get; set; }
        public int ErrorNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}
