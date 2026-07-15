using System.ComponentModel.DataAnnotations;

namespace tCRUD.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(0.01,99999.99, ErrorMessage = "El precio debe estar entre 0.01 y 99999.99")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [StringLength(50)]
        public string? Categoria { get; set; }

        [StringLength(50)]
        public string? Descripcion { get; set; }
    }
}