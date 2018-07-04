using Bussiness;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Managers
{
	internal class AntiAddictionMgr
	{
		private static bool _isASSon;
		public static int count = 0;
		public static bool ISASSon
		{
			get
			{
				return AntiAddictionMgr._isASSon;
			}
		}
		public static void SetASSState(bool ASSState)
		{
			AntiAddictionMgr._isASSon = ASSState;
		}
		public static double GetAntiAddictionCoefficient(int onlineTime)
		{
			if (!AntiAddictionMgr._isASSon)
			{
				return 1.0;
			}
			if (0 <= onlineTime && onlineTime <= 240)
			{
				return 1.0;
			}
			if (240 < onlineTime && onlineTime <= 300)
			{
				return 0.5;
			}
			return 0.0;
		}
		public static int AASStateGet(GamePlayer player)
		{
			int iD = player.PlayerCharacter.ID;
			bool flag = true;
			player.IsAASInfo = false;
			player.IsMinor = true;
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				string aSSInfoSingle = produceBussiness.GetASSInfoSingle(iD);
				if (aSSInfoSingle != "")
				{
					player.IsAASInfo = true;
					flag = false;
					int num = Convert.ToInt32(aSSInfoSingle.Substring(6, 4));
					int value = Convert.ToInt32(aSSInfoSingle.Substring(10, 2));
					if (DateTime.Now.Year.CompareTo(num + 18) > 0 || (DateTime.Now.Year.CompareTo(num + 18) == 0 && DateTime.Now.Month.CompareTo(value) >= 0))
					{
						player.IsMinor = false;
					}
				}
			}
			if (flag && player.PlayerCharacter.IsFirst != 0 && player.PlayerCharacter.DayLoginCount < 1 && AntiAddictionMgr.ISASSon)
			{
				player.Out.SendAASState(flag);
			}
			if (player.IsMinor || (!player.IsAASInfo && AntiAddictionMgr.ISASSon))
			{
				player.Out.SendAASControl(AntiAddictionMgr.ISASSon, player.IsAASInfo, player.IsMinor);
			}
			return 0;
		}
	}
}
