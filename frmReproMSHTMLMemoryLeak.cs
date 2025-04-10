using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace MSHTML.Test
{
    public partial class frmReproMSHTMLMemoryLeak : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Url { get; set; } = "";

        private readonly WebBrowser _browser;

        public frmReproMSHTMLMemoryLeak()
        {
            InitializeComponent();

            _browser = new WebBrowser();
            Controls.Add(_browser);

            _browser.Left = 0;
            _browser.Top = 0;
            _browser.Size = ClientSize;
            _browser.DocumentCompleted += WebBrowser_DocumentCompleted;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // We do everything inside the DocumentCompleted to simplify the repro for this issue
            // In reality the navigates activate outside this method and are controlled by the user 
            // pressing a button, but this is only to get this behaviour/issue as fast as possible

            // We get the field and iterate over to set values from the database
            // Here we only iterate and do nothing, which seems enough to cause the issue
            var elements = _browser.Document.GetElementsByTagName("INPUT");
            foreach (HtmlElement element in elements)
            {
                // Do Nothing, just iterate over the elements to get the memory leak.
            }

            // ISSUE: https://github.com/dotnet/winforms/issues/13195
            // PATCH: by JeremyKuhne
            // This seems to solve the issue (even without the WaitForPendingFinalizers() but in this repro this generates
            // tons of GC's, in the production code we might do this in the DocumentCompleted since we only expect 1 or very few
            // navigates during a call of the Agent
            GC.Collect();
            GC.WaitForPendingFinalizers();

            _browser.Navigate(Url);

            Application.DoEvents();
        }

        private void FrmTest_Load(object sender, System.EventArgs e)
        {
            string filePath = Path.GetFullPath("test.html");
            Url = filePath;

            _browser.Navigate(Url);
        }
    }
}
