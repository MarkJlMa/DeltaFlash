using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ASDA_Safety_Update
{
	// Token: 0x02000010 RID: 16
	public class UpdateControl : UserControl
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00005E40 File Offset: 0x00004040
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00005E22 File Offset: 0x00004022
		public int ProgressValue
		{
			get
			{
				return this.progressValue;
			}
			set
			{
				this.progressValue = value;
				this.progressBar1.Value = this.progressValue;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005E58 File Offset: 0x00004058
		public UpdateControl()
		{
			this.InitializeComponent();
			this.progressBar1.Maximum = 100;
			this.progressBar1.Minimum = 0;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005E92 File Offset: 0x00004092
		private void TaskControl_Load(object sender, EventArgs e)
		{
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005E98 File Offset: 0x00004098
		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00005ED0 File Offset: 0x000040D0
		private void InitializeComponent()
		{
			this.progressBar1 = new ProgressBar();
			this.gbxName = new GroupBox();
			this.gbxName.SuspendLayout();
			base.SuspendLayout();
			this.progressBar1.Location = new Point(16, 21);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new Size(520, 23);
			this.progressBar1.TabIndex = 1;
			this.gbxName.Controls.Add(this.progressBar1);
			this.gbxName.Location = new Point(3, 3);
			this.gbxName.Name = "gbxName";
			this.gbxName.Size = new Size(547, 55);
			this.gbxName.TabIndex = 2;
			this.gbxName.TabStop = false;
			this.gbxName.Text = "groupBox1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.gbxName);
			base.Name = "UpdateControl";
			base.Size = new Size(553, 60);
			base.Load += this.TaskControl_Load;
			this.gbxName.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		// Token: 0x04000048 RID: 72
		private int progressValue = 0;

		// Token: 0x04000049 RID: 73
		private IContainer components = null;

		// Token: 0x0400004A RID: 74
		public ProgressBar progressBar1;

		// Token: 0x0400004B RID: 75
		public GroupBox gbxName;
	}
}
