﻿using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using Button = DiscordRPC.Button;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Win32;

namespace DiscordRichPresence
{
	public partial class MainForm : Form
	{
		#region Form
		public static MainForm self;
		public MainForm(string[] args)
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			self = this;
			InitializeComponent();
			panels = new Panel[]{
				MainPanel, ButtonsPanel, SettingsPanel, TimestampPanel
			};
			MouseDown += MainForm_MouseDown;
			WindowToolbar.MouseDown += MainForm_MouseDown;
			W_TitleText.MouseDown += MainForm_MouseDown;
			W_Icon.MouseDown += MainForm_MouseDown;
			if(args.Length > 0)
			{
				savePath = args[0] + savePath;
			}
			preference = GetPreference();
			if(preference != null)
			{
				SetCodeToForm();
				TryConnect();
			}
			else
			{
				preference = new Preference();
				ChangeTab(2);
			}
			StartupToggle.Checked = IsApplicationOnStartup();
			startTime = DateTime.UtcNow;
		}
		public Panel[] panels;
		public void ChangeTab(int index)
		{
			if(index < panels.Length)
			{
				for(int i = 0; i < panels.Length; i++)
				{
					panels[i].Visible = i == index;
				}
			}
		}
		void W_CloseButtonClick(object sender, EventArgs e)
		{
			if(SystemTrayToggle.Checked)
				Hide();
			else
				this.Dispose();
		}
		void ConnectButtonClick(object sender, EventArgs e)
		{
			TryConnect();
		}
		void ButtonsToggleCheckedChanged(object sender, EventArgs e)
		{
			
		}
		void DisconnectButtonClick(object sender, EventArgs e)
		{
			Disconnect();
		}
		void N_MainPanelClick(object sender, EventArgs e)
		{
			ChangeTab(0);
		}
		void N_ButtonsPanelClick(object sender, EventArgs e)
		{
			ChangeTab(1);
		}
		void N_SettingsPanelClick(object sender, EventArgs e)
		{
			ChangeTab(2);
		}
		void N_TimestampPanelClick(object sender, EventArgs e)
		{
			ChangeTab(3);
		}
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if(client != null) client.Dispose();
		}
		void MainFormMouseClick(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				Show();
			}
		}
		void CloseToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Dispose();
		}
		void OpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			Show();
		}
		void Timer1Tick(object sender, EventArgs e)
		{
			T_StartBox.Text = startTime.ToString("HH:mm:ss");
			try {
				T_ElapsedTime.Text = "Elapsed: " + (DateTime.UtcNow.Add(-(startTime.Add(-offsetStamp.TimeOfDay)).TimeOfDay)).ToString("HH:mm:ss");
			} catch {
				Console.WriteLine("Some error occured");
			}
		}
		void StartupToggleCheckedChanged(object sender, EventArgs e)
		{
			SetApplicationStartup(StartupToggle.Checked);
		}
		#endregion
		
		#region Presence
		// Client
		public static DiscordRpcClient client;
		// Connection
		public static LogLevel logLevel = LogLevel.Trace;
		public static int pipe = -1;
		// Rich Presence
		public static Preference preference;
		public static string savePath = "preferences.json";
		// Timestamp
		public static DateTime startTime;
		public static DateTime offsetStamp;
		
		public static void InitializeClient()
		{
			if(client == null) 
			{
				try 
				{
					self.ProfileStatus.Text = "Connecting";
					try 
					{
						pipe = int.Parse(self.PipeBox.Text);
					}
					catch
					{
						self.PipeBox.Text = pipe.ToString();
					}
					client = new DiscordRpcClient(preference.applicationID, pipe, new ConsoleLogger(logLevel, true), true, new DiscordRPC.IO.ManagedNamedPipeClient());
					client.OnConnectionEstablished += (a, i) => {
						pipe = i.ConnectedPipe;
						self.PipeBox.Text = pipe.ToString();
					};
					client.OnReady += client_OnReady;
					client.OnConnectionFailed += client_OnConnectionFailed;
					client.OnError += client_OnError;
					client.SetPresence(preference.presence);
					client.Initialize();
				}
				catch (Exception e)
				{
					MessageBox.Show("Error: " + e.Message, "Discord Rich Presence", MessageBoxButtons.OK, MessageBoxIcon.Error);
					SetProfile(false);
				}
			}
			else
			{
				if(!client.IsInitialized)
				{
					client.SetPresence(preference.presence);
					client.Initialize();
				}
			}
		}
		public static void UpdateClient()
		{
			self.SetFormToCode();
			client.SetPresence(preference.presence);
		}
        static void client_OnConnectionFailed(object sender, ConnectionFailedMessage args)
        {
            MessageBox.Show("Connection failed! Pipe: " + args.FailedPipe, "Discord Rich Presence", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Disconnect();
        }
        static void client_OnReady(object sender, ReadyMessage args)
        {
            SetProfile(true, args.User);
        }

        static void client_OnError(object sender, ErrorMessage args)
		{
			MessageBox.Show("Error occurred! " + args.Message, "Discord Rich Presence", MessageBoxButtons.OK, MessageBoxIcon.Error);
			Disconnect();
		}
		public static string pfpExt;
		public static User currentUser;
		public static void SetProfile(bool to, User user = null, bool forceGIF = false)
		{
			currentUser = user;
			self.ProfileName.Text = to ? user.Username + "#" + user.Discriminator : "N/A";
			self.ProfileStatus.Text = to ? "Connected" : "Disconnected";
			if(to)
			{
				var g = user.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x1024);
				if(string.IsNullOrWhiteSpace(g) || forceGIF){
					self.ProfilePicture.LoadAsync(user.GetAvatarURL(User.AvatarFormat.GIF, User.AvatarSize.x1024));
					pfpExt = ".gif";
				}
				else
				{
					self.ProfilePicture.LoadAsync(g);
					pfpExt = ".png";
				}
			}
			else
			{
				self.ProfilePicture.Image = null;
				pfpExt = null;
			}
			self.DisconnectButton.Enabled = to;
			self.ConnectButton.Text = to ? "Update" : "Connect";
		}
		#endregion
		
		#region Functions
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool ReleaseCapture();
		private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{     
		    if (e.Button == MouseButtons.Left)
		    {
		        ReleaseCapture();
		        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
		    }
		}
		public void SetFormToCode()
		{
			preference.applicationID = AppIdBox.Text;
			preference.presence = new RichPresence();
			preference.presence.Details = DetailsBox.Text;
			preference.presence.State = StateBox.Text;
			preference.presence.Assets = new Assets();
			preference.presence.Assets.LargeImageKey = LI_NameBox.Text;
			preference.presence.Assets.LargeImageText = LI_TooltipBox.Text;
			preference.presence.Assets.SmallImageKey = SI_NameBox.Text;
			preference.presence.Assets.SmallImageText = SI_TooltipBox.Text;
			preference.buttonEnabled = ButtonsToggle.Checked;
			if(ButtonsToggle.Checked) {
				var buttons = new List<Button>();
				if (Uri.IsWellFormedUriString(TB_LinkBox.Text, UriKind.Absolute))
				{
					buttons.Add(Utility.CreateButton(TB_TextBox.Text, TB_LinkBox.Text));
				}
				if (Uri.IsWellFormedUriString(BB_LinkBox.Text, UriKind.Absolute))
				{
					buttons.Add(Utility.CreateButton(BB_TextBox.Text, BB_LinkBox.Text));
				}
				preference.presence.Buttons = buttons.ToArray();
			} else {
				preference.presence.Buttons = new Button[] { };
			}
			preference.timestampEnabled = TimestampToggle.Checked;
			preference.offset = T_OffsetBox.Text;
			if(TimestampToggle.Checked)
			{
				try
				{
					DateTime ss = DateTime.Parse(preference.offset);
					offsetStamp = ss;
					preference.presence.Timestamps = new Timestamps(startTime.Add(-offsetStamp.TimeOfDay));
				}
				catch
				{
					T_OffsetBox.Text = preference.offset;
				}
			}
			else
			{
				preference.presence.Timestamps = null;
			}
			preference.hideOnSystemTray = SystemTrayToggle.Checked;
			SavePreference();
		}
		public void SetCodeToForm()
		{
			AppIdBox.Text = preference.applicationID;
			DetailsBox.Text = preference.presence.Details;
			StateBox.Text = preference.presence.State;
			if(preference.presence.Assets != null)
			{
				LI_NameBox.Text = preference.presence.Assets.LargeImageKey;
				LI_TooltipBox.Text = preference.presence.Assets.LargeImageText;
				SI_NameBox.Text = preference.presence.Assets.SmallImageKey;
				SI_TooltipBox.Text = preference.presence.Assets.SmallImageText;
			}
			ButtonsToggle.Checked = preference.buttonEnabled;
			TB_TextBox.Text = preference.presence.Buttons.Length > 0 ? preference.presence.Buttons[0].Label : null;
			TB_LinkBox.Text = preference.presence.Buttons.Length > 0 ? preference.presence.Buttons[0].Url : null;
			BB_TextBox.Text = preference.presence.Buttons.Length > 1 ? preference.presence.Buttons[1].Label : null;
			BB_LinkBox.Text = preference.presence.Buttons.Length > 1 ? preference.presence.Buttons[1].Url : null;
			T_OffsetBox.Text = preference.offset;
			TimestampToggle.Checked = preference.timestampEnabled;
			SystemTrayToggle.Checked = preference.hideOnSystemTray;
		}
		public void TryConnect()
		{
			SetFormToCode();
			if(client == null)
				InitializeClient();
			else
				UpdateClient();
		}
		public static Preference GetPreference()
		{
			return File.Exists(savePath) ? JsonConvert.DeserializeObject<Preference>(File.ReadAllText(savePath)) : null;
		}
		public static void SavePreference()
		{
			try
			{
				File.WriteAllText(savePath, JsonConvert.SerializeObject(preference));
			}
			catch (Exception e)
			{
				MessageBox.Show("Failed to save file: " + e.Message + e.StackTrace, "Discord RPC");
			}
		}
		public static void Disconnect()
		{
			if(client != null)
			{
				SetProfile(false);
				client.Dispose();
				client = null;
			}
		}
		public static bool IsApplicationOnStartup()
		{
			RegistryKey rk = Registry.CurrentUser.OpenSubKey
	            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
			bool r = rk.GetValue("DiscordRPC", null) != null;
			rk.Close();
			return r;
		}
		public static void SetApplicationStartup(bool to)
		{
	       	RegistryKey rk = Registry.CurrentUser.OpenSubKey
	            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
	        if (to)
	            rk.SetValue("DiscordRPC", Application.ExecutablePath);
	        else
	            rk.DeleteValue("DiscordRPC", false);
	        rk.Close();
		}
		void ProfilePictureClick(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(pfpExt)) {
				var form = new Form();
				var box = new PictureBox();
				form.Controls.Add(box);
				box.Size = form.Size;
				box.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
				box.SizeMode = PictureBoxSizeMode.Zoom;
				box.Image = ProfilePicture.Image;
				form.MouseClick += (ee, aa) => {
					if (aa.Button == MouseButtons.Right) {
						var cm = new ContextMenuStrip();
						var item = cm.Items.Add("Save");
						item.Click += (i, op) => {
							var d = new SaveFileDialog();
							d.Filter = string.Format("Picture file (*{0})|*{0}", pfpExt);
							if (d.ShowDialog() == DialogResult.OK) {
								ProfilePicture.Image.Save(d.FileName);
							}
						};
						var ftpng = cm.Items.Add("Force to GIF");
						ftpng.Click += (oo, a) =>
						{
							SetProfile(true, currentUser, true);
							ProfilePicture.LoadCompleted += (o, l) => 
							{
								box.Image = ProfilePicture.Image;
							};
						};
						cm.Show(Cursor.Position);
					}
				};
				box.MouseClick += (ee, aa) => 
				{
					if (aa.Button == MouseButtons.Right) {
						var cm = new ContextMenuStrip();
						var item = cm.Items.Add("Save");
						item.Click += (i, op) => {
							var d = new SaveFileDialog();
							d.Filter = string.Format("Picture file (*{0})|*{0}", pfpExt);
							if (d.ShowDialog() == DialogResult.OK) {
								ProfilePicture.Image.Save(d.FileName);
							}
						};
						var ftpng = cm.Items.Add("Force to GIF");
						ftpng.Click += (oo, a) =>
						{
							SetProfile(true, currentUser, true);
							ProfilePicture.LoadCompleted += (o, l) => 
							{
								box.Image = ProfilePicture.Image;
							};
						};
						cm.Show(Cursor.Position);
					}
				};
				form.Icon = Icon;
				form.Text = "Profile Picture Viewer";
				form.Size = new System.Drawing.Size(512, 512);
				form.Show();
			}
		}
		void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://discord.com/developers/applications");
		}
		#endregion
	}
	public class Preference
	{
		public string applicationID;
		public int pipe = -1;
		public bool buttonEnabled;
		public bool timestampEnabled;
		public bool hideOnSystemTray = true;
		public string offset = "00:00:00";
		public RichPresence presence;
	}
	
	public static class Utility
	{
		public static Button CreateButton(string text, string url)
		{
			return new Button() {Label = text, Url = url};
		}
	}
}
