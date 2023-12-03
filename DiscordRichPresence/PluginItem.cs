using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using MoonSharp.Interpreter;

namespace DiscordRichPresence
{
    public partial class PluginItem : UserControl
    {
        public string path;
        public Script script;

        public PluginItem()
        {
            InitializeComponent();

            Load += PluginItem_Load;
        }

        private void PluginItem_Load(object sender, EventArgs e)
        {
            OnResize(sender, e);
            Parent.Resize += OnResize;
        }

        public void OnResize(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = new Size(Parent.Size.Width - 13, Size.Height);
            }
        }

        private void pluginCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CallFunction(pluginCheckBox.Checked ? "onStart" : "onStop");
        }

        public void CallFunction(string callFunction, bool restricted = false)
        {
            if (!pluginCheckBox.Checked && restricted) return;

            var func = script.Globals[callFunction];
            if (func != null) script.Call(func);
        }

        public void Prepare()
        {
            pluginCheckBox.Text = Path.GetFileNameWithoutExtension(path);
        }
    }
}
