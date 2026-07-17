using System.ComponentModel.DataAnnotations;

namespace tCRUD.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public List<Producto> Productos { get; set; } = new();
    }
}