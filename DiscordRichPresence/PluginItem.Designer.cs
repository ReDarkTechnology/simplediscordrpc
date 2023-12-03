namespace DiscordRichPresence
{
    partial class PluginItem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pluginCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // pluginCheckBox
            // 
            this.pluginCheckBox.AutoSize = true;
            this.pluginCheckBox.Location = new System.Drawing.Point(7, 6);
            this.pluginCheckBox.Name = "pluginCheckBox";
            this.pluginCheckBox.Size = new System.Drawing.Size(83, 17);
            this.pluginCheckBox.TabIndex = 0;
            this.pluginCheckBox.Text = "PluginName";
            this.pluginCheckBox.UseVisualStyleBackColor = true;
            this.pluginCheckBox.CheckedChanged += new System.EventHandler(this.pluginCheckBox_CheckedChanged);
            // 
            // PluginItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.Controls.Add(this.pluginCheckBox);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.Name = "PluginItem";
            this.Size = new System.Drawing.Size(298, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox pluginCheckBox;
    }
}
