using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillaLuxeMvcNet.Models
{
    [Table("Provincias")]
    public class Provincias
    {
        [Key]

        [Column("idprovincia")]
        public int IdProvincia { get; set; }

        [Column("nombre")]
        public string Provincia { get; set; }
    }
}
