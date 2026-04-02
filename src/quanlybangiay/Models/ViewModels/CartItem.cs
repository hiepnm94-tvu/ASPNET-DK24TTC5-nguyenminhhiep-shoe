namespace quanlybangiay.Models.ViewModels
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? SizeValue { get; set; }
        public string? ColorName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
