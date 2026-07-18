using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tCRUD.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public List<VentaItem> Items { get; set; } = new();
        [NotMapped]
        public decimal Total => Items.Sum(i => i.Subtotal);
    }
}