using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ASDA_Safety_Update.Task;
using Delta.Threading;
using FtpUpdate;

namespace ASDA_Safety_Update
{
	// Token: 0x02000006 RID: 6
	public class DriveUpdateTask : BaseTask
	{
		// Token: 0x06000006 RID: 6 RVA: 0x000020E8 File Offset: 0x000002E8
		public DriveUpdateTask()
		{
			this.FwUpdateController = new BackgroundWorker
			{
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};
			this.FwUpdateController.DoWork += this.DevFwUpdateController_DoWork;
			this.FwUpdateController.ProgressChanged += this.DevFwUpdateController_ProgressChange;
			this.FwUpdateController.RunWorkerCompleted += this.DevFwUpdateController_RunWorkerCompleted;
			this.ftpController = new FTPController(this.GuiControl);
			this.ftpController.ShowMessageBox(false);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000219B File Offset: 0x0000039B
		public void SetFilePath(string filePath)
		{
			this.ImgFilePath = filePath;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021A8 File Offset: 0x000003A8
		public void DevFwUpdateController_DoWork(object sender, DoWorkEventArgs e)
		{
			UpdateGlobal.ShowUpdateMessage("\nStart Task : " + this.GuiControl.gbxName.Text, false);
			this.ftpController.LoadImgFile(this.ImgFilePath);
			this.lastDetail = string.Empty;
			this.ProcessConpleted = false;
			ThreadHelper.Start(delegate
			{
				try
				{
					this.ftpController.StartUploadProcess();
				}
				finally
				{
					this.ProcessConpleted = true;
				}
			});
			while (!this.FwUpdateController.CancellationPending)
			{
				this.CheckUpdateState();
				Thread.Sleep(1000);
				bool processConpleted = this.ProcessConpleted;
				if (processConpleted)
				{
					break;
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002244 File Offset: 0x00000444
		private void CheckUpdateState()
		{
			bool flag = this.ftpController.DetailText != this.lastDetail;
			if (flag)
			{
				string userState = this.ftpController.DetailText.Substring(this.lastDetail.Length, this.ftpController.DetailText.Length - this.lastDetail.Length - 1);
				this.lastDetail = this.ftpController.DetailText;
				this.FwUpdateController.ReportProgress(this.ftpController.ProgressValue, userState);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000022D1 File Offset: 0x000004D1
		public void DevFwUpdateController_ProgressChange(object sender, ProgressChangedEventArgs e)
		{
			this.GuiControl.progressBar1.Value = e.ProgressPercentage;
			UpdateGlobal.ShowUpdateMessage(e.UserState.ToString(), false);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002300 File Offset: 0x00000500
		public void DevFwUpdateController_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			bool flag = this.ftpController.DetailText != this.lastDetail;
			if (flag)
			{
				string message = this.ftpController.DetailText.Substring(this.lastDetail.Length, this.ftpController.DetailText.Length - this.lastDetail.Length - 1);
				UpdateGlobal.ShowUpdateMessage(message, false);
			}
			bool flag2 = e.Error != null;
			if (flag2)
			{
				string text = Localize.GetMessageCultureStr("msgDownloadError", "Error occured when download file") + " : " + e.Error.Message;
				MessageBox.Show(text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				UpdateGlobal.ShowUpdateMessage(text, true);
			}
			else
			{
				bool cancelled = e.Cancelled;
				if (cancelled)
				{
					string text2 = Localize.GetMessageCultureStr("msgUpdateCancel", "Update is cancelled") + " : " + this.debugMsg;
					MessageBox.Show(text2, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					UpdateGlobal.ShowUpdateMessage(text2, true);
				}
				else
				{
					string text3 = string.Empty;
					bool success = Regex.Match(this.ftpController.DetailText, ">> Success").Success;
					if (success)
					{
						this.GuiControl.progressBar1.Value = 100;
						bool flag3 = this.CompletedEventHandle != null;
						if (flag3)
						{
							this.CompletedEventHandle();
						}
						else
						{
							text3 = Localize.GetMessageCultureStr("msgUpdateOK", "Firmware update completed.") + "\n" + Localize.GetMessageCultureStr("msgReboot", "Please reboot.");
							MessageBox.Show(text3, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
							UpdateGlobal.ShowUpdateMessage(text3, true);
						}
					}
					else
					{
						text3 = Localize.GetMessageCultureStr("msgUpdateFailed", "Firmware update failed.") + "\n" + Localize.GetMessageCultureStr("msgFlashAgain", "Please reboot and Flash again.");
						MessageBox.Show(text3, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						UpdateGlobal.ShowUpdateMessage(text3, true);
					}
				}
			}
			bool flag4 = this.GuiControlHanle != null;
			if (flag4)
			{
				this.GuiControlHanle(true);
			}
		}

		// Token: 0x04000002 RID: 2
		private FTPController ftpController;

		// Token: 0x04000003 RID: 3
		public string ImgFilePath = string.Empty;

		// Token: 0x04000004 RID: 4
		private string lastDetail = string.Empty;

		// Token: 0x04000005 RID: 5
		private bool ProcessConpleted = false;
	}
}
