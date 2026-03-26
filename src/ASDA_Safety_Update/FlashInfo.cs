using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;

namespace ASDA_Safety_Update
{
	// Token: 0x02000008 RID: 8
	public class FlashInfo
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00002588 File Offset: 0x00000788
		public string GetErrMsg()
		{
			return base.GetType().Name + this.errMsg;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000025DC File Offset: 0x000007DC
		public bool LoadFlasfFileInfo(string folderPath)
		{
			try
			{
				bool flag = !Directory.Exists(folderPath);
				if (flag)
				{
					throw new Exception("Flash data folder not exist : " + folderPath);
				}
				string text = folderPath + "\\temp";
				bool flag2 = Directory.Exists(text);
				if (flag2)
				{
					this.DeleteFolderAll(text);
				}
				Directory.CreateDirectory(text);
				this.fileList.Clear();
				string[] files = Directory.GetFiles(folderPath);
				ushort num = 0;
				while ((int)num < files.Length)
				{
					bool flag3 = Path.GetExtension(files[(int)num]) == ".burn";
					if (flag3)
					{
						this.AddFlashInfoToList(files[(int)num], text);
					}
					num += 1;
				}
				return true;
			}
			catch (Exception ex)
			{
				this.errMsg = "::LoadFlasfFileInfo() : " + ex.Message;
			}
			return false;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000026BC File Offset: 0x000008BC
		private bool AddFlashInfoToList(string burnPath, string tempFolderPath)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(burnPath);
			string text = string.Empty;
			string empty = string.Empty;
			try
			{
				bool flag = !this.DeCompressionFile(tempFolderPath, burnPath, "A1DFD457-34E3-4321-9982-022CDEA79@A31");
				if (flag)
				{
					throw new Exception(this.GetErrMsg());
				}
				string path = tempFolderPath + "\\" + Path.GetFileNameWithoutExtension(burnPath) + "\\info.txt";
				string[] array = File.ReadAllLines(path);
				text = array[2].Substring(array[2].IndexOf("=") + 1, array[2].Length - array[2].IndexOf("=") - 1);
				this.fileList.Add(new FlashFileInfo
				{
					SafetyFWVerStr = fileNameWithoutExtension,
					DriveVerStr = text.Substring(text.IndexOf("v("), text.IndexOf(")_cpld") - text.IndexOf("v(") + 1),
					fileNameMcuA = array[0].Substring(array[0].IndexOf("=") + 1, array[0].Length - array[0].IndexOf("=") - 1),
					fileNameMcuB = array[1].Substring(array[1].IndexOf("=") + 1, array[1].Length - array[1].IndexOf("=") - 1),
					fileNameDrv = array[2].Substring(array[2].IndexOf("=") + 1, array[2].Length - array[2].IndexOf("=") - 1)
				});
				return true;
			}
			catch (Exception ex)
			{
				this.errMsg = "::AddFlashInfoToList() : " + ex.Message;
			}
			return false;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000288C File Offset: 0x00000A8C
		public bool DeleteFolderAll(string folderPath)
		{
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					fileInfo.Delete();
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					directoryInfo2.Delete(true);
				}
				Directory.Delete(folderPath);
				return true;
			}
			catch (Exception ex)
			{
				this.errMsg = "::DeleteFolderAll() : " + ex.Message;
			}
			return false;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002940 File Offset: 0x00000B40
		public bool DeCompressionFile(string filePath, string zipPath, string password = "")
		{
			bool result = false;
			using (ZipFile zipFile = ZipFile.Read(zipPath))
			{
				try
				{
					bool flag = !string.IsNullOrWhiteSpace(password);
					if (flag)
					{
						zipFile.Password = password;
					}
					zipFile.ExtractAll(filePath);
					zipFile.Dispose();
					result = true;
				}
				catch (Exception ex)
				{
					result = false;
					bool flag2 = zipFile != null;
					if (flag2)
					{
						zipFile.Dispose();
					}
					this.errMsg = "::DeCompressionFile() : " + ex.Message;
				}
			}
			return result;
		}

		// Token: 0x0400000B RID: 11
		public const string FlashFilePWD = "A1DFD457-34E3-4321-9982-022CDEA79@A31";

		// Token: 0x0400000C RID: 12
		public const string FlashFileExt = ".burn";

		// Token: 0x0400000D RID: 13
		private string errMsg = string.Empty;

		// Token: 0x0400000E RID: 14
		public List<FlashFileInfo> fileList = new List<FlashFileInfo>();

		// Token: 0x0400000F RID: 15
		public string FolderPath = string.Empty;
	}
}
