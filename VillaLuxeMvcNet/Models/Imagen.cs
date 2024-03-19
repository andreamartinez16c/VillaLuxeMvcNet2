using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillaLuxeMvcNet.Models
{
    [Table("Imagenes")]
    public class Imagen
    {
        [Key]
        [Column("idImagen")]
        public int IdImagen { get; set; }

        [Column("idVilla")]
        public int IdVilla { get; set; }

        [Column("imagen")]
        public string Imgn { get; set; }
    }
}
