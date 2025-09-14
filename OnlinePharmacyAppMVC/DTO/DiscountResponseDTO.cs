namespace OnlinePharmacyAppMVC.DTO
{
    public class DiscountResponseDTO
    {
        public decimal DiscountAmount { get; set; }
        public string DiscountCode { get; set; }
        public bool IsPercentage { get; set; }
    }
}
