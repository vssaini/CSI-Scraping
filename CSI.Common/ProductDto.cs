namespace CSI.Common
{
    public class ProductDto
    {
        public string Id { get; set; }

        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Source { get; set; }

        public string Status { get; set; }
        public string IsCamera => string.IsNullOrWhiteSpace(Name) ? string.Empty : Name.Contains("Camera") ? "Yes" : "No";

        public ProductDto()
        {
            Status = Constants.StatusNotFound;
        }
    }
}
