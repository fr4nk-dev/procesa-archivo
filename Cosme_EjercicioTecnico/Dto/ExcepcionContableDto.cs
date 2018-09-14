using System;

namespace Cosme_EjercicioTecnico.Dto
{
    public class ExcepcionContableDto
    {
        public string ProductKey { get; set; }
        public string ProductFamily { get; set; }
        public DateTime Date { get; set; }
        public decimal Cost { get; set; }        
        public string Status { get; set; }
        
    }
}
