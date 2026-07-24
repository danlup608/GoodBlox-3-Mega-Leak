using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Good3dLauncher
{
	// Token: 0x02000002 RID: 2
	public partial class BootstrapForm : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public BootstrapForm()
		{
			this.Text = "Good3d Launcher";
			base.Width = 500;
			base.Height = 280;
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.BackColor = Color.White;
			base.Icon = this.LoadIconFromFile();
			this.logoPictureBox = new PictureBox();
			this.logoPictureBox.Location = new Point(20, 20);
			this.logoPictureBox.Size = new Size(64, 64);
			this.logoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
			Icon icon = this.LoadIconFromFile();
			if (icon != null)
			{
				this.logoPictureBox.Image = icon.ToBitmap();
			}
			base.Controls.Add(this.logoPictureBox);
			this.statusLabel = new Label();
			this.statusLabel.Location = new Point(100, 30);
			this.statusLabel.Size = new Size(360, 50);
			this.statusLabel.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
			this.statusLabel.ForeColor = Color.FromArgb(60, 60, 60);
			this.statusLabel.Text = "Starting...";
			this.statusLabel.AutoSize = false;
			base.Controls.Add(this.statusLabel);
			this.progressBar = new ProgressBar();
			this.progressBar.Location = new Point(20, 140);
			this.progressBar.Size = new Size(440, 25);
			this.progressBar.Style = ProgressBarStyle.Continuous;
			this.progressBar.ForeColor = Color.LimeGreen;
			base.Controls.Add(this.progressBar);
			base.TopMost = true;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002268 File Offset: 0x00000468
		public void UpdateStatus(string statusText, int progressValue)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new Action(delegate()
				{
					this.UpdateStatus(statusText, progressValue);
				}));
			}
			else
			{
				this.statusLabel.Text = statusText;
				this.progressBar.Value = Math.Min(100, Math.Max(0, progressValue));
				Application.DoEvents();
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000022F8 File Offset: 0x000004F8
		private Icon LoadIconFromFile()
		{
			try
			{
				using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Good3dLauncher.logo.ico"))
				{
					if (manifestResourceStream != null)
					{
						return new Icon(manifestResourceStream);
					}
				}
			}
			catch
			{
			}
			return SystemIcons.Application;
		}

		// Token: 0x04000001 RID: 1
		private Label statusLabel;

		// Token: 0x04000002 RID: 2
		private ProgressBar progressBar;

		// Token: 0x04000003 RID: 3
		private PictureBox logoPictureBox;
	}
}
