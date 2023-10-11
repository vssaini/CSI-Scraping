namespace CSI.Common.Wesco
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }

        public string Status { get; set; }

        public Product()
        {
            Status = Constants.StatusNotFound;
        }
    }
}
