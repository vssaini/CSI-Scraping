using System.Windows.Forms;

namespace CSI.Scrapper
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void linkLblWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLblWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start("https://everconnectds.com");
        }
    }
}
