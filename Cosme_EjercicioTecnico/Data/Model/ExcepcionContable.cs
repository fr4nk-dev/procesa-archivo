namespace Cosme_EjercicioTecnico.Data.Model
{
    public class ExcepcionContable
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string ProductFamily { get; set; }
        public decimal Balance { get; set; }
        public int TransactionCount { get; set; }
        public string ReviewDate { get; set; }
    }
}
