using System;

namespace ASDA_Safety_Update
{
	// Token: 0x02000009 RID: 9
	public static class UpdateGlobal
	{
		// Token: 0x06000014 RID: 20 RVA: 0x000029E4 File Offset: 0x00000BE4
		public static void SetDisplayFunction(UpdateGlobal.DisplayMessage func)
		{
			UpdateGlobal.displayFunc = new UpdateGlobal.DisplayMessage(func.Invoke);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000029F8 File Offset: 0x00000BF8
		public static void ShowUpdateMessage(string message, bool showTimeInfo = true)
		{
			bool flag = UpdateGlobal.displayFunc != null;
			if (flag)
			{
				UpdateGlobal.displayFunc(message, showTimeInfo);
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002A1F File Offset: 0x00000C1F
		// Note: this type is marked as 'beforefieldinit'.
		static UpdateGlobal()
		{
			string str = "Delta Drive Safety -Firmware Update Tool -V";
			Version version = typeof(FwUpdate_F).Assembly.GetName().Version;
			UpdateGlobal.AppTitle = str + ((version != null) ? version.ToString() : null);
		}

		// Token: 0x04000010 RID: 16
		public static UpdateGlobal.DisplayMessage displayFunc;

		// Token: 0x04000011 RID: 17
		public static string AppTitle;

		// Token: 0x02000016 RID: 22
		// (Invoke) Token: 0x06000078 RID: 120
		public delegate void DisplayMessage(string message, bool showTimeInfo);
	}
}
