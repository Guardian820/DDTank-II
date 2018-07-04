using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingFlyToAction : BaseAction
	{
		private Living m_living;
		private string m_action;
		private string m_saction;
		private bool m_isSent;
		private int m_toX;
		private int m_toY;
		private int m_fromX;
		private int m_fromY;
		private int m_speed;
		private LivingCallBack m_callback;
		public LivingFlyToAction(Living living, int fromX, int fromY, int toX, int toY, string action, int delay, int speed, LivingCallBack callback) : base(delay, 0)
		{
			this.m_living = living;
			this.m_action = action;
			this.m_speed = speed;
			this.m_toX = toX;
			this.m_toY = toY;
			this.m_fromX = fromX;
			this.m_fromY = fromY;
			this.m_isSent = false;
			this.m_callback = callback;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			if (!this.m_isSent)
			{
				this.m_isSent = true;
				game.SendLivingMoveTo(this.m_living, this.m_fromX, this.m_fromY, this.m_toX, this.m_toY, this.m_action, this.m_speed, "");
			}
			if (this.m_toY < this.m_living.Y - this.m_speed)
			{
				this.m_living.SetXY(this.m_toX, this.m_living.Y - this.m_speed);
				return;
			}
			this.m_living.SetXY(this.m_toX, this.m_toY);
			if (this.m_callback != null)
			{
				this.m_living.CallFuction(this.m_callback, 0);
			}
			base.Finish(tick);
		}
	}
}
