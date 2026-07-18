using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tCRUD.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public Guid Folio { get; set; } = Guid.CreateVersion7();
        public DateTime Fecha { get; set; } = DateTime.Now;
        public List<CompraItem> Items { get; set; } = new();
        [NotMapped]
        public decimal Total => Items.Sum(i => i.Subtotal);
    }
}