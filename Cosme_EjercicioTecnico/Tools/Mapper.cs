using Cosme_EjercicioTecnico.Dto;
using System;

namespace Cosme_EjercicioTecnico.Tools
{
    public class Mapper : IDisposable
    {
        public ExcepcionContableDto ExpecionContable { get; }
        private readonly ExcepcionContableFileDto _excepcionContableFileDto;
        public Mapper(ExcepcionContableFileDto excepcionContableDto)
        {
            _excepcionContableFileDto = excepcionContableDto;

            //Datos por default
            ExpecionContable = new ExcepcionContableDto
            {
                Date = _excepcionContableFileDto.Date,
                ProductKey = _excepcionContableFileDto.ProductKey
            };
            

            //Reglas de nregocio para la transformacion
            ParseTransaction();
            ParseProductFamily();            
            ParseStatus();
        }

        private void ParseTransaction()
        {
            try
            {
                if (_excepcionContableFileDto.Transaction.ToUpper() == "Compra".ToUpper() ||
                    _excepcionContableFileDto.Transaction.ToUpper() == "Devolucion".ToUpper())
                    ExpecionContable.Cost = -1m * _excepcionContableFileDto.Cost;
                else
                    ExpecionContable.Cost = _excepcionContableFileDto.Cost;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        private void ParseProductFamily()
        {
            try
            {
                ExpecionContable.ProductFamily = TextTool.IsDigit(_excepcionContableFileDto.ProductKey[0])
                ? "Productos"
                : "Servicios";
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void ParseStatus()
        {
            try
            {
                switch (_excepcionContableFileDto.Status)
                {
                    case "0":
                        ExpecionContable.Status = "Nuevo";
                        break;
                    case "1":
                        ExpecionContable.Status = "Usado";
                        break;
                    case "2":
                        ExpecionContable.Status = "Reconstruido";
                        break;
                    case "3":
                        ExpecionContable.Status = "Restaurado";
                        break;
                    default:
                        ExpecionContable.Status = "N/A";
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
        }
    }
}
