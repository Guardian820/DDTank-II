using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic.Actions
{
	public class LivingRangeAttackingAction : BaseAction
	{
		private Living m_living;
		private List<Player> m_players;
		private int m_fx;
		private int m_tx;
		private string m_action;
		public LivingRangeAttackingAction(Living living, int fx, int tx, string action, int delay, List<Player> players) : base(delay, 1000)
		{
			this.m_living = living;
			this.m_players = players;
			this.m_fx = fx;
			this.m_tx = tx;
			this.m_action = action;
		}
		private int MakeDamage(Living p)
		{
			double baseDamage = this.m_living.BaseDamage;
			double arg_12_0 = p.BaseGuard;
			double arg_19_0 = p.Defence;
			double attack = this.m_living.Attack;
			bool arg_31_0 = this.m_living.IgnoreArmor;
			float currentDamagePlus = this.m_living.CurrentDamagePlus;
			float currentShootMinus = this.m_living.CurrentShootMinus;
			double num = 0.95 * (p.BaseGuard - (double)(3 * this.m_living.Grade)) / (500.0 + p.BaseGuard - (double)(3 * this.m_living.Grade));
			double num2;
			if (p.Defence - this.m_living.Lucky < 0.0)
			{
				num2 = 0.0;
			}
			else
			{
				num2 = 0.95 * (p.Defence - this.m_living.Lucky) / (600.0 + p.Defence - this.m_living.Lucky);
			}
			double num3 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num + num2 - num * num2)) * (double)currentDamagePlus * (double)currentShootMinus;
			Rectangle directDemageRect = p.GetDirectDemageRect();
			double num4 = Math.Sqrt((double)((directDemageRect.X - this.m_living.X) * (directDemageRect.X - this.m_living.X) + (directDemageRect.Y - this.m_living.Y) * (directDemageRect.Y - this.m_living.Y)));
			num3 *= 1.0 - num4 / (double)Math.Abs(this.m_tx - this.m_fx) / 4.0;
			if (num3 < 0.0)
			{
				return 1;
			}
			return (int)num3;
		}
		private int MakeCriticalDamage(Living p, int baseDamage)
		{
			double lucky = this.m_living.Lucky;
			Random random = new Random();
			bool flag = 75000.0 * lucky / (lucky + 800.0) > (double)random.Next(100000);
			if (flag)
			{
				return (int)((0.5 + lucky * 0.0003) * (double)baseDamage);
			}
			return 0;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, this.m_living.Id);
			gSPacketIn.Parameter1 = this.m_living.Id;
			gSPacketIn.WriteByte(61);
			List<Living> list = game.Map.FindPlayers(this.m_fx, this.m_tx, this.m_players);
			int num = list.Count;
			foreach (Living current in list)
			{
				if (this.m_living.IsFriendly(current))
				{
					num--;
				}
			}
			gSPacketIn.WriteInt(num);
			this.m_living.SyncAtTime = false;
			try
			{
				foreach (Living current2 in list)
				{
					current2.SyncAtTime = false;
					if (!this.m_living.IsFriendly(current2))
					{
						int val = 0;
						current2.IsFrost = false;
						game.SendGameUpdateFrozenState(current2);
						int num2 = this.MakeDamage(current2);
						int num3 = this.MakeCriticalDamage(current2, num2);
						int val2 = 0;
						if (current2.TakeDamage(this.m_living, ref num2, ref num3, "范围攻击"))
						{
							val2 = num2 + num3;
							if (current2 is Player)
							{
								Player player = current2 as Player;
								val = player.Dander;
							}
						}
						gSPacketIn.WriteInt(current2.Id);
						gSPacketIn.WriteInt(val2);
						gSPacketIn.WriteInt(current2.Blood);
						gSPacketIn.WriteInt(val);
						gSPacketIn.WriteInt(1);
					}
				}
				game.SendToAll(gSPacketIn);
				base.Finish(tick);
			}
			finally
			{
				this.m_living.SyncAtTime = true;
				foreach (Living current3 in list)
				{
					current3.SyncAtTime = true;
				}
			}
		}
	}
}
