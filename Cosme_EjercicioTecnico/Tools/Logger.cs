using System;
using System.IO;

namespace Cosme_EjercicioTecnico.Tools
{
    public static class Logger
    {
        //TODO: Delegar al logger mas funcionalidad
        public static void WriteLine(string logFile, string txt)
        {
            using (var writer = File.AppendText(logFile))
            {
                writer.WriteLine($"{DateTime.Now}: {txt}");
            }
        }

    }
}
