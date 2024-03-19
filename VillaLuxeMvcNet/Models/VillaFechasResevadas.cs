namespace VillaLuxeMvcNet.Models
{
    public class VillaFechasResevadas
    {
       public List<DateTime> FechasReservadas { get; set; }
       public Villa Villa { get; set; }

        public VillaFechasResevadas()
        {
            this.FechasReservadas = new List<DateTime>();
        }
    }
}
