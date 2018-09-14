using System;

namespace Cosme_EjercicioTecnico.Dto
{
    public class ExcepcionContableFileDto
    {
        public string ProductKey { get; set; }
        public decimal Cost { get; set; }
        public string Transaction { get; set; }
        public string Status { get; set; }
        public string Commited { get; set; }        
        public DateTime Date { get; set; }
    }
}