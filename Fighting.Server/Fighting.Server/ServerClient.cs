using Bussiness;
using Bussiness.Managers;
using Fighting.Server.GameObjects;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
namespace Fighting.Server
{
	public class ServerClient : BaseClient
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private RSACryptoServiceProvider m_rsa;
		private FightServer m_svr;
		private Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();
		protected override void OnConnect()
		{
			base.OnConnect();
			this.m_rsa = new RSACryptoServiceProvider();
			RSAParameters rSAParameters = this.m_rsa.ExportParameters(false);
			this.SendRSAKey(rSAParameters.Modulus, rSAParameters.Exponent);
		}
		protected override void OnDisconnect()
		{
			base.OnDisconnect();
			this.m_rsa = null;
		}
		public override void OnRecvPacket(GSPacketIn pkg)
		{
			short code = pkg.Code;
			if (code <= 36)
			{
				switch (code)
				{
				case 1:
					this.HandleLogin(pkg);
					return;

				case 2:
					this.HanleSendToGame(pkg);
					return;

				case 3:
					this.HandleSysNotice(pkg);
					return;

				default:
					if (code == 19)
					{
						this.HandlePlayerMessage(pkg);
						return;
					}
					if (code != 36)
					{
						return;
					}
					this.HandlePlayerUsingProp(pkg);
					return;
				}
			}
			else
			{
				switch (code)
				{
				case 64:
					this.HandleGameRoomCreate(pkg);
					return;

				case 65:
					this.HandleGameRoomCancel(pkg);
					return;

				default:
					if (code == 69)
					{
						this.HandleConsortiaAlly(pkg);
						return;
					}
					if (code != 83)
					{
						return;
					}
					this.HandlePlayerExit(pkg);
					return;
				}
			}
		}
		private void HandlePlayerUsingProp(GSPacketIn pkg)
		{
			BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
			if (baseGame != null)
			{
				baseGame.Resume();
				if (pkg.ReadBoolean())
				{
					Player player = baseGame.FindPlayer(pkg.Parameter1);
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(pkg.Parameter2);
					if (player != null && itemTemplateInfo != null)
					{
						player.UseItem(itemTemplateInfo);
					}
				}
			}
		}
		private void HandlePlayerExit(GSPacketIn pkg)
		{
			BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
			if (baseGame != null)
			{
				Player player = baseGame.FindPlayer(pkg.Parameter1);
				if (player != null)
				{
					GSPacketIn pkg2 = new GSPacketIn(83, player.PlayerDetail.PlayerCharacter.ID);
					baseGame.SendToAll(pkg2);
					baseGame.RemovePlayer(player.PlayerDetail, false);
					ProxyRoom roomUnsafe = ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Red.RoomId);
					if (roomUnsafe != null && !roomUnsafe.RemovePlayer(player.PlayerDetail))
					{
						ProxyRoom roomUnsafe2 = ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Blue.RoomId);
						if (roomUnsafe2 != null)
						{
							roomUnsafe2.RemovePlayer(player.PlayerDetail);
						}
					}
				}
			}
		}
		public void HandleConsortiaAlly(GSPacketIn pkg)
		{
			BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
			if (baseGame != null)
			{
				baseGame.ConsortiaAlly = pkg.ReadInt();
				baseGame.RichesRate = pkg.ReadInt();
			}
		}
		private void HandleSysNotice(GSPacketIn pkg)
		{
			BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
			if (baseGame != null)
			{
				Player player = baseGame.FindPlayer(pkg.Parameter1);
				GSPacketIn gSPacketIn = new GSPacketIn(3);
				gSPacketIn.WriteInt(3);
				gSPacketIn.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[]
				{
					player.PlayerDetail.PlayerCharacter.Grade * 12,
					15
				}));
				player.PlayerDetail.SendTCP(gSPacketIn);
				gSPacketIn.ClearContext();
				gSPacketIn.WriteInt(3);
				gSPacketIn.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[]
				{
					player.PlayerDetail.PlayerCharacter.NickName,
					player.PlayerDetail.PlayerCharacter.Grade * 12,
					15
				}));
				baseGame.SendToAll(gSPacketIn, player.PlayerDetail);
			}
		}
		private void HandlePlayerMessage(GSPacketIn pkg)
		{
			BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
			if (baseGame != null)
			{
				Player player = baseGame.FindPlayer(pkg.ReadInt());
				bool flag = pkg.ReadBoolean();
				string str = pkg.ReadString();
				if (player != null)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(19);
					gSPacketIn.ClientID = player.PlayerDetail.PlayerCharacter.ID;
					gSPacketIn.WriteByte(5);
					gSPacketIn.WriteBoolean(false);
					gSPacketIn.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
					gSPacketIn.WriteString(str);
					if (flag)
					{
						baseGame.SendToTeam(pkg, player.Team);
						return;
					}
					baseGame.SendToAll(gSPacketIn);
				}
			}
		}
		public void HandleLogin(GSPacketIn pkg)
		{
			byte[] rgb = pkg.ReadBytes();
			string[] array = Encoding.UTF8.GetString(this.m_rsa.Decrypt(rgb, false)).Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				this.m_rsa = null;
				int.Parse(array[0]);
				base.Strict = false;
				return;
			}
			ServerClient.log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
			this.Disconnect();
		}
		public void HandleGameRoomCreate(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			int gameType = pkg.ReadInt();
			int guildId = pkg.ReadInt();
			int num2 = pkg.ReadInt();
			int num3 = 0;
			IGamePlayer[] array = new IGamePlayer[num2];
			for (int i = 0; i < num2; i++)
			{
				PlayerInfo playerInfo = new PlayerInfo();
				playerInfo.ID = pkg.ReadInt();
				playerInfo.NickName = pkg.ReadString();
				playerInfo.Sex = pkg.ReadBoolean();
				playerInfo.typeVIP = pkg.ReadByte();
				playerInfo.VIPLevel = pkg.ReadInt();
				playerInfo.Hide = pkg.ReadInt();
				playerInfo.Style = pkg.ReadString();
				playerInfo.Colors = pkg.ReadString();
				playerInfo.Skin = pkg.ReadString();
				playerInfo.Offer = pkg.ReadInt();
				playerInfo.GP = pkg.ReadInt();
				playerInfo.Grade = pkg.ReadInt();
				playerInfo.Repute = pkg.ReadInt();
				playerInfo.ConsortiaID = pkg.ReadInt();
				playerInfo.ConsortiaName = pkg.ReadString();
				playerInfo.ConsortiaLevel = pkg.ReadInt();
				playerInfo.ConsortiaRepute = pkg.ReadInt();
				playerInfo.badgeID = pkg.ReadInt();
				playerInfo.weaklessGuildProgress = Base64.decodeToByteArray(pkg.ReadString());
				playerInfo.Attack = pkg.ReadInt();
				playerInfo.Defence = pkg.ReadInt();
				playerInfo.Agility = pkg.ReadInt();
				playerInfo.Luck = pkg.ReadInt();
				playerInfo.hp = pkg.ReadInt();
				playerInfo.FightPower = pkg.ReadInt();
				playerInfo.IsMarried = pkg.ReadBoolean();
				if (playerInfo.IsMarried)
				{
					playerInfo.SpouseID = pkg.ReadInt();
					playerInfo.SpouseName = pkg.ReadString();
				}
				ProxyPlayerInfo proxyPlayerInfo = new ProxyPlayerInfo();
				proxyPlayerInfo.BaseAttack = pkg.ReadDouble();
				proxyPlayerInfo.BaseDefence = pkg.ReadDouble();
				proxyPlayerInfo.BaseAgility = pkg.ReadDouble();
				proxyPlayerInfo.BaseBlood = pkg.ReadDouble();
				proxyPlayerInfo.TemplateId = pkg.ReadInt();
				proxyPlayerInfo.CanUserProp = pkg.ReadBoolean();
				proxyPlayerInfo.SecondWeapon = pkg.ReadInt();
				proxyPlayerInfo.StrengthLevel = pkg.ReadInt();
				proxyPlayerInfo.GPAddPlus = pkg.ReadDouble();
				proxyPlayerInfo.OfferAddPlus = pkg.ReadDouble();
				proxyPlayerInfo.AntiAddictionRate = pkg.ReadDouble();
				proxyPlayerInfo.ServerId = pkg.ReadInt();
				UsersPetinfo usersPetinfo = new UsersPetinfo();
				int num4 = pkg.ReadInt();
				if (num4 == 1)
				{
					usersPetinfo.Place = pkg.ReadInt();
					usersPetinfo.TemplateID = pkg.ReadInt();
					usersPetinfo.ID = pkg.ReadInt();
					usersPetinfo.Name = pkg.ReadString();
					usersPetinfo.UserID = pkg.ReadInt();
					usersPetinfo.Level = pkg.ReadInt();
					usersPetinfo.Skill = pkg.ReadString();
					usersPetinfo.SkillEquip = pkg.ReadString();
				}
				else
				{
					usersPetinfo = null;
				}
				List<BufferInfo> list = new List<BufferInfo>();
				int num5 = pkg.ReadInt();
				for (int j = 0; j < num5; j++)
				{
					BufferInfo bufferInfo = new BufferInfo();
					bufferInfo.Type = pkg.ReadInt();
					bufferInfo.IsExist = pkg.ReadBoolean();
					bufferInfo.BeginDate = pkg.ReadDateTime();
					bufferInfo.ValidDate = pkg.ReadInt();
					bufferInfo.Value = pkg.ReadInt();
					bufferInfo.ValidCount = pkg.ReadInt();
					if (playerInfo != null)
					{
						list.Add(bufferInfo);
					}
				}
				List<ItemInfo> list2 = new List<ItemInfo>();
				int num6 = pkg.ReadInt();
				for (int k = 0; k < num6; k++)
				{
					int templateId = pkg.ReadInt();
					int hole = pkg.ReadInt();
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 1);
					itemInfo.Hole1 = hole;
					list2.Add(itemInfo);
				}
				array[i] = new ProxyPlayer(this, playerInfo, proxyPlayerInfo, usersPetinfo, list, list2);
				array[i].CanUseProp = proxyPlayerInfo.CanUserProp;
				num3 += playerInfo.Grade;
			}
			ProxyRoom proxyRoom = new ProxyRoom(ProxyRoomMgr.NextRoomId(), num, array, this);
			proxyRoom.GuildId = guildId;
			proxyRoom.GameType = (eGameType)gameType;
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (!this.m_rooms.ContainsKey(num))
				{
					this.m_rooms.Add(num, proxyRoom);
				}
				else
				{
					proxyRoom = null;
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (proxyRoom != null)
			{
				ProxyRoomMgr.AddRoom(proxyRoom);
				return;
			}
			ServerClient.log.ErrorFormat("Room already exists:{0}", num);
		}
		public void HandleGameRoomCancel(GSPacketIn pkg)
		{
			ProxyRoom proxyRoom = null;
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(pkg.Parameter1))
				{
					proxyRoom = this.m_rooms[pkg.Parameter1];
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (proxyRoom != null)
			{
				ProxyRoomMgr.RemoveRoom(proxyRoom);
			}
		}
		public void HanleSendToGame(GSPacketIn pkg)
		{
			BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
			if (baseGame != null)
			{
				GSPacketIn pkg2 = pkg.ReadPacket();
				baseGame.ProcessData(pkg2);
			}
		}
		public void SendRSAKey(byte[] m, byte[] e)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(0);
			gSPacketIn.Write(m);
			gSPacketIn.Write(e);
			this.SendTCP(gSPacketIn);
		}
		public void SendPacketToPlayer(int playerId, GSPacketIn pkg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(32, playerId);
			gSPacketIn.WritePacket(pkg);
			this.SendTCP(gSPacketIn);
		}
		public void SendRemoveRoom(int roomId)
		{
			GSPacketIn pkg = new GSPacketIn(65, roomId);
			this.SendTCP(pkg);
		}
		public void SendToRoom(int roomId, GSPacketIn pkg, IGamePlayer except)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(67, roomId);
			if (except != null)
			{
				gSPacketIn.Parameter1 = except.PlayerCharacter.ID;
				gSPacketIn.Parameter2 = except.GamePlayerId;
			}
			else
			{
				gSPacketIn.Parameter1 = 0;
				gSPacketIn.Parameter2 = 0;
			}
			gSPacketIn.WritePacket(pkg);
			this.SendTCP(gSPacketIn);
		}
		public void SendStartGame(int roomId, AbstractGame game)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(66);
			gSPacketIn.Parameter1 = roomId;
			gSPacketIn.Parameter2 = game.Id;
			gSPacketIn.WriteInt((int)game.RoomType);
			gSPacketIn.WriteInt((int)game.GameType);
			gSPacketIn.WriteInt(game.TimeType);
			this.SendTCP(gSPacketIn);
		}
		public void SendStopGame(int roomId, int gameId)
		{
			this.SendTCP(new GSPacketIn(68)
			{
				Parameter1 = roomId,
				Parameter2 = gameId
			});
		}
		public void SendGamePlayerId(IGamePlayer player)
		{
			this.SendTCP(new GSPacketIn(33)
			{
				Parameter1 = player.PlayerCharacter.ID,
				Parameter2 = player.GamePlayerId
			});
		}
		public void SendDisconnectPlayer(int playerId)
		{
			GSPacketIn pkg = new GSPacketIn(34, playerId);
			this.SendTCP(pkg);
		}
		public void SendPlayerOnGameOver(int playerId, int gameId, bool isWin, int gainXp)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(35, playerId);
			gSPacketIn.Parameter1 = gameId;
			gSPacketIn.WriteBoolean(isWin);
			gSPacketIn.WriteInt(gainXp);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerUsePropInGame(int playerId, int bag, int place, int templateId, bool isLiving)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(36, playerId);
			gSPacketIn.Parameter1 = bag;
			gSPacketIn.Parameter2 = place;
			gSPacketIn.WriteInt(templateId);
			gSPacketIn.WriteBoolean(isLiving);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerAddGold(int playerId, int value)
		{
			this.SendTCP(new GSPacketIn(38, playerId)
			{
				Parameter1 = value
			});
		}
		public void SendPlayerAddMoney(int playerId, int value)
		{
			this.SendTCP(new GSPacketIn(70, playerId)
			{
				Parameter1 = value
			});
		}
		public void SendPlayerAddGiftToken(int playerId, int value)
		{
			this.SendTCP(new GSPacketIn(71, playerId)
			{
				Parameter1 = value
			});
		}
		public void SendPlayerAddMedal(int playerId, int value)
		{
			this.SendTCP(new GSPacketIn(72, playerId)
			{
				Parameter1 = value
			});
		}
		public void SendPlayerAddGP(int playerId, int value)
		{
			this.SendTCP(new GSPacketIn(39, playerId)
			{
				Parameter1 = value
			});
		}
		public void SendPlayerRemoveGP(int playerId, int value)
		{
			this.SendTCP(new GSPacketIn(49, playerId)
			{
				Parameter1 = value
			});
		}
		public void SendPlayerOnKillingLiving(int playerId, AbstractGame game, int type, int id, bool isLiving, int demage)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(40, playerId);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteBoolean(isLiving);
			gSPacketIn.WriteInt(demage);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerOnMissionOver(int playerId, AbstractGame game, bool isWin, int MissionID, int turnNum)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(41, playerId);
			gSPacketIn.WriteBoolean(isWin);
			gSPacketIn.WriteInt(MissionID);
			gSPacketIn.WriteInt(turnNum);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerConsortiaFight(int playerId, int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(42, playerId);
			gSPacketIn.WriteInt(consortiaWin);
			gSPacketIn.WriteInt(consortiaLose);
			gSPacketIn.WriteInt(players.Count);
			for (int i = 0; i < players.Count; i++)
			{
				gSPacketIn.WriteInt(players[i].PlayerDetail.PlayerCharacter.ID);
			}
			gSPacketIn.WriteByte((byte)roomType);
			gSPacketIn.WriteByte((byte)gameClass);
			gSPacketIn.WriteInt(totalKillHealth);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerSendConsortiaFight(int playerId, int consortiaID, int riches, string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(43, playerId);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteInt(riches);
			gSPacketIn.WriteString(msg);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerRemoveGold(int playerId, int value)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(44, playerId);
			gSPacketIn.WriteInt(value);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerRemoveMoney(int playerId, int value)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(45, playerId);
			gSPacketIn.WriteInt(value);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerRemoveOffer(int playerId, int value)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(50, playerId);
			gSPacketIn.WriteInt(value);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerAddTemplate(int playerId, ItemInfo cloneItem, eBageType bagType, int count)
		{
			if (cloneItem != null)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(48, playerId);
				gSPacketIn.WriteInt(cloneItem.TemplateID);
				gSPacketIn.WriteByte((byte)bagType);
				gSPacketIn.WriteInt(count);
				gSPacketIn.WriteInt(cloneItem.ValidDate);
				gSPacketIn.WriteBoolean(cloneItem.IsBinds);
				gSPacketIn.WriteBoolean(cloneItem.IsUsed);
				this.SendTCP(gSPacketIn);
			}
		}
		public void SendConsortiaAlly(int Consortia1, int Consortia2, int GameId)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(69);
			gSPacketIn.WriteInt(Consortia1);
			gSPacketIn.WriteInt(Consortia2);
			gSPacketIn.WriteInt(GameId);
			this.SendTCP(gSPacketIn);
		}
		public ServerClient(FightServer svr) : base(new byte[8192], new byte[8192])
		{
			this.m_svr = svr;
		}
		public override string ToString()
		{
			return string.Format("Server Client: {0} IsConnected:{1}  RoomCount:{2}", 0, base.IsConnected, this.m_rooms.Count);
		}
		public void RemoveRoom(int orientId, ProxyRoom room)
		{
			bool flag = false;
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(orientId) && this.m_rooms[orientId] == room)
				{
					flag = this.m_rooms.Remove(orientId);
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (flag)
			{
				this.SendRemoveRoom(orientId);
			}
		}
	}
}
