using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ASDA_Safety_Update.Task;
using ASD_FTP_UpdateTool;
using ASD_Safety;
using ASD_SCOPE;

namespace ASDA_Safety_Update
{
	// Token: 0x0200000D RID: 13
	public partial class FwUpdate_F : Form
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00003120 File Offset: 0x00001320
		public FwUpdate_F(bool is_RD = false, string cultureStr = "EN")
		{
			this.InitializeComponent();
			UpdateGlobal.SetDisplayFunction(new UpdateGlobal.DisplayMessage(this.ShowDeltail));
			this.Text = UpdateGlobal.AppTitle;
			this.TaskList = new List<BaseTask>();
			this.pComm = new DeviceCOMM();
			Localize.SetCulture(cultureStr);
			Localize.SetIniFilePath(Localize.IniFilePath);
			Localize.ApplyCultureToGUI(this);
			this.ModeSwitch(is_RD);
			this.InitialComponments();
			this.AdjustFormHeight();
			this.btnFlash.Visible = false;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000322C File Offset: 0x0000142C
		private void btnStart_Click(object sender, EventArgs e)
		{
			this.ClearTask();
			bool flag = this.tbctlMode.SelectedTab == this.tabCustomer;
			if (flag)
			{
				this.SetupTask_Customer();
			}
			else
			{
				this.SetupTask_RD();
			}
			this.btnFlash.Visible = (this.TaskList.Count > 0);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003284 File Offset: 0x00001484
		private void btnFlash_Click(object sender, EventArgs e)
		{
			bool flag = !this.is_RD && this.CurrentTaskIndex == 0;
			if (flag)
			{
				string text = string.Concat(new string[]
				{
					Localize.GetMessageCultureStr("msgUpdateInfo", "The servo drive firmware will also be updated."),
					"\n",
					Localize.GetMessageCultureStr("msgUpdateConfirm", "Are you sure you want to update?"),
					"\n\n",
					Localize.GetCultureStr("FwUpdate_F", "gbxSafety", "Safety Module"),
					":\n",
					this.onlineSafetyFwVerStr,
					" >> ",
					this.cmbVerList.Text,
					"\n\n",
					Localize.GetCultureStr("FwUpdate_F", "gbxDrive", "Servo Drive"),
					":\n",
					this.onlineDriveFwVerStr,
					" >> ",
					this.tbxDriveVer.Text
				});
				DialogResult dialogResult = MessageBox.Show(text, UpdateGlobal.AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				bool flag2 = dialogResult != DialogResult.Yes;
				if (flag2)
				{
					return;
				}
			}
			this.UIControl(false);
			this.TaskList[this.CurrentTaskIndex].FwUpdateController.RunWorkerAsync();
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000033BC File Offset: 0x000015BC
		public void EpPreUpdateCompletedEvent()
		{
			this.CurrentTaskIndex++;
			string text = Localize.GetMessageCultureStr("msgReboot", "Please reboot.") + "\n" + Localize.GetMessageCultureStr("msgWatingToContinue", "After power-on, press Flash to continue the update process.");
			MessageBox.Show(text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			UpdateGlobal.ShowUpdateMessage(text, true);
			this.UIControl(true);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003420 File Offset: 0x00001620
		public void UpdateCompletedEvent()
		{
			this.CurrentTaskIndex++;
			bool flag = this.CurrentTaskIndex < this.TaskList.Count;
			if (flag)
			{
				this.TaskList[this.CurrentTaskIndex].FwUpdateController.RunWorkerAsync();
			}
			else
			{
				string text = Localize.GetMessageCultureStr("msgUpdateOK", "Firmware update completed.") + "\n" + Localize.GetMessageCultureStr("msgReboot", "Please reboot.");
				MessageBox.Show(text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				UpdateGlobal.ShowUpdateMessage(text, true);
				this.UIControl(true);
				this.btnFlash.Visible = false;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000034CA File Offset: 0x000016CA
		private void ClearTask()
		{
			this.TaskList.Clear();
			this.flowTaskList.Controls.Clear();
			this.AdjustFormHeight();
			this.CurrentTaskIndex = 0;
			this.btnFlash.Visible = false;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003508 File Offset: 0x00001708
		private void SetupTask_Customer()
		{
			this.messageBox.Text = string.Empty;
			bool flag = this.cmbVerList.SelectedIndex < 0;
			if (flag)
			{
				string messageCultureStr = Localize.GetMessageCultureStr("msgInvalidVer", "Please select the firmware version.");
				MessageBox.Show(messageCultureStr, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				bool flag2 = !this.GoOnline();
				if (flag2)
				{
					this.ShowDeltail(this.debugMsg, true);
					string text = string.Concat(new string[]
					{
						Localize.GetMessageCultureStr("msgConnectFailed", "Failed to connect to the following IP Address : "),
						this.tbxIP.Text,
						"\n\n- ",
						Localize.GetMessageCultureStr("msgCheckSetting", "Please check whether the communication setting is correct."),
						"\n- ",
						Localize.GetMessageCultureStr("msgCheckIP", "Check whether the IP Address is occupied by other applications. (Includes Delta Drive Safety)"),
						"\n- ",
						Localize.GetMessageCultureStr("msgCheckCable", "Check whether the network cable is connected properly."),
						"\n- ",
						Localize.GetMessageCultureStr("msgCheckInstall", "Check whether the device is installed properly and powered on."),
						"\n"
					});
					MessageBox.Show(text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					Process[] processes = Process.GetProcesses();
					foreach (Process process in processes)
					{
						bool flag3 = process.ProcessName.Contains(Application.ProductName);
						if (flag3)
						{
							ASD_Scope_Header.Free_All_Port();
						}
					}
					this.UIControl(true);
				}
				else
				{
					bool flag4 = !this.CheckDeviceInfo();
					if (flag4)
					{
						Process[] processes2 = Process.GetProcesses();
						foreach (Process process2 in processes2)
						{
							bool flag5 = process2.ProcessName.Contains(Application.ProductName);
							if (flag5)
							{
								ASD_Scope_Header.Free_All_Port();
							}
						}
						this.UIControl(true);
					}
					else
					{
						int num;
						bool flag6 = this.Is_EpNeedToPreUpdate(out num);
						if (flag6)
						{
							DriveUpdateTask driveUpdateTask = new DriveUpdateTask();
							FTPConnect_Worker.hostName = this.tbxIP.Text;
							string filePath = string.Concat(new string[]
							{
								this.FlashFloderPath,
								this.ModelName,
								"\\temp\\",
								this.flashInfo.fileList[num].SafetyFWVerStr,
								"\\",
								this.flashInfo.fileList[num].fileNameDrv
							});
							driveUpdateTask.SetFilePath(filePath);
							driveUpdateTask.CompletedEventHandle = new BaseTask.CompletedEvent(this.EpPreUpdateCompletedEvent);
							driveUpdateTask.GuiControlHanle = new BaseTask.GUIControl(this.UIControl);
							driveUpdateTask.SetTaskName(Localize.GetCultureStr("FwUpdate_F", "gbxDrive", "Servo Drive") + " >> " + this.flashInfo.fileList[num].DriveVerStr);
							this.TaskList.Add(driveUpdateTask);
							this.flowTaskList.Controls.Add(driveUpdateTask.GuiControl);
						}
						SafetyUpdateTask safetyUpdateTask = new SafetyUpdateTask(this.pComm);
						safetyUpdateTask.GoOnlineEventHandle = new SafetyUpdateTask.GoOnlineEvent(this.SafetyModuleGoOnlineCheck);
						safetyUpdateTask.GuiControlHanle = new BaseTask.GUIControl(this.UIControl);
						safetyUpdateTask.SetTaskName(string.Concat(new string[]
						{
							Localize.GetCultureStr("FwUpdate_F", "gbxSafety", "Safety Module"),
							">>",
							Path.GetFileName(this.tbxPathMCUA.Text),
							", ",
							Path.GetFileName(this.tbxPathMCUB.Text)
						}));
						bool flag7 = !safetyUpdateTask.FwUpdateWorker.FlashFile_MCUA.SetFilePath(string.Concat(new string[]
						{
							this.FlashFloderPath,
							this.ModelName,
							"\\temp\\",
							this.cmbVerList.Text,
							"\\",
							this.flashInfo.fileList[this.cmbVerList.SelectedIndex].fileNameMcuA
						}));
						if (flag7)
						{
							MessageBox.Show("Error occur when loading MCU A file : " + safetyUpdateTask.FwUpdateWorker.FlashFile_MCUA.GetDebugMsg(), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						}
						else
						{
							bool flag8 = !safetyUpdateTask.FwUpdateWorker.FlashFile_MCUB.SetFilePath(string.Concat(new string[]
							{
								this.FlashFloderPath,
								this.ModelName,
								"\\temp\\",
								this.cmbVerList.Text,
								"\\",
								this.flashInfo.fileList[this.cmbVerList.SelectedIndex].fileNameMcuB
							}));
							if (flag8)
							{
								MessageBox.Show("Error occur when loading MCU B file : " + safetyUpdateTask.FwUpdateWorker.FlashFile_MCUB.GetDebugMsg(), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
							}
							else
							{
								safetyUpdateTask.GuiControlHanle = new BaseTask.GUIControl(this.UIControl);
								safetyUpdateTask.CompletedEventHandle = new BaseTask.CompletedEvent(this.UpdateCompletedEvent);
								safetyUpdateTask.SetTaskName(Localize.GetCultureStr("FwUpdate_F", "gbxSafety", "Safety Module") + " >> " + this.flashInfo.fileList[this.cmbVerList.SelectedIndex].SafetyFWVerStr);
								this.TaskList.Add(safetyUpdateTask);
								this.flowTaskList.Controls.Add(safetyUpdateTask.GuiControl);
								bool flag9 = num != this.cmbVerList.SelectedIndex;
								if (flag9)
								{
									DriveUpdateTask driveUpdateTask2 = new DriveUpdateTask();
									FTPConnect_Worker.hostName = this.tbxIP.Text;
									driveUpdateTask2.SetFilePath(string.Concat(new string[]
									{
										this.FlashFloderPath,
										this.ModelName,
										"\\temp\\",
										this.cmbVerList.Text,
										"\\",
										this.flashInfo.fileList[this.cmbVerList.SelectedIndex].fileNameDrv
									}));
									driveUpdateTask2.GuiControlHanle = new BaseTask.GUIControl(this.UIControl);
									driveUpdateTask2.SetTaskName(Localize.GetCultureStr("FwUpdate_F", "gbxDrive", "Servo Drive") + " >> " + this.flashInfo.fileList[this.cmbVerList.SelectedIndex].DriveVerStr);
									this.TaskList.Add(driveUpdateTask2);
									this.flowTaskList.Controls.Add(driveUpdateTask2.GuiControl);
								}
								this.AdjustFormHeight();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003B8C File Offset: 0x00001D8C
		private bool Is_EpNeedToPreUpdate(out int index)
		{
			index = -1;
			for (int i = 0; i < this.flashInfo.fileList.Count; i++)
			{
				bool flag = this.flashInfo.fileList[i].SafetyFWVerStr == this.onlineSafetyFwVerStr && this.flashInfo.fileList[i].DriveVerStr != this.onlineDriveFwVerStr;
				if (flag)
				{
					index = i;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003C18 File Offset: 0x00001E18
		private void SetupTask_RD()
		{
			this.messageBox.Text = string.Empty;
			bool flag = !File.Exists(this.tbxPathMCUA.Text);
			if (flag)
			{
				MessageBox.Show(Localize.GetMessageCultureStr("msgPathNotExist", "Path does not exist.") + "\n" + this.tbxPathMCUA.Text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				bool flag2 = !File.Exists(this.tbxPathMCUB.Text);
				if (flag2)
				{
					MessageBox.Show(Localize.GetMessageCultureStr("msgPathNotExist", "Path does not exist.") + "\n" + this.tbxPathMCUB.Text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				else
				{
					bool flag3 = !this.CheckIPAddress();
					if (flag3)
					{
						MessageBox.Show("Invalind IP Address.", UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
					else
					{
						SafetyUpdateTask safetyUpdateTask = new SafetyUpdateTask(this.pComm);
						safetyUpdateTask.GoOnlineEventHandle = new SafetyUpdateTask.GoOnlineEvent(this.GoOnline);
						safetyUpdateTask.GuiControlHanle = new BaseTask.GUIControl(this.UIControl);
						safetyUpdateTask.SetTaskName(string.Concat(new string[]
						{
							Localize.GetCultureStr("FwUpdate_F", "gbxSafety", "Safety Module"),
							">>",
							Path.GetFileName(this.tbxPathMCUA.Text),
							", ",
							Path.GetFileName(this.tbxPathMCUB.Text)
						}));
						bool flag4 = !safetyUpdateTask.FwUpdateWorker.FlashFile_MCUA.CheckFlashDataMatch(this.tbxPathMCUA.Text);
						if (flag4)
						{
							MessageBox.Show("MCU A file type incorrect : " + safetyUpdateTask.FwUpdateWorker.FlashFile_MCUA.GetDebugMsg(), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						}
						else
						{
							bool flag5 = !safetyUpdateTask.FwUpdateWorker.FlashFile_MCUB.CheckFlashDataMatch(this.tbxPathMCUB.Text);
							if (flag5)
							{
								MessageBox.Show("MCU B file type incorrect : " + safetyUpdateTask.FwUpdateWorker.FlashFile_MCUB.GetDebugMsg(), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
							}
							else
							{
								string fileVersion = safetyUpdateTask.FwUpdateWorker.FlashFile_MCUA.GetFileVersion(this.tbxPathMCUA.Text);
								string fileVersion2 = safetyUpdateTask.FwUpdateWorker.FlashFile_MCUB.GetFileVersion(this.tbxPathMCUB.Text);
								bool flag6 = fileVersion == string.Empty || fileVersion2 == string.Empty || fileVersion != fileVersion2;
								if (flag6)
								{
									MessageBox.Show("File Version not match.", UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
								}
								else
								{
									bool flag7 = !safetyUpdateTask.FwUpdateWorker.FlashFile_MCUA.SetFilePath(this.tbxPathMCUA.Text);
									if (flag7)
									{
										MessageBox.Show("Error occur when loading MCU A file : " + safetyUpdateTask.FwUpdateWorker.FlashFile_MCUA.GetDebugMsg(), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
									}
									else
									{
										bool flag8 = !safetyUpdateTask.FwUpdateWorker.FlashFile_MCUB.SetFilePath(this.tbxPathMCUB.Text);
										if (flag8)
										{
											MessageBox.Show("Error occur when loading MCU B file : " + safetyUpdateTask.FwUpdateWorker.FlashFile_MCUB.GetDebugMsg(), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
										}
										else
										{
											this.UIControl(false);
											bool flag9 = !this.GoOnline();
											if (flag9)
											{
												this.ShowDeltail(this.debugMsg, true);
												string text = string.Concat(new string[]
												{
													Localize.GetMessageCultureStr("msgConnectFailed", "Failed to connect to the following IP Address : "),
													this.tbxIP.Text,
													"\n\n- ",
													Localize.GetMessageCultureStr("msgCheckSetting", "Please check whether the communication setting is correct."),
													"\n- ",
													Localize.GetMessageCultureStr("msgCheckIP", "Check whether the IP Address is occupied by other applications. (Includes Delta Drive Safety)"),
													"\n- ",
													Localize.GetMessageCultureStr("msgCheckCable", "Check whether the network cable is connected properly."),
													"\n- ",
													Localize.GetMessageCultureStr("msgCheckInstall", "Check whether the device is installed properly and powered on."),
													"\n"
												});
												MessageBox.Show(text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
												Process[] processes = Process.GetProcesses();
												foreach (Process process in processes)
												{
													bool flag10 = process.ProcessName.Contains(Application.ProductName);
													if (flag10)
													{
														ASD_Scope_Header.Free_All_Port();
													}
												}
												this.UIControl(true);
											}
											else
											{
												this.TaskList.Add(safetyUpdateTask);
												this.flowTaskList.Controls.Add(safetyUpdateTask.GuiControl);
												this.AdjustFormHeight();
												this.UIControl(true);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000040B0 File Offset: 0x000022B0
		private bool IsExecuteInDll()
		{
			return !Application.StartupPath.Contains(Application.ProductName);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000040D4 File Offset: 0x000022D4
		private bool CheckConnectIsOccupied()
		{
			this.ShowDeltail("Check connection...", true);
			bool flag = this.TcpClientCheck(this.tbxIP.Text, 1501);
			bool result;
			if (flag)
			{
				this.ShowDeltail("Connect is occupied.", true);
				result = true;
			}
			else
			{
				this.ShowDeltail("Connect is available.", true);
				result = false;
			}
			return result;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00004130 File Offset: 0x00002330
		public bool GoOnline()
		{
			try
			{
				bool flag = this.pComm.CheckOnline();
				if (flag)
				{
					return true;
				}
				bool flag2 = !this.Connect();
				if (flag2)
				{
					throw new Exception("Connect to " + this.tbxIP.Text + " failed.\n" + this.debugMsg);
				}
				bool flag3 = !this.pComm.Set_RecvTimeOut(0, 3000000);
				if (flag3)
				{
					throw new Exception("Set_RecvTimeOut failed. " + this.pComm.GetDllErrMsg().ToString());
				}
				return true;
			}
			catch (Exception ex)
			{
				this.debugMsg = "::GoOnline() : " + ex.Message;
			}
			return false;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000041F8 File Offset: 0x000023F8
		private bool CheckDeviceInfo()
		{
			long num;
			bool flag = !this.pComm.ReadSafetyFwVer(out num);
			bool result;
			if (flag)
			{
				MessageBox.Show(Localize.GetMessageCultureStr("msgReadSafetyVerFailed", "Read firmware version from Safety Module failed."), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				result = false;
			}
			else
			{
				long num2;
				bool flag2 = !this.pComm.ReadSafetyAlarm(out num2);
				if (flag2)
				{
					MessageBox.Show(Localize.GetMessageCultureStr("msgReadSafetyAlarmFailed", "Read alarm from Safety Module failed."), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					result = false;
				}
				else
				{
					long num3;
					long num4;
					bool flag3 = !this.pComm.ReadDriveFwVer(out num3, out num4);
					if (flag3)
					{
						MessageBox.Show(Localize.GetMessageCultureStr("msgDriveVerFailed", "Read firmware version from drive failed."), UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						result = false;
					}
					else
					{
						this.onlineSafetyFwVerStr = "V" + string.Format("{0:X4}", num).ToUpper();
						string text = string.Format("{0:X4}", num2).ToUpper();
						this.onlineSafetyAlarmStr = text.Substring(0, 3) + "." + text.Substring(3, 1);
						this.onlineDriveFwVerStr = string.Concat(new string[]
						{
							"v(",
							num3.ToString(),
							")_sub(",
							(num4 % 1000L).ToString(),
							")"
						});
						this.ShowDeltail(string.Concat(new string[]
						{
							"Safety Module Version = ",
							this.onlineSafetyFwVerStr,
							"\nSafety Module Alarm = ",
							this.onlineSafetyAlarmStr,
							"\nDrive Version = ",
							this.onlineDriveFwVerStr
						}), true);
						bool flag4 = num2 >= 22016L && num2 <= 23743L;
						bool flag5 = this.onlineSafetyFwVerStr == "V1001" && flag4;
						if (flag5)
						{
							string text2 = string.Concat(new string[]
							{
								Localize.GetMessageCultureStr("msgHookMode", "Firmware update cannot be performed because safety module is hook mode."),
								"\n",
								Localize.GetMessageCultureStr("msgClearAlarm", "Please clear the alarm before updating."),
								"\n\nSafety Module version = ",
								this.onlineSafetyFwVerStr,
								"\nSafety Alarm = ",
								this.onlineSafetyAlarmStr
							});
							MessageBox.Show(text2, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004454 File Offset: 0x00002654
		public bool SafetyModuleGoOnlineCheck()
		{
			bool flag = !this.GoOnline();
			return !flag;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004478 File Offset: 0x00002678
		public bool CheckIPAddress()
		{
			try
			{
				Regex regex = new Regex("\\b((?:(?:25[0-5]|2[0-4]\\d|((1\\d{2})|([1-9]?\\d)))\\.){3}(?:25[0-5]|2[0-4]\\d|((1\\d{2})|([1-9]?\\d))))\\b");
				return regex.IsMatch(this.tbxIP.Text);
			}
			catch (Exception ex)
			{
				this.debugMsg = "CheckIPAddress() : " + ex.Message;
			}
			return false;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000044D8 File Offset: 0x000026D8
		public void SetIPAddress(string IPstr)
		{
			this.tbxIP.Text = IPstr;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000044E8 File Offset: 0x000026E8
		private bool Connect()
		{
			try
			{
				this.ShowDeltail("Connecting to " + this.tbxIP.Text + "...", true);
				this.pComm.IPAddress = this.tbxIP.Text;
				this.pComm.ComPortID = this.pComm.ConvertIPToPortID(this.pComm.IPAddress);
				bool flag = this.IsExecuteInDll() && this.pComm.CheckOnline();
				if (flag)
				{
					return false;
				}
				bool flag2 = this.pComm.Connect();
				ushort num = 0;
				while (!flag2 && num < 3)
				{
					Thread.Sleep(500);
					flag2 = this.pComm.Connect();
					num += 1;
				}
				bool flag3 = !flag2;
				if (flag3)
				{
					throw new Exception("Failed to establish connection. " + this.pComm.GetDllErrMsg().ToString());
				}
				bool flag4 = !this.pComm.Set_RecvTimeOut(0, 500000);
				if (flag4)
				{
					throw new Exception("Set_RecvTimeOut failed. " + this.pComm.GetDllErrMsg().ToString());
				}
				short num2 = 0;
				bool flag5 = !this.pComm.ReadStationID_fromDrive(out num2);
				if (flag5)
				{
					throw new Exception("Read Station ID failed. " + this.pComm.GetDllErrMsg().ToString());
				}
				bool flag6 = !this.pComm.Set_Station((int)num2);
				if (flag6)
				{
					throw new Exception("ASD_Set_Station Failed! " + this.pComm.GetDllErrMsg().ToString());
				}
				this.pComm.StationID = num2;
				return true;
			}
			catch (Exception ex)
			{
				this.debugMsg = "Connect() : " + ex.Message;
			}
			return false;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000046D8 File Offset: 0x000028D8
		public void ShowDeltail(string showStr, bool showTimeInfo = true)
		{
			bool flag = this.formClosed;
			if (!flag)
			{
				bool invokeRequired = base.InvokeRequired;
				if (invokeRequired)
				{
					FwUpdate_F.DelShowMessage method = new FwUpdate_F.DelShowMessage(this.ShowDeltail);
					base.Invoke(method, new object[]
					{
						showStr,
						showTimeInfo
					});
				}
				else
				{
					if (showTimeInfo)
					{
						string text = DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss");
						RichTextBox richTextBox = this.messageBox;
						richTextBox.Text = string.Concat(new string[]
						{
							richTextBox.Text,
							"[",
							text,
							"] ",
							showStr,
							"\n"
						});
					}
					else
					{
						RichTextBox richTextBox2 = this.messageBox;
						richTextBox2.Text = richTextBox2.Text + showStr + "\n";
					}
					this.messageBox.SelectionStart = this.messageBox.TextLength;
					this.messageBox.ScrollToCaret();
				}
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000047D8 File Offset: 0x000029D8
		private void btnImportMCUA_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".mot",
				Filter = "Program files (.mot)|*.mot"
			};
			bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
			if (flag)
			{
				this.tbxPathMCUA.Text = openFileDialog.FileName;
			}
			bool flag2 = !File.Exists(this.tbxPathMCUA.Text);
			if (flag2)
			{
				MessageBox.Show(Localize.GetMessageCultureStr("msgPathNotExist", "Path does not exist.") + "\n" + this.tbxPathMCUA.Text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				this.tbxCheckSum_A.Text = CheckSum.CalculateCRC32(File.ReadAllBytes(this.tbxPathMCUA.Text)).ToString("X");
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000048A0 File Offset: 0x00002AA0
		private void btnImportMCUB_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".mot",
				Filter = "Program files (.mot)|*.mot"
			};
			bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
			if (flag)
			{
				this.tbxPathMCUB.Text = openFileDialog.FileName;
			}
			bool flag2 = !File.Exists(this.tbxPathMCUB.Text);
			if (flag2)
			{
				MessageBox.Show(Localize.GetMessageCultureStr("msgPathNotExist", "Path does not exist.") + "\n" + this.tbxPathMCUB.Text, UpdateGlobal.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				this.tbxCheckSum_B.Text = CheckSum.CalculateCRC32(File.ReadAllBytes(this.tbxPathMCUB.Text)).ToString("X");
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004968 File Offset: 0x00002B68
		private void btnDetail_Click(object sender, EventArgs e)
		{
			this.messageBox.Visible = !this.messageBox.Visible;
			this.messageBox.Height = (this.messageBox.Visible ? 95 : 0);
			this.AdjustFormHeight();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000049B8 File Offset: 0x00002BB8
		public void AdjustFormHeight()
		{
			this.flowTaskList.Height = this.flowTaskList.Controls.Count * 70;
			this.btnDetail.Location = new Point(this.btnDetail.Location.X, this.flowTaskList.Location.Y + this.flowTaskList.Height + 18);
			base.Height = 48 + this.btnDetail.Location.Y + this.btnDetail.Height + (this.messageBox.Visible ? (this.messageBox.Height + 18) : 0);
			this.messageBox.Location = new Point(this.btnDetail.Location.X, this.btnDetail.Location.Y + 50);
			this.btnFlash.Location = new Point(this.btnStart.Location.X, this.btnDetail.Location.Y);
			this.btnDetail.Text = (this.messageBox.Visible ? (Localize.GetCultureStr("FwUpdate_F", "btnDetail", "Details") + " << ") : (Localize.GetCultureStr("FwUpdate_F", "btnDetail", "Details") + " >> "));
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004B3C File Offset: 0x00002D3C
		private void UIControl(bool enabled)
		{
			this.tbxPathMCUA.Enabled = enabled;
			this.tbxPathMCUB.Enabled = enabled;
			this.btnImportMCUA.Enabled = enabled;
			this.btnImportMCUB.Enabled = enabled;
			this.tbxIP.Enabled = enabled;
			this.btnStart.Enabled = enabled;
			this.btnFlash.Enabled = enabled;
			this.cmbVerList.Enabled = enabled;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004BB4 File Offset: 0x00002DB4
		private void FwUpdate_F_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.pComm.DisConnect();
			this.flashInfo.DeleteFolderAll(this.FlashFloderPath + this.ModelName + "\\temp");
			ASD_Scope_Header.Free_All_Port();
			ASD_Scope_Header.ASD_StopDLL();
			this.formClosed = true;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004C04 File Offset: 0x00002E04
		private bool TcpClientCheck(string ip, int port)
		{
			bool result = false;
			IPAddress address = IPAddress.Parse(ip);
			IPEndPoint remoteEP = new IPEndPoint(address, port);
			TcpClient tcpClient = null;
			try
			{
				tcpClient = new TcpClient();
				tcpClient.Connect(remoteEP);
				MessageBox.Show("open");
				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show("failed");
				result = false;
			}
			finally
			{
				bool flag = tcpClient != null;
				if (flag)
				{
					tcpClient.Close();
				}
			}
			return result;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004C94 File Offset: 0x00002E94
		private bool SocketCheck(string ip, int port)
		{
			bool result = false;
			Socket socket = null;
			try
			{
				IPAddress address = IPAddress.Parse(ip);
				IPEndPoint remoteEP = new IPEndPoint(address, port);
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(remoteEP);
				MessageBox.Show("open");
				result = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show("failed");
				result = false;
			}
			finally
			{
				bool flag = socket != null;
				if (flag)
				{
					socket.Close();
					socket.Dispose();
				}
			}
			return result;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004D2C File Offset: 0x00002F2C
		private void FwUpdate_F_FormClosing(object sender, FormClosingEventArgs e)
		{
			bool flag = this.TaskList.Count > 0 && this.TaskList[this.CurrentTaskIndex].FwUpdateController.IsBusy;
			if (flag)
			{
				DialogResult dialogResult = MessageBox.Show(Localize.GetMessageCultureStr("msgUpdateInProgerss", "Firmware update in progress.") + "\n" + Localize.GetMessageCultureStr("msgTerminateProcess", "Are you sure to terminate the process and leave ?"), UpdateGlobal.AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
				bool flag2 = dialogResult == DialogResult.Yes;
				if (flag2)
				{
					this.TaskList[this.CurrentTaskIndex].FwUpdateController.CancelAsync();
					e.Cancel = false;
				}
				else
				{
					e.Cancel = true;
				}
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004DDC File Offset: 0x00002FDC
		public void ModeSwitch(bool isRD = false)
		{
			this.is_RD = isRD;
			this.tbctlMode.SelectedIndex = (this.is_RD ? 1 : 0);
			this.tbctlMode.SizeMode = (this.is_RD ? TabSizeMode.Normal : TabSizeMode.Fixed);
			this.tbctlMode.ItemSize = (this.is_RD ? new Size(67, 20) : new Size(0, 1));
			this.tbctlMode.Appearance = (this.is_RD ? TabAppearance.Normal : TabAppearance.FlatButtons);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004E60 File Offset: 0x00003060
		private void InitialComponments()
		{
			bool flag = !this.flashInfo.LoadFlasfFileInfo(this.FlashFloderPath + this.ModelName);
			////if (flag)
			////{
			////	throw new Exception(this.flashInfo.GetErrMsg());
			////}
			bool flag2 = this.flashInfo.fileList.Count > 0;
			if (flag2)
			{
				for (int i = 0; i < this.flashInfo.fileList.Count; i++)
				{
					this.cmbVerList.Items.Add(this.flashInfo.fileList[i].SafetyFWVerStr);
				}
			}
			this.cmbVerList.SelectedIndex = -1;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004F10 File Offset: 0x00003110
		private void cmbVerList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.tbxDriveVer.Text = this.flashInfo.fileList[this.cmbVerList.SelectedIndex].DriveVerStr;
			this.ClearTask();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004F46 File Offset: 0x00003146
		private void tbctlMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.ClearTask();
		}

		// Token: 0x0400001A RID: 26
		private DeviceCOMM pComm;

		// Token: 0x0400001B RID: 27
		private string debugMsg = string.Empty;

		// Token: 0x0400001C RID: 28
		private bool is_RD = false;

		// Token: 0x0400001D RID: 29
		private FlashInfo flashInfo = new FlashInfo();

		// Token: 0x0400001E RID: 30
		public string FlashFloderPath = Directory.GetCurrentDirectory() + "\\Tool\\Flash\\Model\\";

		// Token: 0x0400001F RID: 31
		public string ModelName = "ACS3-SF0301";

		// Token: 0x04000020 RID: 32
		private string onlineSafetyFwVerStr = string.Empty;

		// Token: 0x04000021 RID: 33
		private string onlineSafetyAlarmStr = string.Empty;

		// Token: 0x04000022 RID: 34
		private string onlineDriveFwVerStr = string.Empty;

		// Token: 0x04000023 RID: 35
		private List<BaseTask> TaskList = new List<BaseTask>();

		// Token: 0x04000024 RID: 36
		private int CurrentTaskIndex = 0;

		// Token: 0x04000025 RID: 37
		private bool formClosed = false;

		// Token: 0x02000018 RID: 24
		// (Invoke) Token: 0x0600007F RID: 127
		public delegate void DelShowMessage(string sMessage, bool showTimeInfoInfo);
	}
}
