using System;
using System.IO;
using System.Windows.Forms;
using DiscordRPC;
using Microsoft.Win32;

namespace DiscordRPCCustom
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private static DiscordRpcClient client;
		private static DiscordRPC.Logging.LogLevel logLevel = DiscordRPC.Logging.LogLevel.Trace;
		private static int discordPipe = -1;
		private static bool isButtonEnabled;
		private static bool isClientInitialized;
		private static string currentApplicationID;
		private static bool isChanged;
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
				textBox8.Text = splittedRead[8];
				textBox9.Text = splittedRead[9];
				textBox10.Text = splittedRead[10];
				bool setUp = bool.Parse(splittedRead[12]);
				checkBox3.Checked = setUp;
				isButtonEnabled = setUp;
				ButtonGroupChange(setUp);
				connectStartup();
				textBox11.Text = splittedRead[11];
			}
			RegistryKey rk = Registry.CurrentUser.OpenSubKey
	            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
	
			if (rk.GetValue("Simple Discord RPC", "No Path") != "No Path"){
				checkBox2.Checked = true;
			}
	        
	        rk.Close();
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void Button1Click(object sender, EventArgs e)
		{
			if(!isClientInitialized || isChanged){
				client = new DiscordRpcClient(textBox1.Text, pipe: discordPipe){
                	Logger = new DiscordRPC.Logging.ConsoleLogger(logLevel, true)
            	};
				Assets assets = new Assets();
				assets.LargeImageKey = textBox3.Text;
				assets.LargeImageText = textBox5.Text;
				assets.SmallImageKey = textBox7.Text;
				assets.SmallImageText = textBox6.Text;
				RichPresence richPresence = new RichPresence();
				richPresence.Details = textBox2.Text;
				richPresence.State = textBox4.Text;
				richPresence.Assets = assets;
				DiscordRPC.Button buttons1 = MakeButton(textBox8.Text, textBox9.Text);
				DiscordRPC.Button buttons2 = MakeButton(textBox10.Text, textBox11.Text);
				if(isButtonEnabled){
					richPresence.Buttons = new DiscordRPC.Button[]{buttons1, buttons2};
				}
				if(checkBox1.Checked){
					richPresence.Timestamps = new Timestamps()
					{
						Start = DateTime.UtcNow
					};
				}
				client.OnConnectionFailed += (senderi, msg) =>
				{
					label7.Text = "Connection failed : " + msg.ToString();
				};
				client.OnReady += (senderi, msg) =>
				{
					ChangeLabel("Discord RPC connected as : " + msg.User.Username + "#" +  msg.User.Discriminator);
					pictureBox1.Load(msg.User.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x32));
				};
				client.SetPresence(richPresence);
				client.Initialize();
				isClientInitialized = true;
				isChanged = false;
				label7.Text = "Connecting to Discord RPC...";
				button1.Text = "Update";
			}else{
				Assets assets = new Assets();
				assets.LargeImageKey = textBox3.Text;
				assets.LargeImageText = textBox5.Text;
				RichPresence richPresence = new RichPresence();
				richPresence.Details = textBox2.Text;
				richPresence.State = textBox4.Text;
				richPresence.Assets = assets;
				DiscordRPC.Button buttons1 = MakeButton(textBox8.Text, textBox9.Text);
				DiscordRPC.Button buttons2 = MakeButton(textBox10.Text, textBox11.Text);
				if(isButtonEnabled){
					richPresence.Buttons = new DiscordRPC.Button[]{buttons1, buttons2};
				}
				if(checkBox1.Checked){
					richPresence.Timestamps = new Timestamps()
					{
						Start = DateTime.UtcNow
					};
				}
				client.SetPresence(richPresence);
			}
			string[] separatedData = {
				textBox1.Text,
				"|",
				textBox2.Text,
				"|",
				textBox3.Text,
				"|",
				textBox4.Text,
				"|",
				textBox5.Text,
				"|",
				checkBox1.Checked.ToString(),
				"|",
				textBox6.Text,
				"|",
				textBox7.Text,
				"|",
				textBox8.Text,
				"|",
				textBox9.Text,
				"|",
				textBox10.Text,
				"|",
				textBox11.Text,
				"|",
				checkBox3.Checked.ToString()
			};
			string saveData = String.Join("",separatedData);
			File.WriteAllText("preferences.ini", saveData);
		}
		private static bool nullDetect(object thing){
			bool asDetect = false;
			if(thing == null){
				asDetect = true;
			}
			return asDetect;
		}
		void connectStartup(){
			client = new DiscordRpcClient(textBox1.Text, pipe: discordPipe, logger: new DiscordRPC.Logging.ConsoleLogger(logLevel, true), autoEvents: true,client: new DiscordRPC.IO.ManagedNamedPipeClient());
			Assets assets = new Assets();
			assets.LargeImageKey = textBox3.Text;
			assets.LargeImageText = textBox5.Text;
			assets.SmallImageKey = textBox7.Text;
			assets.SmallImageText = textBox6.Text;
			RichPresence richPresence = new RichPresence();
			richPresence.Details = textBox2.Text;
			richPresence.State = textBox4.Text;
			richPresence.Assets = assets;
			DiscordRPC.Button buttons1 = MakeButton(textBox8.Text, textBox9.Text);
			DiscordRPC.Button buttons2 = MakeButton(textBox10.Text, textBox11.Text);
			if(isButtonEnabled){
				richPresence.Buttons = new DiscordRPC.Button[]{buttons1, buttons2};
			}
			if(checkBox1.Checked){
				richPresence.Timestamps = new Timestamps()
				{
					Start = DateTime.UtcNow
				};
			}
			client.OnConnectionFailed += (sender, msg) =>
			{
				ChangeLabel("Connection failed at : " + msg.FailedPipe);
			};
			client.OnReady += (sender, msg) =>
			{
				ChangeLabel("Discord RPC connected as : " + msg.User.Username + "#" + msg.User.Discriminator);
				pictureBox1.Load(msg.User.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x32));
			};
			currentApplicationID = textBox1.Text;
			client.SetPresence(richPresence);
			client.Initialize();
			isClientInitialized = true;
			isChanged = false;
			label7.Text = "Connecting to Discord RPC...";
			button1.Text = "Update";
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
		void ChangeLabel(string to){
			label7.Text = to;
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
		//this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(MainForm_FormClosed);
		private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{     
		    if (e.Button == MouseButtons.Left)
		    {
		        ReleaseCapture();
		        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
		    }
		}
		void TextBox1TextChanged(object sender, EventArgs e)
		{
			if(isClientInitialized){
				if(textBox1.Text == currentApplicationID){
					button1.Text = "Update";
					isChanged = false;
				}else{
					button1.Text = "Connect";
					isChanged = true;
				}
			}
		}
		void CheckBox2CheckedChanged(object sender, EventArgs e)
		{
	       	RegistryKey rk = Registry.CurrentUser.OpenSubKey
	            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
	
	        if (checkBox2.Checked)
	            rk.SetValue("Simple Discord RPC", Application.ExecutablePath);
	        else
	            rk.DeleteValue("Simple Discord RPC",false);
	        
	        rk.Close();
		
		}
		void Button4Click(object sender, EventArgs e)
		{
			panel1.Visible = true;
			panel2.Visible = false;
		}
		void Button5Click(object sender, EventArgs e)
		{
			panel1.Visible = false;
			panel2.Visible = true;
		}
		void CheckBox3CheckedChanged(object sender, EventArgs e)
		{
			isButtonEnabled = checkBox3.Checked;
			ButtonGroupChange(checkBox3.Checked);
		}
		void ButtonGroupChange(bool to){
			textBox8.ReadOnly = !to;
			textBox9.ReadOnly = !to;
			textBox10.ReadOnly = !to;
			textBox11.ReadOnly = !to;
		}
		public DiscordRPC.Button MakeButton(string label, string url){
			DiscordRPC.Button xyz = new DiscordRPC.Button();
			xyz.Label = label;
			if(Uri.IsWellFormedUriString(url, UriKind.Absolute)){
				xyz.Url = url;
			}else{
				xyz.Url = "https://pastebin.com/raw/N1YKnQHP";
			}
			return xyz;
		}
		void TextBox11TextChanged(object sender, EventArgs e)
		{
			string url = textBox11.Text;
			if(Uri.IsWellFormedUriString(url, UriKind.Absolute)){
				label18.Text = "This is URI verified.";
			}else{
				label18.Text = "This is not an URI";
			}
		}
		void TextBox9TextChanged(object sender, EventArgs e)
		{
			string url = textBox9.Text;
			if(Uri.IsWellFormedUriString(url, UriKind.Absolute)){
				label18.Text = "This is URI verified.";
			}else{
				label18.Text = "This is not an URI";
			}
		}
	}
}
