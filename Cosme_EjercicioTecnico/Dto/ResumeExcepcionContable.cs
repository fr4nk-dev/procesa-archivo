using System.Collections.Generic;

namespace Cosme_EjercicioTecnico.Dto
{
    public class ResumeExcepcionContable
    {
        public List<ExcepcionContableDto> Commited { get; set; }
        public List<ExcepcionContableDto> NotCommited { get; set; }
        public List<ExcepcionContableDto> Before2016 { get; set; }

    }
}