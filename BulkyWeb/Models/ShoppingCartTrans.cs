namespace BulkyWeb.Models
{
    public class ShoppingCartTrans
    {
        public static int Id { get; set; }
        public static int ProductId { get; set; }
        public  static int ProductCount { get; set; }
        public static string ApplicationUserId { get; set; }
        public static Product Product { get; set; }
    }
}
