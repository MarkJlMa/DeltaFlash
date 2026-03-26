using System;
using System.ComponentModel;

namespace ASDA_Safety_Update.Task
{
	// Token: 0x02000013 RID: 19
	public class BaseTask
	{
		// Token: 0x06000066 RID: 102 RVA: 0x000060F4 File Offset: 0x000042F4
		public string GetDebugMsg()
		{
			return base.GetType().Name + this.debugMsg;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000613C File Offset: 0x0000433C
		public void SetTaskName(string name)
		{
			this.GuiControl.gbxName.Text = name;
		}

		// Token: 0x0400004F RID: 79
		public UpdateControl GuiControl = new UpdateControl();

		// Token: 0x04000050 RID: 80
		public BackgroundWorker FwUpdateController;

		// Token: 0x04000051 RID: 81
		protected string debugMsg = string.Empty;

		// Token: 0x04000052 RID: 82
		public BaseTask.DelProcess NextProcess;

		// Token: 0x04000053 RID: 83
		public BaseTask.GUIControl GuiControlHanle;

		// Token: 0x04000054 RID: 84
		public BaseTask.CompletedEvent CompletedEventHandle;

		// Token: 0x02000019 RID: 25
		// (Invoke) Token: 0x06000083 RID: 131
		public delegate void DelProcess();

		// Token: 0x0200001A RID: 26
		// (Invoke) Token: 0x06000087 RID: 135
		public delegate void GUIControl(bool enable);

		// Token: 0x0200001B RID: 27
		// (Invoke) Token: 0x0600008B RID: 139
		public delegate void CompletedEvent();
	}
}
