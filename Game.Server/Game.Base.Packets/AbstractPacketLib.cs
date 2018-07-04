using Bussiness;
using Game.Server;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Base.Packets
{
    [PacketLib(1)]
    public class AbstractPacketLib : IPacketLib
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly GameClient m_gameClient;
        public AbstractPacketLib(GameClient client)
        {
            this.m_gameClient = client;
        }
        public GSPacketIn SendEnterFarm(PlayerInfo Player, UserFarmInfo farm, UserFieldInfo[] fields)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteInt(farm.FarmID);
            gSPacketIn.WriteBoolean(farm.isFarmHelper);
            gSPacketIn.WriteInt(farm.isAutoId);
            gSPacketIn.WriteDateTime(farm.AutoPayTime);
            gSPacketIn.WriteInt(farm.AutoValidDate);
            gSPacketIn.WriteInt(farm.GainFieldId);
            gSPacketIn.WriteInt(farm.KillCropId);
            gSPacketIn.WriteInt(fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                UserFieldInfo userFieldInfo = fields[i];
                gSPacketIn.WriteInt(userFieldInfo.FieldID);
                gSPacketIn.WriteInt(userFieldInfo.SeedID);
                gSPacketIn.WriteDateTime(userFieldInfo.PayTime);
                gSPacketIn.WriteDateTime(userFieldInfo.PlantTime);
                gSPacketIn.WriteInt(userFieldInfo.GainCount);
                gSPacketIn.WriteInt(userFieldInfo.FieldValidDate);
                gSPacketIn.WriteInt(userFieldInfo.AccelerateTime);
            }
            if (farm.FarmID == Player.ID)
            {
                gSPacketIn.WriteString(farm.PayFieldMoney);
                gSPacketIn.WriteString(farm.PayAutoMoney);
                gSPacketIn.WriteDateTime(farm.AutoPayTime);
                gSPacketIn.WriteInt(farm.AutoValidDate);
                gSPacketIn.WriteInt(Player.VIPLevel);
                //gSPacketIn.WriteInt(20);//
                gSPacketIn.WriteInt(farm.buyExpRemainNum);
            }
            else
            {
                gSPacketIn.WriteBoolean(farm.isArrange);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendHelperSwitchField(PlayerInfo Player, UserFarmInfo farm)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
            gSPacketIn.WriteByte(9);
            gSPacketIn.WriteBoolean(farm.isFarmHelper);
            gSPacketIn.WriteInt(farm.isAutoId);
            gSPacketIn.WriteDateTime(farm.AutoPayTime);
            gSPacketIn.WriteInt(farm.AutoValidDate);
            gSPacketIn.WriteInt(farm.GainFieldId);
            gSPacketIn.WriteInt(farm.KillCropId);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendSeeding(PlayerInfo Player, UserFieldInfo field)//__onSeeding
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
            gSPacketIn.WriteByte(2);
            gSPacketIn.WriteInt(field.FieldID);
            gSPacketIn.WriteInt(field.SeedID);
            gSPacketIn.WriteDateTime(field.PlantTime);
            gSPacketIn.WriteDateTime(field.PayTime);
            gSPacketIn.WriteInt(field.GainCount);
            gSPacketIn.WriteInt(field.FieldValidDate);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPayFields(GamePlayer Player, List<int> fieldIds)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81, Player.PlayerCharacter.ID);
            gSPacketIn.WriteByte(6);
            gSPacketIn.WriteInt(Player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(fieldIds.Count);
            foreach (int current in fieldIds)
            {
                UserFieldInfo fieldAt = Player.Farm.GetFieldAt(current);
                gSPacketIn.WriteInt(fieldAt.FieldID);
                gSPacketIn.WriteInt(fieldAt.SeedID);
                gSPacketIn.WriteDateTime(fieldAt.PayTime);
                gSPacketIn.WriteDateTime(fieldAt.PlantTime);
                gSPacketIn.WriteInt(fieldAt.GainCount);
                gSPacketIn.WriteInt(fieldAt.FieldValidDate);
                gSPacketIn.WriteInt(fieldAt.AccelerateTime);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendKillCropField(PlayerInfo Player, UserFieldInfo field)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
            gSPacketIn.WriteByte(7);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteInt(field.FieldID);
            gSPacketIn.WriteInt(field.SeedID);
            gSPacketIn.WriteInt(field.AccelerateTime);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn sendCompose(GamePlayer Player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81, Player.PlayerCharacter.ID);
            gSPacketIn.WriteByte(5);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SenddoMature(GamePlayer Player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81, Player.PlayerCharacter.ID);
            gSPacketIn.WriteByte(3);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendtoGather(PlayerInfo Player, UserFieldInfo field)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
            gSPacketIn.WriteByte(4);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteInt(field.FieldID);
            gSPacketIn.WriteInt(field.SeedID);
            gSPacketIn.WriteDateTime(field.PlantTime);
            gSPacketIn.WriteInt(field.GainCount);
            gSPacketIn.WriteInt(field.AccelerateTime);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public void SendPetGuildOptionChange()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(158);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteInt(8);
            this.SendTCP(gSPacketIn);
        }
        public GSPacketIn SendNewPacket(GamePlayer Player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102, Player.PlayerCharacter.ID);
            gSPacketIn.WriteByte(0);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public void SendOpenWorldBoss()//private function __init(event:CrazyTankSocketEvent) : void
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(0);
            gSPacketIn.WriteString("2");//this._bossResourceId = event.pkg.readUTF();
            gSPacketIn.WriteInt(1243);//_currentPVE_ID = event.pkg.readInt();
            gSPacketIn.WriteString("Bá tước hắc ám");
            gSPacketIn.WriteString("Bá tước hắc ám");
            gSPacketIn.WriteLong(8000000L);//_bossInfo.total_Blood = event.pkg.readLong();
            gSPacketIn.WriteInt(5);
            gSPacketIn.WriteInt(5);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(265);
            gSPacketIn.WriteInt(1030);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteDateTime(DateTime.Now.AddDays(1.0));
            gSPacketIn.WriteInt(450);
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteInt(11573);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(30);
            gSPacketIn.WriteInt(30);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("Tăng dame");
            gSPacketIn.WriteInt(30);
            gSPacketIn.WriteString("Tăng dame gấp 10000 lần");
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteBoolean(true);//_autoBlood = event.pkg.readBoolean();
            this.SendTCP(gSPacketIn);
        }
        public void SendLittleGameActived()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(80);
            gSPacketIn.WriteBoolean(true);
            this.SendTCP(gSPacketIn);
        }

        

        public GSPacketIn SendContinuation(GamePlayer player, HotSpringRoomInfo hotSpringRoomInfo)
        {
            throw new NotImplementedException();
        }
        public GSPacketIn SendOpenVIP(PlayerInfo Player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(92, Player.ID);
            gSPacketIn.WriteByte(Player.typeVIP);
            gSPacketIn.WriteInt(Player.VIPLevel);
            gSPacketIn.WriteInt(Player.VIPExp);
            gSPacketIn.WriteDateTime(Player.VIPExpireDay);
            gSPacketIn.WriteDateTime(Player.LastVIPPackTime);
            gSPacketIn.WriteInt(Player.VIPNextLevelDaysNeeded);
            gSPacketIn.WriteBoolean(Player.CanTakeVipReward);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendGetBoxTime(int ID, int receiebox, bool result)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(53, ID);
            gSPacketIn.WriteBoolean(result);
            gSPacketIn.WriteInt(receiebox);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public static IPacketLib CreatePacketLibForVersion(int rawVersion, GameClient client)
        {
            Type[] derivedClasses = ScriptMgr.GetDerivedClasses(typeof(IPacketLib));
            for (int i = 0; i < derivedClasses.Length; i++)
            {
                Type type = derivedClasses[i];
                object[] customAttributes = type.GetCustomAttributes(typeof(PacketLibAttribute), false);
                for (int j = 0; j < customAttributes.Length; j++)
                {
                    PacketLibAttribute packetLibAttribute = (PacketLibAttribute)customAttributes[j];
                    if (packetLibAttribute.RawVersion == rawVersion)
                    {
                        try
                        {
                            return (IPacketLib)Activator.CreateInstance(type, new object[]
							{
								client
							});
                        }
                        catch (Exception exception)
                        {
                            if (AbstractPacketLib.log.IsErrorEnabled)
                            {
                                AbstractPacketLib.log.Error(string.Concat(new object[]
								{
									"error creating packetlib (",
									type.FullName,
									") for raw version ",
									rawVersion
								}), exception);
                            }
                        }
                    }
                }
            }
            return null;
        }
        public void SendTCP(GSPacketIn packet)
        {
            this.m_gameClient.SendTCP(packet);
        }
        public void SendWeaklessGuildProgress(PlayerInfo player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(15, player.ID);
            gSPacketIn.WriteInt(player.weaklessGuildProgress.Length);
            for (int i = 0; i < player.weaklessGuildProgress.Length; i++)
            {
                gSPacketIn.WriteByte(player.weaklessGuildProgress[i]);
            }
            this.SendTCP(gSPacketIn);
        }
        public void SendLoginFailed(string msg)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(1);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteString(msg);
            this.SendTCP(gSPacketIn);
        }
        public void SendLoginSuccess()//__onLoginComplete
        {
            if (this.m_gameClient.Player == null)
            {
                return;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(1, this.m_gameClient.Player.PlayerCharacter.ID);
            gSPacketIn.WriteByte(0);
            gSPacketIn.WriteInt(4);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Attack);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Defence);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Agility);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Luck);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.GP);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Repute);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Gold);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Money);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.GiftToken);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Score);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Hide);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.FightPower);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteDateTime(DateTime.Now.AddDays(50.0));
            gSPacketIn.WriteByte(this.m_gameClient.Player.PlayerCharacter.typeVIP);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.VIPLevel);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.VIPExp);
            gSPacketIn.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.VIPExpireDay);
            gSPacketIn.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.LastDate);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.VIPNextLevelDaysNeeded);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.CanTakeVipReward);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.OptionOnOff);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.AchievementPoint);
            gSPacketIn.WriteString("");// _loc_3.honor = _loc_2.readUTF();
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.OnlineTime);
            gSPacketIn.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.Sex);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.Style + "&" + this.m_gameClient.Player.PlayerCharacter.Colors);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.Skin);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaID);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.ConsortiaName);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.badgeID);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.DutyLevel);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.DutyName);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Right);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.ChairmanName);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaHonor);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaRiches);
            gSPacketIn.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.HasBagPassword);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.PasswordQuest1);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.PasswordQuest2);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.FailedPasswordAttemptCount);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.UserName);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Nimbus);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.PvePermission);
            gSPacketIn.WriteString(this.m_gameClient.Player.PlayerCharacter.FightLabPermission);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.AnswerSite);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.receiebox);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.receieGrade);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.needGetBoxTime);
            gSPacketIn.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.LastSpaDate);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.RichesOffer);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.spdTexpExp);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.attTexpExp);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.defTexpExp);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.hpTexpExp);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.lukTexpExp);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.texpTaskCount);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.texpCount);
            gSPacketIn.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.Texp.texpTaskDate);
            gSPacketIn.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.isOldPlayerHasValidEquitAtLogin);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.badLuckNumber);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.luckyNum);
            gSPacketIn.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.lastLuckyNumDate);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.lastLuckNum);
            gSPacketIn.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.IsOldPlayer);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.CardSoul);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.uesedFinishTime);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.totemId);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.necklaceExp);//_local3.necklaceExp = _local2.readInt();//VNG
            this.SendTCP(gSPacketIn);
        }
        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(7);
            gSPacketIn.Write(m);
            gSPacketIn.Write(e);
            this.SendTCP(gSPacketIn);
        }
        public void SendCheckCode()
        {
            if (this.m_gameClient.Player == null || this.m_gameClient.Player.PlayerCharacter.CheckCount < GameProperties.CHECK_MAX_FAILED_COUNT)
            {
                return;
            }
            if (this.m_gameClient.Player.PlayerCharacter.CheckError == 0)
            {
                this.m_gameClient.Player.PlayerCharacter.CheckCount += 10000;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(200, this.m_gameClient.Player.PlayerCharacter.ID, 10240);
            if (this.m_gameClient.Player.PlayerCharacter.CheckError < 1)
            {
                gSPacketIn.WriteByte(0);
            }
            else
            {
                gSPacketIn.WriteByte(2);
            }
            gSPacketIn.WriteBoolean(true);
            this.m_gameClient.Player.PlayerCharacter.CheckCode = CheckCode.GenerateCheckCode();
            gSPacketIn.Write(CheckCode.CreateImage(this.m_gameClient.Player.PlayerCharacter.CheckCode));
            this.SendTCP(gSPacketIn);
        }
        public void SendKitoff(string msg)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(2);
            gSPacketIn.WriteString(msg);
            this.SendTCP(gSPacketIn);
        }
        public void SendEditionError(string msg)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(12);
            gSPacketIn.WriteString(msg);
            this.SendTCP(gSPacketIn);
        }
        public void SendWaitingRoom(bool result)
        {
            GSPacketIn pkg = new GSPacketIn(16);
            pkg.WriteByte((byte)(result ? 1 : 0));
            this.SendTCP(pkg);
        }
        public GSPacketIn SendPlayerState(int id, byte state)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(32, id);
            gSPacketIn.WriteByte(state);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public virtual GSPacketIn SendMessage(eMessageType type, string message)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(3);
            gSPacketIn.WriteInt((int)type);
            gSPacketIn.WriteString(message);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public void SendReady()
        {
            GSPacketIn packet = new GSPacketIn(0);
            this.SendTCP(packet);
        }
        public void SendUpdatePrivateInfo(PlayerInfo info)//__updatePrivateInfo
        {
            GSPacketIn gSPacketIn = new GSPacketIn(38, info.ID);
            gSPacketIn.WriteInt(info.Money);
            gSPacketIn.WriteInt(info.GiftToken);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(info.Score);
            gSPacketIn.WriteInt(info.Gold);
            gSPacketIn.WriteInt(info.badLuckNumber);
            gSPacketIn.WriteInt(info.damageScores);
            gSPacketIn.WriteInt(info.petScore);
            gSPacketIn.WriteInt(info.myHonor);
            gSPacketIn.WriteInt(info.hardCurrency);
            this.SendTCP(gSPacketIn);
        }
        public GSPacketIn SendUpdatePlayerProperty(PlayerInfo info, Dictionary<string, Dictionary<string, int>> Property)//__updatePlayerProperty
        {
            try
            {
                string[] array = new string[]
				{
					"Attack",
					"Defence",
					"Agility",
					"Luck"
				};
                GSPacketIn gSPacketIn = new GSPacketIn(164, info.ID);
                gSPacketIn.WriteInt(info.ID);
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string key = array2[i];
                    gSPacketIn.WriteInt(info.ID);
                    gSPacketIn.WriteInt(Property["Texp"][key]);
                    gSPacketIn.WriteInt(Property["Card"][key]);
                    gSPacketIn.WriteInt(Property["Pet"][key]);
                    gSPacketIn.WriteInt(Property["Suit"][key]);
                    gSPacketIn.WriteInt(Property["Gem"][key]);
                }
                gSPacketIn.WriteInt(info.ID);
                gSPacketIn.WriteInt(Property["Texp"]["HP"]);
                gSPacketIn.WriteInt(Property["Pet"]["HP"]);
                gSPacketIn.WriteInt(Property["Suit"]["HP"]);
                gSPacketIn.WriteInt(Property["Gem"]["HP"]);
                gSPacketIn.WriteInt(Property["Suit"]["Damage"]);
                gSPacketIn.WriteInt(Property["Suit"]["Guard"]);
                gSPacketIn.WriteInt(Property["Bead"]["Damage"]);
                gSPacketIn.WriteInt(Property["Bead"]["Armor"]);
                this.SendTCP(gSPacketIn);
                return gSPacketIn;
            }
            catch (Exception)
            {
            }
            return null;
        }
        public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info)//__updatePlayerInfo
        {
            GSPacketIn gSPacketIn = new GSPacketIn(67, info.ID);
            gSPacketIn.WriteInt(info.GP);
            gSPacketIn.WriteInt(info.Offer);
            gSPacketIn.WriteInt(info.RichesOffer);
            gSPacketIn.WriteInt(info.RichesRob);
            gSPacketIn.WriteInt(info.Win);
            gSPacketIn.WriteInt(info.Total);
            gSPacketIn.WriteInt(info.Escape);
            gSPacketIn.WriteInt(info.Attack);
            gSPacketIn.WriteInt(info.Defence);
            gSPacketIn.WriteInt(info.Agility);
            gSPacketIn.WriteInt(info.Luck);
            gSPacketIn.WriteInt(info.hp);
            gSPacketIn.WriteInt(info.Hide);
            gSPacketIn.WriteString(info.Style);
            gSPacketIn.WriteString(info.Colors);
            gSPacketIn.WriteString(info.Skin);
            gSPacketIn.WriteBoolean(info.IsShowConsortia);
            gSPacketIn.WriteInt(info.ConsortiaID);
            gSPacketIn.WriteString(info.ConsortiaName);
            gSPacketIn.WriteInt(info.badgeID);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(info.Nimbus);
            gSPacketIn.WriteString(info.PvePermission);
            gSPacketIn.WriteString(info.FightLabPermission);
            gSPacketIn.WriteInt(info.FightPower);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteInt(info.AchievementPoint);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteDateTime(info.LastSpaDate);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteDateTime(DateTime.Now);
            gSPacketIn.WriteInt(info.RichesOffer);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(info.LeagueMoney);
            gSPacketIn.WriteInt(info.Texp.spdTexpExp);
            gSPacketIn.WriteInt(info.Texp.attTexpExp);
            gSPacketIn.WriteInt(info.Texp.defTexpExp);
            gSPacketIn.WriteInt(info.Texp.hpTexpExp);
            gSPacketIn.WriteInt(info.Texp.lukTexpExp);
            gSPacketIn.WriteInt(info.Texp.texpTaskCount);
            gSPacketIn.WriteInt(info.Texp.texpCount);
            gSPacketIn.WriteDateTime(info.Texp.texpTaskDate);
            gSPacketIn.WriteInt(9);
            for (int i = 1; i < 10; i++)
            {
                gSPacketIn.WriteInt(i);
                gSPacketIn.WriteByte(10);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public void SendPingTime(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(4, player.PlayerCharacter.ID);
            player.PingStart = DateTime.Now.Ticks;
            gSPacketIn.WriteInt(player.PlayerCharacter.AntiAddiction);
            this.SendTCP(gSPacketIn);
        }
        public GSPacketIn SendNetWork(GamePlayer player, long delay)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(6, player.PlayerId);
            gSPacketIn.WriteInt((int)delay / 1000 / 10);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendUserEquip(PlayerInfo player, List<ItemInfo> items, List<UserGemStone> UserGemStone, List<ItemInfo> beadItems)
        {
            if (this.m_gameClient.Player == null)
            {
                return null;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(74, this.m_gameClient.Player.PlayerCharacter.ID, 10240);
            gSPacketIn.WriteInt(player.ID);
            gSPacketIn.WriteString(player.NickName);
            gSPacketIn.WriteInt(player.Agility);
            gSPacketIn.WriteInt(player.Attack);
            gSPacketIn.WriteString(player.Colors);
            gSPacketIn.WriteString(player.Skin);
            gSPacketIn.WriteInt(player.Defence);
            gSPacketIn.WriteInt(player.GP);
            gSPacketIn.WriteInt(player.Grade);
            gSPacketIn.WriteInt(player.Luck);
            gSPacketIn.WriteInt(player.hp);
            gSPacketIn.WriteInt(player.Hide);
            gSPacketIn.WriteInt(player.Repute);
            gSPacketIn.WriteBoolean(player.Sex);
            gSPacketIn.WriteString(player.Style);
            gSPacketIn.WriteInt(player.Offer);
            gSPacketIn.WriteByte(player.typeVIP);
            gSPacketIn.WriteInt(player.VIPLevel);
            gSPacketIn.WriteInt(player.Win);
            gSPacketIn.WriteInt(player.Total);
            gSPacketIn.WriteInt(player.Escape);
            gSPacketIn.WriteInt(player.ConsortiaID);
            gSPacketIn.WriteString(player.ConsortiaName);
            gSPacketIn.WriteInt(player.badgeID);
            gSPacketIn.WriteInt(player.RichesOffer);
            gSPacketIn.WriteInt(player.RichesRob);
            gSPacketIn.WriteBoolean(player.IsMarried);
            gSPacketIn.WriteInt(player.SpouseID);
            gSPacketIn.WriteString(player.SpouseName);
            gSPacketIn.WriteString(player.DutyName);
            gSPacketIn.WriteInt(player.Nimbus);
            gSPacketIn.WriteInt(player.FightPower);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteInt(player.AchievementPoint);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteDateTime(DateTime.Now.AddDays(-2.0));
            gSPacketIn.WriteInt(player.Texp.spdTexpExp);
            gSPacketIn.WriteInt(player.Texp.attTexpExp);
            gSPacketIn.WriteInt(player.Texp.defTexpExp);
            gSPacketIn.WriteInt(player.Texp.hpTexpExp);
            gSPacketIn.WriteInt(player.Texp.lukTexpExp);
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(player.totemId);
            gSPacketIn.WriteInt(items.Count);
			gSPacketIn.WriteInt(0);//_local6 = _local2.readInt();
            gSPacketIn.WriteInt(player.necklaceExp);//_local5.necklaceExp = _local2.readInt();//VNG
            foreach (ItemInfo current in items)
            {
                gSPacketIn.WriteByte((byte)current.BagType);
                gSPacketIn.WriteInt(current.UserID);
                gSPacketIn.WriteInt(current.ItemID);
                gSPacketIn.WriteInt(current.Count);
                gSPacketIn.WriteInt(current.Place);
                gSPacketIn.WriteInt(current.TemplateID);
                gSPacketIn.WriteInt(current.AttackCompose);
                gSPacketIn.WriteInt(current.DefendCompose);
                gSPacketIn.WriteInt(current.AgilityCompose);
                gSPacketIn.WriteInt(current.LuckCompose);
                gSPacketIn.WriteInt(current.StrengthenLevel);
                gSPacketIn.WriteBoolean(current.IsBinds);
                gSPacketIn.WriteBoolean(current.IsJudge);
                gSPacketIn.WriteDateTime(current.BeginDate);
                gSPacketIn.WriteInt(current.ValidDate);
                gSPacketIn.WriteString(current.Color);
                gSPacketIn.WriteString(current.Skin);
                gSPacketIn.WriteBoolean(current.IsUsed);
                gSPacketIn.WriteInt(current.Hole1);
                gSPacketIn.WriteInt(current.Hole2);
                gSPacketIn.WriteInt(current.Hole3);
                gSPacketIn.WriteInt(current.Hole4);
                gSPacketIn.WriteInt(current.Hole5);
                gSPacketIn.WriteInt(current.Hole6);
                gSPacketIn.WriteString(current.Template.Pic);
                gSPacketIn.WriteInt(current.Template.RefineryLevel);
                gSPacketIn.WriteDateTime(DateTime.Now);
                gSPacketIn.WriteByte((byte)current.Hole5Level);
                gSPacketIn.WriteInt(current.Hole5Exp);
                gSPacketIn.WriteByte((byte)current.Hole6Level);
                gSPacketIn.WriteInt(current.Hole6Exp);
                if (current.IsGold)
                {
                    gSPacketIn.WriteBoolean(current.IsGold);
                    gSPacketIn.WriteInt(current.goldValidDate);
                    gSPacketIn.WriteDateTime(current.goldBeginTime);
                }
                else
                {
                    gSPacketIn.WriteBoolean(false);
                }
                gSPacketIn.WriteString(current.latentEnergyCurStr);
                gSPacketIn.WriteString(current.latentEnergyNewStr);
                gSPacketIn.WriteDateTime(current.latentEnergyEndTime);
            }
            gSPacketIn.WriteInt(beadItems.Count);
            foreach (ItemInfo current2 in beadItems)
            {
                gSPacketIn.WriteByte((byte)current2.BagType);
                gSPacketIn.WriteInt(current2.UserID);
                gSPacketIn.WriteInt(current2.ItemID);
                gSPacketIn.WriteInt(current2.Count);
                gSPacketIn.WriteInt(current2.Place);
                gSPacketIn.WriteInt(current2.TemplateID);
                gSPacketIn.WriteInt(current2.AttackCompose);
                gSPacketIn.WriteInt(current2.DefendCompose);
                gSPacketIn.WriteInt(current2.AgilityCompose);
                gSPacketIn.WriteInt(current2.LuckCompose);
                gSPacketIn.WriteInt(current2.StrengthenLevel);
                gSPacketIn.WriteBoolean(current2.IsBinds);
                gSPacketIn.WriteBoolean(current2.IsJudge);
                gSPacketIn.WriteDateTime(current2.BeginDate);
                gSPacketIn.WriteInt(current2.ValidDate);
                gSPacketIn.WriteString(current2.Color);
                gSPacketIn.WriteString(current2.Skin);
                gSPacketIn.WriteBoolean(current2.IsUsed);
                gSPacketIn.WriteInt(current2.Hole1);
                gSPacketIn.WriteInt(current2.Hole2);
                gSPacketIn.WriteInt(current2.Hole3);
                gSPacketIn.WriteInt(current2.Hole4);
                gSPacketIn.WriteInt(current2.Hole5);
                gSPacketIn.WriteInt(current2.Hole6);
                gSPacketIn.WriteString(current2.Template.Pic);
                gSPacketIn.WriteInt(current2.Template.RefineryLevel);
                gSPacketIn.WriteDateTime(DateTime.Now);
                gSPacketIn.WriteByte((byte)current2.Hole5Level);
                gSPacketIn.WriteInt(current2.Hole5Exp);
                gSPacketIn.WriteByte((byte)current2.Hole6Level);
                gSPacketIn.WriteInt(current2.Hole6Exp);
                gSPacketIn.WriteBoolean(current2.IsGold);
            }
            gSPacketIn.WriteInt(UserGemStone.Count);
            for (int i = 0; i < UserGemStone.Count; i++)
            {
                gSPacketIn.WriteInt(UserGemStone[i].FigSpiritId);
                gSPacketIn.WriteString(UserGemStone[i].FigSpiritIdValue);
                gSPacketIn.WriteInt(UserGemStone[i].EquipPlace);
            }
            gSPacketIn.Compress();
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public void SendDateTime()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(5);
            gSPacketIn.WriteDateTime(DateTime.Now);
            this.SendTCP(gSPacketIn);
        }
        public GSPacketIn SendDailyAward(GamePlayer player)
        {
            bool val = false;
            if (DateTime.Now.Date != player.PlayerCharacter.LastAward.Date)
            {
                val = true;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(13, player.PlayerCharacter.ID);
            gSPacketIn.WriteBoolean(val);
            gSPacketIn.WriteInt(0);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendUpdateRoomList(List<BaseRoom> roomlist)//__addRoom
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(9);
            gSPacketIn.WriteInt(roomlist.Count);
            int num = (roomlist.Count < 8) ? roomlist.Count : 8;
            gSPacketIn.WriteInt(num);
            for (int i = 0; i < num; i++)
            {
                BaseRoom baseRoom = roomlist[i];
                gSPacketIn.WriteInt(baseRoom.RoomId);
                gSPacketIn.WriteByte((byte)baseRoom.RoomType);
                gSPacketIn.WriteByte(baseRoom.TimeMode);
                gSPacketIn.WriteByte((byte)baseRoom.PlayerCount);
                gSPacketIn.WriteByte((byte)baseRoom.viewerCnt);
                gSPacketIn.WriteByte((byte)baseRoom.maxViewerCnt);
                gSPacketIn.WriteByte((byte)baseRoom.PlacesCount);
                gSPacketIn.WriteBoolean(!string.IsNullOrEmpty(baseRoom.Password));
                gSPacketIn.WriteInt(baseRoom.MapId);
                gSPacketIn.WriteBoolean(baseRoom.IsPlaying);
                gSPacketIn.WriteString(baseRoom.Name);
                gSPacketIn.WriteByte((byte)baseRoom.GameType);
                gSPacketIn.WriteByte((byte)baseRoom.HardLevel);
                gSPacketIn.WriteInt(baseRoom.LevelLimits);
                gSPacketIn.WriteBoolean(baseRoom.isOpenBoss);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendSceneAddPlayer(GamePlayer player)//__addWaitingPlayer
        {
            GSPacketIn gSPacketIn = new GSPacketIn(18, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
            gSPacketIn.WriteString(player.PlayerCharacter.NickName);
            gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
            gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
            gSPacketIn.WriteString(player.PlayerCharacter.ConsortiaName);
            gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
            gSPacketIn.WriteInt(player.PlayerCharacter.Win);
            gSPacketIn.WriteInt(player.PlayerCharacter.Total);
            gSPacketIn.WriteInt(player.PlayerCharacter.Escape);
            gSPacketIn.WriteInt(player.PlayerCharacter.ConsortiaID);
            gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.IsMarried);
            if (player.PlayerCharacter.IsMarried)
            {
                gSPacketIn.WriteInt(player.PlayerCharacter.SpouseID);
                gSPacketIn.WriteString(player.PlayerCharacter.SpouseName);
            }
            gSPacketIn.WriteString(player.PlayerCharacter.UserName);
            gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.Grade > 8);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(21, player.PlayerCharacter.ID);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendGameMissionStart()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(82);
            gSPacketIn.WriteBoolean(true);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendGameMissionPrepare()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(116);
            gSPacketIn.WriteBoolean(true);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomPlayerAdd(GamePlayer player)//__addPlayerInRoom
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerId);
            gSPacketIn.WriteByte(4);
            bool val = false;
            if (player.CurrentRoom.Game != null)
            {
                val = true;
            }
            gSPacketIn.WriteBoolean(val);
            gSPacketIn.WriteByte((byte)player.CurrentRoomIndex);
            gSPacketIn.WriteByte((byte)player.CurrentRoomTeam);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
            gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
            gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
            gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
            gSPacketIn.WriteInt((int)player.PingTime / 1000 / 10);
            gSPacketIn.WriteInt(4);
            gSPacketIn.WriteInt(player.PlayerCharacter.ID);
            gSPacketIn.WriteString(player.PlayerCharacter.NickName);
            gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
            gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
            gSPacketIn.WriteString(player.PlayerCharacter.Style);
            gSPacketIn.WriteString(player.PlayerCharacter.Colors);
            gSPacketIn.WriteString(player.PlayerCharacter.Skin);
            ItemInfo itemAt = player.MainBag.GetItemAt(6);
            gSPacketIn.WriteInt((itemAt == null) ? 7008 : itemAt.TemplateID);
            if (player.SecondWeapon == null)
            {
                gSPacketIn.WriteInt(0);
            }
            else
            {
                gSPacketIn.WriteInt(player.SecondWeapon.TemplateID);
            }
            gSPacketIn.WriteInt(player.PlayerCharacter.ConsortiaID);
            gSPacketIn.WriteString(player.PlayerCharacter.ConsortiaName);
            gSPacketIn.WriteInt(player.PlayerCharacter.badgeID);
            gSPacketIn.WriteInt(player.PlayerCharacter.Win);
            gSPacketIn.WriteInt(player.PlayerCharacter.Total);
            gSPacketIn.WriteInt(player.PlayerCharacter.Escape);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.IsMarried);
            if (player.PlayerCharacter.IsMarried)
            {
                gSPacketIn.WriteInt(player.PlayerCharacter.SpouseID);
                gSPacketIn.WriteString(player.PlayerCharacter.SpouseName);
            }
            gSPacketIn.WriteString(player.PlayerCharacter.UserName);
            gSPacketIn.WriteInt(player.PlayerCharacter.Nimbus);
            gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("Master");
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("HonorOfMaster");
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.IsOldPlayer);
            gSPacketIn.WriteInt(0);//_local18 = _local2.readInt();//vNG
            if (player.Pet == null)
            {
                gSPacketIn.WriteInt(0);
            }
            else
            {
                gSPacketIn.WriteInt(1);
                gSPacketIn.WriteInt(player.Pet.Place);
                gSPacketIn.WriteInt(player.Pet.TemplateID);
                gSPacketIn.WriteInt(player.Pet.ID);
                gSPacketIn.WriteString(player.Pet.Name);
                gSPacketIn.WriteInt(player.PlayerCharacter.ID);
                gSPacketIn.WriteInt(player.Pet.Level);
                List<string> skillEquip = player.Pet.GetSkillEquip();
                gSPacketIn.WriteInt(skillEquip.Count);
                foreach (string current in skillEquip)
                {
                    gSPacketIn.WriteInt(int.Parse(current.Split(new char[]
					{
						','
					})[1]));
                    gSPacketIn.WriteInt(int.Parse(current.Split(new char[]
					{
						','
					})[0]));
                }
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerId);
            gSPacketIn.WriteByte(5);
            gSPacketIn.Parameter1 = player.PlayerId;
            gSPacketIn.WriteInt(4);
            gSPacketIn.WriteInt(4);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(15);
            for (int i = 0; i < states.Length; i++)
            {
                gSPacketIn.WriteByte(states[i]);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(10);
            for (int i = 0; i < states.Length; i++)
            {
                gSPacketIn.WriteInt(states[i]);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerId);
            gSPacketIn.WriteByte(6);
            gSPacketIn.WriteByte((byte)player.CurrentRoomTeam);
            gSPacketIn.WriteByte((byte)player.CurrentRoomIndex);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomCreate(BaseRoom room)//__createRoom
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(0);
            gSPacketIn.WriteInt(room.RoomId);
            gSPacketIn.WriteByte((byte)room.RoomType);
            gSPacketIn.WriteByte((byte)room.HardLevel);
            gSPacketIn.WriteByte(room.TimeMode);
            gSPacketIn.WriteByte((byte)room.PlayerCount);
            gSPacketIn.WriteByte((byte)room.viewerCnt);
            gSPacketIn.WriteByte((byte)room.PlacesCount);
            gSPacketIn.WriteBoolean(!string.IsNullOrEmpty(room.Password));
            gSPacketIn.WriteInt(room.MapId);
            gSPacketIn.WriteBoolean(room.IsPlaying);
            gSPacketIn.WriteString(room.Name);
            gSPacketIn.WriteByte((byte)room.GameType);
            gSPacketIn.WriteInt(room.LevelLimits);
            gSPacketIn.WriteBoolean(room.isCrosszone);
            gSPacketIn.WriteBoolean(room.isWithinLeageTime);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomLoginResult(bool result)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteBoolean(result);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomPairUpStart(BaseRoom room)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(13);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(12);
            gSPacketIn.WriteByte((byte)game.GameStyle);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(11);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(66, player.PlayerCharacter.ID);
            gSPacketIn.WriteByte((byte)place);
            gSPacketIn.WriteInt(goodsID);
            gSPacketIn.WriteString(style);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRoomChange(BaseRoom room)//__settingRoom
        {
            GSPacketIn gSPacketIn = new GSPacketIn(94);
            gSPacketIn.WriteByte(2);
            gSPacketIn.WriteBoolean(room.isOpenBoss);
            if (room.isOpenBoss)
            {
                gSPacketIn.WriteString((room.Pic == null) ? "1.jpg" : room.Pic);
            }
            gSPacketIn.WriteInt(room.MapId);
            gSPacketIn.WriteByte((byte)room.RoomType);
            gSPacketIn.WriteString((room.Password == null) ? "" : room.Password);
            gSPacketIn.WriteString((room.Name == null) ? "GunnyII" : room.Name);
            gSPacketIn.WriteByte(room.TimeMode);
            gSPacketIn.WriteByte((byte)room.HardLevel);
            gSPacketIn.WriteInt(room.LevelLimits);
            gSPacketIn.WriteBoolean(room.isCrosszone);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isbind, int MinValid)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(76, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(previewItemList.Count);
            foreach (KeyValuePair<int, double> current in previewItemList)
            {
                gSPacketIn.WriteInt(current.Key);
                gSPacketIn.WriteInt(MinValid);
                int num = (int)current.Value;
                gSPacketIn.WriteInt((num > 100) ? 100 : ((num < 0) ? 0 : num));
            }
            gSPacketIn.WriteBoolean(isbind);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendFusionResult(GamePlayer player, bool result)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(78, player.PlayerCharacter.ID);
            gSPacketIn.WriteBoolean(result);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendOpenHoleComplete(GamePlayer player, int type, bool result)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(217, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(type);
            gSPacketIn.WriteBoolean(result);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRuneOpenPackage(GamePlayer player, int rand)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(121, player.PlayerCharacter.ID);
            gSPacketIn.WriteByte(3);
            gSPacketIn.WriteInt(rand);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, ItemInfo item)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(111, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(templateid);
            gSPacketIn.WriteInt(item.ValidDate);
            gSPacketIn.WriteBoolean(isbind);
            gSPacketIn.WriteInt(item.AgilityCompose);
            gSPacketIn.WriteInt(item.AttackCompose);
            gSPacketIn.WriteInt(item.DefendCompose);
            gSPacketIn.WriteInt(item.LuckCompose);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)//__updateInventorySlot
        {
            if (this.m_gameClient.Player == null)
            {
                return;
            }
            int size = 10480;
            int num = updatedSlots.Length;
            if (num > 49)
            {
                size = 20480;
            }
            if (num > 99)
            {
                size = 30480;
            }
            if (num > 149)
            {
                size = 40960;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(64, this.m_gameClient.Player.PlayerCharacter.ID, size);
            gSPacketIn.WriteInt(bag.BagType);
            gSPacketIn.WriteInt(num);
            for (int i = 0; i < updatedSlots.Length; i++)
            {
                int num2 = updatedSlots[i];
                gSPacketIn.WriteInt(num2);
                ItemInfo itemAt = bag.GetItemAt(num2);
                if (itemAt == null)
                {
                    gSPacketIn.WriteBoolean(false);
                }
                else
                {
                    gSPacketIn.WriteBoolean(true);
                    gSPacketIn.WriteInt(itemAt.UserID);
                    gSPacketIn.WriteInt(itemAt.ItemID);
                    gSPacketIn.WriteInt(itemAt.Count);
                    gSPacketIn.WriteInt(itemAt.Place);
                    gSPacketIn.WriteInt(itemAt.TemplateID);
                    gSPacketIn.WriteInt(itemAt.AttackCompose);
                    gSPacketIn.WriteInt(itemAt.DefendCompose);
                    gSPacketIn.WriteInt(itemAt.AgilityCompose);
                    gSPacketIn.WriteInt(itemAt.LuckCompose);
                    gSPacketIn.WriteInt(itemAt.StrengthenLevel);
                    gSPacketIn.WriteInt(itemAt.StrengthenExp);
                    gSPacketIn.WriteBoolean(itemAt.IsBinds);
                    gSPacketIn.WriteBoolean(itemAt.IsJudge);
                    gSPacketIn.WriteDateTime(itemAt.BeginDate);
                    gSPacketIn.WriteInt(itemAt.ValidDate);
                    gSPacketIn.WriteString((itemAt.Color == null) ? "" : itemAt.Color);
                    gSPacketIn.WriteString((itemAt.Skin == null) ? "" : itemAt.Skin);
                    gSPacketIn.WriteBoolean(itemAt.IsUsed);
                    gSPacketIn.WriteInt(itemAt.Hole1);
                    gSPacketIn.WriteInt(itemAt.Hole2);
                    gSPacketIn.WriteInt(itemAt.Hole3);
                    gSPacketIn.WriteInt(itemAt.Hole4);
                    gSPacketIn.WriteInt(itemAt.Hole5);
                    gSPacketIn.WriteInt(itemAt.Hole6);
                    gSPacketIn.WriteString(itemAt.Template.Pic);
                    gSPacketIn.WriteInt(itemAt.Template.RefineryLevel);
                    gSPacketIn.WriteDateTime(DateTime.Now);
                    gSPacketIn.WriteInt(itemAt.StrengthenTimes);
                    gSPacketIn.WriteByte((byte)itemAt.Hole5Level);
                    gSPacketIn.WriteInt(itemAt.Hole5Exp);
                    gSPacketIn.WriteByte((byte)itemAt.Hole6Level);
                    gSPacketIn.WriteInt(itemAt.Hole6Exp);
                    if (itemAt.IsGold)
                    {
                        gSPacketIn.WriteBoolean(itemAt.IsGold);
                        gSPacketIn.WriteInt(itemAt.goldValidDate);
                        gSPacketIn.WriteDateTime(itemAt.goldBeginTime);
                    }
                    else
                    {
                        gSPacketIn.WriteBoolean(false);
                    }
                    gSPacketIn.WriteString(itemAt.latentEnergyCurStr);
                    gSPacketIn.WriteString(itemAt.latentEnergyCurStr);
                    gSPacketIn.WriteDateTime(itemAt.latentEnergyEndTime);
                }
            }
            this.SendTCP(gSPacketIn);
        }
        public void SendPlayerCardEquip(PlayerInfo player, List<UsersCardInfo> cards)
        {
            if (this.m_gameClient.Player == null)
            {
                return;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(216, this.m_gameClient.Player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(player.ID);
            gSPacketIn.WriteInt(cards.Count);
            foreach (UsersCardInfo current in cards)
            {
                gSPacketIn.WriteInt(current.Place);
                gSPacketIn.WriteBoolean(true);
                gSPacketIn.WriteInt(current.CardID);
                gSPacketIn.WriteInt(current.CardType);
                gSPacketIn.WriteInt(current.UserID);
                gSPacketIn.WriteInt(current.Place);
                gSPacketIn.WriteInt(current.TemplateID);
                gSPacketIn.WriteBoolean(current.isFirstGet);
                gSPacketIn.WriteInt(current.Attack);
                gSPacketIn.WriteInt(current.Defence);
                gSPacketIn.WriteInt(current.Agility);
                gSPacketIn.WriteInt(current.Luck);
                gSPacketIn.WriteInt(current.Damage);
                gSPacketIn.WriteInt(current.Guard);
            }
            this.SendTCP(gSPacketIn);
        }
        public void SendPlayerCardInfo(CardInventory bag, int[] updatedSlots)
        {
            if (this.m_gameClient.Player == null)
            {
                return;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(216, this.m_gameClient.Player.PlayerCharacter.ID, 10480);
            gSPacketIn.WriteInt(this.m_gameClient.Player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(updatedSlots.Length);
            for (int i = 0; i < updatedSlots.Length; i++)
            {
                int num = updatedSlots[i];
                gSPacketIn.WriteInt(num);
                UsersCardInfo itemAt = bag.GetItemAt(num);
                if (itemAt.TemplateID == 0)
                {
                    gSPacketIn.WriteBoolean(false);
                }
                else
                {
                    gSPacketIn.WriteBoolean(true);
                    gSPacketIn.WriteInt(itemAt.CardID);
                    gSPacketIn.WriteInt(itemAt.CardType);
                    gSPacketIn.WriteInt(itemAt.UserID);
                    gSPacketIn.WriteInt(itemAt.Place);
                    gSPacketIn.WriteInt(itemAt.TemplateID);
                    gSPacketIn.WriteBoolean(itemAt.isFirstGet);
                    gSPacketIn.WriteInt(itemAt.Attack);
                    gSPacketIn.WriteInt(itemAt.Defence);
                    gSPacketIn.WriteInt(itemAt.Agility);
                    gSPacketIn.WriteInt(itemAt.Luck);
                    gSPacketIn.WriteInt(itemAt.Damage);
                    gSPacketIn.WriteInt(itemAt.Guard);
                }
            }
            this.SendTCP(gSPacketIn);
        }
        public GSPacketIn SendGetCard(PlayerInfo player, UsersCardInfo card)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(216, player.ID);
            gSPacketIn.WriteInt(player.ID);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(card.Place);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteInt(card.CardID);
            gSPacketIn.WriteInt(card.CardType);
            gSPacketIn.WriteInt(card.UserID);
            gSPacketIn.WriteInt(card.Place);
            gSPacketIn.WriteInt(card.TemplateID);
            gSPacketIn.WriteBoolean(card.isFirstGet);
            gSPacketIn.WriteInt(card.Attack);
            gSPacketIn.WriteInt(card.Defence);
            gSPacketIn.WriteInt(card.Agility);
            gSPacketIn.WriteInt(card.Luck);
            gSPacketIn.WriteInt(card.Damage);
            gSPacketIn.WriteInt(card.Guard);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public void SendPlayerCardSlot(PlayerInfo player, List<UsersCardInfo> cardslots)
        {
            if (this.m_gameClient.Player == null)
            {
                return;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(170, this.m_gameClient.Player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(player.ID);
            gSPacketIn.WriteInt(player.CardSoul);
            gSPacketIn.WriteInt(cardslots.Count);
            List<UsersCardInfo> list = new List<UsersCardInfo>();
            foreach (UsersCardInfo current in cardslots)
            {
                gSPacketIn.WriteInt(current.Place);
                gSPacketIn.WriteInt(current.Type);
                gSPacketIn.WriteInt(current.Level);
                gSPacketIn.WriteInt(current.CardGP);
                if (current.TemplateID > 0)
                {
                    list.Add(current);
                }
            }
            if (list.Count > 0)
            {
                this.SendPlayerCardEquip(player, list);
            }
            this.SendTCP(gSPacketIn);
        }
        public GSPacketIn SendGetPlayerCard(int playerId)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(18, playerId);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerCardSlot(PlayerInfo player, UsersCardInfo card)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(170, player.ID);
            gSPacketIn.WriteInt(player.ID);
            gSPacketIn.WriteInt(player.CardSoul);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(card.Place);
            gSPacketIn.WriteInt(card.Type);
            gSPacketIn.WriteInt(card.Level);
            gSPacketIn.WriteInt(card.CardGP);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerCardReset(PlayerInfo player, List<int> points)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(196, player.ID);
            gSPacketIn.WriteInt(points.Count);
            foreach (int current in points)
            {
                gSPacketIn.WriteInt(current);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerCardSoul(PlayerInfo player, bool isSoul, int soul)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(208, player.ID);
            gSPacketIn.WriteBoolean(isSoul);
            if (isSoul)
            {
                gSPacketIn.WriteInt(soul);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendUpdateUserPet(PetInventory bag, int[] slots)//__updatePet
        {
            if (this.m_gameClient.Player == null)
            {
                return null;
            }
            int iD = this.m_gameClient.Player.PlayerCharacter.ID;
            GSPacketIn gSPacketIn = new GSPacketIn(68, iD);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteInt(iD);
            gSPacketIn.WriteInt(slots.Length);
            for (int i = 0; i < slots.Length; i++)
            {
                int num = slots[i];
                gSPacketIn.WriteInt(num);
                UsersPetinfo petAt = bag.GetPetAt(num);
                if (petAt == null)
                {
                    gSPacketIn.WriteBoolean(false);
                }
                else
                {
                    gSPacketIn.WriteBoolean(true);
                    gSPacketIn.WriteInt(petAt.ID);
                    gSPacketIn.WriteInt(petAt.TemplateID);
                    gSPacketIn.WriteString(petAt.Name);
                    gSPacketIn.WriteInt(petAt.UserID);
                    gSPacketIn.WriteInt(petAt.Attack);
                    gSPacketIn.WriteInt(petAt.Defence);
                    gSPacketIn.WriteInt(petAt.Luck);
                    gSPacketIn.WriteInt(petAt.Agility);
                    gSPacketIn.WriteInt(petAt.Blood);
                    gSPacketIn.WriteInt(petAt.Damage);
                    gSPacketIn.WriteInt(petAt.Guard);
                    gSPacketIn.WriteInt(petAt.AttackGrow);
                    gSPacketIn.WriteInt(petAt.DefenceGrow);
                    gSPacketIn.WriteInt(petAt.LuckGrow);
                    gSPacketIn.WriteInt(petAt.AgilityGrow);
                    gSPacketIn.WriteInt(petAt.BloodGrow);
                    gSPacketIn.WriteInt(petAt.DamageGrow);
                    gSPacketIn.WriteInt(petAt.GuardGrow);
                    gSPacketIn.WriteInt(petAt.Level);
                    gSPacketIn.WriteInt(petAt.GP);
                    gSPacketIn.WriteInt(petAt.MaxGP);
                    gSPacketIn.WriteInt(petAt.Hunger);
                    gSPacketIn.WriteInt(petAt.PetHappyStar);
                    gSPacketIn.WriteInt(petAt.MP);
                    List<string> skill = petAt.GetSkill();
                    List<string> skillEquip = petAt.GetSkillEquip();
                    gSPacketIn.WriteInt(skill.Count);
                    foreach (string current in skill)
                    {
                        gSPacketIn.WriteInt(int.Parse(current.Split(new char[]
						{
							','
						})[0]));
                        gSPacketIn.WriteInt(int.Parse(current.Split(new char[]
						{
							','
						})[1]));
                    }
                    gSPacketIn.WriteInt(skillEquip.Count);
                    foreach (string current2 in skillEquip)
                    {
                        gSPacketIn.WriteInt(int.Parse(current2.Split(new char[]
						{
							','
						})[1]));
                        gSPacketIn.WriteInt(int.Parse(current2.Split(new char[]
						{
							','
						})[0]));
                    }
                    gSPacketIn.WriteBoolean(petAt.IsEquip);
                }
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendUpdatePetInfo(PlayerInfo info, UsersPetinfo[] pets)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(68, info.ID);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteInt(info.ID);
            gSPacketIn.WriteInt(pets.Length);
            for (int i = 0; i < pets.Length; i++)
            {
                gSPacketIn.WriteInt(i);
                UsersPetinfo usersPetinfo = pets[i];
                if (usersPetinfo == null)
                {
                    gSPacketIn.WriteBoolean(false);
                }
                else
                {
                    gSPacketIn.WriteBoolean(true);
                    gSPacketIn.WriteInt(usersPetinfo.ID);
                    gSPacketIn.WriteInt(usersPetinfo.TemplateID);
                    gSPacketIn.WriteString(usersPetinfo.Name);
                    gSPacketIn.WriteInt(usersPetinfo.UserID);
                    gSPacketIn.WriteInt(usersPetinfo.Attack);
                    gSPacketIn.WriteInt(usersPetinfo.Defence);
                    gSPacketIn.WriteInt(usersPetinfo.Luck);
                    gSPacketIn.WriteInt(usersPetinfo.Agility);
                    gSPacketIn.WriteInt(usersPetinfo.Blood);
                    gSPacketIn.WriteInt(usersPetinfo.Damage);
                    gSPacketIn.WriteInt(usersPetinfo.Guard);
                    gSPacketIn.WriteInt(usersPetinfo.AttackGrow);
                    gSPacketIn.WriteInt(usersPetinfo.DefenceGrow);
                    gSPacketIn.WriteInt(usersPetinfo.LuckGrow);
                    gSPacketIn.WriteInt(usersPetinfo.AgilityGrow);
                    gSPacketIn.WriteInt(usersPetinfo.BloodGrow);
                    gSPacketIn.WriteInt(usersPetinfo.DamageGrow);
                    gSPacketIn.WriteInt(usersPetinfo.GuardGrow);
                    gSPacketIn.WriteInt(usersPetinfo.Level);
                    gSPacketIn.WriteInt(usersPetinfo.GP);
                    gSPacketIn.WriteInt(usersPetinfo.MaxGP);
                    gSPacketIn.WriteInt(usersPetinfo.Hunger);
                    gSPacketIn.WriteInt(usersPetinfo.PetHappyStar);
                    gSPacketIn.WriteInt(usersPetinfo.MP);
                    List<string> skill = usersPetinfo.GetSkill();
                    List<string> skillEquip = usersPetinfo.GetSkillEquip();
                    gSPacketIn.WriteInt(skill.Count);
                    foreach (string current in skill)
                    {
                        gSPacketIn.WriteInt(int.Parse(current.Split(new char[]
						{
							','
						})[0]));
                        gSPacketIn.WriteInt(int.Parse(current.Split(new char[]
						{
							','
						})[1]));
                    }
                    gSPacketIn.WriteInt(skillEquip.Count);
                    foreach (string current2 in skillEquip)
                    {
                        gSPacketIn.WriteInt(int.Parse(current2.Split(new char[]
						{
							','
						})[1]));
                        gSPacketIn.WriteInt(int.Parse(current2.Split(new char[]
						{
							','
						})[0]));
                    }
                    gSPacketIn.WriteBoolean(usersPetinfo.IsEquip);
                }
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendRefreshPet(GamePlayer player, UsersPetinfo[] pets, ItemInfo[] items, bool refreshBtn)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(68, player.PlayerCharacter.ID);
            gSPacketIn.WriteByte(5);
            int val = 10;
            int val2 = 10;
            int val3 = 100;
            if (!player.PlayerCharacter.IsFistGetPet)
            {
                gSPacketIn.WriteBoolean(refreshBtn);
                gSPacketIn.WriteInt(pets.Length);
                for (int i = 0; i < pets.Length; i++)
                {
                    UsersPetinfo usersPetinfo = pets[i];
                    gSPacketIn.WriteInt(usersPetinfo.Place);
                    gSPacketIn.WriteInt(usersPetinfo.TemplateID);
                    gSPacketIn.WriteString(usersPetinfo.Name);
                    gSPacketIn.WriteInt(usersPetinfo.Attack);
                    gSPacketIn.WriteInt(usersPetinfo.Defence);
                    gSPacketIn.WriteInt(usersPetinfo.Luck);
                    gSPacketIn.WriteInt(usersPetinfo.Agility);
                    gSPacketIn.WriteInt(usersPetinfo.Blood);
                    gSPacketIn.WriteInt(usersPetinfo.Damage);
                    gSPacketIn.WriteInt(usersPetinfo.GuardGrow);
                    gSPacketIn.WriteInt(usersPetinfo.AttackGrow);
                    gSPacketIn.WriteInt(usersPetinfo.DefenceGrow);
                    gSPacketIn.WriteInt(usersPetinfo.LuckGrow);
                    gSPacketIn.WriteInt(usersPetinfo.AgilityGrow);
                    gSPacketIn.WriteInt(usersPetinfo.BloodGrow);
                    gSPacketIn.WriteInt(usersPetinfo.DamageGrow);
                    gSPacketIn.WriteInt(usersPetinfo.GuardGrow);
                    gSPacketIn.WriteInt(usersPetinfo.Level);
                    gSPacketIn.WriteInt(usersPetinfo.GP);
                    gSPacketIn.WriteInt(usersPetinfo.MaxGP);
                    gSPacketIn.WriteInt(usersPetinfo.Hunger);
                    gSPacketIn.WriteInt(usersPetinfo.MP);
                    List<string> skill = usersPetinfo.GetSkill();
                    gSPacketIn.WriteInt(skill.Count);
                    foreach (string current in skill)
                    {
                        gSPacketIn.WriteInt(int.Parse(current.Split(new char[]
						{
							','
						})[0]));
                        gSPacketIn.WriteInt(int.Parse(current.Split(new char[]
						{
							','
						})[1]));
                    }
                    gSPacketIn.WriteInt(val);
                    gSPacketIn.WriteInt(val2);
                    gSPacketIn.WriteInt(val3);
                }
                if (items != null)
                {
                    gSPacketIn.WriteInt(items.Length);
                    for (int j = 0; j < items.Length; j++)
                    {
                        ItemInfo itemInfo = items[j];
                        gSPacketIn.WriteInt(itemInfo.Place);
                        gSPacketIn.WriteInt(itemInfo.TemplateID);
                        gSPacketIn.WriteInt(itemInfo.Count);
                    }
                }
                else
                {
                    gSPacketIn.WriteInt(0);
                }
            }
            else
            {
                gSPacketIn.WriteBoolean(refreshBtn);
                gSPacketIn.WriteInt(pets.Length);
                for (int k = 0; k < pets.Length; k++)
                {
                    UsersPetinfo usersPetinfo2 = pets[k];
                    gSPacketIn.WriteInt(usersPetinfo2.Place);
                    gSPacketIn.WriteInt(usersPetinfo2.TemplateID);
                    gSPacketIn.WriteString(usersPetinfo2.Name);
                    gSPacketIn.WriteInt(usersPetinfo2.Attack);
                    gSPacketIn.WriteInt(usersPetinfo2.Defence);
                    gSPacketIn.WriteInt(usersPetinfo2.Luck);
                    gSPacketIn.WriteInt(usersPetinfo2.Agility);
                    gSPacketIn.WriteInt(usersPetinfo2.Blood);
                    gSPacketIn.WriteInt(usersPetinfo2.Damage);
                    gSPacketIn.WriteInt(usersPetinfo2.GuardGrow);
                    gSPacketIn.WriteInt(usersPetinfo2.AttackGrow);
                    gSPacketIn.WriteInt(usersPetinfo2.DefenceGrow);
                    gSPacketIn.WriteInt(usersPetinfo2.LuckGrow);
                    gSPacketIn.WriteInt(usersPetinfo2.AgilityGrow);
                    gSPacketIn.WriteInt(usersPetinfo2.BloodGrow);
                    gSPacketIn.WriteInt(usersPetinfo2.DamageGrow);
                    gSPacketIn.WriteInt(usersPetinfo2.GuardGrow);
                    gSPacketIn.WriteInt(usersPetinfo2.Level);
                    gSPacketIn.WriteInt(usersPetinfo2.GP);
                    gSPacketIn.WriteInt(usersPetinfo2.MaxGP);
                    gSPacketIn.WriteInt(usersPetinfo2.Hunger);
                    gSPacketIn.WriteInt(usersPetinfo2.MP);
                    List<string> skill2 = usersPetinfo2.GetSkill();
                    gSPacketIn.WriteInt(skill2.Count);
                    foreach (string current2 in skill2)
                    {
                        gSPacketIn.WriteInt(int.Parse(current2.Split(new char[]
						{
							','
						})[0]));
                        gSPacketIn.WriteInt(int.Parse(current2.Split(new char[]
						{
							','
						})[1]));
                    }
                    gSPacketIn.WriteInt(val);
                    gSPacketIn.WriteInt(val2);
                    gSPacketIn.WriteInt(val3);
                }
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendAddFriend(PlayerInfo user, int relation, bool state)//__friendAdd
        {
            GSPacketIn gSPacketIn = new GSPacketIn(160, user.ID);
            gSPacketIn.WriteByte(160);
            gSPacketIn.WriteBoolean(state);
            gSPacketIn.WriteInt(user.ID);
            gSPacketIn.WriteString(user.NickName);
            gSPacketIn.WriteByte(user.typeVIP);
            gSPacketIn.WriteInt(user.VIPLevel);
            gSPacketIn.WriteBoolean(user.Sex);
            gSPacketIn.WriteString(user.Style);
            gSPacketIn.WriteString(user.Colors);
            gSPacketIn.WriteString(user.Skin);
            gSPacketIn.WriteInt((user.State == 1) ? 1 : 0);
            gSPacketIn.WriteInt(user.Grade);
            gSPacketIn.WriteInt(user.Hide);
            gSPacketIn.WriteString(user.ConsortiaName);
            gSPacketIn.WriteInt(user.Total);
            gSPacketIn.WriteInt(user.Escape);
            gSPacketIn.WriteInt(user.Win);
            gSPacketIn.WriteInt(user.Offer);
            gSPacketIn.WriteInt(user.Repute);
            gSPacketIn.WriteInt(relation);
            gSPacketIn.WriteString(user.UserName);
            gSPacketIn.WriteInt(user.Nimbus);
            gSPacketIn.WriteInt(user.FightPower);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteString("");
            gSPacketIn.WriteInt(user.AchievementPoint);
            gSPacketIn.WriteInt(user.AchievementPoint);
            gSPacketIn.WriteBoolean(user.IsMarried);
            gSPacketIn.WriteBoolean(user.IsOldPlayer);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendFriendRemove(int FriendID)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(160, FriendID);
            gSPacketIn.WriteByte(161);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendFriendState(int playerID, int state, byte typeVip, int viplevel)//__playerState
        {
            GSPacketIn gSPacketIn = new GSPacketIn(160, playerID);
            gSPacketIn.WriteByte(165);
            gSPacketIn.WriteInt(state);
            gSPacketIn.WriteInt((int)typeVip);
            gSPacketIn.WriteInt(viplevel);
            gSPacketIn.WriteBoolean(true);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendUpdateAllData(PlayerInfo player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(142, player.ID);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendGetSpree(PlayerInfo player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(157, player.ID);
            gSPacketIn.WriteBoolean(true);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendUpdateUpCount(PlayerInfo player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(96, player.ID);
            gSPacketIn.WriteInt(player.MaxBuyHonor);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerRefreshTotem(PlayerInfo player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(136, player.ID);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(player.myHonor);
            gSPacketIn.WriteInt(player.totemId);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendLabyrinthUpdataInfo(int ID)//__onUpdataInfo
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.LABYRINTH, ID);
            pkg.WriteByte((byte)LabyrinthPackageType.REQUEST_UPDATE);
            pkg.WriteInt(0);//_model.myProgress = _loc_2.readInt();
            pkg.WriteInt(1);//_model.currentFloor = _loc_2.readInt();
            pkg.WriteBoolean(false);//_model.completeChallenge = _loc_2.readBoolean();
            pkg.WriteInt(30);//_model.remainTime = _loc_2.readInt();
            pkg.WriteInt(0);//_model.accumulateExp = _loc_2.readInt();
            pkg.WriteInt(0);//_model.cleanOutAllTime = _loc_2.readInt();
            pkg.WriteInt(0);//_model.cleanOutGold = _loc_2.readInt();
            pkg.WriteInt(0);//_model.myRanking = _loc_2.readInt();
            pkg.WriteBoolean(true);//_model.isDoubleAward = _loc_2.readBoolean();
            pkg.WriteBoolean(false);//_model.isInGame = _loc_2.readBoolean();
            pkg.WriteBoolean(false);//_model.isCleanOut = _loc_2.readBoolean();
            pkg.WriteBoolean(true);//this._model.serverMultiplyingPower = _local2.readBoolean();
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn SendPlayerFigSpiritinit(int ID, List<UserGemStone> gems)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(209, ID);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteInt(gems.Count);
            foreach (UserGemStone current in gems)
            {
                gSPacketIn.WriteInt(current.UserID);
                gSPacketIn.WriteInt(current.FigSpiritId);
                gSPacketIn.WriteString(current.FigSpiritIdValue);
                gSPacketIn.WriteInt(current.EquipPlace);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerFigSpiritUp(int ID, UserGemStone gem, bool isUp, bool isMaxLevel, bool isFall, int num, int dir)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(209, ID);
            gSPacketIn.WriteByte(2);
            string[] array = gem.FigSpiritIdValue.Split(new char[]
			{
				'|'
			});
            gSPacketIn.WriteBoolean(isUp);
            gSPacketIn.WriteBoolean(isMaxLevel);
            gSPacketIn.WriteBoolean(isFall);
            gSPacketIn.WriteInt(num);
            gSPacketIn.WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                gSPacketIn.WriteInt(gem.FigSpiritId);
                gSPacketIn.WriteInt(Convert.ToInt32(text.Split(new char[]
				{
					','
				})[0]));
                gSPacketIn.WriteInt(Convert.ToInt32(text.Split(new char[]
				{
					','
				})[1]));
                gSPacketIn.WriteInt(Convert.ToInt32(text.Split(new char[]
				{
					','
				})[2]));
            }
            gSPacketIn.WriteInt(gem.EquipPlace);
            gSPacketIn.WriteInt(dir);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendTrusteeshipStart(int ID)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(140, ID);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(1);
            for (int i = 0; i < 1; i++)
            {
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteDateTime(DateTime.Now);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendDiceActiveOpen(int ID)//initialize
        {
            GSPacketIn gSPacketIn = new GSPacketIn(96, ID);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteInt(1);//this._model.freeCount = param1.readInt();
            gSPacketIn.WriteInt(1);//this._model.refreshPrice = param1.readInt();
            gSPacketIn.WriteInt(1);//this._model.commonDicePrice = param1.readInt();
            gSPacketIn.WriteInt(2);//this._model.doubleDicePrice = param1.readInt();
            gSPacketIn.WriteInt(33);//this._model.bigDicePrice = param1.readInt();
            gSPacketIn.WriteInt(1);//this._model.smallDicePrice = param1.readInt();
            gSPacketIn.WriteInt(1);//this._model.MAX_LEVEL = param1.readInt();
            gSPacketIn.WriteInt(0);//_loc_2 = param1.readInt();
            gSPacketIn.WriteInt(1);//_loc_3 = param1.readInt();
            for (int i = 1; i < 20; i++)
            {
                gSPacketIn.WriteInt(7048);//_loc_4 = _loc_4 + ("," + param1.readInt() + "|" + param1.readInt());
                gSPacketIn.WriteInt(i);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }

        public GSPacketIn SendActivityList(int ID)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(155, ID);
            gSPacketIn.WriteByte(1);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            gSPacketIn.WriteInt(0);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerDrill(int ID, Dictionary<int, UserDrillInfo> drills)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(121, ID);
            gSPacketIn.WriteByte(6);
            gSPacketIn.WriteInt(ID);
            gSPacketIn.WriteInt(drills[0].HoleExp);
            gSPacketIn.WriteInt(drills[1].HoleExp);
            gSPacketIn.WriteInt(drills[2].HoleExp);
            gSPacketIn.WriteInt(drills[3].HoleExp);
            gSPacketIn.WriteInt(drills[4].HoleExp);
            gSPacketIn.WriteInt(drills[5].HoleExp);
            gSPacketIn.WriteInt(drills[0].HoleLv);
            gSPacketIn.WriteInt(drills[1].HoleLv);
            gSPacketIn.WriteInt(drills[2].HoleLv);
            gSPacketIn.WriteInt(drills[3].HoleLv);
            gSPacketIn.WriteInt(drills[4].HoleLv);
            gSPacketIn.WriteInt(drills[5].HoleLv);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] infos)
        {
            if (player == null || states == null || infos == null)
            {
                return null;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(178, player.PlayerCharacter.ID, 20480);
            gSPacketIn.WriteInt(infos.Length);
            for (int i = 0; i < infos.Length; i++)
            {
                BaseQuest baseQuest = infos[i];
                gSPacketIn.WriteInt(baseQuest.Data.QuestID);
                gSPacketIn.WriteBoolean(baseQuest.Data.IsComplete);
                gSPacketIn.WriteInt(baseQuest.Data.Condition1);
                gSPacketIn.WriteInt(baseQuest.Data.Condition2);
                gSPacketIn.WriteInt(baseQuest.Data.Condition3);
                gSPacketIn.WriteInt(baseQuest.Data.Condition4);
                gSPacketIn.WriteDateTime(baseQuest.Data.CompletedDate);
                gSPacketIn.WriteInt(baseQuest.Data.RepeatFinish);
                gSPacketIn.WriteInt(baseQuest.Data.RandDobule);
                gSPacketIn.WriteBoolean(baseQuest.Data.IsExist);
                gSPacketIn.WriteInt(baseQuest.Data.QuestLevel);
            }
            gSPacketIn.Write(states);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos)//__buffObtain or __buffUpdate
        {
            GSPacketIn gSPacketIn = new GSPacketIn(185, player.PlayerId);
            gSPacketIn.WriteInt(infos.Length);
            for (int i = 0; i < infos.Length; i++)
            {
                BufferInfo bufferInfo = infos[i];
                gSPacketIn.WriteInt(bufferInfo.Type);//_loc_5 = _loc_2.readInt();
                gSPacketIn.WriteBoolean(bufferInfo.IsExist);//_loc_6 = _loc_2.readBoolean();
                gSPacketIn.WriteDateTime(bufferInfo.BeginDate);// _loc_7 = _loc_2.readDate();
                gSPacketIn.WriteInt(bufferInfo.ValidDate);//_loc_8 = _loc_2.readInt();
                gSPacketIn.WriteInt(bufferInfo.Value);//_loc_9 = _loc_2.readInt();
                gSPacketIn.WriteInt(bufferInfo.ValidCount);//_loc_10 = _loc_2.readInt();
                gSPacketIn.WriteInt(0);//_loc_11 = _loc_2.readInt(); 
                
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(186, player.PlayerId);
            gSPacketIn.WriteInt(infos.Count);
            foreach (AbstractBuffer current in infos)
            {
                BufferInfo info = current.Info;
                gSPacketIn.WriteInt(info.Type);
                gSPacketIn.WriteBoolean(info.IsExist);
                gSPacketIn.WriteDateTime(info.BeginDate);
                gSPacketIn.WriteInt(info.ValidDate);
                gSPacketIn.WriteInt(info.Value);
                gSPacketIn.WriteInt(info.ValidCount);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(117);
            gSPacketIn.WriteInt(playerID);
            gSPacketIn.WriteInt((int)type);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item)//__updateAuction
        {
            GSPacketIn gSPacketIn = new GSPacketIn(195);
            gSPacketIn.WriteInt(auctionID);
            gSPacketIn.WriteBoolean(isExist);
            if (isExist)
            {
                gSPacketIn.WriteInt(info.AuctioneerID);
                gSPacketIn.WriteString(info.AuctioneerName);
                gSPacketIn.WriteDateTime(info.BeginDate);
                gSPacketIn.WriteInt(info.BuyerID);
                gSPacketIn.WriteString(info.BuyerName);
                gSPacketIn.WriteInt(info.ItemID);
                gSPacketIn.WriteInt(info.Mouthful);
                gSPacketIn.WriteInt(info.PayType);
                gSPacketIn.WriteInt(info.Price);
                gSPacketIn.WriteInt(info.Rise);
                gSPacketIn.WriteInt(info.ValidDate);
                gSPacketIn.WriteBoolean(item != null);
                if (item != null)
                {
                    gSPacketIn.WriteInt(item.Count);
                    gSPacketIn.WriteInt(item.TemplateID);
                    gSPacketIn.WriteInt(item.AttackCompose);
                    gSPacketIn.WriteInt(item.DefendCompose);
                    gSPacketIn.WriteInt(item.AgilityCompose);
                    gSPacketIn.WriteInt(item.LuckCompose);
                    gSPacketIn.WriteInt(item.StrengthenLevel);
                    gSPacketIn.WriteBoolean(item.IsBinds);
                    gSPacketIn.WriteBoolean(item.IsJudge);
                    gSPacketIn.WriteDateTime(item.BeginDate);
                    gSPacketIn.WriteInt(item.ValidDate);
                    gSPacketIn.WriteString(item.Color);
                    gSPacketIn.WriteString(item.Skin);
                    gSPacketIn.WriteBoolean(item.IsUsed);
                    gSPacketIn.WriteInt(item.Hole1);
                    gSPacketIn.WriteInt(item.Hole2);
                    gSPacketIn.WriteInt(item.Hole3);
                    gSPacketIn.WriteInt(item.Hole4);
                    gSPacketIn.WriteInt(item.Hole5);
                    gSPacketIn.WriteInt(item.Hole6);
                    gSPacketIn.WriteString(item.Template.Pic);
                    gSPacketIn.WriteInt(item.Template.RefineryLevel);
                    gSPacketIn.WriteDateTime(DateTime.Now);
                    gSPacketIn.WriteByte((byte)item.Hole5Level);
                    gSPacketIn.WriteInt(item.Hole5Exp);
                    gSPacketIn.WriteByte((byte)item.Hole6Level);
                    gSPacketIn.WriteInt(item.Hole6Exp);
                }
            }
            gSPacketIn.Compress();
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendAASState(bool result)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(224);
            gSPacketIn.WriteBoolean(result);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendIDNumberCheck(bool result)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(226);
            gSPacketIn.WriteBoolean(result);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendAASInfoSet(bool result)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(224);
            gSPacketIn.WriteBoolean(result);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(227);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteBoolean(true);
            gSPacketIn.WriteBoolean(IsMinor);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(241, player.PlayerCharacter.ID);
            bool flag = room != null;
            gSPacketIn.WriteBoolean(flag);
            if (flag)
            {
                gSPacketIn.WriteInt(room.Info.ID);
                gSPacketIn.WriteBoolean(room.Info.IsHymeneal);
                gSPacketIn.WriteString(room.Info.Name);
                gSPacketIn.WriteBoolean(!(room.Info.Pwd == ""));
                gSPacketIn.WriteInt(room.Info.MapIndex);
                gSPacketIn.WriteInt(room.Info.AvailTime);
                gSPacketIn.WriteInt(room.Count);
                gSPacketIn.WriteInt(room.Info.PlayerID);
                gSPacketIn.WriteString(room.Info.PlayerName);
                gSPacketIn.WriteInt(room.Info.GroomID);
                gSPacketIn.WriteString(room.Info.GroomName);
                gSPacketIn.WriteInt(room.Info.BrideID);
                gSPacketIn.WriteString(room.Info.BrideName);
                gSPacketIn.WriteDateTime(room.Info.BeginTime);
                gSPacketIn.WriteByte((byte)room.RoomState);
                gSPacketIn.WriteString(room.Info.RoomIntroduction);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(242, player.PlayerCharacter.ID);
            gSPacketIn.WriteBoolean(result);
            if (result)
            {
                gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.ID);
                gSPacketIn.WriteString(player.CurrentMarryRoom.Info.Name);
                gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.MapIndex);
                gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.AvailTime);
                gSPacketIn.WriteInt(player.CurrentMarryRoom.Count);
                gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.PlayerID);
                gSPacketIn.WriteString(player.CurrentMarryRoom.Info.PlayerName);
                gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.GroomID);
                gSPacketIn.WriteString(player.CurrentMarryRoom.Info.GroomName);
                gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.BrideID);
                gSPacketIn.WriteString(player.CurrentMarryRoom.Info.BrideName);
                gSPacketIn.WriteDateTime(player.CurrentMarryRoom.Info.BeginTime);
                gSPacketIn.WriteBoolean(player.CurrentMarryRoom.Info.IsHymeneal);
                gSPacketIn.WriteByte((byte)player.CurrentMarryRoom.RoomState);
                gSPacketIn.WriteString(player.CurrentMarryRoom.Info.RoomIntroduction);
                gSPacketIn.WriteBoolean(player.CurrentMarryRoom.Info.GuestInvite);
                gSPacketIn.WriteInt(player.MarryMap);
                gSPacketIn.WriteBoolean(player.CurrentMarryRoom.Info.IsGunsaluteUsed);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(243, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
            gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
            gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
            gSPacketIn.WriteInt(player.PlayerCharacter.ID);
            gSPacketIn.WriteString(player.PlayerCharacter.NickName);
            gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
            gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
            gSPacketIn.WriteString(player.PlayerCharacter.Style);
            gSPacketIn.WriteString(player.PlayerCharacter.Colors);
            gSPacketIn.WriteString(player.PlayerCharacter.Skin);
            gSPacketIn.WriteInt(player.X);
            gSPacketIn.WriteInt(player.Y);
            gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
            gSPacketIn.WriteInt(player.PlayerCharacter.Win);
            gSPacketIn.WriteInt(player.PlayerCharacter.Total);
            gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.IsOldPlayer);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(239);
            gSPacketIn.WriteInt(ID);
            gSPacketIn.WriteBoolean(isExist);
            if (isExist)
            {
                gSPacketIn.WriteInt(info.UserID);
                gSPacketIn.WriteBoolean(info.IsPublishEquip);
                gSPacketIn.WriteString(info.Introduction);
            }
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(246, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(userID);
            gSPacketIn.WriteBoolean(isMarried);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int id)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(247, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(userID);
            gSPacketIn.WriteString(userName);
            gSPacketIn.WriteString(loveProclamation);
            gSPacketIn.WriteInt(id);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }

        public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(248, player.PlayerCharacter.ID);
            gSPacketIn.WriteBoolean(result);
            gSPacketIn.WriteBoolean(isProposer);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }

        public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int id)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(250, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(UserID);
            gSPacketIn.WriteBoolean(result);
            gSPacketIn.WriteString(UserName);
            gSPacketIn.WriteBoolean(isApplicant);
            gSPacketIn.WriteInt(id);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }

        public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(244, player.PlayerCharacter.ID);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }

        public GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(252, player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(info.ID);
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
            return gSPacketIn;
        }

        public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(235, player.PlayerCharacter.ID);
            gSPacketIn.WriteString(info.Introduction);
            gSPacketIn.WriteBoolean(info.IsPublishEquip);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }

        public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(249, player.PlayerCharacter.ID);
            gSPacketIn.WriteByte(3);
            gSPacketIn.WriteInt(info.AvailTime);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }

        public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(234, player.PlayerCharacter.ID);
            gSPacketIn.WriteBoolean(info.IsMarried);
            gSPacketIn.WriteInt(info.SpouseID);
            gSPacketIn.WriteString(info.SpouseName);
            gSPacketIn.WriteBoolean(info.IsCreatedMarryRoom);
            gSPacketIn.WriteInt(info.SelfMarryRoomID);
            gSPacketIn.WriteBoolean(info.IsGotRing);
            this.SendTCP(gSPacketIn);
            return gSPacketIn;
        }
        #region Consortia
        public GSPacketIn sendConsortiaTryIn(int id, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_TRYIN);
            pkg.WriteInt(id);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn sendConsortiaTryInDel(int id, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_TRYIN_DEL);
            pkg.WriteInt(id);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn sendConsortiaTryInPass(int id, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_TRYIN_PASS);
            pkg.WriteInt(id);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        //CONSORTIA_DISBAND
        public GSPacketIn SendConsortiaInvite(string username, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_INVITE);
            pkg.WriteString(username);     
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);       
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn sendConsortiaInvitePass(int id, bool result, int consortiaid, string consortianame, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_INVITE_PASS);
            pkg.WriteInt(id);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn SendConsortiaCreate(string name1, bool result, int id, string name2, string msg, int dutyLevel, string DutyName, int dutyRight, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_CREATE);
            pkg.WriteString(name1);
            pkg.WriteBoolean(result);
            pkg.WriteInt(id);
            pkg.WriteString(name2);
            pkg.WriteString(msg);
            pkg.WriteInt(dutyLevel);
            pkg.WriteString(DutyName == null ? "" : DutyName);
            pkg.WriteInt(dutyRight);
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn sendConsortiaUpdatePlacard(string description, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_PLACARD_UPDATE);
            pkg.WriteString(description);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn sendConsortiaEquipConstrol(bool result, List<int> Riches, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_EQUIP_CONTROL);
            pkg.WriteBoolean(result);
            int i = 0;
            while (i < Riches.Count)
            {
                pkg.WriteInt(Riches[i]);
                i++;
            }
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn SendConsortia(int money, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_RICHES_OFFER);
            pkg.WriteInt(money);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        //CONSORTIA_RESPONSE
        public GSPacketIn sendConsortiaOut(int id, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_RENEGADE);
            pkg.WriteInt(id);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn SendConsortiaLevelUp(byte type, byte level, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_LEVEL_UP);
            pkg.WriteByte(type);
            pkg.WriteByte(level);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        //CONSORTIA_CHAIRMAN_CHAHGE
        public GSPacketIn SendConsortiaMemberGrade(int id, bool update, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_USER_GRADE_UPDATE);
            pkg.WriteInt(id);
            pkg.WriteBoolean(update);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        public GSPacketIn sendConsortiaChangeChairman(string nick, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_DESCRIPTION_UPDATE);
            pkg.WriteString(nick);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }
        //SKILL_SOCKET
        //CONSORTIA_MAIL_MESSAGE

        public GSPacketIn sendBuyBadge(int BadgeID, int ValidDate, bool result, string BadgeBuyTime, int playerid)//__buyBadgeHandler
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.BUY_BADGE);
            pkg.WriteInt(BadgeID);//var _loc_3:* = _loc_2.readInt();BadgeID
            pkg.WriteInt(BadgeID);//var _loc_4:* = _loc_2.readInt();BadgeID
            pkg.WriteInt(ValidDate);//var _loc_5:* = _loc_2.readInt();ValidDate
            pkg.WriteDateTime(Convert.ToDateTime(BadgeBuyTime));//var _loc_6:* = _loc_2.readDate();BadgeBuyTime
            pkg.WriteBoolean(result);//var _loc_7:* = _loc_2.readBoolean();
            SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn SendbossOpenCloseHandler(int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.BOSS_OPEN_CLOSE);
            pkg.WriteByte(0);
            SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn bossOpenCloseHandler()
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD);
            gSPacketIn.WriteByte(0); 
            SendTCP(gSPacketIn);
            return gSPacketIn;
        }

        #endregion

        public GSPacketIn sendConsortiaUpdateDescription(string description, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_DESCRIPTION_UPDATE);
            pkg.WriteString(description);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn SendConsortiaRichesOffer(int money, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_RICHES_OFFER);
            pkg.WriteInt(money);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn sendConsortiaInviteDel(int id, bool result, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_INVITE_DELETE);
            pkg.WriteInt(id);
            pkg.WriteBoolean(result);
            pkg.WriteString(msg);
            SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn sendConsortiaApplyStatusOut(bool state, bool result, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTIA_APPLY_STATE);
            pkg.WriteBoolean(result);
            pkg.WriteBoolean(result);
            SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn sendOneOnOneTalk(int receiverID, bool isAutoReply, string SenderNickName, string msg, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.IM_CMD, playerid);
            pkg.WriteByte((byte)IMPackageType.ONE_ON_ONE_TALK);
            pkg.WriteInt(receiverID);//var _loc_3:* = _loc_2.readInt();receiverID
            pkg.WriteString(SenderNickName);//var _loc_4:* = _loc_2.readUTF();sender NickName
            pkg.WriteDateTime(DateTime.Now);//var _loc_5:* = _loc_2.readDate();send date
            pkg.WriteString(msg);//var _loc_6:* = _loc_2.readUTF();msg
            pkg.WriteBoolean(isAutoReply);//var _loc_7:* = _loc_2.readBoolean();isAutoReply
            SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn SendConsortiaMail(bool result, int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_CMD, playerid);
            pkg.WriteByte((byte)ConsortiaPackageType.CONSORTION_MAIL);
            pkg.WriteBoolean(result);
            SendTCP(pkg);
            return pkg;
        }

        //new
        public GSPacketIn SendOpenChickenBox(int ID)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.NEWCHICKENBOX_SYS, ID);
            gSPacketIn.WriteByte((byte)NewChickenBoxPackageType.CHICKENBOXOPEN);
            gSPacketIn.WriteInt(15);//this._model.canOpenCounts = _loc_2.readInt();
            gSPacketIn.WriteInt(15);//this._model.openCardPrice.push(_loc_2.readInt());
            gSPacketIn.WriteInt(1);//this._model.canEagleEyeCounts = _loc_2.readInt();
            gSPacketIn.WriteInt(1);//this._model.eagleEyePrice.push(_loc_2.readInt()); 
            gSPacketIn.WriteInt(1);//this._model.flushPrice = _loc_2.readInt();
            return gSPacketIn;
        }

        public void SendUpdateUerGuild(int ID)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.USER_ANSWER, ID);
            gSPacketIn.WriteInt(0);//var _loc_3:* = event.pkg.readInt();
            gSPacketIn.WriteByte(0);//_loc_5 = event.pkg.readByte();
            SendTCP(gSPacketIn);
        }

        public void SendsysNotice(int ID)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.SYS_NOTICE, ID);
            gSPacketIn.WriteInt(0);//var _loc_3:* = _loc_2.readInt();
            gSPacketIn.WriteString("SendSysNotice");//var _loc_4:* = _loc_2.readUTF();
            gSPacketIn.WriteByte(0);//var _loc_5:* = _loc_2.readByte();
            gSPacketIn.WriteInt(0);//var _loc_6:* = _loc_2.readInt();
            gSPacketIn.WriteInt(0);//var _loc_7:* = _loc_2.readInt();
            gSPacketIn.WriteInt(0);//var _loc_8:* = _loc_2.readInt();
            gSPacketIn.WriteString("SendSysNotice");//
            SendTCP(gSPacketIn);
        }

        public void SendgetGifts(int ID, ItemInfo item)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.USER_GET_GIFTS, ID);
            gSPacketIn.WriteInt(0);//var _loc_4:* = _loc_3.readInt();
            gSPacketIn.WriteInt(0);//this._self.charmGP = _loc_3.readInt();
            gSPacketIn.WriteInt(0);//_loc_2 = _loc_3.readInt();
            gSPacketIn.WriteInt(item.TemplateID);//_loc_8.TemplateID = _loc_3.readInt();
            gSPacketIn.WriteInt(5);//_loc_8.amount = _loc_3.readInt();//soluong
            SendTCP(gSPacketIn);
        }
    }
}
