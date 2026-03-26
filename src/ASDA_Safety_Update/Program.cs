using System;
using System.Windows.Forms;

namespace ASDA_Safety_Update
{
	// Token: 0x0200000F RID: 15
	internal static class Program
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00005DA5 File Offset: 0x00003FA5
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FwUpdate_F(true, "EN"));
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00005DC8 File Offset: 0x00003FC8
		private static bool IsAllowUpdate()
		{
			bool result = true;
			int num = int.Parse(DateTime.Now.ToString("yyyy"));
			int num2 = int.Parse(DateTime.Now.ToString("MM"));
			bool flag = num != 2023;
			if (flag)
			{
				result = false;
			}
			return result;
		}
	}
}
