using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
    [PacketHandler(129, "公会聊天")]
    public class ConsortiaHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = packet.ReadInt();
            bool flag = false;
            string text = "Packet Error!";
            WorldMgr.GetAllPlayers();
            switch (num)
            {
                case 0:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID != 0)
                        {
                            return 0;
                        }
                        int num2 = packet.ReadInt();
                        text = "ConsortiaApplyLoginHandler.ADD_Failed";
                        using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness.AddConsortiaApplyUsers(new ConsortiaApplyUserInfo
                            {
                                ApplyDate = DateTime.Now,
                                ConsortiaID = num2,
                                ConsortiaName = "",
                                IsExist = true,
                                Remark = "",
                                UserID = client.Player.PlayerCharacter.ID,
                                UserName = client.Player.PlayerCharacter.NickName
                            }, ref text))
                            {
                                text = ((num2 != 0) ? "ConsortiaApplyLoginHandler.ADD_Success" : "ConsortiaApplyLoginHandler.DELETE_Success");
                                flag = true;
                            }
                            else
                            {
                                client.Player.SendMessage("db.AddConsortia Error ");
                            }
                        }
                        client.Out.sendConsortiaTryIn(num2, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 1:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID != 0)
                        {
                            return 0;
                        }
                        ConsortiaLevelInfo consortiaLevelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(1);
                        string text2 = packet.ReadString();
                        int num3 = 0;
                        int needGold = consortiaLevelInfo.NeedGold;
                        int num4 = 5;
                        text = "ConsortiaCreateHandler.Failed";
                        ConsortiaDutyInfo consortiaDutyInfo = new ConsortiaDutyInfo();
                        if (!string.IsNullOrEmpty(text2) && client.Player.PlayerCharacter.Gold >= needGold && client.Player.PlayerCharacter.Grade >= num4)
                        {
                            using (ConsortiaBussiness consortiaBussiness2 = new ConsortiaBussiness())
                            {
                                ConsortiaInfo consortiaInfo = new ConsortiaInfo();
                                consortiaInfo.BuildDate = DateTime.Now;
                                consortiaInfo.CelebCount = 0;
                                consortiaInfo.ChairmanID = client.Player.PlayerCharacter.ID;
                                consortiaInfo.ChairmanName = client.Player.PlayerCharacter.NickName;
                                consortiaInfo.ConsortiaName = text2;
                                consortiaInfo.CreatorID = consortiaInfo.ChairmanID;
                                consortiaInfo.CreatorName = consortiaInfo.ChairmanName;
                                consortiaInfo.Description = "";
                                consortiaInfo.Honor = 0;
                                consortiaInfo.IP = "";
                                consortiaInfo.IsExist = true;
                                consortiaInfo.Level = consortiaLevelInfo.Level;
                                consortiaInfo.MaxCount = consortiaLevelInfo.Count;
                                consortiaInfo.Riches = consortiaLevelInfo.Riches;
                                consortiaInfo.Placard = "";
                                consortiaInfo.Port = 0;
                                consortiaInfo.Repute = 0;
                                consortiaInfo.Count = 1;
                                if (consortiaBussiness2.AddConsortia(consortiaInfo, ref text, ref consortiaDutyInfo))
                                {
                                    client.Player.PlayerCharacter.ConsortiaID = consortiaInfo.ConsortiaID;
                                    client.Player.PlayerCharacter.ConsortiaName = consortiaInfo.ConsortiaName;
                                    client.Player.PlayerCharacter.DutyLevel = consortiaDutyInfo.Level;
                                    client.Player.PlayerCharacter.DutyName = consortiaDutyInfo.DutyName;
                                    client.Player.PlayerCharacter.Right = consortiaDutyInfo.Right;
                                    client.Player.PlayerCharacter.ConsortiaLevel = consortiaLevelInfo.Level;
                                    client.Player.RemoveGold(needGold);
                                    text = "ConsortiaCreateHandler.Success";
                                    flag = true;
                                    num3 = consortiaInfo.ConsortiaID;
                                    GameServer.Instance.LoginServer.SendConsortiaCreate(num3, client.Player.PlayerCharacter.Offer, consortiaInfo.ConsortiaName);
                                }
                                else
                                {
                                    client.Player.SendMessage("db.AddConsortia Error ");
                                }
                            }
                        }
                        client.Out.SendConsortiaCreate(text2, flag, num3, text2, LanguageMgr.GetTranslation(text, new object[0]), consortiaDutyInfo.Level, consortiaDutyInfo.DutyName, consortiaDutyInfo.Right, client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 2:
                case 8:
                case 16:
                case 17:
                case 22:
                case 23:
                case 25:
                case 26:
                case 27:
                    return 0;

                case 3:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        int num5 = packet.ReadInt();
                        string nickName = "";
                        text = ((num5 == client.Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitFailed" : "ConsortiaUserDeleteHandler.KickFailed");
                        using (ConsortiaBussiness consortiaBussiness3 = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness3.DeleteConsortiaUser(client.Player.PlayerCharacter.ID, num5, client.Player.PlayerCharacter.ConsortiaID, ref text, ref nickName))
                            {
                                text = ((num5 == client.Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitSuccess" : "ConsortiaUserDeleteHandler.KickSuccess");
                                int consortiaID = client.Player.PlayerCharacter.ConsortiaID;
                                if (num5 == client.Player.PlayerCharacter.ID)
                                {
                                    client.Player.ClearConsortia();
                                    client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                }
                                GameServer.Instance.LoginServer.SendConsortiaUserDelete(num5, consortiaID, num5 != client.Player.PlayerCharacter.ID, nickName, client.Player.PlayerCharacter.NickName);
                                flag = true;
                            }
                        }
                        client.Out.sendConsortiaOut(num5, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 4:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        int num6 = packet.ReadInt();
                        text = "ConsortiaApplyLoginPassHandler.Failed";
                        using (ConsortiaBussiness consortiaBussiness4 = new ConsortiaBussiness())
                        {
                            int consortiaRepute = 0;
                            ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
                            if (consortiaBussiness4.PassConsortiaApplyUsers(num6, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ConsortiaID, ref text, consortiaUserInfo, ref consortiaRepute))
                            {
                                text = "ConsortiaApplyLoginPassHandler.Success";
                                flag = true;
                                if (consortiaUserInfo.UserID != 0)
                                {
                                    consortiaUserInfo.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
                                    consortiaUserInfo.ConsortiaName = client.Player.PlayerCharacter.ConsortiaName;
                                    GameServer.Instance.LoginServer.SendConsortiaUserPass(client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, consortiaUserInfo, false, consortiaRepute, consortiaUserInfo.LoginName, client.Player.PlayerCharacter.FightPower, client.Player.PlayerCharacter.Offer);
                                }
                            }
                        }
                        client.Out.sendConsortiaTryInPass(num6, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 5:
                    {
                        int num7 = packet.ReadInt();
                        text = "ConsortiaApplyAllyDeleteHandler.Failed";
                        using (ConsortiaBussiness consortiaBussiness5 = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness5.DeleteConsortiaApplyUsers(num7, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref text))
                            {
                                text = ((client.Player.PlayerCharacter.ID == 0) ? "ConsortiaApplyAllyDeleteHandler.Success" : "ConsortiaApplyAllyDeleteHandler.Success2");
                                flag = true;
                            }
                        }
                        client.Out.sendConsortiaTryInDel(num7, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 6:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        int num8 = packet.ReadInt();
                        if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
                            return 1;
                        }
                        if (num8 < 1 || client.Player.PlayerCharacter.Money < num8)
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaRichesOfferHandler.NoMoney", new object[0]));
                            return 1;
                        }
                        text = "ConsortiaRichesOfferHandler.Failed";
                        using (ConsortiaBussiness consortiaBussiness6 = new ConsortiaBussiness())
                        {
                            int num9 = num8 / 2;
                            if (consortiaBussiness6.ConsortiaRichAdd(client.Player.PlayerCharacter.ConsortiaID, ref num9, 5, client.Player.PlayerCharacter.NickName))
                            {
                                flag = true;
                                client.Player.PlayerCharacter.RichesOffer += num9;
                                client.Player.RemoveMoney(num8);
                                text = "ConsortiaRichesOfferHandler.Successed";
                                GameServer.Instance.LoginServer.SendConsortiaRichesOffer(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, num9);
                            }
                        }
                        client.Out.SendConsortiaRichesOffer(num8, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 7:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 1;
                        }
                        bool state = packet.ReadBoolean();
                        text = "CONSORTIA_APPLY_STATE.Failed";
                        using (ConsortiaBussiness consortiaBussiness7 = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness7.UpdateConsotiaApplyState(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, state, ref text))
                            {
                                text = "CONSORTIA_APPLY_STATE.Success";
                                flag = true;
                            }
                        }
                        client.Out.sendConsortiaApplyStatusOut(state, flag, client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 9:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        int dutyID = packet.ReadInt();
                        text = "ConsortiaDutyDeleteHandler.Failed";
                        using (ConsortiaBussiness consortiaBussiness8 = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness8.DeleteConsortiaDuty(dutyID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref text))
                            {
                                text = "ConsortiaDutyDeleteHandler.Success";
                                flag = true;
                            }
                            return 0;
                        }
                        break;
                    }

                case 10:
                    break;

                case 11:
                    goto IL_B83;

                case 12:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID != 0)
                        {
                            return 0;
                        }
                        int num10 = packet.ReadInt();
                        int num11 = 0;
                        string text3 = "";
                        text = "ConsortiaInvitePassHandler.Failed";
                        int playerid = 0;
                        string playerName = "";
                        using (ConsortiaBussiness consortiaBussiness9 = new ConsortiaBussiness())
                        {
                            int consortiaRepute2 = 0;
                            ConsortiaUserInfo consortiaUserInfo2 = new ConsortiaUserInfo();
                            if (consortiaBussiness9.PassConsortiaInviteUsers(num10, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, ref num11, ref text3, ref text, consortiaUserInfo2, ref playerid, ref playerName, ref consortiaRepute2))
                            {
                                client.Player.PlayerCharacter.ConsortiaID = num11;
                                client.Player.PlayerCharacter.ConsortiaName = text3;
                                client.Player.PlayerCharacter.DutyLevel = consortiaUserInfo2.Level;
                                client.Player.PlayerCharacter.DutyName = consortiaUserInfo2.DutyName;
                                client.Player.PlayerCharacter.Right = consortiaUserInfo2.Right;
                                ConsortiaInfo consortiaInfo2 = ConsortiaMgr.FindConsortiaInfo(num11);
                                if (consortiaInfo2 != null)
                                {
                                    client.Player.PlayerCharacter.ConsortiaLevel = consortiaInfo2.Level;
                                }
                                text = "ConsortiaInvitePassHandler.Success";
                                flag = true;
                                consortiaUserInfo2.UserID = client.Player.PlayerCharacter.ID;
                                consortiaUserInfo2.UserName = client.Player.PlayerCharacter.NickName;
                                consortiaUserInfo2.Grade = client.Player.PlayerCharacter.Grade;
                                consortiaUserInfo2.Offer = client.Player.PlayerCharacter.Offer;
                                consortiaUserInfo2.RichesOffer = client.Player.PlayerCharacter.RichesOffer;
                                consortiaUserInfo2.RichesRob = client.Player.PlayerCharacter.RichesRob;
                                consortiaUserInfo2.Win = client.Player.PlayerCharacter.Win;
                                consortiaUserInfo2.Total = client.Player.PlayerCharacter.Total;
                                consortiaUserInfo2.Escape = client.Player.PlayerCharacter.Escape;
                                consortiaUserInfo2.ConsortiaID = num11;
                                consortiaUserInfo2.ConsortiaName = text3;
                                GameServer.Instance.LoginServer.SendConsortiaUserPass(playerid, playerName, consortiaUserInfo2, true, consortiaRepute2, client.Player.PlayerCharacter.UserName, client.Player.PlayerCharacter.FightPower, client.Player.PlayerCharacter.Offer);
                            }
                        }
                        client.Out.sendConsortiaInvitePass(num10, flag, num11, text3, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 13:
                    {
                        int num12 = packet.ReadInt();
                        text = "ConsortiaInviteDeleteHandler.Failed";
                        using (ConsortiaBussiness consortiaBussiness10 = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness10.DeleteConsortiaInviteUsers(num12, client.Player.PlayerCharacter.ID))
                            {
                                text = "ConsortiaInviteDeleteHandler.Success";
                                flag = true;
                            }
                        }
                        client.Out.sendConsortiaInviteDel(num12, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 14:
                    {
                        string text4 = packet.ReadString();
                        if (Encoding.Default.GetByteCount(text4) > 300)
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDescriptionUpdateHandler.Long", new object[0]));
                            return 1;
                        }
                        text = "ConsortiaDescriptionUpdateHandler.Failed";
                        using (ConsortiaBussiness consortiaBussiness11 = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness11.UpdateConsortiaDescription(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, text4, ref text))
                            {
                                text = "ConsortiaDescriptionUpdateHandler.Success";
                                flag = true;
                            }
                        }
                        client.Out.sendConsortiaUpdateDescription(text4, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 15:
                    {
                        string text5 = packet.ReadString();
                        if (Encoding.Default.GetByteCount(text5) > 300)
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaPlacardUpdateHandler.Long", new object[0]));
                            return 1;
                        }
                        text = "ConsortiaPlacardUpdateHandler.Failed";
                        using (ConsortiaBussiness consortiaBussiness12 = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness12.UpdateConsortiaPlacard(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, text5, ref text))
                            {
                                text = "ConsortiaPlacardUpdateHandler.Success";
                                flag = true;
                            }
                        }
                        client.Out.sendConsortiaUpdatePlacard(text5, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 18:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        int num13 = packet.ReadInt();
                        bool flag2 = packet.ReadBoolean();
                        text = "ConsortiaUserGradeUpdateHandler.Failed";
                        using (ConsortiaBussiness consortiaBussiness13 = new ConsortiaBussiness())
                        {
                            string playerName2 = "";
                            ConsortiaDutyInfo info = new ConsortiaDutyInfo();
                            if (consortiaBussiness13.UpdateConsortiaUserGrade(num13, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, flag2, ref text, ref info, ref playerName2))
                            {
                                text = "ConsortiaUserGradeUpdateHandler.Success";
                                flag = true;
                                GameServer.Instance.LoginServer.SendConsortiaDuty(info, flag2 ? 6 : 7, client.Player.PlayerCharacter.ConsortiaID, num13, playerName2, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName);
                            }
                        }
                        client.Out.SendConsortiaMemberGrade(num13, flag2, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 19:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        string text6 = packet.ReadString();
                        text = "ConsortiaChangeChairmanHandler.Failed";
                        if (string.IsNullOrEmpty(text6))
                        {
                            text = "ConsortiaChangeChairmanHandler.NoName";
                        }
                        else
                        {
                            if (text6 == client.Player.PlayerCharacter.NickName)
                            {
                                text = "ConsortiaChangeChairmanHandler.Self";
                            }
                            else
                            {
                                using (ConsortiaBussiness consortiaBussiness14 = new ConsortiaBussiness())
                                {
                                    string playerName3 = "";
                                    int playerID = 0;
                                    ConsortiaDutyInfo info2 = new ConsortiaDutyInfo();
                                    if (consortiaBussiness14.UpdateConsortiaChairman(text6, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref text, ref info2, ref playerID, ref playerName3))
                                    {
                                        ConsortiaDutyInfo consortiaDutyInfo2 = new ConsortiaDutyInfo();
                                        consortiaDutyInfo2.Level = client.Player.PlayerCharacter.DutyLevel;
                                        consortiaDutyInfo2.DutyName = client.Player.PlayerCharacter.DutyName;
                                        consortiaDutyInfo2.Right = client.Player.PlayerCharacter.Right;
                                        text = "ConsortiaChangeChairmanHandler.Success1";
                                        flag = true;
                                        GameServer.Instance.LoginServer.SendConsortiaDuty(consortiaDutyInfo2, 9, client.Player.PlayerCharacter.ConsortiaID, playerID, playerName3, 0, "");
                                        GameServer.Instance.LoginServer.SendConsortiaDuty(info2, 8, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, 0, "");
                                    }
                                }
                            }
                        }
                        string str = LanguageMgr.GetTranslation(text, new object[0]);
                        if (text == "ConsortiaChangeChairmanHandler.Success1")
                        {
                            str = str + text6 + LanguageMgr.GetTranslation("ConsortiaChangeChairmanHandler.Success2", new object[0]);
                        }
                        client.Out.sendConsortiaChangeChairman(text6, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 20:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        if (client.Player.PlayerCharacter.IsBanChat)
                        {
                            client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat", new object[0]));
                            return 1;
                        }
                        packet.ClientID = client.Player.PlayerCharacter.ID;
                        packet.ReadByte();
                        packet.ReadString();
                        packet.ReadString();
                        packet.WriteInt(client.Player.PlayerCharacter.ConsortiaID);
                        GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
                        for (int i = 0; i < allPlayers.Length; i++)
                        {
                            GamePlayer gamePlayer = allPlayers[i];
                            if (gamePlayer.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID)
                            {
                                gamePlayer.Out.SendTCP(packet);
                            }
                        }
                        GameServer.Instance.LoginServer.SendPacket(packet);
                        return 0;
                    }

                case 21:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        byte b = packet.ReadByte();
                        byte level = 0;
                        if (b == 1)
                        {
                            text = "ConsortiaUpGradeHandler.Failed";
                            using (ConsortiaBussiness consortiaBussiness15 = new ConsortiaBussiness())
                            {
                                ConsortiaInfo consortiaSingle = consortiaBussiness15.GetConsortiaSingle(client.Player.PlayerCharacter.ConsortiaID);
                                if (consortiaSingle == null)
                                {
                                    text = "ConsortiaUpGradeHandler.NoConsortia";
                                }
                                else
                                {
                                    ConsortiaLevelInfo consortiaLevelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(consortiaSingle.Level + 1);
                                    if (consortiaLevelInfo == null)
                                    {
                                        text = "ConsortiaUpGradeHandler.NoUpGrade";
                                    }
                                    else
                                    {
                                        if (consortiaLevelInfo.NeedGold > client.Player.PlayerCharacter.Gold)
                                        {
                                            text = "ConsortiaUpGradeHandler.NoGold";
                                        }
                                        else
                                        {
                                            using (ConsortiaBussiness consortiaBussiness16 = new ConsortiaBussiness())
                                            {
                                                if (consortiaBussiness16.UpGradeConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref text))
                                                {
                                                    consortiaSingle.Level++;
                                                    client.Player.RemoveGold(consortiaLevelInfo.NeedGold);
                                                    GameServer.Instance.LoginServer.SendConsortiaUpGrade(consortiaSingle);
                                                    text = "ConsortiaUpGradeHandler.Success";
                                                    flag = true;
                                                    level = (byte)consortiaSingle.Level;
                                                }
                                            }
                                        }
                                    }
                                }
                                goto IL_18ED;
                            }
                        }
                        if (b == 2)
                        {
                            text = "ConsortiaStoreUpGradeHandler.Failed";
                            ConsortiaInfo consortiaInfo3 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                            if (consortiaInfo3 == null)
                            {
                                text = "ConsortiaStoreUpGradeHandler.NoConsortia";
                                goto IL_18ED;
                            }
                            using (ConsortiaBussiness consortiaBussiness17 = new ConsortiaBussiness())
                            {
                                if (consortiaBussiness17.UpGradeStoreConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref text))
                                {
                                    consortiaInfo3.StoreLevel++;
                                    GameServer.Instance.LoginServer.SendConsortiaStoreUpGrade(consortiaInfo3);
                                    text = "ConsortiaStoreUpGradeHandler.Success";
                                    flag = true;
                                    level = (byte)consortiaInfo3.StoreLevel;
                                }
                                goto IL_18ED;
                            }
                        }
                        if (b != 3)
                        {
                            goto IL_17A1;
                        }
                        text = "ConsortiaShopUpGradeHandler.Failed";
                        ConsortiaInfo consortiaInfo4 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                        if (consortiaInfo4 != null)
                        {
                            using (ConsortiaBussiness consortiaBussiness18 = new ConsortiaBussiness())
                            {
                                if (consortiaBussiness18.UpGradeShopConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref text))
                                {
                                    consortiaInfo4.ShopLevel++;
                                    GameServer.Instance.LoginServer.SendConsortiaShopUpGrade(consortiaInfo4);
                                    text = "ConsortiaShopUpGradeHandler.Success";
                                    flag = true;
                                    level = (byte)consortiaInfo4.ShopLevel;
                                }
                                goto IL_18ED;
                            }
                            goto IL_17A1;
                        }
                        text = "ConsortiaShopUpGradeHandler.NoConsortia";
                    IL_18ED:
                        client.Out.SendConsortiaLevelUp(b, level, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
                        return 0;
                    IL_17A1:
                        if (b == 4)
                        {
                            text = "ConsortiaSmithUpGradeHandler.Failed";
                            ConsortiaInfo consortiaInfo5 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                            if (consortiaInfo5 == null)
                            {
                                text = "ConsortiaSmithUpGradeHandler.NoConsortia";
                                goto IL_18ED;
                            }
                            using (ConsortiaBussiness consortiaBussiness19 = new ConsortiaBussiness())
                            {
                                if (consortiaBussiness19.UpGradeSmithConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref text))
                                {
                                    consortiaInfo5.SmithLevel++;
                                    GameServer.Instance.LoginServer.SendConsortiaSmithUpGrade(consortiaInfo5);
                                    text = "ConsortiaSmithUpGradeHandler.Success";
                                    flag = true;
                                    level = (byte)consortiaInfo5.SmithLevel;
                                }
                                goto IL_18ED;
                            }
                        }
                        if (b != 5)
                        {
                            goto IL_18ED;
                        }
                        text = "ConsortiaBufferUpGradeHandler.Failed";
                        ConsortiaInfo consortiaInfo6 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                        if (consortiaInfo6 == null)
                        {
                            text = "ConsortiaUpGradeHandler.NoConsortia";
                            goto IL_18ED;
                        }
                        using (ConsortiaBussiness consortiaBussiness20 = new ConsortiaBussiness())
                        {
                            if (consortiaBussiness20.UpGradeSkillConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref text))
                            {
                                consortiaInfo6.SkillLevel++;
                                GameServer.Instance.LoginServer.SendConsortiaKillUpGrade(consortiaInfo6);
                                text = "ConsortiaBufferUpGradeHandler.Success";
                                flag = true;
                                level = (byte)consortiaInfo6.SkillLevel;
                            }
                        }
                        goto IL_18ED;
                    }

                case 24:
                    {
                        if (client.Player.PlayerCharacter.ConsortiaID == 0)
                        {
                            return 0;
                        }
                        int item = 0;
                        int item2 = 0;
                        int item3 = 0;
                        int item4 = 0;
                        int item5 = 0;
                        int item6 = 0;
                        int item7 = 0;
                        text = "ConsortiaEquipControlHandler.Fail";
                        ConsortiaEquipControlInfo consortiaEquipControlInfo = new ConsortiaEquipControlInfo();
                        consortiaEquipControlInfo.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
                        using (ConsortiaBussiness consortiaBussiness21 = new ConsortiaBussiness())
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                consortiaEquipControlInfo.Riches = packet.ReadInt();
                                consortiaEquipControlInfo.Type = 1;
                                consortiaEquipControlInfo.Level = j + 1;
                                consortiaBussiness21.AddAndUpdateConsortiaEuqipControl(consortiaEquipControlInfo, client.Player.PlayerCharacter.ID, ref text);
                                switch (j + 1)
                                {
                                    case 1:
                                        item = consortiaEquipControlInfo.Riches;
                                        break;

                                    case 2:
                                        item2 = consortiaEquipControlInfo.Riches;
                                        break;

                                    case 3:
                                        item3 = consortiaEquipControlInfo.Riches;
                                        break;

                                    case 4:
                                        item4 = consortiaEquipControlInfo.Riches;
                                        break;

                                    case 5:
                                        item5 = consortiaEquipControlInfo.Riches;
                                        break;
                                }
                            }
                            consortiaEquipControlInfo.Riches = packet.ReadInt();
                            consortiaEquipControlInfo.Type = 2;
                            consortiaEquipControlInfo.Level = 0;
                            item6 = consortiaEquipControlInfo.Riches;
                            consortiaBussiness21.AddAndUpdateConsortiaEuqipControl(consortiaEquipControlInfo, client.Player.PlayerCharacter.ID, ref text);
                            consortiaEquipControlInfo.Riches = packet.ReadInt();
                            consortiaEquipControlInfo.Type = 3;
                            consortiaEquipControlInfo.Level = 0;
                            item7 = consortiaEquipControlInfo.Riches;
                            consortiaBussiness21.AddAndUpdateConsortiaEuqipControl(consortiaEquipControlInfo, client.Player.PlayerCharacter.ID, ref text);
                            text = "ConsortiaEquipControlHandler.Success";
                            flag = true;
                        }
                        List<int> riches = new List<int>
					{
						item,
						item2,
						item3,
						item4,
						item5,
						item6,
						item7
					};
                        client.Out.sendConsortiaEquipConstrol(flag, riches, client.Player.PlayerCharacter.ID);
                        return 0;
                    }

                case 28:
                    {
                        int num14 = packet.ReadInt();
                        text = "BuyBadgeHandler.Fail";
                        int validDate = 30;
                        string badgeBuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        ConsortiaInfo consortiaInfo7 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                        using (ConsortiaBussiness consortiaBussiness22 = new ConsortiaBussiness())
                        {
                            consortiaInfo7.BadgeID = num14;
                            consortiaInfo7.ValidDate = validDate;
                            consortiaInfo7.BadgeBuyTime = badgeBuyTime;
                            if (consortiaBussiness22.BuyBadge(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, consortiaInfo7, ref text))
                            {
                                text = "BuyBadgeHandler.Success";
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            using (PlayerBussiness playerBussiness = new PlayerBussiness())
                            {
                                ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(client.Player.PlayerCharacter.ConsortiaID);
                                ConsortiaUserInfo[] array = allMemberByConsortia;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    ConsortiaUserInfo consortiaUserInfo3 = array[i];
                                    GamePlayer playerById = WorldMgr.GetPlayerById(consortiaUserInfo3.UserID);
                                    if (playerById != null && playerById.PlayerId != client.Player.PlayerCharacter.ID)
                                    {
                                        playerById.UpdateBadgeId(num14);
                                        playerById.SendMessage("Guild của bạn đã thay đổi huy hiệu mới!");
                                        playerById.UpdateProperties();
                                    }
                                }
                            }
                        }
                        client.Player.SendMessage(text);
                        client.Out.sendBuyBadge(num14, validDate, flag, badgeBuyTime, client.Player.PlayerCharacter.ID);
                        client.Player.UpdateBadgeId(num14);
                        client.Player.UpdateProperties();
                        return 0;
                    }

                case 29:
                    {
                        string title = packet.ReadString();
                        string content = packet.ReadString();
                        text = "ConsortiaRichiUpdateHandler.Failed";
                        ConsortiaInfo consortiaInfo8 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                        using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
                        {
                            ConsortiaUserInfo[] allMemberByConsortia2 = playerBussiness2.GetAllMemberByConsortia(client.Player.PlayerCharacter.ConsortiaID);
                            MailInfo mailInfo = new MailInfo();
                            ConsortiaUserInfo[] array = allMemberByConsortia2;
                            for (int i = 0; i < array.Length; i++)
                            {
                                ConsortiaUserInfo consortiaUserInfo4 = array[i];
                                mailInfo.SenderID = client.Player.PlayerCharacter.ID;
                                mailInfo.Sender = "Chủ Guild " + consortiaInfo8.ConsortiaName;
                                mailInfo.ReceiverID = consortiaUserInfo4.UserID;
                                mailInfo.Receiver = consortiaUserInfo4.UserName;
                                mailInfo.Title = title;
                                mailInfo.Content = content;
                                mailInfo.Type = 59;
                                if (consortiaUserInfo4.UserID != client.Player.PlayerCharacter.ID && playerBussiness2.SendMail(mailInfo))
                                {
                                    text = "ConsortiaRichiUpdateHandler.Success";
                                    flag = true;
                                    if (consortiaUserInfo4.State != 0)
                                    {
                                        client.Player.Out.SendMailResponse(consortiaUserInfo4.UserID, eMailRespose.Receiver);
                                    }
                                    client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
                                }
                                if (!flag)
                                {
                                    client.Player.SendMessage("SendMail Error!");
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            using (ConsortiaBussiness consortiaBussiness23 = new ConsortiaBussiness())
                            {
                                consortiaBussiness23.UpdateConsortiaRiches(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, 1000, ref text);
                            }
                        }
                        client.Out.SendConsortiaMail(flag, client.Player.PlayerCharacter.ID);
                        return 0;
                    }
                case 30:
                    {
                        byte _arg1 = packet.ReadByte();//_local2.writeByte(_arg1);

                        client.Out.SendbossOpenCloseHandler(client.Player.PlayerCharacter.ID);
                        Console.Write("CONSORTIA_BOSS_INFO");
                        return 0;
                    }
                default:
                    return 0;
            }
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                return 0;
            }
            int dutyID2 = packet.ReadInt();
            int updateType = (int)packet.ReadByte();
            text = "ConsortiaDutyUpdateHandler.Failed";
            using (ConsortiaBussiness consortiaBussiness24 = new ConsortiaBussiness())
            {
                ConsortiaDutyInfo consortiaDutyInfo3 = new ConsortiaDutyInfo();
                consortiaDutyInfo3.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
                consortiaDutyInfo3.DutyID = dutyID2;
                consortiaDutyInfo3.IsExist = true;
                consortiaDutyInfo3.DutyName = "";
                switch (updateType)
                {
                    case 1:
                        {
                            int result = 1;
                            return result;
                        }

                    case 2:
                        consortiaDutyInfo3.DutyName = packet.ReadString();
                        if (string.IsNullOrEmpty(consortiaDutyInfo3.DutyName) || Encoding.Default.GetByteCount(consortiaDutyInfo3.DutyName) > 10)
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDutyUpdateHandler.Long", new object[0]));
                            int result = 1;
                            return result;
                        }
                        consortiaDutyInfo3.Right = packet.ReadInt();
                        break;
                }
                if (consortiaBussiness24.UpdateConsortiaDuty(consortiaDutyInfo3, client.Player.PlayerCharacter.ID, updateType, ref text))
                {
                    dutyID2 = consortiaDutyInfo3.DutyID;
                    text = "ConsortiaDutyUpdateHandler.Success";
                    flag = true;
                    GameServer.Instance.LoginServer.SendConsortiaDuty(consortiaDutyInfo3, updateType, client.Player.PlayerCharacter.ConsortiaID);
                }
                return 0;
            }
        IL_B83:
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
            {
                return 0;
            }
            string text7 = packet.ReadString();
            text = "ConsortiaInviteAddHandler.Failed";
            if (string.IsNullOrEmpty(text7))
            {
                return 0;
            }
            using (ConsortiaBussiness consortiaBussiness25 = new ConsortiaBussiness())
            {
                ConsortiaInviteUserInfo consortiaInviteUserInfo = new ConsortiaInviteUserInfo();
                consortiaInviteUserInfo.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
                consortiaInviteUserInfo.ConsortiaName = client.Player.PlayerCharacter.ConsortiaName;
                consortiaInviteUserInfo.InviteDate = DateTime.Now;
                consortiaInviteUserInfo.InviteID = client.Player.PlayerCharacter.ID;
                consortiaInviteUserInfo.InviteName = client.Player.PlayerCharacter.NickName;
                consortiaInviteUserInfo.IsExist = true;
                consortiaInviteUserInfo.Remark = "";
                consortiaInviteUserInfo.UserID = 0;
                consortiaInviteUserInfo.UserName = text7;
                if (consortiaBussiness25.AddConsortiaInviteUsers(consortiaInviteUserInfo, ref text))
                {
                    text = "ConsortiaInviteAddHandler.Success";
                    flag = true;
                    GameServer.Instance.LoginServer.SendConsortiaInvite(consortiaInviteUserInfo.ID, consortiaInviteUserInfo.UserID, consortiaInviteUserInfo.UserName, consortiaInviteUserInfo.InviteID, consortiaInviteUserInfo.InviteName, consortiaInviteUserInfo.ConsortiaName, consortiaInviteUserInfo.ConsortiaID);
                }
            }
            client.Out.SendConsortiaInvite(text7, flag, LanguageMgr.GetTranslation(text, new object[0]), client.Player.PlayerCharacter.ID);
            return 0;
        }
    }
}