using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VillaLuxeMvcNet.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("idusuario")]
        public int IdUsuario { get; set; }
        [Column("nombre")]
        public string Nombre { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("contrasenia")]
        public byte[] Contrasenia { get; set; }
        [Column("salt")]
        public string Salt { get; set; }
        [Column("telefono")]
        public string Telefono { get; set; }
        [Column("idrol")]
        public int IdRol { get; set; }
    }
}
