namespace OnlinePharmacyAppMVC.Models
{
    public class CartPageViewModel
    {
        public List<CartModel> CartItems { get; set; } = new();
        public decimal SubTotal => CartItems.Sum(i => i.Amount);
        public decimal DiscountAmount { get; set; }
        public string DiscountCode { get; set; }
        public bool IsPercentage { get; set; }
        public decimal FinalTotal => SubTotal - DiscountAmount;
    }
}
