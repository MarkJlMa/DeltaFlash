namespace ASDA_Safety_Update
{
	// Token: 0x0200000D RID: 13
	public partial class FwUpdate_F : global::System.Windows.Forms.Form
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00004F50 File Offset: 0x00003150
		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004F88 File Offset: 0x00003188
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::ASDA_Safety_Update.FwUpdate_F));
			this.btnImportMCUA = new global::System.Windows.Forms.Button();
			this.btnImportMCUB = new global::System.Windows.Forms.Button();
			this.tbxPathMCUA = new global::System.Windows.Forms.TextBox();
			this.tbxPathMCUB = new global::System.Windows.Forms.TextBox();
			this.lbPath_MCUA = new global::System.Windows.Forms.Label();
			this.lbPath_MCUB = new global::System.Windows.Forms.Label();
			this.btnStart = new global::System.Windows.Forms.Button();
			this.btnDetail = new global::System.Windows.Forms.Button();
			this.tbxIP = new global::System.Windows.Forms.TextBox();
			this.lbIPAddress = new global::System.Windows.Forms.Label();
			this.messageBox = new global::System.Windows.Forms.RichTextBox();
			this.tbxCheckSum_A = new global::System.Windows.Forms.TextBox();
			this.tbxCheckSum_B = new global::System.Windows.Forms.TextBox();
			this.gbxMCUA = new global::System.Windows.Forms.GroupBox();
			this.lbCheckSumA = new global::System.Windows.Forms.Label();
			this.gbxMCUB = new global::System.Windows.Forms.GroupBox();
			this.lbCheckSumB = new global::System.Windows.Forms.Label();
			this.tbctlMode = new global::System.Windows.Forms.TabControl();
			this.tabCustomer = new global::System.Windows.Forms.TabPage();
			this.gbxDrive = new global::System.Windows.Forms.GroupBox();
			this.tbxDriveVer = new global::System.Windows.Forms.TextBox();
			this.lbFwVerDrive = new global::System.Windows.Forms.Label();
			this.gbxSafety = new global::System.Windows.Forms.GroupBox();
			this.cmbVerList = new global::System.Windows.Forms.ComboBox();
			this.lbFwVerSM = new global::System.Windows.Forms.Label();
			this.tabEngineer = new global::System.Windows.Forms.TabPage();
			this.flowTaskList = new global::System.Windows.Forms.FlowLayoutPanel();
			this.btnFlash = new global::System.Windows.Forms.Button();
			this.gbxMCUA.SuspendLayout();
			this.gbxMCUB.SuspendLayout();
			this.tbctlMode.SuspendLayout();
			this.tabCustomer.SuspendLayout();
			this.gbxDrive.SuspendLayout();
			this.gbxSafety.SuspendLayout();
			this.tabEngineer.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.btnImportMCUA, "btnImportMCUA");
			this.btnImportMCUA.Name = "btnImportMCUA";
			this.btnImportMCUA.UseVisualStyleBackColor = true;
			this.btnImportMCUA.Click += new global::System.EventHandler(this.btnImportMCUA_Click);
			componentResourceManager.ApplyResources(this.btnImportMCUB, "btnImportMCUB");
			this.btnImportMCUB.Name = "btnImportMCUB";
			this.btnImportMCUB.UseVisualStyleBackColor = true;
			this.btnImportMCUB.Click += new global::System.EventHandler(this.btnImportMCUB_Click);
			componentResourceManager.ApplyResources(this.tbxPathMCUA, "tbxPathMCUA");
			this.tbxPathMCUA.Name = "tbxPathMCUA";
			componentResourceManager.ApplyResources(this.tbxPathMCUB, "tbxPathMCUB");
			this.tbxPathMCUB.Name = "tbxPathMCUB";
			componentResourceManager.ApplyResources(this.lbPath_MCUA, "lbPath_MCUA");
			this.lbPath_MCUA.Name = "lbPath_MCUA";
			componentResourceManager.ApplyResources(this.lbPath_MCUB, "lbPath_MCUB");
			this.lbPath_MCUB.Name = "lbPath_MCUB";
			componentResourceManager.ApplyResources(this.btnStart, "btnStart");
			this.btnStart.Name = "btnStart";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new global::System.EventHandler(this.btnStart_Click);
			componentResourceManager.ApplyResources(this.btnDetail, "btnDetail");
			this.btnDetail.Name = "btnDetail";
			this.btnDetail.UseVisualStyleBackColor = true;
			this.btnDetail.Click += new global::System.EventHandler(this.btnDetail_Click);
			componentResourceManager.ApplyResources(this.tbxIP, "tbxIP");
			this.tbxIP.Name = "tbxIP";
			componentResourceManager.ApplyResources(this.lbIPAddress, "lbIPAddress");
			this.lbIPAddress.Name = "lbIPAddress";
			componentResourceManager.ApplyResources(this.messageBox, "messageBox");
			this.messageBox.Name = "messageBox";
			this.messageBox.TabStop = false;
			componentResourceManager.ApplyResources(this.tbxCheckSum_A, "tbxCheckSum_A");
			this.tbxCheckSum_A.Name = "tbxCheckSum_A";
			this.tbxCheckSum_A.ReadOnly = true;
			componentResourceManager.ApplyResources(this.tbxCheckSum_B, "tbxCheckSum_B");
			this.tbxCheckSum_B.Name = "tbxCheckSum_B";
			this.tbxCheckSum_B.ReadOnly = true;
			this.gbxMCUA.Controls.Add(this.lbCheckSumA);
			this.gbxMCUA.Controls.Add(this.tbxPathMCUA);
			this.gbxMCUA.Controls.Add(this.btnImportMCUA);
			this.gbxMCUA.Controls.Add(this.tbxCheckSum_A);
			this.gbxMCUA.Controls.Add(this.lbPath_MCUA);
			componentResourceManager.ApplyResources(this.gbxMCUA, "gbxMCUA");
			this.gbxMCUA.Name = "gbxMCUA";
			this.gbxMCUA.TabStop = false;
			componentResourceManager.ApplyResources(this.lbCheckSumA, "lbCheckSumA");
			this.lbCheckSumA.Name = "lbCheckSumA";
			this.gbxMCUB.Controls.Add(this.lbCheckSumB);
			this.gbxMCUB.Controls.Add(this.tbxPathMCUB);
			this.gbxMCUB.Controls.Add(this.btnImportMCUB);
			this.gbxMCUB.Controls.Add(this.tbxCheckSum_B);
			this.gbxMCUB.Controls.Add(this.lbPath_MCUB);
			componentResourceManager.ApplyResources(this.gbxMCUB, "gbxMCUB");
			this.gbxMCUB.Name = "gbxMCUB";
			this.gbxMCUB.TabStop = false;
			componentResourceManager.ApplyResources(this.lbCheckSumB, "lbCheckSumB");
			this.lbCheckSumB.Name = "lbCheckSumB";
			this.tbctlMode.Controls.Add(this.tabCustomer);
			this.tbctlMode.Controls.Add(this.tabEngineer);
			componentResourceManager.ApplyResources(this.tbctlMode, "tbctlMode");
			this.tbctlMode.Name = "tbctlMode";
			this.tbctlMode.SelectedIndex = 0;
			this.tbctlMode.SelectedIndexChanged += new global::System.EventHandler(this.tbctlMode_SelectedIndexChanged);
			this.tabCustomer.BackColor = global::System.Drawing.SystemColors.Control;
			this.tabCustomer.Controls.Add(this.gbxDrive);
			this.tabCustomer.Controls.Add(this.gbxSafety);
			componentResourceManager.ApplyResources(this.tabCustomer, "tabCustomer");
			this.tabCustomer.Name = "tabCustomer";
			this.gbxDrive.Controls.Add(this.tbxDriveVer);
			this.gbxDrive.Controls.Add(this.lbFwVerDrive);
			componentResourceManager.ApplyResources(this.gbxDrive, "gbxDrive");
			this.gbxDrive.Name = "gbxDrive";
			this.gbxDrive.TabStop = false;
			componentResourceManager.ApplyResources(this.tbxDriveVer, "tbxDriveVer");
			this.tbxDriveVer.Name = "tbxDriveVer";
			this.tbxDriveVer.ReadOnly = true;
			componentResourceManager.ApplyResources(this.lbFwVerDrive, "lbFwVerDrive");
			this.lbFwVerDrive.Name = "lbFwVerDrive";
			this.gbxSafety.Controls.Add(this.cmbVerList);
			this.gbxSafety.Controls.Add(this.lbFwVerSM);
			componentResourceManager.ApplyResources(this.gbxSafety, "gbxSafety");
			this.gbxSafety.Name = "gbxSafety";
			this.gbxSafety.TabStop = false;
			this.cmbVerList.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbVerList.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.cmbVerList, "cmbVerList");
			this.cmbVerList.Name = "cmbVerList";
			this.cmbVerList.TabStop = false;
			this.cmbVerList.SelectedIndexChanged += new global::System.EventHandler(this.cmbVerList_SelectedIndexChanged);
			componentResourceManager.ApplyResources(this.lbFwVerSM, "lbFwVerSM");
			this.lbFwVerSM.Name = "lbFwVerSM";
			this.tabEngineer.BackColor = global::System.Drawing.SystemColors.Control;
			this.tabEngineer.Controls.Add(this.gbxMCUA);
			this.tabEngineer.Controls.Add(this.gbxMCUB);
			componentResourceManager.ApplyResources(this.tabEngineer, "tabEngineer");
			this.tabEngineer.Name = "tabEngineer";
			componentResourceManager.ApplyResources(this.flowTaskList, "flowTaskList");
			this.flowTaskList.Name = "flowTaskList";
			componentResourceManager.ApplyResources(this.btnFlash, "btnFlash");
			this.btnFlash.Name = "btnFlash";
			this.btnFlash.UseVisualStyleBackColor = true;
			this.btnFlash.Click += new global::System.EventHandler(this.btnFlash_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.btnFlash);
			base.Controls.Add(this.flowTaskList);
			base.Controls.Add(this.btnDetail);
			base.Controls.Add(this.messageBox);
			base.Controls.Add(this.tbctlMode);
			base.Controls.Add(this.lbIPAddress);
			base.Controls.Add(this.tbxIP);
			base.Controls.Add(this.btnStart);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FwUpdate_F";
			base.FormClosing += new global::System.Windows.Forms.FormClosingEventHandler(this.FwUpdate_F_FormClosing);
			base.FormClosed += new global::System.Windows.Forms.FormClosedEventHandler(this.FwUpdate_F_FormClosed);
			this.gbxMCUA.ResumeLayout(false);
			this.gbxMCUA.PerformLayout();
			this.gbxMCUB.ResumeLayout(false);
			this.gbxMCUB.PerformLayout();
			this.tbctlMode.ResumeLayout(false);
			this.tabCustomer.ResumeLayout(false);
			this.gbxDrive.ResumeLayout(false);
			this.gbxDrive.PerformLayout();
			this.gbxSafety.ResumeLayout(false);
			this.gbxSafety.PerformLayout();
			this.tabEngineer.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000026 RID: 38
		private global::System.ComponentModel.IContainer components = null;

		// Token: 0x04000027 RID: 39
		private global::System.Windows.Forms.Button btnImportMCUA;

		// Token: 0x04000028 RID: 40
		private global::System.Windows.Forms.Button btnImportMCUB;

		// Token: 0x04000029 RID: 41
		private global::System.Windows.Forms.TextBox tbxPathMCUA;

		// Token: 0x0400002A RID: 42
		private global::System.Windows.Forms.TextBox tbxPathMCUB;

		// Token: 0x0400002B RID: 43
		private global::System.Windows.Forms.Label lbPath_MCUA;

		// Token: 0x0400002C RID: 44
		private global::System.Windows.Forms.Label lbPath_MCUB;

		// Token: 0x0400002D RID: 45
		private global::System.Windows.Forms.Button btnStart;

		// Token: 0x0400002E RID: 46
		private global::System.Windows.Forms.Button btnDetail;

		// Token: 0x0400002F RID: 47
		private global::System.Windows.Forms.TextBox tbxIP;

		// Token: 0x04000030 RID: 48
		private global::System.Windows.Forms.Label lbIPAddress;

		// Token: 0x04000031 RID: 49
		private global::System.Windows.Forms.RichTextBox messageBox;

		// Token: 0x04000032 RID: 50
		private global::System.Windows.Forms.TextBox tbxCheckSum_A;

		// Token: 0x04000033 RID: 51
		private global::System.Windows.Forms.TextBox tbxCheckSum_B;

		// Token: 0x04000034 RID: 52
		private global::System.Windows.Forms.GroupBox gbxMCUA;

		// Token: 0x04000035 RID: 53
		private global::System.Windows.Forms.Label lbCheckSumA;

		// Token: 0x04000036 RID: 54
		private global::System.Windows.Forms.GroupBox gbxMCUB;

		// Token: 0x04000037 RID: 55
		private global::System.Windows.Forms.Label lbCheckSumB;

		// Token: 0x04000038 RID: 56
		private global::System.Windows.Forms.TabControl tbctlMode;

		// Token: 0x04000039 RID: 57
		private global::System.Windows.Forms.TabPage tabCustomer;

		// Token: 0x0400003A RID: 58
		private global::System.Windows.Forms.GroupBox gbxDrive;

		// Token: 0x0400003B RID: 59
		private global::System.Windows.Forms.TabPage tabEngineer;

		// Token: 0x0400003C RID: 60
		private global::System.Windows.Forms.GroupBox gbxSafety;

		// Token: 0x0400003D RID: 61
		private global::System.Windows.Forms.Label lbFwVerDrive;

		// Token: 0x0400003E RID: 62
		private global::System.Windows.Forms.Label lbFwVerSM;

		// Token: 0x0400003F RID: 63
		private global::System.Windows.Forms.TextBox tbxDriveVer;

		// Token: 0x04000040 RID: 64
		private global::System.Windows.Forms.ComboBox cmbVerList;

		// Token: 0x04000041 RID: 65
		private global::System.Windows.Forms.FlowLayoutPanel flowTaskList;

		// Token: 0x04000042 RID: 66
		private global::System.Windows.Forms.Button btnFlash;
	}
}
