using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tCRUD.Models
{
    public class VentaItem
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public Venta? Venta { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; } 
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        [NotMapped]
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
