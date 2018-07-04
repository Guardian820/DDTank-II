using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Text.RegularExpressions;
namespace Game.Server.Packets.Client
{
	internal class AASInfoSetHandle : IPacketHandler
	{
		private static Regex _objRegex1 = new Regex("/^[1-9]\\d{7}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{3}$/");
		private static Regex _objRegex2 = new Regex("/^[1-9]\\d{5}[1-9]\\d{3}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{4}$/");
		private static Regex _objRegex = new Regex("\\d{18}|\\d{15}");
		private static string[] cities = new string[]
		{
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"北京",
			"天津",
			"河北",
			"山西",
			"内蒙古",
			null,
			null,
			null,
			null,
			null,
			"辽宁",
			"吉林",
			"黑龙江",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"上海",
			"江苏",
			"浙江",
			"安微",
			"福建",
			"江西",
			"山东",
			null,
			null,
			null,
			"河南",
			"湖北",
			"湖南",
			"广东",
			"广西",
			"海南",
			null,
			null,
			null,
			"重庆",
			"四川",
			"贵州",
			"云南",
			"西藏",
			null,
			null,
			null,
			null,
			null,
			null,
			"陕西",
			"甘肃",
			"青海",
			"宁夏",
			"新疆",
			null,
			null,
			null,
			null,
			null,
			"台湾",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"香港",
			"澳门",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"国外"
		};
		private static int[] WI = new int[]
		{
			7,
			9,
			10,
			5,
			8,
			4,
			2,
			1,
			6,
			3,
			7,
			9,
			10,
			5,
			8,
			4,
			2
		};
		private static char[] checkCode = new char[]
		{
			'1',
			'0',
			'X',
			'9',
			'8',
			'7',
			'6',
			'5',
			'4',
			'3',
			'2'
		};
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			AASInfo aASInfo = new AASInfo();
			aASInfo.UserID = client.Player.PlayerCharacter.ID;
			bool flag = false;
			bool flag2 = packet.ReadBoolean();
			bool flag3;
			if (flag2)
			{
				aASInfo.Name = "";
				aASInfo.IDNumber = "";
				aASInfo.State = 0;
				flag3 = true;
			}
			else
			{
				aASInfo.Name = packet.ReadString();
				aASInfo.IDNumber = packet.ReadString();
				flag3 = this.CheckIDNumber(aASInfo.IDNumber);
				if (aASInfo.IDNumber != "")
				{
					client.Player.IsAASInfo = true;
					int num = Convert.ToInt32(aASInfo.IDNumber.Substring(6, 4));
					int value = Convert.ToInt32(aASInfo.IDNumber.Substring(10, 2));
					if (DateTime.Now.Year.CompareTo(num + 18) > 0 || (DateTime.Now.Year.CompareTo(num + 18) == 0 && DateTime.Now.Month.CompareTo(value) >= 0))
					{
						client.Player.IsMinor = false;
					}
				}
				if (aASInfo.Name != "" && flag3)
				{
					aASInfo.State = 1;
				}
				else
				{
					aASInfo.State = 0;
				}
			}
			if (flag3)
			{
				client.Out.SendAASState(false);
				using (ProduceBussiness produceBussiness = new ProduceBussiness())
				{
					flag = produceBussiness.AddAASInfo(aASInfo);
					client.Out.SendAASInfoSet(flag);
				}
			}
			if (flag && aASInfo.State == 1)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(11019);
				if (itemTemplateInfo != null)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 107);
					if (itemInfo != null)
					{
						itemInfo.IsBinds = true;
						AbstractInventory itemInventory = client.Player.GetItemInventory(itemInfo.Template);
						if (itemInventory.AddItem(itemInfo, itemInventory.BeginSlot))
						{
							client.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ASSInfoSetHandle.Success", new object[]
							{
								itemInfo.Template.Name
							}));
						}
						else
						{
							client.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ASSInfoSetHandle.NoPlace", new object[0]));
						}
					}
				}
			}
			return 0;
		}
		private bool CheckIDNumber(string IDNum)
		{
			bool result = false;
			if (!AASInfoSetHandle._objRegex.IsMatch(IDNum))
			{
				return false;
			}
			int num = int.Parse(IDNum.Substring(0, 2));
			if (AASInfoSetHandle.cities[num] == null)
			{
				return false;
			}
			if (IDNum.Length == 18)
			{
				int num2 = 0;
				for (int i = 0; i < 17; i++)
				{
					num2 += int.Parse(IDNum[i].ToString()) * AASInfoSetHandle.WI[i];
				}
				int num3 = num2 % 11;
				if (IDNum[17] == AASInfoSetHandle.checkCode[num3])
				{
					result = true;
				}
			}
			return result;
		}
	}
}
