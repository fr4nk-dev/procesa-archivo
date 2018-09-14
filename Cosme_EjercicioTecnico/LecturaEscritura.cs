using Cosme_EjercicioTecnico.Data;
using Cosme_EjercicioTecnico.Data.Model;
using Cosme_EjercicioTecnico.Dto;
using Cosme_EjercicioTecnico.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Cosme_EjercicioTecnico
{
    public class LecturaEscritura
    {
        private readonly string _dataFile;
        private readonly string _dataFileLog;
        private readonly string _xmlDirectory;
        private readonly string _today;
        public LecturaEscritura(string dataFile)
        {
            //TODO: Leer el directorio desde app.config
            var logDirectory = "c:/datos/log";

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            _xmlDirectory = "c:/datos/xml";
            if (!Directory.Exists(_xmlDirectory))
                Directory.CreateDirectory(_xmlDirectory);

            _dataFile = dataFile;
            _today = DateTime.Now.ToString("yyyyMMdd");
            _dataFileLog = $"{logDirectory}/{_today}.log";
        }

        public IList<ExcepcionContableFileDto> LeerTxt()
        {
            Logger.WriteLine(_dataFileLog, "INFO: Comienza el proceso: LeerTxt");
            var result = new List<ExcepcionContableFileDto>();

            //Lectura del archivo
            using (var file = new StreamReader(_dataFile))
            {
                try
                {
                    string line;
                    int i = 0;
                    // Lee todas las lineas y crea los dto's 
                    while ((line = file.ReadLine()) != null)
                    {
                        //Saltamos la primera linea
                        if (i > 0)
                        {
                            var item = line.Split(',');
                            result.Add(new ExcepcionContableFileDto()
                            {
                                ProductKey = item[0],
                                Cost = Convert.ToDecimal(item[1]),
                                Transaction = item[2],
                                Status = item[3],
                                Commited = item[4],
                                Date = DateTime.ParseExact(item[5], "yyyyMMdd", null)
                            });
                        }
                        i++;
                    }

                    Logger.WriteLine(_dataFileLog, "INFO: Termina el proceso: LeerTxt");
                }

                catch (Exception e)
                {                 
                    Logger.WriteLine(_dataFileLog, $"ERROR: Ocurrio un error en LeerTxt: {e.Message}");
                    throw new Exception("Ocurrio un error en LeerTxt");
                }
            }

            return result;
        }

        public ResumeExcepcionContable ClasificaRegistros(IList<ExcepcionContableFileDto> data)
        {
            Logger.WriteLine(_dataFileLog, "INFO: Comienza el proceso: ClasificaRegistros");
            var result = new ResumeExcepcionContable()
            {
                NotCommited = new List<ExcepcionContableDto>(),
                Commited = new List<ExcepcionContableDto>(),
                Before2016 = new List<ExcepcionContableDto>()
            };

            try
            {
                Logger.WriteLine(_dataFileLog, $"INFO: Comienza la clasificacion: ANTES 2016");
                foreach (var item in data.Where(x => x.Date.Year < 2016))
                {
                    using (var classify = new Mapper(item))
                    {
                        result.Before2016.Add(classify.ExpecionContable);
                        Logger.WriteLine(_dataFileLog, $"@{classify.ExpecionContable.Date:yyyyMMdd},{classify.ExpecionContable.Cost},{classify.ExpecionContable.ProductFamily},{classify.ExpecionContable.ProductKey},{classify.ExpecionContable.Status}");
                    }
                }

                Logger.WriteLine(_dataFileLog, $"INFO: Se claisificaron como ANTES 2016 {result.Before2016.Count} ");


                var after = data.Where(x => x.Date.Year >= 2016).ToList();

                Logger.WriteLine(_dataFileLog, $"INFO: Comienza la clasificacion: COMMITED");
                foreach (var item in after.Where(x => x.Commited == "1"))
                {
                    using (var classify = new Mapper(item))
                    {
                        result.Commited.Add(classify.ExpecionContable);
                        Logger.WriteLine(_dataFileLog, $"@@{classify.ExpecionContable.Date:yyyyMMdd},{classify.ExpecionContable.Cost},{classify.ExpecionContable.ProductFamily},{classify.ExpecionContable.ProductKey},{classify.ExpecionContable.Status}");
                    }
                }
                Logger.WriteLine(_dataFileLog, $"INFO: Se claisificaron como COMMITED {result.Commited.Count} ");

                Logger.WriteLine(_dataFileLog, $"INFO: Comienza la clasificacion: NOT-COMMITED");
                foreach (var item in after.Where(x => x.Commited == "0"))
                {
                    using (var classify = new Mapper(item))
                    {
                        result.NotCommited.Add(classify.ExpecionContable);
                        Logger.WriteLine(_dataFileLog, $"@@@{classify.ExpecionContable.Date:yyyyMMdd},{classify.ExpecionContable.Cost},{classify.ExpecionContable.ProductFamily},{classify.ExpecionContable.ProductKey},{classify.ExpecionContable.Status}");
                    }
                }
                Logger.WriteLine(_dataFileLog, $"INFO: Se claisificaron como NOT-COMMITED {result.NotCommited.Count} ");

                Logger.WriteLine(_dataFileLog, "INFO: Termina el proceso: ClasificaRegistros");
            }
            catch (Exception e)
            {
                Logger.WriteLine(_dataFileLog, $"ERROR: Ocurrio un error en ClasificaRegistros: {e.Message}");
                throw new Exception("Ocurrio un error en ClasificaRegistros");
            }

            return result;
        }

        public void EscribirSQl(IList<ExcepcionContableDto> dtos)
        {
            Logger.WriteLine(_dataFileLog, "INFO: Inicia el proceso: EscribirSQl");
            try
            {
                using (var db = new ApplicationContext())
                {
                    //Agrupamos los datos por "Year" y "ProductFamily"
                    var data = dtos.GroupBy(x => new
                    {
                        x.Date.Year,
                        x.ProductFamily
                    }).Select(x => new ExcepcionContable
                    {
                        Year = x.Key.Year,
                        ProductFamily = x.Key.ProductFamily,
                        Balance = x.Sum(y => y.Cost),
                        TransactionCount = x.Count(),
                        ReviewDate = _today
                    });

                    //Almacenamos los datos
                    db.ExceptionContables.AddRange(data);
                    db.SaveChanges();
                }
                Logger.WriteLine(_dataFileLog, "INFO: Termina el proceso: EscribirSQl");
            }
            catch (Exception e)
            {
                Logger.WriteLine(_dataFileLog, $"ERROR: Ocurrio un error en EscribirSQl: {e.Message}");
                throw new Exception("Ocurrio un error en EscribirSQl");
            }
        }


        public void EscribirFileSystem(IList<ExcepcionContableDto> commited, IList<ExcepcionContableDto> notCommited)
        {
            try
            {
                Logger.WriteLine(_dataFileLog, "INFO: Comienza el proceso: EscribirFileSystem:Commited");

                XElement xmlNotCommited = new XElement("ExcepcionContable", notCommited.Select(
                    i => new XElement("not-commited",
                        new XAttribute("Date", i.Date),
                        new XAttribute("Cost", i.Cost),
                        new XAttribute("Status", i.Status),
                        new XAttribute("ProductFamily", i.ProductFamily),
                        new XAttribute("ProductKey", i.ProductKey)
                    )));

                xmlNotCommited.Save($"{_xmlDirectory}/0_{_today}.xml");
                Logger.WriteLine(_dataFileLog, "INFO: Termina el proceso: EscribirFileSystem:Commited");

                Logger.WriteLine(_dataFileLog, "INFO: Comienza el proceso: EscribirFileSystem:Not-Commited");
                XElement xmlCommited = new XElement("ExcepcionContable", commited.Select(
                    i => new XElement("commited",
                        new XAttribute("Date", i.Date),
                        new XAttribute("Cost", i.Cost),
                        new XAttribute("Status", i.Status),
                        new XAttribute("ProductFamily", i.ProductFamily),
                        new XAttribute("ProductKey", i.ProductKey)
                    )));

                xmlCommited.Save($"{_xmlDirectory}/1_{_today}.xml");
                Logger.WriteLine(_dataFileLog, "INFO: Termina el proceso: EscribirFileSystem:Not-Commited");
            }
            catch (Exception e)
            {
                Logger.WriteLine(_dataFileLog, $"ERROR: Ocurrio un error en EscribirFileSystem: {e.Message}");
                throw new Exception("Ocurrio un error en EscribirFileSystem");
            }
            Logger.WriteLine(_dataFileLog, "INFO: Termina el proceso: EscribirFileSystem");
        }

    }
}
