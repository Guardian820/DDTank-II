using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class FocusAction : BaseAction
	{
		private Physics m_obj;
		private int m_type;
		public FocusAction(Physics obj, int type, int delay, int finishTime) : base(delay, finishTime)
		{
			this.m_obj = obj;
			this.m_type = type;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			game.SendPhysicalObjFocus(this.m_obj, this.m_type);
			base.Finish(tick);
		}
	}
}
