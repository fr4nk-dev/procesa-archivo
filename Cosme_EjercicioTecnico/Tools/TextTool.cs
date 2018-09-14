using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosme_EjercicioTecnico.Tools
{
     static class TextTool
    {
        public static bool IsDigit(char c)
        {
            return ((c >= '0') && (c <= '9'));
        }
    }
}
