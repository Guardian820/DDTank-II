using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(209, "场景用户离开")]
	public class FigSpiritUpGradeHandler : IPacketHandler
	{
		private static readonly string[] places = new string[]
		{
			"0",
			"1",
			"2"
		};
		private static readonly int[] exps = new int[]
		{
			0,
			600,
			5220,
			21660,
			60680,
			150260
		};
		public int getNeedExp(int _curExp, int _curLv)
		{
			int num = FigSpiritUpGradeHandler.exps[_curLv + 1];
			return num - _curExp;
		}
		public bool getMax(string[] SpiritIdValue)
		{
			int num = 0;
			if (SpiritIdValue[0].Split(new char[]
			{
				','
			})[0] == "5")
			{
				num = 1;
			}
			if (SpiritIdValue[1].Split(new char[]
			{
				','
			})[0] == "5")
			{
				num = 2;
			}
			if (SpiritIdValue[2].Split(new char[]
			{
				','
			})[0] == "5")
			{
				num = 3;
			}
			return num == 3;
		}
		public int[] getOldLv(string[] curLvs)
		{
			int[] array = new int[curLvs.Length];
			for (int i = 0; i < curLvs.Length; i++)
			{
				array[i] = Convert.ToInt32(curLvs[i].Split(new char[]
				{
					','
				})[0]);
			}
			return array;
		}
		public int[] getOldExp(string[] curLvs)
		{
			int[] array = new int[curLvs.Length];
			for (int i = 0; i < curLvs.Length; i++)
			{
				array[i] = Convert.ToInt32(curLvs[i].Split(new char[]
				{
					','
				})[1]);
			}
			return array;
		}
		public bool canUpLv(int exp, int _curLv)
		{
			return (exp >= FigSpiritUpGradeHandler.exps[1] && _curLv == 0) || (exp >= FigSpiritUpGradeHandler.exps[2] && _curLv == 1) || (exp >= FigSpiritUpGradeHandler.exps[3] && _curLv == 2) || (exp >= FigSpiritUpGradeHandler.exps[4] && _curLv == 3) || (exp >= FigSpiritUpGradeHandler.exps[5] && _curLv == 4);
		}
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.Grade < 30)
			{
                client.Out.SendMessage(eMessageType.Normal, "Hack level sao bạn!");
				return 0;
			}
			packet.ReadByte();
			int num = packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			int templateId = packet.ReadInt();
			int figSpiritId = packet.ReadInt();
			int place = packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
			int itemCount = client.Player.PropBag.GetItemCount(templateId);
			UserGemStone gemStone = client.Player.GetGemStone(place);
			string[] array = gemStone.FigSpiritIdValue.Split(new char[]
			{
				'|'
			});
			int iD = client.Player.PlayerCharacter.ID;
			bool flag = false;
			bool max = this.getMax(array);
			bool isFall = true;
			int num2 = 1;
			int dir = 0;
			int[] oldExp = this.getOldExp(array);
			int[] oldLv = this.getOldLv(array);
			if (itemCount <= 0)
			{
				client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, max, isFall, 0, dir);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Chiến hồn không đủ!", new object[0]));
				return 0;
			}
			if (DateTime.Compare(client.Player.LastFigUpTime.AddSeconds(2.0), DateTime.Now) > 0)
			{
				client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, max, isFall, 0, dir);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Quá nhiều thao tác!", new object[0]));
				return 1;
			}
			if (!max && itemByTemplateID != null)
			{
				if (num == 0)
				{
					int property = itemByTemplateID.Template.Property2;
					for (int i = 0; i < FigSpiritUpGradeHandler.places.Length; i++)
					{
						if (oldLv[i] < 5)
						{
							oldExp[i] += property;
							flag = this.canUpLv(oldExp[i], oldLv[i]);
							if (flag)
							{
								oldLv[i]++;
								oldExp[i] = 0;
							}
						}
					}
					client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
				}
				if (num == 1)
				{
					int num3 = 1;
					for (int j = 0; j < FigSpiritUpGradeHandler.places.Length; j++)
					{
						num3 = this.getNeedExp(oldExp[j], oldLv[j]) / itemByTemplateID.Template.Property2;
						if (itemCount < num3)
						{
							num3 = itemCount;
						}
						int num4 = itemByTemplateID.Template.Property2 * num3;
						if (oldLv[j] < 5)
						{
							oldExp[j] += num4;
							flag = this.canUpLv(oldExp[j], oldLv[j]);
							if (flag)
							{
								oldLv[j]++;
								oldExp[j] = 0;
							}
						}
					}
					client.Player.PropBag.RemoveTemplate(templateId, num3);
				}
			}
			if (flag)
			{
				isFall = false;
				dir = 1;
				client.Player.MainBag.UpdatePlayerProperties();
			}
			string text = string.Concat(new object[]
			{
				oldLv[0],
				",",
				oldExp[0],
				",",
				FigSpiritUpGradeHandler.places[0]
			});
			for (int k = 1; k < FigSpiritUpGradeHandler.places.Length; k++)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"|",
					oldLv[k],
					",",
					oldExp[k],
					",",
					FigSpiritUpGradeHandler.places[k]
				});
			}
			gemStone.FigSpiritId = figSpiritId;
			gemStone.FigSpiritIdValue = text;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.UpdateGemStoneInfo(gemStone);
			}
			client.Player.OnUserToemGemstoneEvent();
			client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, max, isFall, num2, dir);
			client.Player.LastFigUpTime = DateTime.Now;
			return 0;
		}
	}
}
