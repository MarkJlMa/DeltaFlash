using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ASDA_Safety_Update
{
	// Token: 0x0200000C RID: 12
	public static class IniLoader
	{
		// Token: 0x06000020 RID: 32
		[DllImport("kernel32")]
		private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

		// Token: 0x06000021 RID: 33
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

		// Token: 0x06000022 RID: 34 RVA: 0x00002DC4 File Offset: 0x00000FC4
		public static void SetFilePath(string iniPath)
		{
			FileInfo fileInfo = new FileInfo(iniPath);
			bool flag = !fileInfo.Exists;
			if (flag)
			{
				throw new ApplicationException("File not exist.");
			}
			IniLoader.FileName = fileInfo.FullName;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002DFC File Offset: 0x00000FFC
		public static void WriteString(string Section, string Ident, string Value)
		{
			bool flag = !IniLoader.WritePrivateProfileString(Section, Ident, Value, IniLoader.FileName);
			if (flag)
			{
				throw new ApplicationException("WriteString Error!");
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002E2C File Offset: 0x0000102C
		public static string ReadString(string Section, string Ident, string Default)
		{
			byte[] array = new byte[65535];
			int privateProfileString = IniLoader.GetPrivateProfileString(Section, Ident, Default, array, array.GetUpperBound(0), IniLoader.FileName);
			string text = Encoding.GetEncoding("utf-8").GetString(array);
			text = text.Substring(0, privateProfileString);
			return text.Trim();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002E80 File Offset: 0x00001080
		public static int ReadInteger(string Section, string Ident, int Default)
		{
			string value = IniLoader.ReadString(Section, Ident, Convert.ToString(Default));
			int result;
			try
			{
				result = Convert.ToInt32(value);
			}
			catch (Exception ex)
			{
				Console.WriteLine("ReadInteger() : " + ex.Message);
				result = Default;
			}
			return result;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002ED4 File Offset: 0x000010D4
		public static void WriteInteger(string Section, string Ident, int Value)
		{
			IniLoader.WriteString(Section, Ident, Value.ToString());
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002EE8 File Offset: 0x000010E8
		public static bool ReadBool(string Section, string Ident, bool Default)
		{
			bool result;
			try
			{
				result = Convert.ToBoolean(IniLoader.ReadString(Section, Ident, Convert.ToString(Default)));
			}
			catch (Exception ex)
			{
				Console.WriteLine("ReadBool() : " + ex.Message);
				result = Default;
			}
			return result;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002F3C File Offset: 0x0000113C
		public static void WriteBool(string Section, string Ident, bool Value)
		{
			IniLoader.WriteString(Section, Ident, Convert.ToString(Value));
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002F50 File Offset: 0x00001150
		public static void ReadSection(string Section, StringCollection Idents)
		{
			byte[] array = new byte[16384];
			int privateProfileString = IniLoader.GetPrivateProfileString(Section, null, null, array, array.GetUpperBound(0), IniLoader.FileName);
			IniLoader.GetStringsFromBuffer(array, privateProfileString, Idents);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002F88 File Offset: 0x00001188
		private static void GetStringsFromBuffer(byte[] Buffer, int bufLen, StringCollection Strings)
		{
			Strings.Clear();
			bool flag = bufLen != 0;
			if (flag)
			{
				int num = 0;
				for (int i = 0; i < bufLen; i++)
				{
					bool flag2 = Buffer[i] == 0 && i - num > 0;
					if (flag2)
					{
						string @string = Encoding.GetEncoding(0).GetString(Buffer, num, i - num);
						Strings.Add(@string);
						num = i + 1;
					}
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002FF0 File Offset: 0x000011F0
		public static void ReadSections(StringCollection SectionList)
		{
			byte[] array = new byte[65535];
			int privateProfileString = IniLoader.GetPrivateProfileString(null, null, null, array, array.GetUpperBound(0), IniLoader.FileName);
			IniLoader.GetStringsFromBuffer(array, privateProfileString, SectionList);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000302C File Offset: 0x0000122C
		public static void ReadSectionValues(string Section, NameValueCollection Values)
		{
			StringCollection stringCollection = new StringCollection();
			IniLoader.ReadSection(Section, stringCollection);
			Values.Clear();
			foreach (string text in stringCollection)
			{
				Values.Add(text, IniLoader.ReadString(Section, text, ""));
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000030A4 File Offset: 0x000012A4
		public static void EraseSection(string Section)
		{
			bool flag = !IniLoader.WritePrivateProfileString(Section, null, null, IniLoader.FileName);
			if (flag)
			{
				throw new ApplicationException("EraseSection Error!");
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000030D2 File Offset: 0x000012D2
		public static void DeleteKey(string Section, string Ident)
		{
			IniLoader.WritePrivateProfileString(Section, Ident, null, IniLoader.FileName);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000030E3 File Offset: 0x000012E3
		public static void UpdateFile()
		{
			IniLoader.WritePrivateProfileString(null, null, null, IniLoader.FileName);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000030F4 File Offset: 0x000012F4
		public static bool ValueExists(string Section, string Ident)
		{
			StringCollection stringCollection = new StringCollection();
			IniLoader.ReadSection(Section, stringCollection);
			return stringCollection.IndexOf(Ident) > -1;
		}

		// Token: 0x04000019 RID: 25
		private static string FileName;
	}
}
