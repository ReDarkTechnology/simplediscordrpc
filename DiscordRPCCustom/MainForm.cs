using System;
using System.IO;
using System.Windows.Forms;
using DiscordRPC;

namespace DiscordRPCCustom
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private static DiscordRpcClient client;
		private static bool isClientInitialized;
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			if(File.Exists("preferences.ini")){
				string fileRead = File.ReadAllText("preferences.ini");
				string[] splittedRead = fileRead.Split(new char[] {'|'});
				textBox1.Text = splittedRead[0];
				textBox2.Text = splittedRead[1];
				textBox3.Text = splittedRead[2];
				textBox4.Text = splittedRead[3];
				textBox5.Text = splittedRead[4];
				checkBox1.Checked = bool.Parse(splittedRead[5]);
				textBox6.Text = splittedRead[6];
				textBox7.Text = splittedRead[7];
			}
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void Button1Click(object sender, EventArgs e)
		{
			if(!isClientInitialized){
				client = new DiscordRpcClient(textBox1.Text);
				Assets assets = new Assets();
				assets.LargeImageKey = textBox3.Text;
				assets.LargeImageText = textBox5.Text;
				assets.SmallImageKey = textBox7.Text;
				assets.SmallImageText = textBox6.Text;
				RichPresence richPresence = new RichPresence();
				richPresence.Details = textBox2.Text;
				richPresence.State = textBox4.Text;
				richPresence.Assets = assets;
				if(checkBox1.Checked){
					richPresence.Timestamps = new Timestamps()
					{
						Start = DateTime.UtcNow
					};
				}
				client.SetPresence(richPresence);
				client.Initialize();
				isClientInitialized = true;
				label7.Text = "Discord RPC is running";
				button1.Text = "Update";
			}else{
				Assets assets = new Assets();
				assets.LargeImageKey = textBox3.Text;
				assets.LargeImageText = textBox5.Text;
				RichPresence richPresence = new RichPresence();
				richPresence.Details = textBox2.Text;
				richPresence.State = textBox4.Text;
				richPresence.Assets = assets;
				if(checkBox1.Checked){
					richPresence.Timestamps = new Timestamps()
					{
						Start = DateTime.UtcNow
					};
				}
				client.SetPresence(richPresence);
			}
			string saveData = textBox1.Text + "|" + textBox2.Text + "|" + textBox3.Text + "|" + textBox4.Text + "|" + textBox5.Text + "|" + checkBox1.Checked.ToString() + "|" + textBox6.Text + "|" + textBox7.Text;
			File.WriteAllText("preferences.ini", saveData);
		}
		void NotifyIcon1MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Show();
			notifyIcon1.Visible = false;
		}
		void Button2Click(object sender, EventArgs e)
		{
			Hide();
			notifyIcon1.Visible = true;
		}
		void Button3Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool ReleaseCapture();
		//this.MouseDown += this.MainForm_MouseDown;
		private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{     
		    if (e.Button == MouseButtons.Left)
		    {
		        ReleaseCapture();
		        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
		    }
		}
	}
}
