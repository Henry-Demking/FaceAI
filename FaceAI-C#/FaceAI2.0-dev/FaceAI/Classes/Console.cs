using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceAI.Classes
{
    class Console
    {
        ListBox console;

        public Console(ListBox console)
        {
            this.console = console;
        }

        public void Out(string message)
        {
            if(console.Items.Count > 150)
            {
                console.Items.RemoveAt(0);
            }
            console.Items.Add(message);
        }

        public void Clear(string message = "Ready")
        {
            console.Items.Clear();
            console.Items.Add(message);
        }
    }
}
