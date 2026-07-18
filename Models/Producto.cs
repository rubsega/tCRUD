using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tCRUD.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; } = Guid.CreateVersion7();

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(0.01,99999.99, ErrorMessage = "El precio debe estar entre 0.01 y 99999.99")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [NotMapped]
        public bool StockBajo => Stock < 5; 

         // ---- La relación, ahora explícita ----
        public int? CategoriaId { get; set; }
       // "mi FK es la propiedad CategoriaId"
        public Categoria? Categoria { get; set; }

        [StringLength(50)]
        public string? Descripcion { get; set; }

        
    }
}