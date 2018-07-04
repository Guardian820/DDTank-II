using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Logic.Protocol;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
namespace Game.Server.Battle
{
	public class FightServerConnector : BaseConnector
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private BattleServer m_server;
		private string m_key;
		protected override void OnDisconnect()
		{
			base.OnDisconnect();
		}
		public override void OnRecvPacket(GSPacketIn pkg)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsynProcessPacket), pkg);
		}
		protected void AsynProcessPacket(object state)
		{
			try
			{
				GSPacketIn gSPacketIn = state as GSPacketIn;
				int code = (int)gSPacketIn.Code;
				int num = code;
				if (num != 0)
				{
					switch (num)
					{
					case 19:
						this.HandlePlayerChatSend(gSPacketIn);
						goto IL_1F7;

					case 20:
					case 21:
					case 22:
					case 23:
					case 24:
					case 25:
					case 26:
					case 27:
					case 28:
					case 29:
					case 30:
					case 31:
					case 37:
					case 46:
					case 47:
						break;

					case 32:
						this.HandleSendToPlayer(gSPacketIn);
						goto IL_1F7;

					case 33:
						this.HandleUpdatePlayerGameId(gSPacketIn);
						goto IL_1F7;

					case 34:
						this.HandleDisconnectPlayer(gSPacketIn);
						goto IL_1F7;

					case 35:
						this.HandlePlayerOnGameOver(gSPacketIn);
						goto IL_1F7;

					case 36:
						this.HandlePlayerOnUsingItem(gSPacketIn);
						goto IL_1F7;

					case 38:
						this.HandlePlayerAddGold(gSPacketIn);
						goto IL_1F7;

					case 39:
						this.HandlePlayerAddGP(gSPacketIn);
						goto IL_1F7;

					case 40:
						this.HandlePlayerOnKillingLiving(gSPacketIn);
						goto IL_1F7;

					case 41:
						this.HandlePlayerOnMissionOver(gSPacketIn);
						goto IL_1F7;

					case 42:
						this.HandlePlayerConsortiaFight(gSPacketIn);
						goto IL_1F7;

					case 43:
						this.HandlePlayerSendConsortiaFight(gSPacketIn);
						goto IL_1F7;

					case 44:
						this.HandlePlayerRemoveGold(gSPacketIn);
						goto IL_1F7;

					case 45:
						this.HandlePlayerRemoveMoney(gSPacketIn);
						goto IL_1F7;

					case 48:
						this.HandlePlayerAddTemplate1(gSPacketIn);
						goto IL_1F7;

					case 49:
						this.HandlePlayerRemoveGP(gSPacketIn);
						goto IL_1F7;

					case 50:
						this.HandlePlayerRemoveOffer(gSPacketIn);
						goto IL_1F7;

					default:
						switch (num)
						{
						case 65:
							this.HandleRoomRemove(gSPacketIn);
							goto IL_1F7;

						case 66:
							this.HandleStartGame(gSPacketIn);
							goto IL_1F7;

						case 67:
							this.HandleSendToRoom(gSPacketIn);
							goto IL_1F7;

						case 68:
							this.HandleStopGame(gSPacketIn);
							goto IL_1F7;

						case 69:
							this.HandleFindConsortiaAlly(gSPacketIn);
							goto IL_1F7;

						case 70:
							this.HandlePlayerAddMoney(gSPacketIn);
							goto IL_1F7;

						case 71:
							this.HandlePlayerAddGiftToken(gSPacketIn);
							goto IL_1F7;

						case 72:
							this.HandlePlayerAddMedal(gSPacketIn);
							goto IL_1F7;
						}
						break;
					}
					Console.WriteLine("??????????LoginServerConnector: " + (eFightPackageType)code);
				}
				else
				{
					this.HandleRSAKey(gSPacketIn);
				}
				IL_1F7:;
			}
			catch (Exception exception)
			{
				GameServer.log.Error("AsynProcessPacket", exception);
			}
		}
		private void HandlePlayerChatSend(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.SendMessage(pkg.ReadString());
			}
		}
		public void HandleFindConsortiaAlly(GSPacketIn pkg)
		{
			int state = ConsortiaMgr.FindConsortiaAlly(pkg.ReadInt(), pkg.ReadInt());
			this.SendFindConsortiaAlly(state, pkg.ReadInt());
		}
		private void HandlePlayerOnKillingLiving(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			AbstractGame game = playerById.CurrentRoom.Game;
			if (playerById != null)
			{
				playerById.OnKillingLiving(game, pkg.ReadInt(), pkg.ClientID, pkg.ReadBoolean(), pkg.ReadInt());
			}
		}
		private void HandlePlayerOnMissionOver(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			AbstractGame game = playerById.CurrentRoom.Game;
			if (playerById != null)
			{
				playerById.OnMissionOver(game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadInt());
			}
		}
		private void HandlePlayerConsortiaFight(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			Dictionary<int, Player> dictionary = new Dictionary<int, Player>();
			int consortiaWin = pkg.ReadInt();
			int consortiaLose = pkg.ReadInt();
			int num = pkg.ReadInt();
			for (int i = 0; i < num; i++)
			{
				GamePlayer playerById2 = WorldMgr.GetPlayerById(pkg.ReadInt());
				if (playerById2 != null)
				{
					Player value = new Player(playerById2, 0, null, 0, playerById2.PlayerCharacter.hp);
					dictionary.Add(i, value);
				}
			}
			eRoomType roomType = (eRoomType)pkg.ReadByte();
			eGameType gameClass = (eGameType)pkg.ReadByte();
			int totalKillHealth = pkg.ReadInt();
			if (playerById != null)
			{
				int num2 = playerById.ConsortiaFight(consortiaWin, consortiaLose, dictionary, roomType, gameClass, totalKillHealth, num);
			}
		}
		private void HandlePlayerSendConsortiaFight(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.SendConsortiaFight(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString());
			}
		}
		private void HandlePlayerRemoveGold(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.RemoveGold(pkg.ReadInt());
			}
		}
		private void HandlePlayerRemoveMoney(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.RemoveMoney(pkg.ReadInt());
			}
		}
		private void HandlePlayerRemoveOffer(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.RemoveOffer(pkg.ReadInt());
			}
		}
		private void HandlePlayerAddTemplate1(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(pkg.ReadInt());
				eBageType bagType = (eBageType)pkg.ReadByte();
				if (itemTemplateInfo != null)
				{
					int count = pkg.ReadInt();
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, 118);
					itemInfo.Count = count;
					itemInfo.ValidDate = pkg.ReadInt();
					itemInfo.IsBinds = pkg.ReadBoolean();
					itemInfo.IsUsed = pkg.ReadBoolean();
					playerById.AddTemplate(itemInfo, bagType, itemInfo.Count);
				}
			}
		}
		private void HandlePlayerAddGP(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.AddGP(pkg.Parameter1);
			}
		}
		private void HandlePlayerAddMoney(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.AddMoney(pkg.Parameter1);
			}
		}
		private void HandlePlayerAddGiftToken(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.AddGiftToken(pkg.Parameter1);
			}
		}
		private void HandlePlayerAddMedal(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.AddMedal(pkg.Parameter1);
			}
		}
		private void HandlePlayerRemoveGP(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.RemoveGP(pkg.Parameter1);
			}
		}
		private void HandlePlayerAddGold(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.AddGold(pkg.Parameter1);
			}
		}
		private void HandlePlayerOnUsingItem(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				int templateId = pkg.ReadInt();
				bool result = playerById.UsePropItem(null, pkg.Parameter1, pkg.Parameter2, templateId, pkg.ReadBoolean());
				this.SendUsingPropInGame(playerById.CurrentRoom.Game.Id, playerById.GamePlayerId, templateId, result);
			}
		}
		private void SendUsingPropInGame(int gameId, int playerId, int templateId, bool result)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(36, gameId);
			gSPacketIn.Parameter1 = playerId;
			gSPacketIn.Parameter2 = templateId;
			gSPacketIn.WriteBoolean(result);
			this.SendTCP(gSPacketIn);
		}
		public void SendPlayerDisconnet(int gameId, int playerId, int roomid)
		{
			this.SendTCP(new GSPacketIn(83, gameId)
			{
				Parameter1 = playerId
			});
		}
		private void HandlePlayerOnGameOver(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null && playerById.CurrentRoom != null && playerById.CurrentRoom.Game != null)
			{
				playerById.OnGameOver(playerById.CurrentRoom.Game, pkg.ReadBoolean(), pkg.ReadInt());
			}
		}
		private void HandleDisconnectPlayer(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			if (playerById != null)
			{
				playerById.Disconnect();
			}
		}
		public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(1);
			gSPacketIn.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
			this.SendTCP(gSPacketIn);
		}
		protected void HandleRSAKey(GSPacketIn packet)
		{
			RSAParameters parameters = default(RSAParameters);
			parameters.Modulus = packet.ReadBytes(128);
			parameters.Exponent = packet.ReadBytes();
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.ImportParameters(parameters);
			this.SendRSALogin(rSACryptoServiceProvider, this.m_key);
		}
		public FightServerConnector(BattleServer server, string ip, int port, string key) : base(ip, port, true, new byte[30720], new byte[30720])
		{
			this.m_server = server;
			this.m_key = key;
			base.Strict = true;
		}
		public void SendAddRoom(BaseRoom room)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(64);
			gSPacketIn.WriteInt(room.RoomId);
			gSPacketIn.WriteInt((int)room.GameType);
			gSPacketIn.WriteInt(room.GuildId);
			List<GamePlayer> players = room.GetPlayers();
			gSPacketIn.WriteInt(players.Count);
			foreach (GamePlayer current in players)
			{
				gSPacketIn.WriteInt(current.PlayerCharacter.ID);
				gSPacketIn.WriteString(current.PlayerCharacter.NickName);
				gSPacketIn.WriteBoolean(current.PlayerCharacter.Sex);
				gSPacketIn.WriteByte(current.PlayerCharacter.typeVIP);
				gSPacketIn.WriteInt(current.PlayerCharacter.VIPLevel);
				gSPacketIn.WriteInt(current.PlayerCharacter.Hide);
				gSPacketIn.WriteString(current.PlayerCharacter.Style);
				gSPacketIn.WriteString(current.PlayerCharacter.Colors);
				gSPacketIn.WriteString(current.PlayerCharacter.Skin);
				gSPacketIn.WriteInt(current.PlayerCharacter.Offer);
				gSPacketIn.WriteInt(current.PlayerCharacter.GP);
				gSPacketIn.WriteInt(current.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(current.PlayerCharacter.Repute);
				gSPacketIn.WriteInt(current.PlayerCharacter.ConsortiaID);
				gSPacketIn.WriteString(current.PlayerCharacter.ConsortiaName);
				gSPacketIn.WriteInt(current.PlayerCharacter.ConsortiaLevel);
				gSPacketIn.WriteInt(current.PlayerCharacter.ConsortiaRepute);
				gSPacketIn.WriteInt(current.PlayerCharacter.badgeID);
				gSPacketIn.WriteString(current.PlayerCharacter.WeaklessGuildProgressStr);
				gSPacketIn.WriteInt(current.PlayerCharacter.Attack);
				gSPacketIn.WriteInt(current.PlayerCharacter.Defence);
				gSPacketIn.WriteInt(current.PlayerCharacter.Agility);
				gSPacketIn.WriteInt(current.PlayerCharacter.Luck);
				gSPacketIn.WriteInt(current.PlayerCharacter.hp);
				gSPacketIn.WriteInt(current.PlayerCharacter.FightPower);
				gSPacketIn.WriteBoolean(current.PlayerCharacter.IsMarried);
				if (current.PlayerCharacter.IsMarried)
				{
					gSPacketIn.WriteInt(current.PlayerCharacter.SpouseID);
					gSPacketIn.WriteString(current.PlayerCharacter.SpouseName);
				}
				gSPacketIn.WriteDouble(current.GetBaseAttack());
				gSPacketIn.WriteDouble(current.GetBaseDefence());
				gSPacketIn.WriteDouble(current.GetBaseAgility());
				gSPacketIn.WriteDouble(current.GetBaseBlood());
				gSPacketIn.WriteInt(current.MainWeapon.TemplateID);
				gSPacketIn.WriteBoolean(current.CanUseProp);
				if (current.SecondWeapon != null)
				{
					gSPacketIn.WriteInt(current.SecondWeapon.TemplateID);
					gSPacketIn.WriteInt(current.SecondWeapon.StrengthenLevel);
				}
				else
				{
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
				}
				gSPacketIn.WriteDouble((double)RateMgr.GetRate(eRateType.Experience_Rate) * AntiAddictionMgr.GetAntiAddictionCoefficient(current.PlayerCharacter.AntiAddiction) * ((current.GPAddPlus == 0.0) ? 1.0 : current.GPAddPlus));
				gSPacketIn.WriteDouble(AntiAddictionMgr.GetAntiAddictionCoefficient(current.PlayerCharacter.AntiAddiction) * ((current.OfferAddPlus == 0.0) ? 1.0 : current.OfferAddPlus));
				gSPacketIn.WriteDouble((double)RateMgr.GetRate(eRateType.Experience_Rate));
				gSPacketIn.WriteInt(GameServer.Instance.Configuration.ServerID);
				if (current.Pet == null)
				{
					gSPacketIn.WriteInt(0);
				}
				else
				{
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(current.Pet.Place);
					gSPacketIn.WriteInt(current.Pet.TemplateID);
					gSPacketIn.WriteInt(current.Pet.ID);
					gSPacketIn.WriteString(current.Pet.Name);
					gSPacketIn.WriteInt(current.Pet.UserID);
					gSPacketIn.WriteInt(current.Pet.Level);
					gSPacketIn.WriteString(current.Pet.Skill);
					gSPacketIn.WriteString(current.Pet.SkillEquip);
				}
				List<AbstractBuffer> allBuffer = current.BufferList.GetAllBuffer();
				gSPacketIn.WriteInt(allBuffer.Count);
				foreach (AbstractBuffer current2 in allBuffer)
				{
					BufferInfo info = current2.Info;
					gSPacketIn.WriteInt(info.Type);
					gSPacketIn.WriteBoolean(info.IsExist);
					gSPacketIn.WriteDateTime(info.BeginDate);
					gSPacketIn.WriteInt(info.ValidDate);
					gSPacketIn.WriteInt(info.Value);
					gSPacketIn.WriteInt(info.ValidCount);
				}
				gSPacketIn.WriteInt(current.EquipEffect.Count);
				foreach (ItemInfo current3 in current.EquipEffect)
				{
					gSPacketIn.WriteInt(current3.TemplateID);
					gSPacketIn.WriteInt(current3.Hole1);
				}
			}
			this.SendTCP(gSPacketIn);
		}
		public void SendRemoveRoom(BaseRoom room)
		{
			this.SendTCP(new GSPacketIn(65)
			{
				Parameter1 = room.RoomId
			});
		}
		public void SendToGame(int gameId, GSPacketIn pkg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(2, gameId);
			gSPacketIn.WritePacket(pkg);
			this.SendTCP(gSPacketIn);
		}
		protected void HandleRoomRemove(GSPacketIn packet)
		{
			this.m_server.RemoveRoomImp(packet.ClientID);
		}
		protected void HandleStartGame(GSPacketIn pkg)
		{
			ProxyGame game = new ProxyGame(pkg.Parameter2, this, (eRoomType)pkg.ReadInt(), (eGameType)pkg.ReadInt(), pkg.ReadInt());
			this.m_server.StartGame(pkg.Parameter1, game);
		}
		protected void HandleStopGame(GSPacketIn pkg)
		{
			int parameter = pkg.Parameter1;
			int parameter2 = pkg.Parameter2;
			this.m_server.StopGame(parameter, parameter2);
		}
		protected void HandleSendToRoom(GSPacketIn pkg)
		{
			int clientID = pkg.ClientID;
			GSPacketIn pkg2 = pkg.ReadPacket();
			this.m_server.SendToRoom(clientID, pkg2, pkg.Parameter1, pkg.Parameter2);
		}
		protected void HandleSendToPlayer(GSPacketIn pkg)
		{
			int clientID = pkg.ClientID;
			try
			{
				GSPacketIn pkg2 = pkg.ReadPacket();
				this.m_server.SendToUser(clientID, pkg2);
			}
			catch (Exception exception)
			{
				FightServerConnector.log.Error(string.Format("pkg len:{0}", pkg.Length), exception);
				FightServerConnector.log.Error(Marshal.ToHexDump("pkg content:", pkg.Buffer, 0, pkg.Length));
			}
		}
		private void HandleUpdatePlayerGameId(GSPacketIn pkg)
		{
			this.m_server.UpdatePlayerGameId(pkg.Parameter1, pkg.Parameter2);
		}
		public void SendChangeGameType(BaseRoom room)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(73)
			{
				Parameter1 = room.RoomId
			};
			gSPacketIn.WriteInt((int)room.GameType);
			this.SendTCP(gSPacketIn);
		}
		public void SendChatMessage(string msg, GamePlayer player, bool team)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(19, player.CurrentRoom.Game.Id);
			gSPacketIn.WriteInt(player.GamePlayerId);
			gSPacketIn.WriteBoolean(team);
			gSPacketIn.WriteString(msg);
			this.SendTCP(gSPacketIn);
		}
		public void SendFightNotice(GamePlayer player, int GameId)
		{
			this.SendTCP(new GSPacketIn(3, GameId)
			{
				Parameter1 = player.GamePlayerId
			});
		}
		public void SendFindConsortiaAlly(int state, int gameid)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(69, gameid);
			gSPacketIn.WriteInt(state);
			gSPacketIn.WriteInt((int)RateMgr.GetRate(eRateType.Riches_Rate));
			this.SendTCP(gSPacketIn);
		}
	}
}
