using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ASDA_Safety_Update
{
	// Token: 0x0200000B RID: 11
	public static class Localize
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002A58 File Offset: 0x00000C58
		public static void SetCulture(string cultureStr)
		{
			bool flag = cultureStr.ToUpper() == ASDALang.CHT.ToString();
			if (flag)
			{
				Localize.CultureValue = 1;
			}
			else
			{
				bool flag2 = cultureStr.ToUpper() == ASDALang.JA.ToString();
				if (flag2)
				{
					Localize.CultureValue = 7;
				}
				else
				{
					bool flag3 = cultureStr.ToUpper() == ASDALang.CHS.ToString();
					if (flag3)
					{
						Localize.CultureValue = 2;
					}
					else
					{
						Localize.CultureValue = 0;
					}
				}
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002AE0 File Offset: 0x00000CE0
		public static bool SetIniFilePath(string iniPath)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(iniPath);
				bool flag = !fileInfo.Exists;
				if (flag)
				{
					throw new Exception("File not exist.");
				}
				IniLoader.SetFilePath(iniPath);
				return true;
			}
			catch (Exception ex)
			{
			}
			return false;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002B34 File Offset: 0x00000D34
		public static void ApplyCultureToGUI(Form form)
		{
			string cultureStr = Localize.GetCultureStr(form.Name, form.Name, "");
			form.Text = ((cultureStr != "") ? cultureStr : form.Text);
			string text = string.Empty;
			List<Control> allControls = Localize.GetAllControls(form);
			foreach (Control control in allControls)
			{
				text = Localize.GetCultureStr(form.Name, control.Name, "");
				bool flag = text != "";
				if (flag)
				{
					control.Text = text;
				}
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002BF8 File Offset: 0x00000DF8
		public static string GetCultureStr(string sectionStr, string keyStr, string defaultStr)
		{
			string ident = keyStr + "." + Localize.CultureValue.ToString() + ".Cap";
			return IniLoader.ReadString(sectionStr, ident, defaultStr).Trim(new char[1]);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002C38 File Offset: 0x00000E38
		public static string GetMessageCultureStr(string keyStr, string defaultStr)
		{
			string ident = keyStr + "." + Localize.CultureValue.ToString() + ".Cap";
			return IniLoader.ReadString("Message", ident, defaultStr).Trim(new char[1]);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002C7C File Offset: 0x00000E7C
		private static List<Control> GetAllControls(Form form)
		{
			return Localize.GetAllControls(Localize.ToList(form.Controls));
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002CA0 File Offset: 0x00000EA0
		private static List<Control> ToList(Control.ControlCollection controls)
		{
			List<Control> list = new List<Control>();
			foreach (object obj in controls)
			{
				Control item = (Control)obj;
				list.Add(item);
			}
			return list;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002D08 File Offset: 0x00000F08
		private static List<Control> GetAllControls(List<Control> inputList)
		{
			List<Control> list = new List<Control>(inputList);
			IEnumerable enumerable = from control in inputList
			where control is GroupBox | control is TabControl | control is Panel | control is FlowLayoutPanel | control is TableLayoutPanel | control is ContainerControl
			select control;
			foreach (object obj in enumerable)
			{
				Control control2 = (Control)obj;
				list.AddRange(Localize.GetAllControls(Localize.ToList(control2.Controls)));
			}
			return list;
		}

		// Token: 0x04000017 RID: 23
		private static int CultureValue = 0;

		// Token: 0x04000018 RID: 24
		public static string IniFilePath = Directory.GetCurrentDirectory() + "\\Tool\\Language\\FwUpdateTool.ini";
	}
}
