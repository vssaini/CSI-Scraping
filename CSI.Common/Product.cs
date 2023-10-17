namespace CSI.Common
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }

        public string Status { get; set; }
        public string IsCamera => string.IsNullOrWhiteSpace(Name) ? "No" : Name.Contains("Camera") ? "Yes" : "No";

        public Product()
        {
            Status = Constants.StatusNotFound;
        }
    }
}
