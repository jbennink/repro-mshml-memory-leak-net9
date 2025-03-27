using System;
using System.Windows.Forms;

namespace MSHTML.Test
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var form = new frmReproMSHTMLMemoryLeak();
            Application.Run(form);
        }
    }
}
