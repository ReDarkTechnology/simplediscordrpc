using System.Windows.Forms;

namespace DiscordRichPresence
{
    public partial class ConsoleForm : Form
    {
        public ConsoleForm()
        {
            InitializeComponent();
        }

        public void Write(object text)
        {
            consoleBox.Text += text.ToString();
        }

        public void WriteLine(object line)
        {
            consoleBox.Text += string.IsNullOrWhiteSpace(consoleBox.Text) ? line.ToString() : "\n" + line.ToString();
        }
    }
}
