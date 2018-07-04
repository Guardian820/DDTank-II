using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Fighting.Server.GameObjects
{
	public class ProxyPlayer : IGamePlayer
	{
		private ServerClient m_client;
		private PlayerInfo m_character;
		private ItemTemplateInfo m_currentWeapon;
		private ItemInfo m_secondWeapon;
		private UsersPetinfo m_pet;
		private bool m_canUseProp;
		private int m_gamePlayerId;
		private double GPRate;
		private double OfferRate;
		public double m_antiAddictionRate;
		public List<BufferInfo> Buffers;
		public int m_serverid;
		private double m_baseAglilty;
		private double m_baseAttack;
		private double m_baseDefence;
		private double m_baseBlood;
		private List<ItemInfo> m_equipEffect;
		public int ServerID
		{
			get
			{
				return this.m_serverid;
			}
			set
			{
				this.m_serverid = value;
			}
		}
		public int GamePlayerId
		{
			get
			{
				return this.m_gamePlayerId;
			}
			set
			{
				this.m_gamePlayerId = value;
				this.m_client.SendGamePlayerId(this);
			}
		}
		public PlayerInfo PlayerCharacter
		{
			get
			{
				return this.m_character;
			}
		}
		public ItemTemplateInfo MainWeapon
		{
			get
			{
				return this.m_currentWeapon;
			}
		}
		public ItemInfo SecondWeapon
		{
			get
			{
				return this.m_secondWeapon;
			}
		}
		public UsersPetinfo Pet
		{
			get
			{
				return this.m_pet;
			}
		}
		public bool CanUseProp
		{
			get
			{
				return this.m_canUseProp;
			}
			set
			{
				this.m_canUseProp = value;
			}
		}
		public List<ItemInfo> EquipEffect
		{
			get
			{
				return this.m_equipEffect;
			}
			set
			{
				this.m_equipEffect = value;
			}
		}
		public ProxyPlayer(ServerClient client, PlayerInfo character, ProxyPlayerInfo proxyPlayer, UsersPetinfo pet, List<BufferInfo> infos, List<ItemInfo> euipEffects)
		{
			this.m_client = client;
			this.m_character = character;
			this.m_pet = pet;
			this.m_serverid = proxyPlayer.ServerId;
			this.m_baseAttack = proxyPlayer.BaseAttack;
			this.m_baseDefence = proxyPlayer.BaseDefence;
			this.m_baseAglilty = proxyPlayer.BaseAgility;
			this.m_baseBlood = proxyPlayer.BaseBlood;
			this.m_currentWeapon = proxyPlayer.GetItemTemplateInfo();
			this.m_secondWeapon = proxyPlayer.GetItemInfo();
			this.GPRate = proxyPlayer.GPAddPlus;
			this.OfferRate = proxyPlayer.OfferAddPlus;
			this.m_antiAddictionRate = proxyPlayer.AntiAddictionRate;
			this.m_equipEffect = euipEffects;
			this.Buffers = infos;
		}
		public double GetBaseAgility()
		{
			return this.m_baseAglilty;
		}
		public double GetBaseAttack()
		{
			return this.m_baseAttack;
		}
		public double GetBaseDefence()
		{
			return this.m_baseDefence;
		}
		public double GetBaseBlood()
		{
			return this.m_baseBlood;
		}
		public int AddGP(int gp)
		{
			if (gp > 0)
			{
				this.m_client.SendPlayerAddGP(this.PlayerCharacter.ID, gp);
			}
			return (int)(this.GPRate * (double)gp);
		}
		public int RemoveGP(int gp)
		{
			this.m_client.SendPlayerRemoveGP(this.PlayerCharacter.ID, gp);
			return gp;
		}
		public int AddGold(int value)
		{
			if (value > 0)
			{
				this.m_client.SendPlayerAddGold(this.PlayerCharacter.ID, value);
			}
			return value;
		}
		public int RemoveGold(int value)
		{
			this.m_client.SendPlayerRemoveGold(this.m_character.ID, value);
			return 0;
		}
		public int AddMoney(int value)
		{
			if (value > 0)
			{
				this.m_client.SendPlayerAddMoney(this.m_character.ID, value);
			}
			return value;
		}
		public int RemoveMoney(int value)
		{
			this.m_client.SendPlayerRemoveMoney(this.m_character.ID, value);
			return 0;
		}
		public int AddGiftToken(int value)
		{
			if (value > 0)
			{
				this.m_client.SendPlayerAddGiftToken(this.m_character.ID, value);
			}
			return value;
		}
		public int RemoveGiftToken(int value)
		{
			return 0;
		}
		public int AddMedal(int value)
		{
			if (value > 0)
			{
				this.m_client.SendPlayerAddMedal(this.m_character.ID, value);
			}
			return value;
		}
		public int RemoveMedal(int value)
		{
			return 0;
		}
		public int AddOffer(int baseoffer)
		{
			if (baseoffer < 0)
			{
				return baseoffer;
			}
			return (int)((double)baseoffer * this.OfferRate * this.m_antiAddictionRate);
		}
		public int RemoveOffer(int value)
		{
			this.m_client.SendPlayerRemoveOffer(this.m_character.ID, value);
			return value;
		}
		public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
		{
		}
		public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
		{
			this.m_client.SendPlayerUsePropInGame(this.PlayerCharacter.ID, bag, place, templateId, isLiving);
			game.Pause(500);
			return false;
		}
		public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
		{
			Console.WriteLine("游戏结束:玩家编号【{0}】 房间编号【{1}】 是否胜利【{2}】  伤害【{3}】", new object[]
			{
				this.PlayerCharacter.ID,
				game.Id,
				isWin,
				gainXp
			});
			this.m_client.SendPlayerOnGameOver(this.PlayerCharacter.ID, game.Id, isWin, gainXp);
		}
		public void Disconnect()
		{
			this.m_client.SendDisconnectPlayer(this.m_character.ID);
		}
		public void SendTCP(GSPacketIn pkg)
		{
			this.m_client.SendPacketToPlayer(this.m_character.ID, pkg);
		}
		public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
		{
			Console.WriteLine("游戏结束:玩家编号【{0}】 目标类型【{1}】 目标编号【{2}】  是否活着【{3}】 伤害【{4}】", new object[]
			{
				this.m_character.ID,
				type,
				id,
				isLiving,
				demage
			});
			this.m_client.SendPlayerOnKillingLiving(this.m_character.ID, game, type, id, isLiving, demage);
		}
		public void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int turnNum)
		{
			this.m_client.SendPlayerOnMissionOver(this.m_character.ID, game, isWin, MissionID, turnNum);
		}
		public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
		{
			this.m_client.SendPlayerConsortiaFight(this.m_character.ID, consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth);
			return 0;
		}
		public void SendConsortiaFight(int consortiaID, int riches, string msg)
		{
			this.m_client.SendPlayerSendConsortiaFight(this.m_character.ID, consortiaID, riches, msg);
		}
		public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count)
		{
			this.m_client.SendPlayerAddTemplate(this.m_character.ID, cloneItem, bagType, count);
			return true;
		}
		public void SendMessage(string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteString(msg);
			this.SendTCP(gSPacketIn);
		}
		public bool IsPvePermission(int missionId, eHardLevel hardLevel)
		{
			return true;
		}
		public bool SetPvePermission(int missionId, eHardLevel hardLevel)
		{
			return true;
		}
		public void SendInsufficientMoney(int type)
		{
		}
		public bool ClearTempBag()
		{
			return true;
		}
		public bool ClearFightBag()
		{
			return true;
		}
	}
}
