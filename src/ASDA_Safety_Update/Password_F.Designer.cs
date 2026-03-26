namespace ASDA_Safety_Update
{
	// Token: 0x0200000E RID: 14
	public partial class Password_F : global::System.Windows.Forms.Form
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00005ABC File Offset: 0x00003CBC
		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00005AF4 File Offset: 0x00003CF4
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::ASDA_Safety_Update.Password_F));
			this.lbPWD = new global::System.Windows.Forms.Label();
			this.tbxPwd = new global::System.Windows.Forms.TextBox();
			this.btnCheck = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.lbPWD.AutoSize = true;
			this.lbPWD.Font = new global::System.Drawing.Font("Arial", 9f);
			this.lbPWD.Location = new global::System.Drawing.Point(12, 22);
			this.lbPWD.Name = "lbPWD";
			this.lbPWD.Size = new global::System.Drawing.Size(69, 15);
			this.lbPWD.TabIndex = 0;
			this.lbPWD.Text = "Password :";
			this.tbxPwd.Font = new global::System.Drawing.Font("Arial", 9f);
			this.tbxPwd.Location = new global::System.Drawing.Point(85, 19);
			this.tbxPwd.Name = "tbxPwd";
			this.tbxPwd.PasswordChar = '*';
			this.tbxPwd.Size = new global::System.Drawing.Size(195, 21);
			this.tbxPwd.TabIndex = 1;
			this.tbxPwd.KeyDown += new global::System.Windows.Forms.KeyEventHandler(this.tbxPwd_KeyDown);
			this.btnCheck.Font = new global::System.Drawing.Font("Arial", 9f);
			this.btnCheck.Location = new global::System.Drawing.Point(101, 55);
			this.btnCheck.Name = "btnCheck";
			this.btnCheck.Size = new global::System.Drawing.Size(75, 28);
			this.btnCheck.TabIndex = 2;
			this.btnCheck.Text = "OK";
			this.btnCheck.UseVisualStyleBackColor = true;
			this.btnCheck.Click += new global::System.EventHandler(this.btnCheck_Click);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(296, 97);
			base.Controls.Add(this.btnCheck);
			base.Controls.Add(this.tbxPwd);
			base.Controls.Add(this.lbPWD);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Password_F";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Input Password";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000044 RID: 68
		private global::System.ComponentModel.IContainer components = null;

		// Token: 0x04000045 RID: 69
		private global::System.Windows.Forms.Label lbPWD;

		// Token: 0x04000046 RID: 70
		private global::System.Windows.Forms.TextBox tbxPwd;

		// Token: 0x04000047 RID: 71
		private global::System.Windows.Forms.Button btnCheck;
	}
}
