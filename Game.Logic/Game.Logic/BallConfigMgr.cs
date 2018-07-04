using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class BallConfigMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, BallConfigInfo> m_infos;
		public static bool Init()
		{
			return BallConfigMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, BallConfigInfo> dictionary = BallConfigMgr.LoadFromDatabase();
				if (dictionary.Values.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, BallConfigInfo>>(ref BallConfigMgr.m_infos, dictionary);
					return true;
				}
			}
			catch (Exception exception)
			{
				BallConfigMgr.log.Error("Ball Mgr init error:", exception);
			}
			return false;
		}
		private static Dictionary<int, BallConfigInfo> LoadFromDatabase()
		{
			Dictionary<int, BallConfigInfo> dictionary = new Dictionary<int, BallConfigInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				BallConfigInfo[] allBallConfig = produceBussiness.GetAllBallConfig();
				BallConfigInfo[] array = allBallConfig;
				for (int i = 0; i < array.Length; i++)
				{
					BallConfigInfo ballConfigInfo = array[i];
					if (!dictionary.ContainsKey(ballConfigInfo.TemplateID))
					{
						dictionary.Add(ballConfigInfo.TemplateID, ballConfigInfo);
					}
				}
			}
			return dictionary;
		}
		public static BallConfigInfo FindBall(int id)
		{
			if (BallConfigMgr.m_infos.ContainsKey(id))
			{
				return BallConfigMgr.m_infos[id];
			}
			return null;
		}
	}
}
