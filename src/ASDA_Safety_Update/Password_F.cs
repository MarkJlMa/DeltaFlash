using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ASDA_Safety_Update
{
	// Token: 0x0200000E RID: 14
	public partial class Password_F : Form
	{
		// Token: 0x06000052 RID: 82 RVA: 0x00005A05 File Offset: 0x00003C05
		public Password_F(string cultureStr = "EN")
		{
			this.InitializeComponent();
			Localize.SetCulture(cultureStr);
			Localize.SetIniFilePath(Localize.IniFilePath);
			Localize.ApplyCultureToGUI(this);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005A38 File Offset: 0x00003C38
		private void btnCheck_Click(object sender, EventArgs e)
		{
			bool flag = this.tbxPwd.Text != "confirmupdate";
			if (flag)
			{
				MessageBox.Show(Localize.GetMessageCultureStr("strPwdError", "Password Error."));
				this.tbxPwd.Clear();
			}
			else
			{
				base.DialogResult = DialogResult.OK;
				base.Close();
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00005A94 File Offset: 0x00003C94
		private void tbxPwd_KeyDown(object sender, KeyEventArgs e)
		{
			bool flag = e.KeyCode == Keys.Return;
			if (flag)
			{
				this.btnCheck_Click(null, null);
			}
		}

		// Token: 0x04000043 RID: 67
		private const string pwd = "confirmupdate";
	}
}
