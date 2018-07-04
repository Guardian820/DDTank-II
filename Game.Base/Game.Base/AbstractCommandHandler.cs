using System;
namespace Game.Base
{
	public abstract class AbstractCommandHandler
	{
		public virtual void DisplayMessage(BaseClient client, string format, params object[] args)
		{
			this.DisplayMessage(client, string.Format(format, args));
		}
		public virtual void DisplayMessage(BaseClient client, string message)
		{
			if (client != null)
			{
				client.DisplayMessage(message);
			}
		}
		public virtual void DisplaySyntax(BaseClient client)
		{
			if (client != null)
			{
				CmdAttribute[] array = (CmdAttribute[])base.GetType().GetCustomAttributes(typeof(CmdAttribute), false);
				if (array.Length > 0)
				{
					client.DisplayMessage(array[0].Description);
					string[] usage = array[0].Usage;
					for (int i = 0; i < usage.Length; i++)
					{
						string msg = usage[i];
						client.DisplayMessage(msg);
					}
				}
			}
		}
	}
}
