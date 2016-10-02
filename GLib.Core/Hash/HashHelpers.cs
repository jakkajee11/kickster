using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLib.Core.Hash
{
    public static class HashHelpers
    {
        public static int RSHash(params object[] input)
        {
            const int b = 378551;
            int a = 63689;
            int hash = 0;

            // The unchecked keyword to make sure 
            // not get an overflow exception.
            // It can be enhanced later by catching the OverflowException.

            unchecked
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] != null)
                    {
                        hash = hash * a + input[i].GetHashCode();
                        a = a * b;
                    }
                }
            }

            return hash;
        }
    }
}
