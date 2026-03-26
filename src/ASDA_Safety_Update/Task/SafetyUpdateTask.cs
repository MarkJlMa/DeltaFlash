using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using ASD_Safety;

namespace ASDA_Safety_Update.Task
{
	// Token: 0x02000014 RID: 20
	public class SafetyUpdateTask : BaseTask
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00006154 File Offset: 0x00004354
		public SafetyUpdateTask(DeviceCOMM pComm)
		{
			this.FwUpdateWorker = new SM_FwUpdate(pComm);
			this.FwUpdateController = new BackgroundWorker
			{
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};
			this.FwUpdateController.DoWork += this.SmFwUpdateController_DoWork;
			this.FwUpdateController.ProgressChanged += this.SmFwUpdateController_ProgressChange;
			this.FwUpdateController.RunWorkerCompleted += this.SmFwUpdateController_RunWorkerCompleted;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000061E8 File Offset: 0x000043E8
		private bool CheckFwUpdateSatausWithRetry(int maxRetryCount, int tInterval, out UPDATE_FLAG getFlagA, out UPDATE_FLAG getFlagB)
		{
			getFlagA = UPDATE_FLAG.NONE;
			getFlagB = UPDATE_FLAG.NONE;
			try
			{
				for (int i = 0; i < maxRetryCount; i++)
				{
					bool flag = this.FwUpdateWorker.SendCmd(UPDATE_CMD_FLAG.CHECK_STATE, out getFlagA, out getFlagB);
					if (flag)
					{
						return true;
					}
					Thread.Sleep(tInterval);
				}
			}
			catch (Exception ex)
			{
				this.debugMsg = "::CheckFwUpdateSatausWithRetry() : " + ex.Message;
			}
			return false;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000626C File Offset: 0x0000446C
		private void ProcessControlDependOnFlag(in UPDATE_FLAG rxflagA, in UPDATE_FLAG rxflagB)
		{
			bool flag = !Enum.IsDefined(typeof(UPDATE_FLAG), (int)rxflagA) || !Enum.IsDefined(typeof(UPDATE_FLAG), (int)rxflagB);
			if (flag)
			{
				MessageBox.Show("Un - know Flag, MCU A = " + rxflagA.ToString() + ", MCU B = " + rxflagB.ToString(), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				UpdateGlobal.ShowUpdateMessage("Check Firmware Update Flag A = " + rxflagA.ToString() + ",Flag B = " + rxflagB.ToString(), true);
				bool flag2 = rxflagA == UPDATE_FLAG.ALARM || rxflagB == UPDATE_FLAG.ALARM;
				UPDATE_FLAG update_FLAG;
				if (flag2)
				{
					update_FLAG = UPDATE_FLAG.ALARM;
				}
				else
				{
					bool flag3 = rxflagA != rxflagB;
					if (flag3)
					{
						bool flag4 = (rxflagA == UPDATE_FLAG.UPDATE_MODE || rxflagA == UPDATE_FLAG.BANK_MODE) && (rxflagB == UPDATE_FLAG.UPDATE_MODE || rxflagB == UPDATE_FLAG.BANK_MODE);
						if (flag4)
						{
							this.NextProcess = new BaseTask.DelProcess(this.Process_DiscardAndReDownload);
							return;
						}
						string text = string.Concat(new string[]
						{
							"Safety Module reply exception.\n MCU replies with different status, Flag A = ",
							rxflagA.ToString(),
							",Flag B = ",
							rxflagB.ToString(),
							"\nPlease reboot."
						});
						MessageBox.Show(text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						this.NextProcess = null;
						return;
					}
					else
					{
						update_FLAG = rxflagA;
					}
				}
				UPDATE_FLAG update_FLAG2 = update_FLAG;
				UPDATE_FLAG update_FLAG3 = update_FLAG2;
				switch (update_FLAG3)
				{
				case UPDATE_FLAG.ENTER_UPDATE_MODE:
					this.NextProcess = new BaseTask.DelProcess(this.Process_DualModeReset);
					break;
				case UPDATE_FLAG.UPDATE_MODE:
					this.NextProcess = new BaseTask.DelProcess(this.Process_DownloadFile);
					break;
				case UPDATE_FLAG.BANK_MODE:
					this.NextProcess = new BaseTask.DelProcess(this.Process_SwapBank);
					break;
				case UPDATE_FLAG.REBOOT:
					this.NextProcess = null;
					break;
				default:
					if (update_FLAG3 != UPDATE_FLAG.ALARM)
					{
						if (update_FLAG3 != UPDATE_FLAG.DISCARD_AND_REUPDATE_Rx)
						{
							this.NextProcess = null;
						}
						else
						{
							Thread.Sleep(5000);
							this.NextProcess = new BaseTask.DelProcess(this.Process_CheckUpdateState);
						}
					}
					else
					{
						this.NextProcess = null;
					}
					break;
				}
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00006494 File Offset: 0x00004694
		private void Process_CheckUpdateState()
		{
			UPDATE_FLAG update_FLAG;
			UPDATE_FLAG update_FLAG2;
			bool flag = !this.CheckFwUpdateSatausWithRetry(10, 500, out update_FLAG, out update_FLAG2);
			if (flag)
			{
				this.Process_CancelUpdate();
			}
			else
			{
				this.ProcessControlDependOnFlag(update_FLAG, update_FLAG2);
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000064D0 File Offset: 0x000046D0
		public void Process_UpdateStart()
		{
			this.swapBankCompleted = false;
			this.downloadCompleted = false;
			UpdateGlobal.ShowUpdateMessage("==Update Start==", true);
			UPDATE_FLAG update_FLAG;
			UPDATE_FLAG update_FLAG2;
			bool flag = !this.FwUpdateWorker.SendCmd(UPDATE_CMD_FLAG.CHECK_STATE, out update_FLAG, out update_FLAG2);
			if (flag)
			{
				this.Process_CancelUpdate();
			}
			else
			{
				this.ProcessControlDependOnFlag(update_FLAG, update_FLAG2);
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00006524 File Offset: 0x00004724
		private void Process_DualModeReset()
		{
			UpdateGlobal.ShowUpdateMessage("==Dual Mode Reset==", true);
			Thread.Sleep(5000);
			this.Process_CheckUpdateState();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00006548 File Offset: 0x00004748
		private void Process_DiscardAndReDownload()
		{
			UPDATE_FLAG update_FLAG;
			UPDATE_FLAG update_FLAG2;
			bool flag = !this.FwUpdateWorker.SendCmd(UPDATE_CMD_FLAG.DISCARD_AND_REUPDATE, out update_FLAG, out update_FLAG2);
			if (flag)
			{
				this.Process_CancelUpdate();
			}
			else
			{
				this.ProcessControlDependOnFlag(update_FLAG, update_FLAG2);
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00006584 File Offset: 0x00004784
		private void Process_DownloadFile()
		{
			this.FwUpdateController.ReportProgress(10, "Start File Transfer.");
			UPDATE_FLAG update_FLAG;
			UPDATE_FLAG update_FLAG2;
			bool flag = !this.FwUpdateWorker.SendCmd(UPDATE_CMD_FLAG.START_UPDATE, out update_FLAG, out update_FLAG2);
			if (flag)
			{
				throw new Exception(base.GetDebugMsg());
			}
			bool flag2 = update_FLAG != UPDATE_FLAG.UPDATE_MODE || update_FLAG2 != UPDATE_FLAG.UPDATE_MODE;
			if (flag2)
			{
				string message = string.Concat(new string[]
				{
					"Safety Module reply exception.\n MCU replies unexpected flag (not",
					UPDATE_FLAG.UPDATE_MODE.ToString(),
					"), Flag A = ",
					update_FLAG.ToString(),
					",Flag B = ",
					update_FLAG2.ToString(),
					"\nPlease reboot."
				});
				throw new Exception(message);
			}
			Thread.Sleep(5000);
			int num = Math.Max(this.FwUpdateWorker.FlashFile_MCUA.FlashFileDataList.Count, this.FwUpdateWorker.FlashFile_MCUB.FlashFileDataList.Count);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				bool cancellationPending = this.FwUpdateController.CancellationPending;
				if (cancellationPending)
				{
					return;
				}
				byte[] mcuA_data = null;
				byte[] mcuB_data = null;
				bool flag3 = i < this.FwUpdateWorker.FlashFile_MCUA.FlashFileDataList.Count;
				if (flag3)
				{
					mcuA_data = this.FwUpdateWorker.FlashFile_MCUA.FlashFileDataList[i].FlashLineByte;
				}
				bool flag4 = i < this.FwUpdateWorker.FlashFile_MCUB.FlashFileDataList.Count;
				if (flag4)
				{
					mcuB_data = this.FwUpdateWorker.FlashFile_MCUB.FlashFileDataList[i].FlashLineByte;
				}
				bool flag5 = !this.FwUpdateWorker.SendFlashData(mcuA_data, mcuB_data);
				if (flag5)
				{
					string text = "Send Flash Data failed: " + base.GetDebugMsg() + "\n";
					text = text + "MCU A: " + ((i < this.FwUpdateWorker.FlashFile_MCUA.FlashFileDataList.Count) ? this.FwUpdateWorker.FlashFile_MCUA.FlashFileDataList[i].AddressStr : "null") + "\n";
					text = text + "MCU B: " + ((i < this.FwUpdateWorker.FlashFile_MCUB.FlashFileDataList.Count) ? this.FwUpdateWorker.FlashFile_MCUB.FlashFileDataList[i].AddressStr : "null") + "\n";
					throw new Exception(text);
				}
				bool flag6 = i == (num2 + 1) * (num / 8);
				if (flag6)
				{
					this.FwUpdateController.ReportProgress(10 + num2 * 10, "File Transfer progress..." + i.ToString() + "/" + num.ToString());
					num2++;
				}
			}
			this.FwUpdateController.ReportProgress(90, "Send EOF...");
			bool flag7 = !this.FwUpdateWorker.SendEOF(this.FwUpdateWorker.FlashFile_MCUA.GetCheckSum(), this.FwUpdateWorker.FlashFile_MCUB.GetCheckSum());
			if (flag7)
			{
				throw new Exception("Send EOF failed : " + base.GetDebugMsg());
			}
			this.downloadCompleted = true;
			Thread.Sleep(10000);
			this.Process_CheckUpdateState();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000068D8 File Offset: 0x00004AD8
		private void Process_SwapBank()
		{
			UpdateGlobal.ShowUpdateMessage("==Swap Bank Set to Linear mode==", true);
			Thread.Sleep(500);
			UPDATE_FLAG update_FLAG;
			UPDATE_FLAG update_FLAG2;
			bool flag = !this.FwUpdateWorker.SendCmd(UPDATE_CMD_FLAG.START_BANKING, out update_FLAG, out update_FLAG2);
			if (flag)
			{
				throw new Exception(base.GetDebugMsg());
			}
			bool flag2 = update_FLAG != UPDATE_FLAG.BANK_MODE || update_FLAG2 != UPDATE_FLAG.BANK_MODE;
			if (flag2)
			{
				string message = string.Concat(new string[]
				{
					"Safety Module reply exception.\n MCU replies unexpected flag (not",
					UPDATE_FLAG.BANK_MODE.ToString(),
					"), Flag A = ",
					update_FLAG.ToString(),
					",Flag B = ",
					update_FLAG2.ToString(),
					"\nPlease reboot."
				});
				throw new Exception(message);
			}
			Thread.Sleep(10000);
			this.swapBankCompleted = true;
			this.Process_CheckUpdateState();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000069B4 File Offset: 0x00004BB4
		private void Process_CancelUpdate()
		{
			UpdateGlobal.ShowUpdateMessage("Start cancellation process...", true);
			UPDATE_FLAG update_FLAG;
			UPDATE_FLAG update_FLAG2;
			this.FwUpdateWorker.SendCmd(UPDATE_CMD_FLAG.CANCEL_CMD_1, out update_FLAG, out update_FLAG2);
			this.FwUpdateWorker.SendCmd(UPDATE_CMD_FLAG.CANCEL_CMD_2, out update_FLAG, out update_FLAG2);
			this.Process_End();
			MessageBox.Show(Localize.GetMessageCultureStr("msgSafety ModuleError", "Safety Module return Error. Please reboot and Flash again."), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00006A18 File Offset: 0x00004C18
		private void Process_End()
		{
			this.FwUpdateWorker.pDevCOMM.DisConnect();
			bool isBusy = this.FwUpdateController.IsBusy;
			if (isBusy)
			{
				this.FwUpdateController.CancelAsync();
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00006A54 File Offset: 0x00004C54
		public void SmFwUpdateController_DoWork(object sender, DoWorkEventArgs e)
		{
			UpdateGlobal.ShowUpdateMessage("\nStart Task : " + this.GuiControl.gbxName.Text, false);
			Thread.Sleep(5000);
			bool flag = this.GoOnlineEventHandle != null;
			if (flag)
			{
				bool flag2 = !this.GoOnlineEventHandle();
				if (flag2)
				{
					return;
				}
			}
			this.NextProcess = new BaseTask.DelProcess(this.Process_UpdateStart);
			Thread.Sleep(3000);
			while (!this.FwUpdateController.CancellationPending && this.NextProcess != null)
			{
				this.NextProcess();
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00006AFB File Offset: 0x00004CFB
		public void SmFwUpdateController_ProgressChange(object sender, ProgressChangedEventArgs e)
		{
			this.GuiControl.progressBar1.Value = e.ProgressPercentage;
			UpdateGlobal.ShowUpdateMessage(e.UserState.ToString(), true);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00006B28 File Offset: 0x00004D28
		public void SmFwUpdateController_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			bool flag = e.Error != null;
			if (flag)
			{
				string text = Localize.GetMessageCultureStr("msgDownloadError", "Error occured when download file") + " : " + e.Error.Message;
				MessageBox.Show(text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				UpdateGlobal.ShowUpdateMessage(text, true);
				bool flag2 = this.GuiControlHanle != null;
				if (flag2)
				{
					this.GuiControlHanle(true);
				}
			}
			else
			{
				bool cancelled = e.Cancelled;
				if (cancelled)
				{
					string text2 = Localize.GetMessageCultureStr("msgUpdateCancel", "Update is cancelled") + " : " + this.debugMsg;
					MessageBox.Show(text2, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					UpdateGlobal.ShowUpdateMessage(text2, true);
					bool flag3 = this.GuiControlHanle != null;
					if (flag3)
					{
						this.GuiControlHanle(true);
					}
				}
				else
				{
					this.FwUpdateWorker.pDevCOMM.DisConnect();
					string text3 = string.Empty;
					bool flag4 = this.swapBankCompleted && this.downloadCompleted;
					if (flag4)
					{
						this.GuiControl.progressBar1.Value = 100;
						bool flag5 = this.CompletedEventHandle != null;
						if (flag5)
						{
							this.CompletedEventHandle();
						}
						else
						{
							text3 = Localize.GetMessageCultureStr("msgUpdateOK", "Firmware update completed.") + "\n" + Localize.GetMessageCultureStr("msgReboot", "Please reboot.");
							MessageBox.Show(text3, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
							UpdateGlobal.ShowUpdateMessage(text3, true);
							bool flag6 = this.GuiControlHanle != null;
							if (flag6)
							{
								this.GuiControlHanle(true);
							}
						}
					}
					else
					{
						text3 = Localize.GetMessageCultureStr("msgUpdateInComplete", "The last firmware update was not completed.") + "\n" + Localize.GetMessageCultureStr("msgFlashAgain", "Please reboot and Flash again.");
						MessageBox.Show(text3, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						UpdateGlobal.ShowUpdateMessage(text3, true);
						bool flag7 = this.GuiControlHanle != null;
						if (flag7)
						{
							this.GuiControlHanle(true);
						}
					}
				}
			}
		}

		// Token: 0x04000055 RID: 85
		public SafetyUpdateTask.GoOnlineEvent GoOnlineEventHandle;

		// Token: 0x04000056 RID: 86
		public SM_FwUpdate FwUpdateWorker;

		// Token: 0x04000057 RID: 87
		private bool swapBankCompleted = false;

		// Token: 0x04000058 RID: 88
		private bool downloadCompleted = false;

		// Token: 0x0200001C RID: 28
		// (Invoke) Token: 0x0600008F RID: 143
		public delegate bool GoOnlineEvent();
	}
}
