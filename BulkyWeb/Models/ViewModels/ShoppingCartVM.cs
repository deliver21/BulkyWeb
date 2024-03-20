using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyWeb.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; } 
        public OrderHeader OrderHeader { get; set; }
        
    }
}
