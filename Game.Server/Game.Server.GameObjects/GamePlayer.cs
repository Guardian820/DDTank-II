using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Buffer;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using Game.Server.Statics;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server.GameObjects
{
	public class GamePlayer : IGamePlayer
	{
		public delegate void PlayerItemPropertyEventHandle(int templateID);
		public delegate void PlayerGameOverEventHandle(AbstractGame game, bool isWin, int gainXp);
		public delegate void PlayerMissionOverEventHandle(AbstractGame game, int missionId, bool isWin);
		public delegate void PlayerMissionTurnOverEventHandle(AbstractGame game, int missionId, int turnNum);
		public delegate void PlayerItemStrengthenEventHandle(int categoryID, int level);
		public delegate void PlayerShopEventHandle(int money, int gold, int offer, int gifttoken, int medal, string payGoods);
		public delegate void PlayerAdoptPetEventHandle();
		public delegate void PlayerNewGearEventHandle(int CategoryID);
		public delegate void PlayerCropPrimaryEventHandle();
		public delegate void PlayerSeedFoodPetEventHandle();
		public delegate void PlayerUserToemGemstoneEventHandle();
		public delegate void PlayerItemInsertEventHandle();
		public delegate void PlayerItemFusionEventHandle(int fusionType);
		public delegate void PlayerItemMeltEventHandle(int categoryID);
		public delegate void PlayerGameKillEventHandel(AbstractGame game, int type, int id, bool isLiving, int demage);
		public delegate void PlayerOwnConsortiaEventHandle();
		public delegate void PlayerItemComposeEventHandle(int composeType);
		public delegate void GameKillDropEventHandel(AbstractGame game, int type, int npcId, bool playResult);
		public delegate void PlayerEventHandle(GamePlayer player);
		private DiceSystemInfo m_DicePlace;
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private object charge_locker = new object();
		protected BaseGame m_game;
		private int m_playerId;
		protected GameClient m_client;
		protected Player m_players;
		private PlayerInfo m_character;
		private string m_account;
		private int m_immunity = 255;
		private Dictionary<string, Dictionary<string, int>> m_playerProperty;
		public bool m_toemview;
		public int FightPower;
		private bool m_isMinor;
		private bool m_isAASInfo;
		private long m_pingTime;
		private byte[] m_pvepermissions;
		public long PingStart;
		private UsersPetinfo m_pet;
		private PlayerEquipInventory m_mainBag;
		private PlayerInventory m_propBag;
		private PlayerInventory m_fightBag;
		private PlayerInventory m_ConsortiaBag;
		private PlayerInventory m_storeBag;
		private PlayerInventory m_tempBag;
		private PlayerInventory m_caddyBag;
		private PlayerInventory m_farmBag;
		private PlayerInventory m_vegetable;
		private PlayerInventory m_food;
		private PlayerInventory m_petEgg;
		private PlayerBeadInventory m_BeadBag;
		private CardInventory m_cardBag;
		private PlayerFarm m_farm;
		private PetInventory m_petBag;
		private QuestInventory m_questInventory;
		private BufferList m_bufferList;
		private BufferList m_buffEffect;
		private List<UserGemStone> m_GemStone;
		private List<TreasureTempInfo> m_TreasureTem;
		private List<TreasureTempInfo> m_TreasureAdd;
		private UserTreasureInfo m_Treasure;
		private Dictionary<int, UserDrillInfo> m_userDrills;
		private List<ItemInfo> m_equipEffect;
		private int m_changed;
		public double GPAddPlus;
		public double OfferAddPlus = 1.0;
		public double GuildRichAddPlus = 1.0;
		public DateTime LastChatTime;
		public DateTime LastFigUpTime;
		public DateTime LastDrillUpTime;
		public bool KickProtect;
		private ItemInfo m_MainWeapon;
		private ItemInfo m_currentSecondWeapon;
		private List<int> _viFarms;
		private Dictionary<int, int> _friends;
		private BaseRoom m_currentRoom;
		public int CurrentRoomIndex;
		public int CurrentRoomTeam;
		public int WorldBossX;
		public int WorldBossY;
		public int WorldBossMap;
		public byte WorldBossState;
		public bool IsInWorldBossRoom;
		public int ScreenStyle;
		public int X;
		public int Y;
		public int MarryMap;
		private MarryRoom _currentMarryRoom;
		public int Hot_X;
		public int Hot_Y;
		public int HotMap;
		private UTF8Encoding m_converter;
		private Dictionary<string, object> m_tempProperties = new Dictionary<string, object>();
		public event GamePlayer.PlayerEventHandle LevelUp;
		public event GamePlayer.PlayerItemPropertyEventHandle AfterUsingItem;
		public event GamePlayer.PlayerGameOverEventHandle GameOver;
		public event GamePlayer.PlayerMissionOverEventHandle MissionOver;
		public event GamePlayer.PlayerMissionTurnOverEventHandle MissionTurnOver;
		public event GamePlayer.PlayerItemStrengthenEventHandle ItemStrengthen;
		public event GamePlayer.PlayerShopEventHandle Paid;
		public event GamePlayer.PlayerAdoptPetEventHandle AdoptPetEvent;
		public event GamePlayer.PlayerNewGearEventHandle NewGearEvent;
		public event GamePlayer.PlayerCropPrimaryEventHandle CropPrimaryEvent;
		public event GamePlayer.PlayerSeedFoodPetEventHandle SeedFoodPetEvent;
		public event GamePlayer.PlayerUserToemGemstoneEventHandle UserToemGemstonetEvent;
		public event GamePlayer.PlayerItemInsertEventHandle ItemInsert;
		public event GamePlayer.PlayerItemFusionEventHandle ItemFusion;
		public event GamePlayer.PlayerItemMeltEventHandle ItemMelt;
		public event GamePlayer.PlayerGameKillEventHandel AfterKillingLiving;
		public event GamePlayer.PlayerOwnConsortiaEventHandle GuildChanged;
		public event GamePlayer.PlayerItemComposeEventHandle ItemCompose;
		public event GamePlayer.GameKillDropEventHandel GameKillDrop;
		public event GamePlayer.PlayerEventHandle UseBuffer;
		public DiceSystemInfo DicePlace
		{
			get
			{
				return this.m_DicePlace;
			}
			set
			{
				this.m_DicePlace = value;
			}
		}
		public BaseGame game
		{
			get
			{
				return this.m_game;
			}
			set
			{
				this.m_game = value;
			}
		}
		public Dictionary<string, Dictionary<string, int>> PlayerProperty
		{
			get
			{
				return this.m_playerProperty;
			}
			set
			{
				this.m_playerProperty = value;
			}
		}
		public int Immunity
		{
			get
			{
				return this.m_immunity;
			}
			set
			{
				this.m_immunity = value;
			}
		}
		public int PlayerId
		{
			get
			{
				return this.m_playerId;
			}
		}
		public bool Toemview
		{
			get
			{
				return this.m_toemview;
			}
			set
			{
				this.m_toemview = value;
			}
		}
		public string Account
		{
			get
			{
				return this.m_account;
			}
		}
		public PlayerInfo PlayerCharacter
		{
			get
			{
				return this.m_character;
			}
		}
		public GameClient Client
		{
			get
			{
				return this.m_client;
			}
		}
		public Player Players
		{
			get
			{
				return this.m_players;
			}
		}
		public bool IsActive
		{
			get
			{
				return this.m_client.IsConnected;
			}
		}
		public IPacketLib Out
		{
			get
			{
				return this.m_client.Out;
			}
		}
		public bool IsMinor
		{
			get
			{
				return this.m_isMinor;
			}
			set
			{
				this.m_isMinor = value;
			}
		}
		public bool IsAASInfo
		{
			get
			{
				return this.m_isAASInfo;
			}
			set
			{
				this.m_isAASInfo = value;
			}
		}
		public long PingTime
		{
			get
			{
				return this.m_pingTime;
			}
			set
			{
				this.m_pingTime = value;
				GSPacketIn pkg = this.Out.SendNetWork(this, this.m_pingTime);
				if (this.m_currentRoom != null)
				{
					this.m_currentRoom.SendToAll(pkg, this);
				}
			}
		}
		public UsersPetinfo Pet
		{
			get
			{
				return this.m_pet;
			}
		}
		public PetInventory PetBag
		{
			get
			{
				return this.m_petBag;
			}
		}
		public PlayerFarm Farm
		{
			get
			{
				return this.m_farm;
			}
		}
		public PlayerEquipInventory MainBag
		{
			get
			{
				return this.m_mainBag;
			}
		}
		public PlayerInventory PropBag
		{
			get
			{
				return this.m_propBag;
			}
		}
		public PlayerInventory FightBag
		{
			get
			{
				return this.m_fightBag;
			}
		}
		public PlayerInventory TempBag
		{
			get
			{
				return this.m_tempBag;
			}
		}
		public PlayerInventory ConsortiaBag
		{
			get
			{
				return this.m_ConsortiaBag;
			}
		}
		public PlayerInventory StoreBag
		{
			get
			{
				return this.m_storeBag;
			}
		}
		public PlayerInventory CaddyBag
		{
			get
			{
				return this.m_caddyBag;
			}
		}
		public PlayerInventory FarmBag
		{
			get
			{
				return this.m_farmBag;
			}
		}
		public PlayerInventory Vegetable
		{
			get
			{
				return this.m_vegetable;
			}
		}
		public PlayerInventory Food
		{
			get
			{
				return this.m_food;
			}
		}
		public PlayerInventory PetEgg
		{
			get
			{
				return this.m_petEgg;
			}
		}
		public PlayerBeadInventory BeadBag
		{
			get
			{
				return this.m_BeadBag;
			}
		}
		public CardInventory CardBag
		{
			get
			{
				return this.m_cardBag;
			}
		}
		public QuestInventory QuestInventory
		{
			get
			{
				return this.m_questInventory;
			}
		}
		public BufferList BufferList
		{
			get
			{
				return this.m_bufferList;
			}
		}
		public BufferList BuffEffect
		{
			get
			{
				return this.m_buffEffect;
			}
		}
		public List<UserGemStone> GemStone
		{
			get
			{
				return this.m_GemStone;
			}
			set
			{
				this.m_GemStone = value;
			}
		}
		public List<TreasureTempInfo> TreasureTem
		{
			get
			{
				return this.m_TreasureTem;
			}
			set
			{
				this.m_TreasureTem = value;
			}
		}
		public List<TreasureTempInfo> TreasureAdd
		{
			get
			{
				return this.m_TreasureAdd;
			}
			set
			{
				this.m_TreasureAdd = value;
			}
		}
		public UserTreasureInfo Treasure
		{
			get
			{
				return this.m_Treasure;
			}
			set
			{
				this.m_Treasure = value;
			}
		}
		public Dictionary<int, UserDrillInfo> UserDrills
		{
			get
			{
				return this.m_userDrills;
			}
			set
			{
				this.m_userDrills = value;
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
		public bool CanUseProp
		{
			get;
			set;
		}
		public int Level
		{
			get
			{
				return this.m_character.Grade;
			}
			set
			{
				if (value != this.m_character.Grade)
				{
					this.m_character.Grade = value;
					this.OnLevelUp(value);
					this.OnPropertiesChanged();
				}
			}
		}
		public int LevelPlusBlood
		{
			get
			{
				return LevelMgr.FindLevel(this.m_character.Grade).Blood;
			}
		}
		public ItemTemplateInfo MainWeapon
		{
			get
			{
				if (this.m_MainWeapon == null)
				{
					return null;
				}
				return this.m_MainWeapon.Template;
			}
		}
		public ItemInfo SecondWeapon
		{
			get
			{
				if (this.m_currentSecondWeapon == null)
				{
					return null;
				}
				return this.m_currentSecondWeapon;
			}
		}
		public List<int> ViFarms
		{
			get
			{
				return this._viFarms;
			}
		}
		public Dictionary<int, int> Friends
		{
			get
			{
				return this._friends;
			}
		}
		public BaseRoom CurrentRoom
		{
			get
			{
				return this.m_currentRoom;
			}
			set
			{
				BaseRoom baseRoom = Interlocked.Exchange<BaseRoom>(ref this.m_currentRoom, value);
				if (baseRoom != null)
				{
					RoomMgr.ExitRoom(baseRoom, this);
				}
			}
		}
		public int GamePlayerId
		{
			get;
			set;
		}
		public MarryRoom CurrentMarryRoom
		{
			get
			{
				return this._currentMarryRoom;
			}
			set
			{
				this._currentMarryRoom = value;
			}
		}
		public bool IsInMarryRoom
		{
			get
			{
				return this._currentMarryRoom != null;
			}
		}
		public int ServerID
		{
			get;
			set;
		}
		public Dictionary<string, object> TempProperties
		{
			get
			{
				return this.m_tempProperties;
			}
		}
		public GamePlayer(int playerId, string account, GameClient client, PlayerInfo info)
		{
			this.m_playerId = playerId;
			this.m_account = account;
			this.m_client = client;
			this.m_character = info;
			this.LastChatTime = DateTime.Today;
			this.m_mainBag = new PlayerEquipInventory(this);
			this.m_BeadBag = new PlayerBeadInventory(this);
			this.m_propBag = new PlayerInventory(this, true, 49, 1, 0, true);
			this.m_ConsortiaBag = new PlayerInventory(this, true, 100, 11, 0, true);
			this.m_storeBag = new PlayerInventory(this, true, 20, 12, 0, true);
			this.m_fightBag = new PlayerInventory(this, false, 3, 3, 0, false);
			this.m_tempBag = new PlayerInventory(this, false, 61, 4, 0, true);
			this.m_caddyBag = new PlayerInventory(this, false, 30, 5, 0, true);
			this.m_farmBag = new PlayerInventory(this, true, 30, 13, 0, true);
			this.m_vegetable = new PlayerInventory(this, true, 30, 14, 0, true);
			this.m_food = new PlayerInventory(this, true, 30, 34, 0, true);
			this.m_petEgg = new PlayerInventory(this, true, 30, 35, 0, false);
			this.m_cardBag = new CardInventory(this, true, 100, 0);
			this.m_farm = new PlayerFarm(this, true, 16, 0);
			this.m_petBag = new PetInventory(this, true, 10, 8, 0);
			this.m_questInventory = new QuestInventory(this);
			this.m_bufferList = new BufferList(this);
			this.m_equipEffect = new List<ItemInfo>();
			this.m_GemStone = new List<UserGemStone>();
			this.m_TreasureTem = new List<TreasureTempInfo>();
			this.m_TreasureAdd = new List<TreasureTempInfo>();
			this.m_userDrills = new Dictionary<int, UserDrillInfo>();
			this.m_Treasure = new UserTreasureInfo();
			this.m_playerProperty = new Dictionary<string, Dictionary<string, int>>();
			this.GPAddPlus = 1.0;
			this.m_toemview = true;
			this.X = 646;
			this.Y = 1241;
			this.MarryMap = 0;
			this.ScreenStyle = 0;
			this.m_converter = new UTF8Encoding();
		}
		public PlayerInventory GetInventory(eBageType bageType)
		{
			switch (bageType)
			{
			case eBageType.MainBag:
				return this.m_mainBag;

			case eBageType.PropBag:
				return this.m_propBag;

			case eBageType.TaskBag:
			case (eBageType)6:
			case (eBageType)7:
			case (eBageType)8:
			case (eBageType)9:
			case (eBageType)10:
				break;

			case eBageType.FightBag:
				return this.m_fightBag;

			case eBageType.TempBag:
				return this.m_tempBag;

			case eBageType.CaddyBag:
				return this.m_caddyBag;

			case eBageType.Consortia:
				return this.m_ConsortiaBag;

			case eBageType.Store:
				return this.m_storeBag;

			case eBageType.Farm:
				return this.m_farmBag;

			case eBageType.Vegetable:
				return this.m_vegetable;

			default:
				if (bageType == eBageType.BeadBag)
				{
					return this.m_BeadBag;
				}
				switch (bageType)
				{
				case eBageType.Food:
					return this.m_food;

				case eBageType.PetEgg:
					return this.m_petEgg;
				}
				break;
			}
			throw new NotSupportedException(string.Format("Did not support this type bag: {0}", bageType));
		}
		public string GetInventoryName(eBageType bageType)
		{
			switch (bageType)
			{
			case eBageType.MainBag:
				return LanguageMgr.GetTranslation("Game.Server.GameObjects.Equip", new object[0]);

			case eBageType.PropBag:
				return LanguageMgr.GetTranslation("Game.Server.GameObjects.Prop", new object[0]);

			default:
				return bageType.ToString();
			}
		}
		public PlayerInventory GetItemInventory(ItemTemplateInfo template)
		{
			return this.GetInventory(template.BagType);
		}
		public ItemInfo GetItemAt(eBageType bagType, int place)
		{
			PlayerInventory inventory = this.GetInventory(bagType);
			if (inventory != null)
			{
				return inventory.GetItemAt(place);
			}
			return null;
		}
		public int GetItemCount(int templateId)
		{
			return this.m_propBag.GetItemCount(templateId) + this.m_mainBag.GetItemCount(templateId) + this.m_ConsortiaBag.GetItemCount(templateId);
		}
		public bool AddItem(ItemInfo item)
		{
			AbstractInventory itemInventory = this.GetItemInventory(item.Template);
			return itemInventory.AddItem(item, itemInventory.BeginSlot);
		}
		public bool StackItemToAnother(ItemInfo item)
		{
			AbstractInventory itemInventory = this.GetItemInventory(item.Template);
			return itemInventory.StackItemToAnother(item);
		}
		public void UpdateItem(ItemInfo item)
		{
			this.m_mainBag.UpdateItem(item);
			this.m_propBag.UpdateItem(item);
		}
		public bool RemoveItem(ItemInfo item)
		{
			if (item.BagType == this.m_propBag.BagType)
			{
				return this.m_propBag.RemoveItem(item);
			}
			if (item.BagType == this.m_fightBag.BagType)
			{
				return this.m_fightBag.RemoveItem(item);
			}
			if (item.BagType == this.m_BeadBag.BagType)
			{
				return this.m_BeadBag.RemoveItem(item);
			}
			return this.m_mainBag.RemoveItem(item);
		}
		public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count)
		{
			PlayerInventory inventory = this.GetInventory(bagType);
			if (inventory != null && inventory.AddTemplate(cloneItem, count))
			{
				if (this.CurrentRoom != null && this.CurrentRoom.IsPlaying)
				{
					this.SendItemNotice(cloneItem);
				}
				return true;
			}
			return false;
		}
		public bool RemoveTemplate(int templateId, int count)
		{
			int itemCount = this.m_mainBag.GetItemCount(templateId);
			int itemCount2 = this.m_propBag.GetItemCount(templateId);
			int itemCount3 = this.m_ConsortiaBag.GetItemCount(templateId);
			int num = itemCount + itemCount2 + itemCount3;
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
			if (itemTemplateInfo != null && num >= count)
			{
				if (itemCount > 0 && count > 0 && this.RemoveTempate(eBageType.MainBag, itemTemplateInfo, (itemCount > count) ? count : itemCount))
				{
					count = ((count < itemCount) ? 0 : (count - itemCount));
				}
				if (itemCount2 > 0 && count > 0 && this.RemoveTempate(eBageType.PropBag, itemTemplateInfo, (itemCount2 > count) ? count : itemCount2))
				{
					count = ((count < itemCount2) ? 0 : (count - itemCount2));
				}
				if (itemCount3 > 0 && count > 0 && this.RemoveTempate(eBageType.Consortia, itemTemplateInfo, (itemCount3 > count) ? count : itemCount3))
				{
					count = ((count < itemCount3) ? 0 : (count - itemCount3));
				}
				if (count == 0)
				{
					return true;
				}
				if (GamePlayer.log.IsErrorEnabled)
				{
					GamePlayer.log.Error(string.Format("Item Remover Error：PlayerId {0} Remover TemplateId{1} Is Not Zero!", this.m_playerId, templateId));
				}
			}
			return false;
		}
		public bool RemoveTempate(eBageType bagType, ItemTemplateInfo template, int count)
		{
			PlayerInventory inventory = this.GetInventory(bagType);
			return inventory != null && inventory.RemoveTemplate(template.TemplateID, count);
		}
		public bool RemoveTemplate(ItemTemplateInfo template, int count)
		{
			PlayerInventory itemInventory = this.GetItemInventory(template);
			return itemInventory != null && itemInventory.RemoveTemplate(template.TemplateID, count);
		}
		public bool ClearTempBag()
		{
			this.TempBag.ClearBag();
			return true;
		}
		public bool ClearFightBag()
		{
			this.FightBag.ClearBag();
			return true;
		}
		public void ClearCaddyBag()
		{
			List<ItemInfo> list = new List<ItemInfo>();
			for (int i = 0; i < this.CaddyBag.Capalility; i++)
			{
				ItemInfo itemAt = this.CaddyBag.GetItemAt(i);
				if (itemAt != null)
				{
					if (itemAt.Template.BagType == eBageType.PropBag)
					{
						int place = this.PropBag.FindFirstEmptySlot();
						if (this.PropBag.StackItemToAnother(itemAt) || this.PropBag.AddItemTo(itemAt, place))
						{
							this.CaddyBag.TakeOutItem(itemAt);
						}
						else
						{
							list.Add(itemAt);
						}
					}
					else
					{
						if (itemAt.Template.BagType == eBageType.BeadBag)
						{
							int place2 = this.BeadBag.FindFirstEmptySlot(32);
							if (this.BeadBag.AddItemTo(itemAt, place2))
							{
								this.CaddyBag.TakeOutItem(itemAt);
							}
							else
							{
								list.Add(itemAt);
							}
						}
						else
						{
							int place2 = this.MainBag.FindFirstEmptySlot(31);
							if (this.MainBag.StackItemToAnother(itemAt) || this.MainBag.AddItemTo(itemAt, place2))
							{
								this.CaddyBag.TakeOutItem(itemAt);
							}
							else
							{
								list.Add(itemAt);
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				this.SendItemsToMail(list, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), eMailType.ItemOverdue);
				this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
		public void ClearStoreBag()
		{
			List<ItemInfo> items = this.m_storeBag.GetItems();
			foreach (ItemInfo current in items)
			{
				PlayerInventory itemInventory = this.GetItemInventory(current.Template);
				string key = string.Format("temp_place_{0}", current.ItemID);
				if (this.TempProperties.ContainsKey(key))
				{
					int place = (int)this.TempProperties[key];
					this.TempProperties.Remove(key);
					if (itemInventory.AddItemTo(current, place))
					{
						this.m_storeBag.TakeOutItem(current);
					}
				}
				else
				{
					if (itemInventory.StackItemToAnother(current))
					{
						this.m_storeBag.RemoveItem(current, eItemRemoveType.Stack);
					}
					else
					{
						if (itemInventory.AddItem(current))
						{
							this.m_storeBag.TakeOutItem(current);
						}
					}
				}
			}
			items = this.m_storeBag.GetItems();
			if (items.Count > 0)
			{
				string translation = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content2", new object[0]);
				string translation2 = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title2", new object[0]);
				this.SendItemsToMail(items, translation, translation2, eMailType.ItemOverdue);
				this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
				this.m_storeBag.Clear();
			}
		}
		public void OnLevelUp(int grade)
		{
			if (this.LevelUp != null)
			{
				this.LevelUp(this);
			}
		}
		public void OnUsingItem(int templateID)
		{
			if (this.AfterUsingItem != null)
			{
				this.AfterUsingItem(templateID);
			}
		}
		public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
		{
			if (game.RoomType == eRoomType.Match)
			{
				if (isWin)
				{
					this.m_character.Win++;
				}
				this.m_character.Total++;
			}
			if (this.GameOver != null)
			{
				this.GameOver(game, isWin, gainXp);
			}
		}
		public void OnMissionOver(AbstractGame game, bool isWin, int missionId, int turnNum)
		{
			if (this.MissionOver != null)
			{
				this.MissionOver(game, missionId, isWin);
			}
			if (this.MissionTurnOver != null && isWin)
			{
				this.MissionTurnOver(game, missionId, turnNum);
			}
		}
		public void OnItemStrengthen(int categoryID, int level)
		{
			if (this.ItemStrengthen != null)
			{
				this.ItemStrengthen(categoryID, level);
			}
		}
		public void OnPaid(int money, int gold, int offer, int gifttoken, int medal, string payGoods)
		{
			if (this.Paid != null)
			{
				this.Paid(money, gold, offer, gifttoken, medal, payGoods);
			}
		}
		public void OnAdoptPetEvent()
		{
			if (this.AdoptPetEvent != null)
			{
				this.AdoptPetEvent();
			}
		}
		public void OnNewGearEvent(int CategoryID)
		{
			if (this.NewGearEvent != null)
			{
				this.NewGearEvent(CategoryID);
			}
		}
		public void OnCropPrimaryEvent()
		{
			if (this.CropPrimaryEvent != null)
			{
				this.CropPrimaryEvent();
			}
		}
		public void OnSeedFoodPetEvent()
		{
			if (this.SeedFoodPetEvent != null)
			{
				this.SeedFoodPetEvent();
			}
		}
		public void OnUserToemGemstoneEvent()
		{
			if (this.UserToemGemstonetEvent != null)
			{
				this.UserToemGemstonetEvent();
			}
		}
		public void OnItemInsert()
		{
			if (this.ItemInsert != null)
			{
				this.ItemInsert();
			}
		}
		public void OnItemFusion(int fusionType)
		{
			if (this.ItemFusion != null)
			{
				this.ItemFusion(fusionType);
			}
		}
		public void OnItemMelt(int categoryID)
		{
			if (this.ItemMelt != null)
			{
				this.ItemMelt(categoryID);
			}
		}
		public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int damage)
		{
			if (this.AfterKillingLiving != null)
			{
				this.AfterKillingLiving(game, type, id, isLiving, damage);
			}
			if (this.GameKillDrop != null && !isLiving)
			{
				this.GameKillDrop(game, type, id, isLiving);
			}
		}
		public void OnGuildChanged()
		{
			if (this.GuildChanged != null)
			{
				this.GuildChanged();
			}
		}
		public void OnItemCompose(int composeType)
		{
			if (this.ItemCompose != null)
			{
				this.ItemCompose(composeType);
			}
		}
		public void OnUseBuffer()
		{
			if (this.UseBuffer != null)
			{
				this.UseBuffer(this);
			}
		}
		public void AddBeadEffect(ItemInfo item)
		{
			this.m_equipEffect.Add(item);
		}
		public void BeginAllChanges()
		{
			this.BeginChanges();
			this.m_bufferList.BeginChanges();
			this.m_mainBag.BeginChanges();
			this.m_propBag.BeginChanges();
			this.m_BeadBag.BeginChanges();
			this.m_farmBag.BeginChanges();
			this.m_vegetable.BeginChanges();
			this.m_cardBag.BeginChanges();
		}
		public void CommitAllChanges()
		{
			this.CommitChanges();
			this.m_bufferList.CommitChanges();
			this.m_mainBag.CommitChanges();
			this.m_propBag.CommitChanges();
			this.m_BeadBag.BeginChanges();
			this.m_farmBag.CommitChanges();
			this.m_vegetable.CommitChanges();
			this.m_cardBag.CommitChanges();
		}
		public void BeginChanges()
		{
			Interlocked.Increment(ref this.m_changed);
		}
		public void CommitChanges()
		{
			Interlocked.Decrement(ref this.m_changed);
			this.OnPropertiesChanged();
		}
		protected void OnPropertiesChanged()
		{
			if (this.m_changed <= 0)
			{
				if (this.m_changed < 0)
				{
					GamePlayer.log.Error("Player changed count < 0");
					Thread.VolatileWrite(ref this.m_changed, 0);
				}
				this.UpdateProperties();
			}
		}
		public void UpdateDrill(int index, UserDrillInfo drill)
		{
			this.m_userDrills[index] = drill;
		}
		public void UpdateBadgeId(int Id)
		{
			this.m_character.badgeID = Id;
		}
		public void UpdateTimeBox(int receiebox, int receieGrade, int needGetBoxTime)
		{
			this.m_character.receiebox = receiebox;
			this.m_character.receieGrade = receieGrade;
			this.m_character.needGetBoxTime = needGetBoxTime;
		}
		public void UpdateProperties()
		{
			this.Out.SendUpdatePrivateInfo(this.m_character);
			GSPacketIn pkg = this.Out.SendUpdatePublicPlayer(this.m_character);
			if (this.m_currentRoom != null)
			{
				this.m_currentRoom.SendToAll(pkg, this);
			}
		}
		public int AddGold(int value)
		{
			if (value > 0)
			{
				this.m_character.Gold += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemoveGold(int value)
		{
			if (value > 0 && value <= this.m_character.Gold)
			{
				this.m_character.Gold -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddMoney(int value)
		{
			if (value > 0)
			{
				this.m_character.Money += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddHonor(int value)
		{
			if (value > 0)
			{
				this.m_character.myHonor += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddTotem(int value)
		{
			if (value > 0)
			{
				this.m_character.totemId += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddMaxHonor(int value)
		{
			if (value > 0)
			{
				this.m_character.MaxBuyHonor += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemoveMoney(int value)
		{
			if (value > 0 && value <= this.m_character.Money)
			{
				this.m_character.Money -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddLeagueMoney(int value)
		{
			if (value > 0)
			{
				this.m_character.LeagueMoney += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemoveLeagueMoney(int value)
		{
			if (value > 0 && value <= this.m_character.LeagueMoney)
			{
				this.m_character.LeagueMoney -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public void RemoveFistGetPet()
		{
			this.m_character.IsFistGetPet = false;
			this.m_character.LastRefreshPet = DateTime.Now.AddDays(-1.0);
		}
		public void RemoveLastRefreshPet()
		{
			this.m_character.LastRefreshPet = DateTime.Now;
		}
		public void UpdateAnswerSite(int id)
		{
			if (this.PlayerCharacter.AnswerSite < id)
			{
				this.PlayerCharacter.AnswerSite = id;
			}
			this.UpdateWeaklessGuildProgress();
			this.Out.SendWeaklessGuildProgress(this.PlayerCharacter);
		}
		public void UpdateWeaklessGuildProgress()
		{
			if (this.PlayerCharacter.weaklessGuildProgress == null)
			{
				this.PlayerCharacter.weaklessGuildProgress = Base64.decodeToByteArray(this.PlayerCharacter.WeaklessGuildProgressStr);
			}
			this.PlayerCharacter.CheckLevelFunction();
			if (this.PlayerCharacter.Grade == 1)
			{
				this.PlayerCharacter.openFunction(Step.POP_MOVE);
			}
			if (this.PlayerCharacter.IsOldPlayer)
			{
				this.PlayerCharacter.openFunction(Step.OLD_PLAYER);
			}
			this.PlayerCharacter.WeaklessGuildProgressStr = Base64.encodeByteArray(this.PlayerCharacter.weaklessGuildProgress);
		}
		public bool canUpLv(int exp, int _curLv)
		{
			List<int> list = GameProperties.VIPExp();
			return (exp >= list[0] && _curLv == 0) || (exp >= list[1] && _curLv == 1) || (exp >= list[2] && _curLv == 2) || (exp >= list[3] && _curLv == 3) || (exp >= list[4] && _curLv == 4) || (exp >= list[5] && _curLv == 5) || (exp >= list[6] && _curLv == 6) || (exp >= list[7] && _curLv == 7) || (exp >= list[8] && _curLv == 8) || (exp >= list[9] && _curLv == 9) || (exp >= list[10] && _curLv == 10) || (exp >= list[11] && _curLv == 11);
		}
		public void AddExpVip(int value)
		{
			List<int> list = GameProperties.VIPExp();
			if (value >= 10)
			{
				this.m_character.VIPExp += value / 10;
			}
			for (int i = 0; i < list.Count; i++)
			{
				int vIPExp = this.m_character.VIPExp;
				int vIPLevel = this.m_character.VIPLevel;
				if (vIPLevel == 12)
				{
					this.m_character.VIPExp = list[11];
					break;
				}
				if (vIPLevel < 12)
				{
					bool flag = this.canUpLv(vIPExp, vIPLevel);
					if (flag)
					{
						this.m_character.VIPLevel++;
					}
				}
			}
			this.Out.SendOpenVIP(this.PlayerCharacter);
		}
		public int AddCardSoul(int value)
		{
			if (value > 0)
			{
				this.m_character.CardSoul += value;
				return value;
			}
			return 0;
		}
		public int RemoveCardSoul(int value)
		{
			if (value > 0 && value <= this.m_character.CardSoul)
			{
				this.m_character.CardSoul -= value;
				return value;
			}
			return 0;
		}
		public int AddScore(int value)
		{
			if (value > 0)
			{
				this.m_character.Score += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemoveScore(int value)
		{
			if (value > 0 && value <= this.m_character.Score)
			{
				this.m_character.Score -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddDamageScores(int value)
		{
			if (value > 0)
			{
				this.m_character.damageScores += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemoveDamageScores(int value)
		{
			if (value > 0 && value <= this.m_character.damageScores)
			{
				this.m_character.damageScores -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddPetScore(int value)
		{
			if (value > 0)
			{
				this.m_character.petScore += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemovePetScore(int value)
		{
			if (value > 0 && value <= this.m_character.petScore)
			{
				this.m_character.petScore -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddmyHonor(int value)
		{
			if (value > 0)
			{
				this.m_character.myHonor += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemovemyHonor(int value)
		{
			if (value > 0 && value <= this.m_character.myHonor)
			{
				this.m_character.myHonor -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddMedal(int value)
		{
			if (value > 0)
			{
				this.m_character.medal += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemoveMedal(int value)
		{
			if (value > 0 && value <= this.m_character.medal)
			{
				this.m_character.medal -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddOffer(int value)
		{
			return this.AddOffer(value, true);
		}
		public int AddOffer(int value, bool IsRate)
		{
			if (value > 0)
			{
				if (AntiAddictionMgr.ISASSon)
				{
					value = (int)((double)value * AntiAddictionMgr.GetAntiAddictionCoefficient(this.PlayerCharacter.AntiAddiction));
				}
				if (IsRate)
				{
					value *= (((int)this.OfferAddPlus == 0) ? 1 : ((int)this.OfferAddPlus));
				}
				this.m_character.Offer += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemoveOffer(int value)
		{
			if (value > 0)
			{
				if (value >= this.m_character.Offer)
				{
					value = this.m_character.Offer;
				}
				this.m_character.Offer -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int RemoveGiftToken(int value)
		{
			if (value > 0 && value <= this.m_character.GiftToken)
			{
				this.m_character.GiftToken -= value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddGP(int gp)
		{
			if (gp >= 0)
			{
				if (AntiAddictionMgr.ISASSon)
				{
					gp = (int)((double)gp * AntiAddictionMgr.GetAntiAddictionCoefficient(this.PlayerCharacter.AntiAddiction));
				}
				gp = (int)((float)gp * RateMgr.GetRate(eRateType.Experience_Rate));
				if (this.GPAddPlus > 0.0)
				{
					gp = (int)((double)gp * this.GPAddPlus);
				}
				this.m_character.GP += gp;
				if (this.m_character.GP < 1)
				{
					this.m_character.GP = 1;
				}
				this.Level = LevelMgr.GetLevel(this.m_character.GP);
				this.MainBag.UpdatePlayerProperties();
				return gp;
			}
			return 0;
		}
		public int RemoveGP(int gp)
		{
			if (gp > 0)
			{
				this.m_character.GP -= gp;
				if (this.m_character.GP < 1)
				{
					this.m_character.GP = 1;
				}
				this.Level = LevelMgr.GetLevel(this.m_character.GP);
				this.OnPropertiesChanged();
				return gp;
			}
			return 0;
		}
		public int AddRobRiches(int value)
		{
			if (value > 0)
			{
				if (AntiAddictionMgr.ISASSon)
				{
					value = (int)((double)value * AntiAddictionMgr.GetAntiAddictionCoefficient(this.PlayerCharacter.AntiAddiction));
				}
				this.m_character.RichesRob += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddRichesOffer(int value)
		{
			if (value > 0)
			{
				this.m_character.RichesOffer += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public int AddGiftToken(int value)
		{
			if (value > 0)
			{
				this.m_character.GiftToken += value;
				this.OnPropertiesChanged();
				return value;
			}
			return 0;
		}
		public bool CanEquip(ItemTemplateInfo item)
		{
			bool flag = true;
			string message = "";
			if (!item.CanEquip)
			{
				flag = false;
				message = LanguageMgr.GetTranslation("Game.Server.GameObjects.NoEquip", new object[0]);
			}
			else
			{
				if (item.NeedSex != 0 && item.NeedSex != (this.m_character.Sex ? 1 : 2))
				{
					flag = false;
					message = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanEquip", new object[0]);
				}
				else
				{
					if (this.m_character.Grade < item.NeedLevel)
					{
						flag = false;
						message = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanLevel", new object[0]);
					}
				}
			}
			if (!flag)
			{
				this.Out.SendMessage(eMessageType.ERROR, message);
			}
			return flag;
		}
		public void UpdateBaseProperties(int attack, int defence, int agility, int lucky, int hp)
		{
			if (attack != this.m_character.Attack || defence != this.m_character.Defence || agility != this.m_character.Agility || lucky != this.m_character.Luck)
			{
				this.m_character.Attack = attack;
				this.m_character.Defence = defence;
				this.m_character.Agility = agility;
				this.m_character.Luck = lucky;
				this.OnPropertiesChanged();
			}
			this.m_character.hp = (int)(((double)(hp + this.LevelPlusBlood + this.m_character.Defence / 10) + this.GetGoldBlood()) * this.GetBaseBlood());
		}
		public void UpdateStyle(string style, string colors, string skin)
		{
			if (style != this.m_character.Style || colors != this.m_character.Colors || skin != this.m_character.Skin)
			{
				this.m_character.Style = style;
				this.m_character.Colors = colors;
				this.m_character.Skin = skin;
				this.OnPropertiesChanged();
			}
		}
		public void UpdateFightPower()
		{
			int num = 0;
			this.FightPower = 0;
			int num2 = 0;
			num2 += this.PlayerCharacter.hp;
			num += this.PlayerCharacter.Attack;
			num += this.PlayerCharacter.Defence;
			num += this.PlayerCharacter.Agility;
			num += this.PlayerCharacter.Luck;
			this.FightPower += (int)((double)(num + 1000) * (this.GetBaseAttack() * this.GetBaseAttack() * this.GetBaseAttack() + 3.5 * this.GetBaseDefence() * this.GetBaseDefence() * this.GetBaseDefence()) / 100000000.0 + (double)num2 * 0.95);
			if (this.m_currentSecondWeapon != null)
			{
				this.FightPower += (int)((double)this.m_currentSecondWeapon.Template.Property7 * Math.Pow(1.1, (double)this.m_currentSecondWeapon.StrengthenLevel));
			}
			this.PlayerCharacter.FightPower = this.FightPower;
		}
		public void UpdateHide(int hide)
		{
			if (hide != this.m_character.Hide)
			{
				this.m_character.Hide = hide;
				this.OnPropertiesChanged();
			}
		}
		public void UpdateWeapon(ItemInfo item)
		{
			if (item != this.m_MainWeapon)
			{
				this.m_MainWeapon = item;
				this.OnPropertiesChanged();
			}
		}
		public void UpdatePet(UsersPetinfo pet)
		{
			this.m_pet = pet;
		}
		public void UpdateSecondWeapon(ItemInfo item)
		{
			if (item != this.m_currentSecondWeapon)
			{
				this.m_currentSecondWeapon = item;
				this.OnPropertiesChanged();
			}
		}
		public void HideEquip(int categoryID, bool hide)
		{
			if (categoryID >= 0 && categoryID < 10)
			{
				this.EquipShowImp(categoryID, hide ? 2 : 1);
			}
		}
		public void ApertureEquip(int level)
		{
			this.EquipShowImp(0, (level < 5) ? 1 : ((level < 7) ? 2 : 3));
		}
		private void EquipShowImp(int categoryID, int para)
		{
			this.UpdateHide((int)((double)this.m_character.Hide + Math.Pow(10.0, (double)categoryID) * (double)(para - this.m_character.Hide / (int)Math.Pow(10.0, (double)categoryID) % 10)));
		}
		public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
		{
			LogMgr.LogMoneyAdd((LogMoneyType)masterType, (LogMoneyType)sonType, userId, moneys, SpareMoney, 0, 0, 0, 0, "", "", "");
		}
		public bool Login()
		{
			if (WorldMgr.AddPlayer(this.m_character.ID, this))
			{
				try
				{
					if (this.LoadFromDatabase())
					{
						this.Out.SendLoginSuccess();
						this.Out.SendUpdatePublicPlayer(this.PlayerCharacter);
						this.Out.SendUpdatePlayerProperty(this.PlayerCharacter, this.PlayerProperty);
						this.Out.SendWeaklessGuildProgress(this.PlayerCharacter);
						this.Out.SendDateTime();
						this.Out.SendDailyAward(this);
						this.LoadMarryMessage();
                        Managers.AntiAddictionMgr.AASStateGet(this);//them     
						this.Out.SendActivityList(this.PlayerCharacter.ID);
						this.Out.SendPlayerDrill(this.PlayerCharacter.ID, this.UserDrills);
						this.Out.SendOpenVIP(this.PlayerCharacter);
                        this.Out.SendLittleGameActived();

						if (this.PlayerCharacter.Grade >= 20)
						{
							this.Out.SendOpenWorldBoss();
						}
						this.Out.SendGetSpree(this.PlayerCharacter);
						if (this.PlayerCharacter.Grade >= 30)
						{
							this.Out.SendPlayerFigSpiritinit(this.PlayerCharacter.ID, this.GemStone);
						}
						this.Out.SendTrusteeshipStart(this.PlayerCharacter.ID);
						if (this.PlayerCharacter.Grade <= 20)
						{
							this.Out.SendGetBoxTime(this.PlayerCharacter.ID, this.PlayerCharacter.receiebox, false);
						}
						this.LoadDiceSystem();
						return true;
					}
					WorldMgr.RemovePlayer(this.m_character.ID);
				}
				catch (Exception exception)
				{
					GamePlayer.log.Error("Error Login!", exception);
				}
				return false;
			}
			return false;
		}
		private void LoadDiceSystem()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(134);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(DiceSystemMgr.MoneyLamMS);
			gSPacketIn.WriteInt(DiceSystemMgr.MoneyMacDinh);
			gSPacketIn.WriteInt(DiceSystemMgr.MoneyXUDoi);
			gSPacketIn.WriteInt(DiceSystemMgr.MoneyXULon);
			gSPacketIn.WriteInt(DiceSystemMgr.MoneyXUNho);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(50);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(11036);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(200549);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(250);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(40002);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(200549);
			gSPacketIn.WriteInt(1);
			this.Out.SendTCP(gSPacketIn);
			GSPacketIn gSPacketIn2 = new GSPacketIn(10);
			gSPacketIn2.WriteInt(0);
            gSPacketIn2.WriteString("Bem vindo ao Servidor Continuado por SkelletonX");
			this.Out.SendTCP(gSPacketIn2);
		}

		public void LoadMarryMessage()
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				MarryApplyInfo[] playerMarryApply = playerBussiness.GetPlayerMarryApply(this.PlayerCharacter.ID);
				if (playerMarryApply != null)
				{
					MarryApplyInfo[] array = playerMarryApply;
					for (int i = 0; i < array.Length; i++)
					{
						MarryApplyInfo marryApplyInfo = array[i];
						switch (marryApplyInfo.ApplyType)
						{
						case 1:
							this.Out.SendPlayerMarryApply(this, marryApplyInfo.ApplyUserID, marryApplyInfo.ApplyUserName, marryApplyInfo.LoveProclamation, marryApplyInfo.ID);
							break;

						case 2:
							this.Out.SendMarryApplyReply(this, marryApplyInfo.ApplyUserID, marryApplyInfo.ApplyUserName, marryApplyInfo.ApplyResult, true, marryApplyInfo.ID);
							if (!marryApplyInfo.ApplyResult)
							{
								this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
							}
							break;

						case 3:
							this.Out.SendPlayerDivorceApply(this, true, false);
							break;
						}
					}
				}
			}
		}
		public void ChargeToUser()
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				int num = 0;
				object obj;
				Monitor.Enter(obj = this.charge_locker);
				try
				{
					playerBussiness.ChargeToUser(this.m_character.UserName, ref num, this.m_character.NickName);
				}
				finally
				{
					Monitor.Exit(obj);
				}
				if (num > 0)
				{
					this.AddMoney(num);
					MailInfo mail = new MailInfo
					{
						Content = LanguageMgr.GetTranslation("ChargeToUser.Content", new object[]
						{
							num
						}),
						Title = LanguageMgr.GetTranslation("ChargeToUser.Title", new object[0]),
						Gold = 0,
						IsExist = true,
						Money = 0,
						GiftToken = 0,
						Receiver = this.PlayerCharacter.NickName,
						ReceiverID = this.PlayerCharacter.ID,
						Sender = this.PlayerCharacter.NickName,
						SenderID = this.PlayerCharacter.ID,
						Type = 1
					};
					if (playerBussiness.SendMail(mail))
					{
						this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
					}
				}
			}
		}
		public bool LoadFromDatabase()
		{
			bool result;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(this.m_character.ID);
				if (userSingleByUserID == null)
				{
					this.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid", new object[0]));
					this.Client.Disconnect();
					result = false;
				}
				else
				{
					this.m_character = userSingleByUserID;
					this.m_character.Texp = playerBussiness.GetUserTexpInfoSingle(this.m_character.ID);
					if (this.m_character.Texp.texpTaskDate.Date != DateTime.Now.Date)
					{
						this.m_character.Texp.texpCount = 0;
						playerBussiness.UpdateUserTexpInfo(this.m_character.Texp);
					}
					if (DateTime.Now.Date > this.m_character.TimeBox.Date && this.m_character.Grade < 21)
					{
						this.m_character.TimeBox = DateTime.Now;
						this.m_character.receiebox = 0;
						this.m_character.MaxBuyHonor = 0;
					}
					if (this.m_character.Grade > 19)
					{
						this.UpdateGemStone(playerBussiness);
					}
					this.m_playerProperty.Clear();
					this.UpdateDrills(playerBussiness);
					this.ChargeToUser();
					int[] updatedSlots = new int[]
					{
						0,
						1,
						2
					};
					this.Out.SendUpdateInventorySlot(this.FightBag, updatedSlots);
					this.UpdateWeaklessGuildProgress();
					this.UpdateItemForUser(1);
					this.ClearStoreBag();
					this.CheckExpireDay();
					this.UpdateWeapon(this.m_mainBag.GetItemAt(6));
					this.UpdatePet(this.m_petBag.GetPetIsEquip());
					this.UpdateSecondWeapon(this.m_mainBag.GetItemAt(15));
					this.m_pvepermissions = (string.IsNullOrEmpty(this.m_character.PvePermission) ? this.InitPvePermission() : this.m_converter.GetBytes(this.m_character.PvePermission));
					this.LoadPvePermission();
					this._friends = new Dictionary<int, int>();
					this._friends = playerBussiness.GetFriendsIDAll(this.m_character.ID);
					this._viFarms = new List<int>();
					this.m_character.State = 1;
					playerBussiness.UpdatePlayer(this.m_character);
					this.DicePlace = DiceSystemMgr.GetDiceByUserID(this.m_character.ID);
					result = true;
				}
			}
			return result;
		}
		public void UpdatePlayerProperty(string key, Dictionary<string, int> propAdd)
		{
			if (!this.m_playerProperty.ContainsKey(key))
			{
				this.m_playerProperty.Add(key, propAdd);
				return;
			}
			this.m_playerProperty[key] = propAdd;
		}
		public void UpdateDrills(PlayerBussiness db)
		{
			this.m_userDrills = db.GetPlayerDrillByID(this.m_character.ID);
			if (this.m_userDrills.Count == 0)
			{
				List<int> list = new List<int>
				{
					13,
					14,
					15,
					16,
					17,
					18
				};
				List<int> list2 = new List<int>
				{
					0,
					1,
					2,
					3,
					4,
					5
				};
				for (int i = 0; i < list.Count; i++)
				{
					UserDrillInfo userDrillInfo = new UserDrillInfo();
					userDrillInfo.UserID = this.m_character.ID;
					userDrillInfo.BeadPlace = list[i];
					userDrillInfo.HoleLv = 0;
					userDrillInfo.HoleExp = 0;
					userDrillInfo.DrillPlace = list2[i];
					db.AddUserUserDrill(userDrillInfo);
					if (!this.m_userDrills.ContainsKey(userDrillInfo.DrillPlace))
					{
						this.m_userDrills.Add(userDrillInfo.DrillPlace, userDrillInfo);
					}
				}
			}
		}
		public void UpdateGemStone(PlayerBussiness db)
		{
			this.m_GemStone = db.GetSingleGemStones(this.m_character.ID);
			if (this.m_GemStone.Count == 0)
			{
				List<int> list = new List<int>
				{
					11,
					5,
					2,
					3,
					13
				};
				List<int> list2 = new List<int>
				{
					100002,
					100003,
					100001,
					100004,
					100005
				};
				for (int i = 0; i < list.Count; i++)
				{
					UserGemStone userGemStone = new UserGemStone();
					userGemStone.ID = 0;
					userGemStone.UserID = this.m_character.ID;
					userGemStone.FigSpiritId = list2[i];
					userGemStone.FigSpiritIdValue = "0,0,0|0,0,1|0,0,2";
					userGemStone.EquipPlace = list[i];
					this.m_GemStone.Add(userGemStone);
					db.AddUserGemStone(userGemStone);
				}
			}
		}
		public UserGemStone GetGemStone(int place)
		{
			foreach (UserGemStone current in this.m_GemStone)
			{
				if (place == current.EquipPlace)
				{
					return current;
				}
			}
			return null;
		}
		public void UpdateItemForUser(object state)
		{
			this.m_mainBag.LoadFromDatabase();
			this.m_propBag.LoadFromDatabase();
			this.m_ConsortiaBag.LoadFromDatabase();
			this.m_BeadBag.LoadFromDatabase();
			this.m_farmBag.LoadFromDatabase();
			this.m_petBag.LoadFromDatabase();
			this.m_vegetable.LoadFromDatabase();
			this.m_cardBag.LoadFromDatabase();
			this.m_questInventory.LoadFromDatabase(this.m_character.ID);
			this.m_bufferList.LoadFromDatabase(this.m_character.ID);
			this.m_farm.LoadFromDatabase();
		}
		public void CheckExpireDay()
		{
			if (!(this.m_character.VIPExpireDay.Date >= DateTime.Now.Date))
			{
				this.m_character.CanTakeVipReward = false;
				return;
			}
			if (this.m_character.LastVIPPackTime.Date < DateTime.Now.Date)
			{
				this.m_character.CanTakeVipReward = true;
				return;
			}
			this.m_character.CanTakeVipReward = false;
		}
		public void LastVIPPackTime()
		{
			this.m_character.LastVIPPackTime = DateTime.Now;
			this.m_character.CanTakeVipReward = false;
		}
		public void OpenVIP(DateTime ExpireDayOut)
		{
			this.m_character.typeVIP = 1;
			this.m_character.VIPLevel = 1;
			this.m_character.VIPExp = 0;
			this.m_character.VIPExpireDay = ExpireDayOut;
			this.m_character.VIPLastDate = DateTime.Now;
			this.m_character.VIPNextLevelDaysNeeded = 0;
			this.m_character.CanTakeVipReward = true;
		}
		public void ContinousVIP(DateTime ExpireDayOut)
		{
			this.m_character.VIPExpireDay = ExpireDayOut;
		}
		public bool SaveIntoDatabase()
		{
			bool result;
			try
			{
				if (this.m_character.IsDirty)
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						playerBussiness.UpdatePlayer(this.m_character);
					}
				}
				this.MainBag.SaveToDatabase();
				this.PropBag.SaveToDatabase();
				this.ConsortiaBag.SaveToDatabase();
				this.BeadBag.SaveToDatabase();
				this.Vegetable.SaveToDatabase();
				this.Farm.SaveToDatabase();
				this.FarmBag.SaveToDatabase();
				this.PetBag.SaveToDatabase();
				this.CardBag.SaveToDatabase();
				this.QuestInventory.SaveToDatabase();
				this.BufferList.SaveToDatabase();
				this.Farm.SaveToDatabase();
				result = true;
			}
			catch (Exception exception)
			{
				GamePlayer.log.Error("Error saving player " + this.m_character.NickName + "!", exception);
				result = false;
			}
			return result;
		}
		public virtual bool Quit()
		{
			try
			{
				try
				{
					if (this.CurrentRoom != null)
					{
						this.CurrentRoom.RemovePlayerUnsafe(this);
						this.CurrentRoom = null;
					}
					else
					{
						RoomMgr.WaitingRoom.RemovePlayer(this);
					}
					if (this._currentMarryRoom != null)
					{
						this._currentMarryRoom.RemovePlayer(this);
						this._currentMarryRoom = null;
					}
				}
				catch (Exception exception)
				{
					GamePlayer.log.Error("Player exit Game Error!", exception);
				}
				this.m_character.State = 0;
				this.SaveIntoDatabase();
			}
			catch (Exception exception2)
			{
				GamePlayer.log.Error("Player exit Error!!!", exception2);
			}
			finally
			{
				WorldMgr.RemovePlayer(this.m_character.ID);
			}
			return true;
		}
		public void ViFarmsAdd(int playerID)
		{
			if (!this._viFarms.Contains(playerID))
			{
				this._viFarms.Add(playerID);
			}
		}
		public void ViFarmsRemove(int playerID)
		{
			if (this._viFarms.Contains(playerID))
			{
				this._viFarms.Remove(playerID);
			}
		}
		public void FriendsAdd(int playerID, int relation)
		{
			if (!this._friends.ContainsKey(playerID))
			{
				this._friends.Add(playerID, relation);
				return;
			}
			this._friends[playerID] = relation;
		}
		public void FriendsRemove(int playerID)
		{
			if (this._friends.ContainsKey(playerID))
			{
				this._friends.Remove(playerID);
			}
		}
		public bool IsBlackFriend(int playerID)
		{
			return this._friends == null || (this._friends.ContainsKey(playerID) && this._friends[playerID] == 1);
		}
		public void ClearConsortia()
		{
			this.PlayerCharacter.ClearConsortia();
			this.OnPropertiesChanged();
			this.QuestInventory.ClearConsortiaQuest();
			string translation = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender", new object[0]);
			string translation2 = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]);
			this.ConsortiaBag.SendAllItemsToMail(translation, translation2, eMailType.StoreCanel);
		}
		public void AddProperty(ItemInfo item, ref double defence, ref double attack)
		{
			RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneTemplateID(item.TemplateID);
			if (runeTemplateInfo != null)
			{
				string[] array = runeTemplateInfo.Attribute1.Split(new char[]
				{
					'|'
				});
				string[] array2 = runeTemplateInfo.Attribute2.Split(new char[]
				{
					'|'
				});
				int num = 0;
				int num2 = 0;
				if (item.Hole1 > runeTemplateInfo.BaseLevel)
				{
					if (array.Length > 1)
					{
						num = 1;
					}
					if (array2.Length > 1)
					{
						num2 = 1;
					}
				}
				int num3 = Convert.ToInt32(array[num]);
				Convert.ToInt32(array2[num2]);
				switch (runeTemplateInfo.Type1)
				{
				case 35:
					attack += (double)num3;
					return;

				case 36:
					defence += (double)num3;
					break;

				default:
					return;
				}
			}
		}
		public void UpdateSuitBonus(ref double basedefence, ref double baseattack)
		{
			int num = 0;
			int num2 = 0;
			int value = 0;
			int value2 = 0;
			int value3 = 0;
			int value4 = 0;
			int value5 = 0;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			basedefence += (double)num;
			baseattack += (double)num2;
			dictionary.Add("Attack", value);
			dictionary.Add("Defence", value2);
			dictionary.Add("Agility", value3);
			dictionary.Add("Luck", value4);
			dictionary.Add("HP", value5);
			dictionary.Add("Damage", num);
			dictionary.Add("Guard", num2);
			this.UpdatePlayerProperty("Suit", dictionary);
		}
		public double GetBaseAttack()
		{
			double num = 0.0;
			double num2 = 0.0;
			this.UpdateSuitBonus(ref num2, ref num);
			for (int i = 0; i < 31; i++)
			{
				ItemInfo itemAt = this.m_BeadBag.GetItemAt(i);
				if (itemAt != null)
				{
					this.AddProperty(itemAt, ref num2, ref num);
				}
			}
			this.UpdatePlayerProperty("Bead", new Dictionary<string, int>
			{

				{
					"Damage",
					(int)num
				},

				{
					"Armor",
					(int)num2
				}
			});
			num += (double)TotemMgr.getProp(this.m_character.totemId, "dam");
			num2 += (double)TotemMgr.getProp(this.m_character.totemId, "gua");
			List<UsersCardInfo> items = this.m_cardBag.GetItems(0, 5);
			foreach (UsersCardInfo current in items)
			{
				if (current.CardID != 0)
				{
					num += (double)current.Damage;
					num2 += (double)current.Guard;
				}
			}
			ItemInfo itemAt2 = this.m_mainBag.GetItemAt(6);
			if (itemAt2 != null)
			{
				num += (double)itemAt2.Template.Property7 * Math.Pow(1.1, (double)itemAt2.StrengthenLevel);
			}
			ItemInfo itemAt3 = this.m_mainBag.GetItemAt(0);
			if (itemAt3 != null)
			{
				num2 += (double)((int)((double)itemAt3.Template.Property7 * Math.Pow(1.1, (double)itemAt3.StrengthenLevel)));
			}
			ItemInfo itemAt4 = this.m_mainBag.GetItemAt(4);
			if (itemAt4 != null)
			{
				num2 += (double)((int)((double)itemAt4.Template.Property7 * Math.Pow(1.1, (double)itemAt4.StrengthenLevel)));
			}
			return num;
		}
		public double GetBaseDefence()
		{
			double num = 0.0;
			double num2 = 0.0;
			ItemInfo itemAt = this.m_mainBag.GetItemAt(6);
			if (itemAt != null)
			{
				this.AddProperty(itemAt, ref num, ref num2);
				num2 += (double)itemAt.Template.Property7 * Math.Pow(1.1, (double)itemAt.StrengthenLevel);
			}
			ItemInfo itemAt2 = this.m_mainBag.GetItemAt(0);
			if (itemAt2 != null)
			{
				this.AddProperty(itemAt2, ref num, ref num2);
				num += (double)((int)((double)itemAt2.Template.Property7 * Math.Pow(1.1, (double)itemAt2.StrengthenLevel)));
			}
			ItemInfo itemAt3 = this.m_mainBag.GetItemAt(4);
			if (itemAt3 != null)
			{
				this.AddProperty(itemAt3, ref num, ref num2);
				num += (double)((int)((double)itemAt3.Template.Property7 * Math.Pow(1.1, (double)itemAt3.StrengthenLevel)));
			}
			return num;
		}
		public double GetBaseAgility()
		{
			return 1.0 - (double)this.m_character.Agility * 0.001;
		}
		public double GetBaseBlood()
		{
			ItemInfo itemAt = this.MainBag.GetItemAt(12);
			if (itemAt != null)
			{
				return (100.0 + (double)itemAt.Template.Property1) / 100.0;
			}
			return 1.0;
		}
		public double GetGoldBlood()
		{
			ItemInfo itemAt = this.MainBag.GetItemAt(0);
			ItemInfo itemAt2 = this.MainBag.GetItemAt(4);
			double num = 0.0;
			if (itemAt != null)
			{
				GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipCategoryID(itemAt.Template.CategoryID);
				if (itemAt.IsGold)
				{
					num += (double)goldEquipTemplateLoadInfo.Boold;
				}
			}
			if (itemAt2 != null)
			{
				GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipCategoryID(itemAt2.Template.CategoryID);
				if (itemAt2.IsGold)
				{
					num += (double)goldEquipTemplateLoadInfo.Boold;
				}
			}
			return num;
		}
		private void SendItemNotice(ItemInfo info)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(14);
			gSPacketIn.WriteString(this.PlayerCharacter.NickName);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(info.TemplateID);
			gSPacketIn.WriteBoolean(info.IsBinds);
			if (info.Template.Quality > 3)
			{
				gSPacketIn.WriteInt(3);
				gSPacketIn.WriteString(info.Template.Name);
			}
			else
			{
				gSPacketIn.WriteInt(1);
			}
			if (info.Template.Quality < 4 || this.CurrentRoom == null)
			{
				return;
			}
			this.CurrentRoom.SendToTeam(gSPacketIn, this.CurrentRoomTeam, this);
		}
		public bool RemoveAt(eBageType bagType, int place)
		{
			PlayerInventory inventory = this.GetInventory(bagType);
			return inventory != null && inventory.RemoveItemAt(place);
		}
		public bool DeletePropItem(int place)
		{
			this.FightBag.RemoveItemAt(place);
			return true;
		}
		public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
		{
			if (bag == 1)
			{
				ItemTemplateInfo itemTemplateInfo = PropItemMgr.FindFightingProp(templateId);
				if (isLiving && itemTemplateInfo != null)
				{
					this.OnUsingItem(itemTemplateInfo.TemplateID);
					if (place == -1 && this.CanUseProp)
					{
						return true;
					}
					ItemInfo itemAt = this.GetItemAt(eBageType.PropBag, place);
					if (itemAt != null && itemAt.IsValidItem() && itemAt.Count >= 0)
					{
						itemAt.Count--;
						this.UpdateItem(itemAt);
						return true;
					}
				}
			}
			else
			{
				ItemInfo itemAt2 = this.GetItemAt(eBageType.FightBag, place);
				if (itemAt2.TemplateID == templateId)
				{
					this.OnUsingItem(itemAt2.TemplateID);
					return this.RemoveAt(eBageType.FightBag, place);
				}
			}
			return false;
		}
		public void Disconnect()
		{
			this.m_client.Disconnect();
		}
		public void SendTCP(GSPacketIn pkg)
		{
			if (this.m_client.IsConnected)
			{
				this.m_client.SendTCP(pkg);
			}
		}
		public void LoadMarryProp()
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				MarryProp marryProp = playerBussiness.GetMarryProp(this.PlayerCharacter.ID);
				this.PlayerCharacter.IsMarried = marryProp.IsMarried;
				this.PlayerCharacter.SpouseID = marryProp.SpouseID;
				this.PlayerCharacter.SpouseName = marryProp.SpouseName;
				this.PlayerCharacter.IsCreatedMarryRoom = marryProp.IsCreatedMarryRoom;
				this.PlayerCharacter.SelfMarryRoomID = marryProp.SelfMarryRoomID;
				this.PlayerCharacter.IsGotRing = marryProp.IsGotRing;
				this.Out.SendMarryProp(this, marryProp);
			}
		}
		public override string ToString()
		{
			return string.Format("Id:{0} nickname:{1} room:{2} ", this.PlayerId, this.PlayerCharacter.NickName, this.CurrentRoom);
		}
		public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
		{
			return ConsortiaMgr.ConsortiaFight(consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth, count);
		}
		public void SendConsortiaFight(int consortiaID, int riches, string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(158);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteInt(riches);
			gSPacketIn.WriteString(msg);
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
		}
		public void LoadPvePermission()
		{
			using (PveBussiness pveBussiness = new PveBussiness())
			{
				PveInfo[] allPveInfos = pveBussiness.GetAllPveInfos();
				PveInfo[] array = allPveInfos;
				for (int i = 0; i < array.Length; i++)
				{
					PveInfo pveInfo = array[i];
					if (this.m_character.Grade > pveInfo.LevelLimits)
					{
						bool flag = this.SetPvePermission(pveInfo.ID, eHardLevel.Simple);
						if (flag)
						{
							this.m_pvepermissions = (string.IsNullOrEmpty(this.m_character.PvePermission) ? this.InitPvePermission() : this.m_converter.GetBytes(this.m_character.PvePermission));
							flag = this.SetPvePermission(pveInfo.ID, eHardLevel.Normal);
						}
					}
				}
			}
		}
		public byte[] InitPvePermission()
		{
			byte[] array = new byte[50];
			for (int i = 0; i < 50; i++)
			{
				array[i] = 17;
			}
			return array;
		}
		public bool SetPvePermission(int missionId, eHardLevel hardLevel)
		{
			if (missionId > this.m_pvepermissions.Length * 2)
			{
				return false;
			}
			if (hardLevel == eHardLevel.Terror)
			{
				return true;
			}
			if (!this.IsPvePermission(missionId, hardLevel))
			{
				return false;
			}
			string str = string.Empty;
			string a = this.m_converter.GetString(this.m_pvepermissions).Substring(missionId - 1, 1);
			if (hardLevel == eHardLevel.Simple && a == "1")
			{
				str = "3";
			}
			else
			{
				if (hardLevel == eHardLevel.Normal && a == "3")
				{
					str = "7";
				}
				else
				{
					if (hardLevel != eHardLevel.Hard || !(a == "7"))
					{
						return false;
					}
					str = "F";
				}
			}
			string text = this.m_converter.GetString(this.m_pvepermissions);
			int length = text.Length;
			text = text.Substring(0, missionId - 1) + str + text.Substring(missionId, length - missionId);
			this.m_character.PvePermission = text;
			this.OnPropertiesChanged();
			return true;
		}
		public bool IsPvePermission(int missionId, eHardLevel hardLevel)
		{
			if (hardLevel == eHardLevel.Simple)
			{
				return true;
			}
			string a = this.m_converter.GetString(this.m_pvepermissions).Substring(missionId - 1, 1);
			if (hardLevel == eHardLevel.Normal)
			{
				if (a == "3" || a == "7" || a == "F")
				{
					return true;
				}
			}
			else
			{
				if (hardLevel == eHardLevel.Hard)
				{
					if (a == "7" || a == "F")
					{
						return true;
					}
				}
				else
				{
					if (hardLevel == eHardLevel.Terror && a == "F")
					{
						return true;
					}
				}
			}
			return false;
		}
		public void SendInsufficientMoney(int type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(88, this.PlayerId);
			gSPacketIn.WriteByte((byte)type);
			gSPacketIn.WriteBoolean(false);
			this.SendTCP(gSPacketIn);
		}
		public void SendMessage(string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString(msg);
			this.SendTCP(gSPacketIn);
		}
		public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type)
		{
			bool result;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				result = this.SendItemsToMail(items, content, title, type, playerBussiness);
			}
			return result;
		}
		public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type, PlayerBussiness pb)
		{
			bool result = true;
			int i = 0;
			while (i < items.Count)
			{
				MailInfo mailInfo = new MailInfo
				{
					Title = (title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]),
					Gold = 0,
					IsExist = true,
					Money = 0,
					Receiver = this.PlayerCharacter.NickName,
					ReceiverID = this.PlayerId,
					Sender = this.PlayerCharacter.NickName,
					SenderID = this.PlayerId,
					Type = (int)type,
					GiftToken = 0
				};
				List<ItemInfo> list = new List<ItemInfo>();
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark", new object[0]));
				content = ((content != null) ? LanguageMgr.GetTranslation(content, new object[0]) : "");
				int num = i;
				if (items.Count > num)
				{
					ItemInfo itemInfo = items[num];
					if (itemInfo.ItemID == 0)
					{
						pb.AddGoods(itemInfo);
					}
					mailInfo.Annex1 = itemInfo.ItemID.ToString();
					mailInfo.Annex1Name = itemInfo.Template.Name;
					mailInfo.Title = itemInfo.Template.Name;
					stringBuilder.Append(string.Concat(new object[]
					{
						"1、",
						mailInfo.Annex1Name,
						"x",
						itemInfo.Count,
						";"
					}));
					stringBuilder2.Append(string.Concat(new object[]
					{
						"1、",
						mailInfo.Annex1Name,
						"x",
						itemInfo.Count,
						";"
					}));
					list.Add(itemInfo);
				}
				num = i + 1;
				if (items.Count > num)
				{
					ItemInfo itemInfo = items[num];
					if (itemInfo.ItemID == 0)
					{
						pb.AddGoods(itemInfo);
					}
					mailInfo.Annex2 = itemInfo.ItemID.ToString();
					mailInfo.Annex2Name = itemInfo.Template.Name;
					stringBuilder.Append(string.Concat(new object[]
					{
						"2、",
						mailInfo.Annex2Name,
						"x",
						itemInfo.Count,
						";"
					}));
					stringBuilder2.Append(string.Concat(new object[]
					{
						"2、",
						mailInfo.Annex2Name,
						"x",
						itemInfo.Count,
						";"
					}));
					list.Add(itemInfo);
				}
				num = i + 2;
				if (items.Count > num)
				{
					ItemInfo itemInfo = items[num];
					if (itemInfo.ItemID == 0)
					{
						pb.AddGoods(itemInfo);
					}
					mailInfo.Annex3 = itemInfo.ItemID.ToString();
					mailInfo.Annex3Name = itemInfo.Template.Name;
					stringBuilder.Append(string.Concat(new object[]
					{
						"3、",
						mailInfo.Annex3Name,
						"x",
						itemInfo.Count,
						";"
					}));
					stringBuilder2.Append(string.Concat(new object[]
					{
						"3、",
						mailInfo.Annex3Name,
						"x",
						itemInfo.Count,
						";"
					}));
					list.Add(itemInfo);
				}
				num = i + 3;
				if (items.Count > num)
				{
					ItemInfo itemInfo = items[num];
					if (itemInfo.ItemID == 0)
					{
						pb.AddGoods(itemInfo);
					}
					mailInfo.Annex4 = itemInfo.ItemID.ToString();
					mailInfo.Annex4Name = itemInfo.Template.Name;
					stringBuilder.Append(string.Concat(new object[]
					{
						"4、",
						mailInfo.Annex4Name,
						"x",
						itemInfo.Count,
						";"
					}));
					stringBuilder2.Append(string.Concat(new object[]
					{
						"4、",
						mailInfo.Annex4Name,
						"x",
						itemInfo.Count,
						";"
					}));
					list.Add(itemInfo);
				}
				num = i + 4;
				if (items.Count > num)
				{
					ItemInfo itemInfo = items[num];
					if (itemInfo.ItemID == 0)
					{
						pb.AddGoods(itemInfo);
					}
					mailInfo.Annex5 = itemInfo.ItemID.ToString();
					mailInfo.Annex5Name = itemInfo.Template.Name;
					stringBuilder.Append(string.Concat(new object[]
					{
						"5、",
						mailInfo.Annex5Name,
						"x",
						itemInfo.Count,
						";"
					}));
					stringBuilder2.Append(string.Concat(new object[]
					{
						"5、",
						mailInfo.Annex5Name,
						"x",
						itemInfo.Count,
						";"
					}));
					list.Add(itemInfo);
				}
				mailInfo.AnnexRemark = stringBuilder.ToString();
				if (content == null && stringBuilder2.ToString() == null)
				{
					mailInfo.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content", new object[0]);
				}
				else
				{
					if (content != "")
					{
						mailInfo.Content = content;
					}
					else
					{
						mailInfo.Content = stringBuilder2.ToString();
					}
				}
				if (pb.SendMail(mailInfo))
				{
					using (List<ItemInfo>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ItemInfo current = enumerator.Current;
							this.TakeOutItem(current);
						}
						goto IL_631;
					}
					goto IL_62F;
				}
				goto IL_62F;
				IL_631:
				i += 5;
				continue;
				IL_62F:
				result = false;
				goto IL_631;
			}
			return result;
		}
		public bool SendItemToMail(ItemInfo item, string content, string title, eMailType type)
		{
			bool result;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				result = this.SendItemToMail(item, playerBussiness, content, title, type);
			}
			return result;
		}
		public bool SendItemToMail(ItemInfo item, PlayerBussiness pb, string content, string title, eMailType type)
		{
			MailInfo mailInfo = new MailInfo
			{
				Content = (content != null) ? content : LanguageMgr.GetTranslation("Game.Server.GameUtils.Content", new object[0]),
				Title = (title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]),
				Gold = 0,
				IsExist = true,
				Money = 0,
				GiftToken = 0,
				Receiver = this.PlayerCharacter.NickName,
				ReceiverID = this.PlayerCharacter.ID,
				Sender = this.PlayerCharacter.NickName,
				SenderID = this.PlayerCharacter.ID,
				Type = (int)type
			};
			if (item.ItemID == 0)
			{
				pb.AddGoods(item);
			}
			mailInfo.Annex1 = item.ItemID.ToString();
			mailInfo.Annex1Name = item.Template.Name;
			if (pb.SendMail(mailInfo))
			{
				this.TakeOutItem(item);
				return true;
			}
			return false;
		}
		public bool TakeOutItem(ItemInfo item)
		{
			if (item.BagType == this.m_propBag.BagType)
			{
				return this.m_propBag.TakeOutItem(item);
			}
			if (item.BagType == this.m_fightBag.BagType)
			{
				return this.m_fightBag.TakeOutItem(item);
			}
			if (item.BagType == this.m_ConsortiaBag.BagType)
			{
				return this.m_ConsortiaBag.TakeOutItem(item);
			}
			if (item.BagType == this.m_BeadBag.BagType)
			{
				return this.m_BeadBag.TakeOutItem(item);
			}
			return this.m_mainBag.TakeOutItem(item);
		}
		public void AddGift(eGiftType type)
		{
			ItemInfo itemInfo = null;
			List<ItemInfo> list = new List<ItemInfo>();
			switch (type)
			{
			case eGiftType.MONEY:
				this.AddMoney(Convert.ToInt32(GameProperties.FreeMoney));
				break;

			case eGiftType.SMALL_EXP:
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(GameProperties.FreeExp));
					if (itemTemplateInfo != null)
					{
						itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
					}
					break;
				}

			case eGiftType.BIG_EXP:
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(GameProperties.BigExp));
					if (itemTemplateInfo != null)
					{
						itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 50, 102);
					}
					break;
				}

			case eGiftType.PET_EXP:
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(GameProperties.PetExp));
					if (itemTemplateInfo != null)
					{
						itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 999, 102);
					}
					break;
				}

			case eGiftType.NEWBIE:
				{
					int[] array = new int[]
					{
						7015,
						7016,
						7017,
						7018,
						7019,
						7021,
						7022,
						7023,
						7048
					};
					Random random = new Random();
					int num = random.Next(0, array.Length - 1);
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(array[num]);
					if (itemTemplateInfo != null)
					{
						list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102));
					}
					itemTemplateInfo = ItemMgr.FindItemTemplate(11905);
					if (itemTemplateInfo != null)
					{
						for (int i = 0; i < 11; i++)
						{
							list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102));
						}
					}
					itemTemplateInfo = ItemMgr.FindItemTemplate(11025);
					if (itemTemplateInfo != null)
					{
						list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, 999, 102));
					}
					itemTemplateInfo = ItemMgr.FindItemTemplate(17001);
					if (itemTemplateInfo != null)
					{
						list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102));
					}
					break;
				}
			}
			if (itemInfo != null)
			{
				itemInfo.IsBinds = true;
				this.AddTemplate(itemInfo, itemInfo.Template.BagType, itemInfo.Count);
			}
			if (list.Count > 0)
			{
				foreach (ItemInfo current in list)
				{
					current.IsBinds = true;
					this.AddTemplate(current, current.Template.BagType, current.Count);
				}
			}
		}
		public void SendAllMessage(string msg)
		{
			GSPacketIn packet = this.Out.SendMessage(eMessageType.ChatERROR, msg);
			GameServer.Instance.LoginServer.SendPacket(packet);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			for (int i = 0; i < allPlayers.Length; i++)
			{
				GamePlayer gamePlayer = allPlayers[i];
				if (gamePlayer != this)
				{
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void CreatTreasure()
		{
			int i = 0;
			List<int> list = new List<int>
			{
				70154,
				70164,
				70184,
				70194,
				70204,
				70214,
				70224,
				70234,
				70284,
				70324,
				70574,
				70364,
				70414,
				70454,
				70464,
				70474
			};
			while (i < list.Count)
			{
				TreasureTempInfo treasureTempInfo = new TreasureTempInfo();
				treasureTempInfo.TemplateID = list[i];
				treasureTempInfo.ValidDate = 0;
				treasureTempInfo.Count = 1;
				this.m_TreasureTem.Add(treasureTempInfo);
				i++;
			}
		}
		public void UpdateTreasureAdd(TreasureTempInfo info)
		{
			if (!this.m_TreasureAdd.Contains(info))
			{
				this.m_TreasureAdd.Add(info);
			}
		}
	}
}
