using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillaLuxeMvcNet.Models
{
    [Table("v_misreservas")]
    public class MisReservas
    {
        [Key]
        [Column("idreserva")]
        public int IdReserva { get; set; }

        [Column("idusuario")]
        public int IdUsuario { get; set; }

        [Column("idvilla")]
        public int IdVilla { get; set; }

        [Column("fechainicio")]
        public DateTime FechaInicio { get; set; }

        [Column("fechafinal")]
        public DateTime FechaFin { get; set; }

        [Column("preciototal")]
        /*[Display(Name = "Precio Total")]*/
        public decimal PrecioTotal { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("ubicacion")]
        public string Ubicacion { get; set; }

        [Column("imagen")]
        public string Imagen { get; set; }
    }
}
