using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic.Actions
{
	public class LivingMoveToAction : BaseAction
	{
		private Living m_living;
		private List<Point> m_path;
		private string m_action;
		private bool m_isSent;
		private string m_saction;
		private int m_index;
		private int m_speed;
		private LivingCallBack m_callback;
		public LivingMoveToAction(Living living, List<Point> path, string action, int delay, int speed, string sAction, LivingCallBack callback) : base(delay, 0)
		{
			this.m_living = living;
			this.m_path = path;
			this.m_action = action;
			this.m_saction = sAction;
			this.m_isSent = false;
			this.m_index = 0;
			this.m_callback = callback;
			this.m_speed = speed;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			if (!this.m_isSent)
			{
				this.m_isSent = true;
				game.SendLivingMoveTo(this.m_living, this.m_living.X, this.m_living.Y, this.m_path[this.m_path.Count - 1].X, this.m_path[this.m_path.Count - 1].Y, this.m_action, this.m_speed, this.m_saction);
			}
			this.m_index++;
			if (this.m_index >= this.m_path.Count)
			{
				if (this.m_path[this.m_index - 1].X > this.m_living.X)
				{
					this.m_living.Direction = 1;
				}
				else
				{
					this.m_living.Direction = -1;
				}
				this.m_living.SetXY(this.m_path[this.m_index - 1].X, this.m_path[this.m_index - 1].Y);
				if (this.m_callback != null)
				{
					this.m_living.CallFuction(this.m_callback, 0);
				}
				base.Finish(tick);
			}
		}
	}
}
