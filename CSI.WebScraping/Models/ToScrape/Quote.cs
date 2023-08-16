namespace CSI.WebScraping.Models.ToScrape
{
    public class Quote
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public override string ToString()
        {
            return Author + " says, " + Text;
        }
    }
}
