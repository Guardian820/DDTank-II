using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.ChatServer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
namespace Game.Server
{
	public class LoginServerConnector : BaseConnector
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private int m_serverId;
		private string m_loginKey;
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
				if (num <= 117)
				{
					if (num <= 38)
					{
						switch (num)
						{
						case 0:
							this.HandleRSAKey(gSPacketIn);
							goto IL_20C;

						case 1:
						case 6:
						case 11:
						case 12:
						case 16:
						case 17:
						case 18:
							break;

						case 2:
							this.HandleKitoffPlayer(gSPacketIn);
							goto IL_20C;

						case 3:
							this.HandleAllowUserLogin(gSPacketIn);
							goto IL_20C;

						case 4:
							this.HandleUserOffline(gSPacketIn);
							goto IL_20C;

						case 5:
							this.HandleUserOnline(gSPacketIn);
							goto IL_20C;

						case 7:
							this.HandleASSState(gSPacketIn);
							goto IL_20C;

						case 8:
							this.HandleConfigState(gSPacketIn);
							goto IL_20C;

						case 9:
							this.HandleChargeMoney(gSPacketIn);
							goto IL_20C;

						case 10:
							this.HandleSystemNotice(gSPacketIn);
							goto IL_20C;

						case 13:
							this.HandleUpdatePlayerMarriedState(gSPacketIn);
							goto IL_20C;

						case 14:
							this.HandleMarryRoomInfoToPlayer(gSPacketIn);
							goto IL_20C;

						case 15:
							this.HandleShutdown(gSPacketIn);
							goto IL_20C;

						case 19:
							this.HandleChatConsortia(gSPacketIn);
							goto IL_20C;

						default:
							switch (num)
							{
							case 37:
								this.HandleChatPersonal(gSPacketIn);
								goto IL_20C;

							case 38:
								this.HandleSysMess(gSPacketIn);
								goto IL_20C;
							}
							break;
						}
					}
					else
					{
						if (num == 72)
						{
							this.HandleBigBugle(gSPacketIn);
							goto IL_20C;
						}
						if (num == 117)
						{
							this.HandleMailResponse(gSPacketIn);
							goto IL_20C;
						}
					}
				}
				else
				{
					if (num <= 158)
					{
						switch (num)
						{
						case 128:
							this.HandleConsortiaResponse(gSPacketIn);
							goto IL_20C;

						case 129:
							break;

						case 130:
							this.HandleConsortiaCreate(gSPacketIn);
							goto IL_20C;

						default:
							if (num == 158)
							{
								this.HandleConsortiaFight(gSPacketIn);
								goto IL_20C;
							}
							break;
						}
					}
					else
					{
						switch (num)
						{
						case 165:
							this.HandleFriendState(gSPacketIn);
							goto IL_20C;

						case 166:
							this.HandleFirendResponse(gSPacketIn);
							goto IL_20C;

						default:
							switch (num)
							{
							case 177:
								this.HandleRate(gSPacketIn);
								goto IL_20C;

							case 178:
								this.HandleMacroDrop(gSPacketIn);
								goto IL_20C;
							}
							break;
						}
					}
				}
				Console.WriteLine("??????????LoginServerConnector: " + (eChatServerPacket)code);
				IL_20C:;
			}
			catch (Exception exception)
			{
				GameServer.log.Error("AsynProcessPacket", exception);
			}
		}
		protected void HandleRSAKey(GSPacketIn packet)
		{
			RSAParameters parameters = default(RSAParameters);
			parameters.Modulus = packet.ReadBytes(128);
			parameters.Exponent = packet.ReadBytes();
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.ImportParameters(parameters);
			this.SendRSALogin(rSACryptoServiceProvider, this.m_loginKey);
			this.SendListenIPPort(IPAddress.Parse(GameServer.Instance.Configuration.Ip), GameServer.Instance.Configuration.Port);
		}
		protected void HandleKitoffPlayer(object stateInfo)
		{
			try
			{
				GSPacketIn gSPacketIn = (GSPacketIn)stateInfo;
				int num = gSPacketIn.ReadInt();
				GamePlayer playerById = WorldMgr.GetPlayerById(num);
				if (playerById != null)
				{
					string msg = gSPacketIn.ReadString();
					playerById.Out.SendKitoff(msg);
					playerById.Client.Disconnect();
				}
				else
				{
					this.SendUserOffline(num, 0);
				}
			}
			catch (Exception exception)
			{
				GameServer.log.Error("HandleKitoffPlayer", exception);
			}
		}
		protected void HandleAllowUserLogin(object stateInfo)
		{
			try
			{
				GSPacketIn gSPacketIn = (GSPacketIn)stateInfo;
				int num = gSPacketIn.ReadInt();
				if (gSPacketIn.ReadBoolean())
				{
					GamePlayer gamePlayer = LoginMgr.LoginClient(num);
					if (gamePlayer != null)
					{
						if (gamePlayer.Login())
						{
							this.SendUserOnline(num, gamePlayer.PlayerCharacter.ConsortiaID);
							WorldMgr.OnPlayerOnline(num, gamePlayer.PlayerCharacter.ConsortiaID);
						}
						else
						{
							gamePlayer.Client.Disconnect();
							this.SendUserOffline(num, 0);
						}
					}
					else
					{
						this.SendUserOffline(num, 0);
					}
				}
			}
			catch (Exception exception)
			{
				GameServer.log.Error("HandleAllowUserLogin", exception);
			}
		}
		protected void HandleUserOffline(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int num2 = packet.ReadInt();
				int consortiaID = packet.ReadInt();
				if (LoginMgr.ContainsUser(num2))
				{
					this.SendAllowUserLogin(num2);
				}
				WorldMgr.OnPlayerOffline(num2, consortiaID);
			}
		}
		protected void HandleUserOnline(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int num2 = packet.ReadInt();
				int consortiaID = packet.ReadInt();
				LoginMgr.ClearLoginPlayer(num2);
				GamePlayer playerById = WorldMgr.GetPlayerById(num2);
				if (playerById != null)
				{
					GameServer.log.Error("Player hang in server!!!");
					playerById.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext", new object[0]));
					playerById.Client.Disconnect();
				}
				WorldMgr.OnPlayerOnline(num2, consortiaID);
			}
		}
		protected void HandleChatPersonal(GSPacketIn packet)
		{
			int playerId = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null && !playerById.IsBlackFriend(packet.ClientID))
			{
				playerById.Out.SendTCP(packet);
			}
		}
		protected void HandleBigBugle(GSPacketIn packet)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				gamePlayer.Out.SendTCP(packet);
			}
		}
		public void HandleFriendState(GSPacketIn pkg)
		{
			WorldMgr.ChangePlayerState(pkg.ClientID, pkg.ReadInt(), pkg.ReadInt());
		}
		public void HandleFirendResponse(GSPacketIn packet)
		{
			int playerId = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null)
			{
				playerById.Out.SendTCP(packet);
			}
		}
		public void HandleMailResponse(GSPacketIn packet)
		{
			int playerId = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null)
			{
				playerById.Out.SendTCP(packet);
			}
		}
		public void HandleReload(GSPacketIn packet)
		{
			eReloadType eReloadType = (eReloadType)packet.ReadInt();
			bool val = false;
			switch (eReloadType)
			{
			case eReloadType.ball:
				val = BallMgr.ReLoad();
				break;

			case eReloadType.map:
				val = MapMgr.ReLoadMap();
				break;

			case eReloadType.mapserver:
				val = MapMgr.ReLoadMapServer();
				break;

			case eReloadType.item:
				val = ItemMgr.ReLoad();
				break;

			case eReloadType.quest:
				val = QuestMgr.ReLoad();
				break;

			case eReloadType.fusion:
				val = FusionMgr.ReLoad();
				break;

			case eReloadType.server:
				GameServer.Instance.Configuration.Refresh();
				break;

			case eReloadType.rate:
				val = RateMgr.ReLoad();
				break;

			case eReloadType.consortia:
				val = ConsortiaMgr.ReLoad();
				break;

			case eReloadType.shop:
				val = ShopMgr.ReLoad();
				break;

			case eReloadType.fight:
				val = FightRateMgr.ReLoad();
				break;

			case eReloadType.dailyaward:
				val = AwardMgr.ReLoad();
				break;

			case eReloadType.language:
				val = LanguageMgr.Reload("");
				break;
			}
			packet.WriteInt(GameServer.Instance.Configuration.ServerID);
			packet.WriteBoolean(val);
			this.SendTCP(packet);
		}
		public void HandleChargeMoney(GSPacketIn packet)
		{
			int clientID = packet.ClientID;
			GamePlayer playerById = WorldMgr.GetPlayerById(clientID);
			if (playerById != null)
			{
				playerById.ChargeToUser();
			}
		}
		public void HandleSystemNotice(GSPacketIn packet)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				gamePlayer.Out.SendTCP(packet);
			}
		}
		public void HandleASSState(GSPacketIn packet)
		{
			bool flag = packet.ReadBoolean();
			AntiAddictionMgr.SetASSState(flag);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				gamePlayer.Out.SendAASControl(flag, gamePlayer.IsAASInfo, gamePlayer.IsMinor);
			}
		}
		public void HandleConfigState(GSPacketIn packet)
		{
			bool flag = packet.ReadBoolean();
			bool dailyAwardState = packet.ReadBoolean();
			AwardMgr.DailyAwardState = dailyAwardState;
			AntiAddictionMgr.SetASSState(flag);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				gamePlayer.Out.SendAASControl(flag, gamePlayer.IsAASInfo, gamePlayer.IsMinor);
			}
		}
		public void HandleSysMess(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = num;
			if (num2 != 1)
			{
				return;
			}
			int playerId = packet.ReadInt();
			string text = packet.ReadString().Replace("\0", "");
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null)
			{
				playerById.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg1", new object[]
				{
					text
				}));
			}
		}
		protected void HandleChatConsortia(GSPacketIn packet)
		{
			packet.ReadByte();
			packet.ReadBoolean();
			packet.ReadString();
			packet.ReadString();
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		protected void HandleConsortiaResponse(GSPacketIn packet)
		{
			switch (packet.ReadByte())
			{
			case 1:
				this.HandleConsortiaUserPass(packet);
				return;

			case 2:
				this.HandleConsortiaDelete(packet);
				return;

			case 3:
				this.HandleConsortiaUserDelete(packet);
				return;

			case 4:
				this.HandleConsortiaUserInvite(packet);
				return;

			case 5:
				this.HandleConsortiaBanChat(packet);
				return;

			case 6:
				this.HandleConsortiaUpGrade(packet);
				return;

			case 7:
				this.HandleConsortiaAlly(packet);
				return;

			case 8:
				this.HandleConsortiaDuty(packet);
				return;

			case 9:
				this.HandleConsortiaRichesOffer(packet);
				return;

			case 10:
				this.HandleConsortiaShopUpGrade(packet);
				return;

			case 11:
				this.HandleConsortiaSmithUpGrade(packet);
				return;

			case 12:
				this.HandleConsortiaStoreUpGrade(packet);
				return;

			default:
				return;
			}
		}
		public void HandleConsortiaFight(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadInt();
			string message = packet.ReadString();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.Out.SendMessage(eMessageType.ChatNormal, message);
				}
			}
		}
		public void HandleConsortiaCreate(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			packet.ReadInt();
			ConsortiaMgr.AddConsortia(consortiaID);
		}
		public void HandleConsortiaUserPass(GSPacketIn packet)
		{
			packet.ReadInt();
			packet.ReadBoolean();
			int num = packet.ReadInt();
			string consortiaName = packet.ReadString();
			int num2 = packet.ReadInt();
			packet.ReadString();
			packet.ReadInt();
			packet.ReadString();
			packet.ReadInt();
			string dutyName = packet.ReadString();
			packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			packet.ReadDateTime();
			packet.ReadInt();
			int dutyLevel = packet.ReadInt();
			packet.ReadInt();
			packet.ReadBoolean();
			int right = packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			int consortiaRepute = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ID == num2)
				{
					gamePlayer.BeginChanges();
					gamePlayer.PlayerCharacter.ConsortiaID = num;
					gamePlayer.PlayerCharacter.ConsortiaName = consortiaName;
					gamePlayer.PlayerCharacter.DutyName = dutyName;
					gamePlayer.PlayerCharacter.DutyLevel = dutyLevel;
					gamePlayer.PlayerCharacter.Right = right;
					gamePlayer.PlayerCharacter.ConsortiaRepute = consortiaRepute;
					ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(num);
					if (consortiaInfo != null)
					{
						gamePlayer.PlayerCharacter.ConsortiaLevel = consortiaInfo.Level;
					}
					gamePlayer.CommitChanges();
				}
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaDelete(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.ClearConsortia();
					gamePlayer.AddRobRiches(-gamePlayer.PlayerCharacter.RichesRob);
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaUserDelete(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num2 || gamePlayer.PlayerCharacter.ID == num)
				{
					if (gamePlayer.PlayerCharacter.ID == num)
					{
						gamePlayer.ClearConsortia();
					}
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaUserInvite(GSPacketIn packet)
		{
			packet.ReadInt();
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ID == num)
				{
					gamePlayer.Out.SendTCP(packet);
					break;
				}
			}
		}
		public void HandleConsortiaBanChat(GSPacketIn packet)
		{
			bool isBanChat = packet.ReadBoolean();
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ID == num)
				{
					gamePlayer.PlayerCharacter.IsBanChat = isBanChat;
					gamePlayer.Out.SendTCP(packet);
					break;
				}
			}
		}
		public void HandleConsortiaUpGrade(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadString();
			int consortiaLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaUpGrade(num, consortiaLevel);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.PlayerCharacter.ConsortiaLevel = consortiaLevel;
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaStoreUpGrade(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadString();
			int storeLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaStoreUpGrade(num, storeLevel);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.PlayerCharacter.StoreLevel = storeLevel;
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaShopUpGrade(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadString();
			int shopLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaShopUpGrade(num, shopLevel);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.PlayerCharacter.ShopLevel = shopLevel;
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaSmithUpGrade(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadString();
			int smithLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaSmithUpGrade(num, smithLevel);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.PlayerCharacter.SmithLevel = smithLevel;
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaAlly(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			int state = packet.ReadInt();
			ConsortiaMgr.UpdateConsortiaAlly(num, num2, state);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num || gamePlayer.PlayerCharacter.ConsortiaID == num2)
				{
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaDuty(GSPacketIn packet)
		{
			int num = (int)packet.ReadByte();
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			packet.ReadString();
			int num4 = packet.ReadInt();
			string dutyName = packet.ReadString();
			int right = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num2)
				{
					if (num == 2 && gamePlayer.PlayerCharacter.DutyLevel == num4)
					{
						gamePlayer.PlayerCharacter.DutyName = dutyName;
					}
					else
					{
						if (gamePlayer.PlayerCharacter.ID == num3 && (num == 5 || num == 6 || num == 7 || num == 8 || num == 9))
						{
							gamePlayer.PlayerCharacter.DutyLevel = num4;
							gamePlayer.PlayerCharacter.DutyName = dutyName;
							gamePlayer.PlayerCharacter.Right = right;
						}
					}
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleRate(GSPacketIn packet)
		{
			RateMgr.ReLoad();
		}
		public void HandleConsortiaRichesOffer(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.ConsortiaID == num)
				{
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void HandleUpdatePlayerMarriedState(GSPacketIn packet)
		{
			int playerId = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null)
			{
				playerById.LoadMarryProp();
				playerById.LoadMarryMessage();
				playerById.QuestInventory.ClearMarryQuest();
			}
		}
		public void HandleMarryRoomInfoToPlayer(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			if (playerById != null)
			{
				packet.Code = 252;
				packet.ClientID = num;
				playerById.Out.SendTCP(packet);
			}
		}
		public void HandleShutdown(GSPacketIn pkg)
		{
			GameServer.Instance.Shutdown();
		}
		public void HandleMacroDrop(GSPacketIn pkg)
		{
			Dictionary<int, MacroDropInfo> dictionary = new Dictionary<int, MacroDropInfo>();
			int num = pkg.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int key = pkg.ReadInt();
				int dropCount = pkg.ReadInt();
				int maxDropCount = pkg.ReadInt();
				MacroDropInfo value = new MacroDropInfo(dropCount, maxDropCount);
				dictionary.Add(key, value);
			}
			MacroDropMgr.UpdateDropInfo(dictionary);
		}
		public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(1);
			gSPacketIn.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
			this.SendTCP(gSPacketIn);
		}
		public void SendListenIPPort(IPAddress ip, int port)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(240);
			gSPacketIn.Write(ip.GetAddressBytes());
			gSPacketIn.WriteInt(port);
			this.SendTCP(gSPacketIn);
		}
		public void SendPingCenter()
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			int val = (allPlayers == null) ? 0 : allPlayers.Length;
			GSPacketIn gSPacketIn = new GSPacketIn(12);
			gSPacketIn.WriteInt(val);
			this.SendTCP(gSPacketIn);
		}
		public GSPacketIn SendUserOnline(Dictionary<int, int> users)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(5);
			gSPacketIn.WriteInt(users.Count);
			foreach (KeyValuePair<int, int> current in users)
			{
				gSPacketIn.WriteInt(current.Key);
				gSPacketIn.WriteInt(current.Value);
			}
			this.SendTCP(gSPacketIn);
			return gSPacketIn;
		}
		public GSPacketIn SendUserOnline(int playerid, int consortiaID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(5);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(playerid);
			gSPacketIn.WriteInt(consortiaID);
			this.SendTCP(gSPacketIn);
			return gSPacketIn;
		}
		public GSPacketIn SendUserOffline(int playerid, int consortiaID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(4);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(playerid);
			gSPacketIn.WriteInt(consortiaID);
			this.SendTCP(gSPacketIn);
			return gSPacketIn;
		}
		public void SendAllowUserLogin(int playerid)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(playerid);
			this.SendTCP(gSPacketIn);
		}
		public void SendMailResponse(int playerid)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(117);
			gSPacketIn.WriteInt(playerid);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaUserPass(int playerid, string playerName, ConsortiaUserInfo info, bool isInvite, int consortiaRepute, string loginName, int fightpower, int Offer)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128, playerid);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(info.ID);
			gSPacketIn.WriteBoolean(isInvite);
			gSPacketIn.WriteInt(info.ConsortiaID);
			gSPacketIn.WriteString(info.ConsortiaName);
			gSPacketIn.WriteInt(info.UserID);
			gSPacketIn.WriteString(info.UserName);
			gSPacketIn.WriteInt(playerid);
			gSPacketIn.WriteString(playerName);
			gSPacketIn.WriteInt(info.DutyID);
			gSPacketIn.WriteString(info.DutyName);
			gSPacketIn.WriteInt(info.Offer);
			gSPacketIn.WriteInt(info.RichesOffer);
			gSPacketIn.WriteInt(info.RichesRob);
			gSPacketIn.WriteDateTime(info.LastDate);
			gSPacketIn.WriteInt(info.Grade);
			gSPacketIn.WriteInt(info.Level);
			gSPacketIn.WriteInt(info.State);
			gSPacketIn.WriteBoolean(info.Sex);
			gSPacketIn.WriteInt(info.Right);
			gSPacketIn.WriteInt(info.Win);
			gSPacketIn.WriteInt(info.Total);
			gSPacketIn.WriteInt(info.Escape);
			gSPacketIn.WriteInt(consortiaRepute);
			gSPacketIn.WriteString(loginName);
			gSPacketIn.WriteInt(fightpower);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString("Honor");
			gSPacketIn.WriteInt(info.RichesOffer);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaDelete(int consortiaID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(consortiaID);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaUserDelete(int playerid, int consortiaID, bool isKick, string nickName, string kickName)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(playerid);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteBoolean(isKick);
			gSPacketIn.WriteString(nickName);
			gSPacketIn.WriteString(kickName);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaInvite(int ID, int playerid, string playerName, int inviteID, string intviteName, string consortiaName, int consortiaID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteInt(ID);
			gSPacketIn.WriteInt(playerid);
			gSPacketIn.WriteString(playerName);
			gSPacketIn.WriteInt(inviteID);
			gSPacketIn.WriteString(intviteName);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteString(consortiaName);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaBanChat(int playerid, string playerName, int handleID, string handleName, bool isBan)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteBoolean(isBan);
			gSPacketIn.WriteInt(playerid);
			gSPacketIn.WriteString(playerName);
			gSPacketIn.WriteInt(handleID);
			gSPacketIn.WriteString(handleName);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaFight(int consortiaID, int riches, string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(158);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteInt(riches);
			gSPacketIn.WriteString(msg);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaOffer(int consortiaID, int offer, int riches)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(156);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteInt(offer);
			gSPacketIn.WriteInt(riches);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaCreate(int consortiaID, int offer, string consotiaName)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(130);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteInt(offer);
			gSPacketIn.WriteString(consotiaName);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaUpGrade(ConsortiaInfo info)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(6);
			gSPacketIn.WriteInt(info.ConsortiaID);
			gSPacketIn.WriteString(info.ConsortiaName);
			gSPacketIn.WriteInt(info.Level);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaShopUpGrade(ConsortiaInfo info)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(10);
			gSPacketIn.WriteInt(info.ConsortiaID);
			gSPacketIn.WriteString(info.ConsortiaName);
			gSPacketIn.WriteInt(info.ShopLevel);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaSmithUpGrade(ConsortiaInfo info)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(11);
			gSPacketIn.WriteInt(info.ConsortiaID);
			gSPacketIn.WriteString(info.ConsortiaName);
			gSPacketIn.WriteInt(info.SmithLevel);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaStoreUpGrade(ConsortiaInfo info)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(12);
			gSPacketIn.WriteInt(info.ConsortiaID);
			gSPacketIn.WriteString(info.ConsortiaName);
			gSPacketIn.WriteInt(info.StoreLevel);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaKillUpGrade(ConsortiaInfo info)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(13);
			gSPacketIn.WriteInt(info.ConsortiaID);
			gSPacketIn.WriteString(info.ConsortiaName);
			gSPacketIn.WriteInt(info.SkillLevel);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaAlly(int consortiaID1, int consortiaID2, int state)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(7);
			gSPacketIn.WriteInt(consortiaID1);
			gSPacketIn.WriteInt(consortiaID2);
			gSPacketIn.WriteInt(state);
			this.SendTCP(gSPacketIn);
			ConsortiaMgr.UpdateConsortiaAlly(consortiaID1, consortiaID2, state);
		}
		public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID)
		{
			this.SendConsortiaDuty(info, updateType, consortiaID, 0, "", 0, "");
		}
		public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID, int playerID, string playerName, int handleID, string handleName)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(8);
			gSPacketIn.WriteByte((byte)updateType);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteInt(playerID);
			gSPacketIn.WriteString(playerName);
			gSPacketIn.WriteInt(info.Level);
			gSPacketIn.WriteString(info.DutyName);
			gSPacketIn.WriteInt(info.Right);
			gSPacketIn.WriteInt(handleID);
			gSPacketIn.WriteString(handleName);
			this.SendTCP(gSPacketIn);
		}
		public void SendConsortiaRichesOffer(int consortiaID, int playerID, string playerName, int riches)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(9);
			gSPacketIn.WriteInt(consortiaID);
			gSPacketIn.WriteInt(playerID);
			gSPacketIn.WriteString(playerName);
			gSPacketIn.WriteInt(riches);
			this.SendTCP(gSPacketIn);
		}
		public void SendUpdatePlayerMarriedStates(int playerId)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(13);
			gSPacketIn.WriteInt(playerId);
			this.SendTCP(gSPacketIn);
		}
		public void SendMarryRoomDisposeToPlayer(int roomId)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(241);
			gSPacketIn.WriteInt(roomId);
			this.SendTCP(gSPacketIn);
		}
		public void SendMarryRoomInfoToPlayer(int playerId, bool state, MarryRoomInfo info)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(14);
			gSPacketIn.WriteInt(playerId);
			gSPacketIn.WriteBoolean(state);
			if (state)
			{
				gSPacketIn.WriteInt(info.ID);
				gSPacketIn.WriteString(info.Name);
				gSPacketIn.WriteInt(info.MapIndex);
				gSPacketIn.WriteInt(info.AvailTime);
				gSPacketIn.WriteInt(info.PlayerID);
				gSPacketIn.WriteInt(info.GroomID);
				gSPacketIn.WriteInt(info.BrideID);
				gSPacketIn.WriteDateTime(info.BeginTime);
				gSPacketIn.WriteBoolean(info.IsGunsaluteUsed);
			}
			this.SendTCP(gSPacketIn);
		}
		public void SendShutdown(bool isStoping)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(15);
			gSPacketIn.WriteInt(this.m_serverId);
			gSPacketIn.WriteBoolean(isStoping);
			this.SendTCP(gSPacketIn);
		}
		public void SendPacket(GSPacketIn packet)
		{
			this.SendTCP(packet);
		}
		public LoginServerConnector(string ip, int port, int serverid, string name, byte[] readBuffer, byte[] sendBuffer) : base(ip, port, true, readBuffer, sendBuffer)
		{
			this.m_serverId = serverid;
			this.m_loginKey = string.Format("{0},{1}", serverid, name);
			base.Strict = true;
		}
	}
}
