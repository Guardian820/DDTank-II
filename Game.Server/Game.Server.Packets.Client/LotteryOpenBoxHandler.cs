using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(26, "打开物品")]
	public class LotteryOpenBoxHandler : IPacketHandler
	{
        public static List<int[]> listRandomitem = new List<int[]>
		{
			LotteryOpenBoxHandler.list1,
			LotteryOpenBoxHandler.list2,
			LotteryOpenBoxHandler.list3,
			LotteryOpenBoxHandler.list4,
			LotteryOpenBoxHandler.list5,
			LotteryOpenBoxHandler.list6,
			LotteryOpenBoxHandler.list7,
			LotteryOpenBoxHandler.list8,
			LotteryOpenBoxHandler.list9,
			LotteryOpenBoxHandler.list10,
			LotteryOpenBoxHandler.list11
		};
        public static int[] list1 = new int[]
		{
			8014,
			8006,
			9203,
			15037,
			7011,
			8302,
			8102,
			1325,
			9406,
			7008,
			7006,
			7039,
			7040,
			8504,
			8404,
			7024,
			7026,
			7027
		};
        public static int[] list2 = new int[]
		{
			8002,
			8014,
			11905,
			9106,
			9306,
			70111,
			70411,
			9303,
			9503,
			70311,
			14010,
			7024,
			70401,
			7039,
			7039,
			7039,
			17002,
			14006
		};
        public static int[] list3 = new int[]
		{
			11011,
			11007,
			11003,
			8004,
			8005,
			8006,
			8314,
			8304,
			8303,
			8306,
			7005,
			7018,
			7017,
			7016,
			7015,
			7024,
			7031,
			7041
		};
        public static int[] list4 = new int[]
		{
			9106,
			9205,
			11904,
			11903,
			70081,
			11905,
			14010,
			17001,
			70061,
			70143,
			14006,
			70161,
			313199,
			313207,
			311402,
			13300,
			70151,
			70241
		};
        public static int[] list5 = new int[]
		{
			11021,
			5251,
			7007,
			8102,
			11263,
			11998,
			13202,
			17001,
			70061,
			70143,
			14006,
			14007,
			313201,
			313208,
			311403,
			13301,
			70151,
			70261
		};
        public static int[] list6 = new int[]
		{
			11001,
			5256,
			7008,
			9102,
			11262,
			11999,
			13203,
			17001,
			70061,
			70143,
			14006,
			14008,
			313202,
			313209,
			311404,
			13302,
			70152,
			70271
		};
        public static int[] list7 = new int[]
		{
			11005,
			5259,
			7009,
			9103,
			11261,
			11997,
			13204,
			17001,
			70061,
			70143,
			14006,
			14009,
			313203,
			313210,
			311405,
			13303,
			70153,
			70281
		};
        public static int[] list8 = new int[]
		{
			8004,
			11010,
			5265,
			7010,
			9105,
			11258,
			11998,
			13205,
			17001,
			70061,
			70143,
			14010,
			313204,
			313211,
			311406,
			13153,
			70161,
			70321
		};
        public static int[] list9 = new int[]
		{
			8005,
			5273,
			7011,
			9106,
			11264,
			11998,
			13206,
			17001,
			70061,
			70143,
			14013,
			14011,
			313205,
			313212,
			311407,
			13154,
			70162,
			70381
		};
        public static int[] list10 = new int[]
		{
			8004,
			8005,
			7005,
			5185,
			7012,
			9122,
			11264,
			11998,
			17001,
			70061,
			70143,
			14012,
			313206,
			311401,
			311408,
			70271,
			70163,
			70411
		};
        public static int[] list11 = new int[]
		{
			11022,
			11007,
			9123,
			9125,
			11906,
			14009,
			14010,
			17001,
			70061,
			70143,
			17002,
			70161,
			70193,
			70233,
			70261,
			70271,
			70281,
			70411
		};
        
        public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			new ProduceBussiness();
			if (client.Lottery != -1)
			{
				client.Out.SendMessage(eMessageType.Normal, "Rương đang hoạt động!");
				return 1;
			}
			int bageType = (int)packet.ReadByte();
			int slot = packet.ReadInt();
			int num = packet.ReadInt();
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			inventory.GetItemAt(slot);
			if (inventory.FindFirstEmptySlot() == -1)
			{
				client.Out.SendMessage(eMessageType.Normal, "Rương đã đầy không thể mở thêm!");
				return 1;
			}

			PlayerInventory propBag = client.Player.PropBag;
			ItemInfo itemByTemplateID = propBag.GetItemByTemplateID(0, 11456);
			new List<ItemInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			List<ItemBoxInfo> list = ItemBoxMgr.FindItemBoxAward(num);
			int index = ThreadSafeRandom.NextStatic(list.Count);
			ItemMgr.FindItemTemplate(num);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list[index].TemplateId), 1, 105);
			bool val = true;
			int num2 = num;
			string str;

            int result = 0;
            PlayerInventory playerInventory = null;
            if (itemInfo != null && itemInfo.TemplateID == 112019)
            {
                client.tempData = "start";
                if (itemInfo.Count > 1)
                {
                    itemInfo.Count--;
                    playerInventory.UpdateItem(itemInfo);
                }
                else
                {
                    playerInventory.RemoveItem(itemInfo);
                }
                GSPacketIn gSPacketIn = new GSPacketIn(29, client.Player.PlayerId);
                int[] array = LotteryOpenBoxHandler.listRandomitem[num];
                for (int i = 0; i < array.Length; i++)
                {
                    int num6 = array[i];
                    gSPacketIn.WriteInt(num6);
                    gSPacketIn.WriteBoolean(false);
                    gSPacketIn.WriteByte(1);
                    gSPacketIn.WriteByte(1);
                }
                client.Out.SendTCP(gSPacketIn);
                result = 1;
                return result;
            }

			if (num2 != 112047)
			{
				switch (num2)
				{
				case 112100:
				case 112101:
					break;

				default:
					str = itemInfo.Template.Name;
					goto IL_17F;
				}
			}
			str = client.Player.PlayerCharacter.NickName;
			itemInfo.BeginDate = DateTime.Now;
			itemInfo.ValidDate = 7;
			itemInfo.RemoveDate = DateTime.Now.AddDays(7.0);
			if (itemByTemplateID.Count > 4)
			{
				itemByTemplateID.Count -= 4;
				propBag.UpdateItem(itemByTemplateID);
			}
			else
			{
				propBag.RemoveItem(itemByTemplateID);
			}
			IL_17F:
			GSPacketIn pkg = new GSPacketIn(245, client.Player.PlayerId);
            pkg.WriteBoolean(val);
            pkg.WriteInt(1);
            pkg.WriteString(str);
            pkg.WriteInt(itemInfo.TemplateID);
            pkg.WriteInt(4);
            pkg.WriteBoolean(false);
            client.Out.SendTCP(pkg);
			inventory.AddItem(itemInfo);
			stringBuilder.Append(itemInfo.Template.Name);
			ItemInfo itemByTemplateID2 = client.Player.PropBag.GetItemByTemplateID(0, num);
			if (itemByTemplateID2.Count > 1)
			{
				itemByTemplateID2.Count--;
				client.Player.PropBag.UpdateItem(itemByTemplateID2);
			}
			else
			{
				client.Player.PropBag.RemoveItem(itemByTemplateID2);
			}
			client.Lottery = -1;
			if (stringBuilder != null)
			{
				client.Out.SendMessage(eMessageType.Normal, "Bạn nhận được " + stringBuilder.ToString());
			}
			return 1;
		}

		public void OpenUpItem()
		{
		}
	}
}
