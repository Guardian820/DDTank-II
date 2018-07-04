using Bussiness.CenterService;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Security;
namespace Bussiness.Interface
{
	public abstract class BaseInterface
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static string GetInterName
		{
			get
			{
				return ConfigurationManager.AppSettings["InterName"].ToLower();
			}
		}
		public static string GetLoginKey
		{
			get
			{
				return ConfigurationManager.AppSettings["LoginKey"];
			}
		}
		public static string GetChargeKey
		{
			get
			{
				return ConfigurationManager.AppSettings["ChargeKey"];
			}
		}
		public static string LoginUrl
		{
			get
			{
				return ConfigurationManager.AppSettings["LoginUrl"];
			}
		}
		public virtual int ActiveGold
		{
			get
			{
				return int.Parse(ConfigurationManager.AppSettings["DefaultGold"]);
			}
		}
		public virtual int ActiveMoney
		{
			get
			{
				return int.Parse(ConfigurationManager.AppSettings["DefaultMoney"]);
			}
		}
		public static string GetNameBySite(string user, string site)
		{
			if (!string.IsNullOrEmpty(site))
			{
				string value = ConfigurationManager.AppSettings[string.Format("LoginKey_{0}", site)];
				if (!string.IsNullOrEmpty(value))
				{
					user = string.Format("{0}_{1}", site, user);
				}
			}
			return user;
		}
		public static DateTime ConvertIntDateTime(double d)
		{
			DateTime minValue = DateTime.MinValue;
			return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(d);
		}
		public static int ConvertDateTimeInt(DateTime time)
		{
			DateTime d = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			double totalSeconds = (time - d).TotalSeconds;
			return (int)totalSeconds;
		}
		public static string md5(string str)
		{
			return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToLower();
		}
		public static string RequestContent(string Url)
		{
			return BaseInterface.RequestContent(Url, 2560);
		}
		public static string RequestContent(string Url, int byteLength)
		{
			byte[] array = new byte[byteLength];
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
			httpWebRequest.ContentType = "text/plain";
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			int count = responseStream.Read(array, 0, array.Length);
			string @string = Encoding.UTF8.GetString(array, 0, count);
			responseStream.Close();
			return @string;
		}
		public static string RequestContent(string Url, string param, string code)
		{
			Encoding encoding = Encoding.GetEncoding(code);
			byte[] bytes = encoding.GetBytes(param);
			encoding.GetString(bytes);
			byte[] array = new byte[2560];
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			httpWebRequest.ContentLength = (long)bytes.Length;
			using (Stream requestStream = httpWebRequest.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}
			string result;
			using (WebResponse response = httpWebRequest.GetResponse())
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)response;
				Stream responseStream = httpWebResponse.GetResponseStream();
				int count = responseStream.Read(array, 0, array.Length);
				string @string = Encoding.UTF8.GetString(array, 0, count);
				result = @string;
			}
			return result;
		}
		public static BaseInterface CreateInterface()
		{
			string getInterName;
			if ((getInterName = BaseInterface.GetInterName) != null)
			{
				if (getInterName == "qunying")
				{
					return new QYInterface();
				}
				if (getInterName == "sevenroad")
				{
					return new SRInterface();
				}
				if (getInterName == "duowan")
				{
					return new DWInterface();
				}
			}
			return null;
		}
		public virtual PlayerInfo CreateLogin(string name, string password, ref string message, ref int isFirst, string IP, ref bool isError, bool firstValidate, ref bool isActive, string site, string nickname)
		{
			try
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					bool flag = true;
					DateTime now = DateTime.Now;
					PlayerInfo playerInfo = playerBussiness.LoginGame(name, ref isFirst, ref flag, ref isError, firstValidate, ref now, nickname, IP);
					if (playerInfo == null)
					{
						if (!playerBussiness.ActivePlayer(ref playerInfo, name, password, true, this.ActiveGold, this.ActiveMoney, IP, site))
						{
							playerInfo = null;
							message = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Fail", new object[0]);
							goto IL_11B;
						}
						isActive = true;
						using (CenterServiceClient centerServiceClient = new CenterServiceClient())
						{
							centerServiceClient.ActivePlayer(true);
							goto IL_11B;
						}
					}
					if (flag)
					{
						using (CenterServiceClient centerServiceClient2 = new CenterServiceClient())
						{
							centerServiceClient2.CreatePlayer(playerInfo.ID, name, password, isFirst == 0);
							goto IL_11B;
						}
					}
					message = LanguageMgr.GetTranslation("ManageBussiness.Forbid1", new object[]
					{
						now.Year,
						now.Month,
						now.Day,
						now.Hour,
						now.Minute
					});
					PlayerInfo result = null;
					return result;
					IL_11B:
					result = playerInfo;
					return result;
				}
			}
			catch (Exception exception)
			{
				BaseInterface.log.Error("LoginAndUpdate", exception);
			}
			return null;
		}
		public virtual PlayerInfo LoginGame(string name, string pass, ref bool isFirst)
		{
			try
			{
				using (CenterServiceClient centerServiceClient = new CenterServiceClient())
				{
					int iD = 0;
					if (centerServiceClient.ValidateLoginAndGetID(name, pass, ref iD, ref isFirst))
					{
						return new PlayerInfo
						{
							ID = iD,
							UserName = name
						};
					}
				}
			}
			catch (Exception exception)
			{
				BaseInterface.log.Error("LoginGame", exception);
			}
			return null;
		}
		public virtual string[] UnEncryptLogin(string content, ref int result, string site)
		{
			try
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(site))
				{
					text = ConfigurationManager.AppSettings[string.Format("LoginKey_{0}", site)];
				}
				if (string.IsNullOrEmpty(text))
				{
					text = BaseInterface.GetLoginKey;
				}
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = content.Split(new char[]
					{
						'|'
					});
					if (array.Length > 3)
					{
						string a = BaseInterface.md5(array[0] + array[1] + array[2] + text);
						if (a == array[3].ToLower())
						{
							return array;
						}
						result = 5;
					}
					else
					{
						result = 2;
					}
				}
				else
				{
					result = 4;
				}
			}
			catch (Exception exception)
			{
				BaseInterface.log.Error("UnEncryptLogin", exception);
			}
			return new string[0];
		}
		public virtual string[] UnEncryptCharge(string content, ref int result, string site)
		{
			try
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(site))
				{
					text = ConfigurationManager.AppSettings[string.Format("ChargeKey_{0}", site)];
				}
				if (string.IsNullOrEmpty(text))
				{
					text = BaseInterface.GetChargeKey;
				}
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = content.Split(new char[]
					{
						'|'
					});
					string a = BaseInterface.md5(string.Concat(new string[]
					{
						array[0],
						array[1],
						array[2],
						array[3],
						array[4],
						text
					}));
					if (array.Length > 5)
					{
						if (a == array[5].ToLower())
						{
							return array;
						}
						result = 7;
					}
					else
					{
						result = 8;
					}
				}
				else
				{
					result = 6;
				}
			}
			catch (Exception exception)
			{
				BaseInterface.log.Error("UnEncryptCharge", exception);
			}
			return new string[0];
		}
		public virtual string[] UnEncryptSentReward(string content, ref int result, string key)
		{
			try
			{
				string[] array = content.Split(new char[]
				{
					'#'
				});
				if (array.Length == 8)
				{
					string text = ConfigurationManager.AppSettings["SentRewardTimeSpan"];
					int num = int.Parse(string.IsNullOrEmpty(text) ? "1" : text);
					TimeSpan timeSpan = string.IsNullOrEmpty(array[6]) ? new TimeSpan(1, 1, 1) : (DateTime.Now - BaseInterface.ConvertIntDateTime(double.Parse(array[6])));
					if (timeSpan.Days == 0 && timeSpan.Hours == 0 && timeSpan.Minutes < num)
					{
						if (string.IsNullOrEmpty(key))
						{
							string[] result2 = array;
							return result2;
						}
						string a = BaseInterface.md5(string.Concat(new string[]
						{
							array[2],
							array[3],
							array[4],
							array[5],
							array[6],
							key
						}));
						if (a == array[7].ToLower())
						{
							string[] result2 = array;
							return result2;
						}
						result = 5;
					}
					else
					{
						result = 7;
					}
				}
				else
				{
					result = 6;
				}
			}
			catch (Exception exception)
			{
				BaseInterface.log.Error("UnEncryptSentReward", exception);
			}
			return new string[0];
		}
		public virtual bool GetUserSex(string name)
		{
			return true;
		}
	}
}
