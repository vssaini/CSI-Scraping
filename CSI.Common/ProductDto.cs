namespace CSI.Common
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public string Status { get; set; }
        public string IsCamera => string.IsNullOrWhiteSpace(Name) ? "No" : Name.Contains("Camera") ? "Yes" : "No";

        public ProductDto()
        {
            Status = Constants.StatusNotFound;
        }
    }
}
