using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VillaLuxeMvcNet.Models
{
    [Table("v_villas")]
    public class Villa
    {
        [Key]
        [Column("idvilla")]
        public int IdVilla { get; set; }

        [Column("nombre_villa")]
        public string Nombre { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("comodidades")]
        public string Comodidades { get; set; }

        [Column("personas")]
        public int Personas { get; set; }

        [Column("numhabitaciones")]
        public int NumHabitaciones { get; set; }

        [Column("numbanios")]
        public int NumBanios { get; set; }

        [Column("ubicacion")]
        public string Ubicacion { get; set; }

        [Column("precionoche")]
        public decimal PrecioNoche { get; set; }

        [Column("nombre_provincia")]
        public string NombreProvincia { get; set; }

        [Column("imagen")]
        public string Imagenes { get; set; }

        [Column("imagencollage")]
        public string ImageneCollage { get; set; }
    }
}
