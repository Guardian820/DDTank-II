using Game.Logic.Phy.Maths;
using Game.Logic.Phy.Object;
using System;
using System.Drawing;
namespace Game.Logic.Actions
{
	public class GhostMoveAction : BaseAction
	{
		private Point m_target;
		private Player m_player;
		private Point m_v;
		private bool m_isSend;
		public GhostMoveAction(Player player, Point target) : base(0, 1000)
		{
			this.m_player = player;
			this.m_target = target;
			this.m_v = new Point(target.X - this.m_player.X, target.Y - this.m_player.Y);
			this.m_v.Normalize(2);
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			if (!this.m_isSend)
			{
				this.m_isSend = true;
				game.SendPlayerMove(this.m_player, 2, this.m_target.X, this.m_target.Y, (byte)((this.m_v.X > 0) ? 1 : -1));
			}
			if (this.m_target.Distance(this.m_player.X, this.m_player.Y) > 2.0)
			{
				this.m_player.SetXY(this.m_player.X + this.m_v.X, this.m_player.Y + this.m_v.Y);
				return;
			}
			this.m_player.SetXY(this.m_target.X, this.m_target.Y);
			base.Finish(tick);
		}
	}
}
