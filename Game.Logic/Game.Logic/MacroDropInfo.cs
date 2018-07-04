using System;
namespace Game.Logic
{
	public class MacroDropInfo
	{
		public int SelfDropCount
		{
			get;
			set;
		}
		public int DropCount
		{
			get;
			set;
		}
		public int MaxDropCount
		{
			get;
			set;
		}
		public MacroDropInfo(int dropCount, int maxDropCount)
		{
			this.DropCount = dropCount;
			this.MaxDropCount = maxDropCount;
		}
	}
}
