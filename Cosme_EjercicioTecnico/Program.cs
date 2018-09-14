using System;

namespace Cosme_EjercicioTecnico
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                //TODO: Leer el nombre de archivo o directorios y limitar a ciertas extensiones desde app.config

                Console.WriteLine("Comienza el proceso");                
                var reader = new LecturaEscritura(@"c:\datos\ExcepcionesContables.txt");

                Console.WriteLine("Leyendo el archivo");                
                var dataFromFile = reader.LeerTxt();

                Console.WriteLine("Clasificando los datos");
                var dataClassify = reader.ClasificaRegistros(dataFromFile);

                Console.WriteLine("Almacenando en Base de Datos");
                reader.EscribirSQl(dataClassify.NotCommited);

                Console.WriteLine("Escribiendo XML's");
                reader.EscribirFileSystem(dataClassify.Commited, dataClassify.NotCommited);

                Console.WriteLine("Termina el proceso");
            }
            catch (Exception e)
            {
                Console.WriteLine($@"{e.Message}, para mas información revisar el log");                
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}
