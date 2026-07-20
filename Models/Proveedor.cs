using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tCRUD.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>(); //Muchos a muchos

        [StringLength(100)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
    }
}