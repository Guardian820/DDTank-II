using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Server
{
	public class ConsolePacketLib : IPacketLib
	{
		public GSPacketIn SendDiceActiveOpen(int ID)
		{
			throw new NotImplementedException();
		}
        public void SendUpdateUerGuild(int ID)
        {
            throw new NotImplementedException();
        }
        public void SendsysNotice(int ID)
        {
            throw new NotImplementedException();
        }
        public void SendgetGifts(int ID, ItemInfo item)
        {
            throw new NotImplementedException();
        }
        public GSPacketIn SendbossOpenCloseHandler(int playerid)
        {
            throw new NotImplementedException();
        }//
		public GSPacketIn SendRuneOpenPackage(GamePlayer player, int rand)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerDrill(int ID, Dictionary<int, UserDrillInfo> drills)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendActivityList(int ID)
		{
			throw new NotImplementedException();
		}
		public void SendWeaklessGuildProgress(PlayerInfo player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendGetBoxTime(int ID, int receiebox, bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMessage(eMessageType type, string message)
		{
			Console.WriteLine(message);
			return null;
		}
		public GSPacketIn sendBuyBadge(int BadgeID, int ValidDate, bool result, string BadgeBuyTime, int playerid)
		{
			throw new NotImplementedException();
		}
        public GSPacketIn bossOpenCloseHandler()//bossg
        {
            throw new NotImplementedException();
        }
		public GSPacketIn sendConsortiaApplyStatusOut(bool state, bool result, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaUpdatePlacard(string description, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaChangeChairman(string nick, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaOut(int id, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaEquipConstrol(bool result, List<int> Riches, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendConsortiaMemberGrade(int id, bool update, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
        public GSPacketIn SendConsortiaCreate(string name1, bool result, int id, string name2, string msg, int dutyLevel, string DutyName, int dutyRight, int playerid)//mrphuong
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendConsortiaRichesOffer(int money, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendConsortiaInvite(string username, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaInvitePass(int id, bool result, int consortiaid, string consortianame, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaInviteDel(int id, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendConsortiaLevelUp(byte type, byte level, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaTryIn(int id, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaTryInPass(int id, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaTryInDel(int id, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendConsortiaUpdateDescription(string description, bool result, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendHelperSwitchField(PlayerInfo Player, UserFarmInfo farm)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPayFields(GamePlayer Player, List<int> fieldIds)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSeeding(PlayerInfo Player, UserFieldInfo field)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendKillCropField(PlayerInfo Player, UserFieldInfo field)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendtoGather(PlayerInfo Player, UserFieldInfo field)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendEnterFarm(PlayerInfo Player, UserFarmInfo farm, UserFieldInfo[] fields)
		{
			throw new NotImplementedException();
		}
		public void SendPetGuildOptionChange()
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRefreshPet(GamePlayer player, UsersPetinfo[] pets, ItemInfo[] items, bool refreshBtn)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendGameMissionPrepare()
		{
			throw new NotImplementedException();
		}
		public void SendOpenWorldBoss()
		{
			throw new NotImplementedException();
		}
		public void SendLittleGameActived()
		{
			throw new NotImplementedException();
		}
		public void SendTCP(GSPacketIn packet)
		{
			throw new NotImplementedException();
		}
		public void SendLoginSuccess()
		{
			throw new NotImplementedException();
		}
		public void SendCheckCode()
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendGameMissionStart()
		{
			throw new NotImplementedException();
		}
		public void SendLoginFailed(string msg)
		{
			throw new NotImplementedException();
		}
		public void SendKitoff(string msg)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendOpenHoleComplete(GamePlayer player, int type, bool result)
		{
			throw new NotImplementedException();
		}
		public void SendEditionError(string msg)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendOpenVIP(PlayerInfo player)
		{
			throw new NotImplementedException();
		}
		public void SendDateTime()
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendDailyAward(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public void SendPingTime(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public void SendUpdatePrivateInfo(PlayerInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdatePlayerProperty(PlayerInfo info, Dictionary<string, Dictionary<string, int>> Property)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn sendOneOnOneTalk(int receiverID, bool isAutoReply, string SenderNickName, string msg, int playerid)
		{
			throw new NotImplementedException();
		}
        
		public GSPacketIn SendConsortiaMail(bool result, int playerid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendNetWork(GamePlayer player, long delay)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUserEquip(PlayerInfo info, List<ItemInfo> pets, List<UserGemStone> UserGemStone, List<ItemInfo> beadItems)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerCardReset(PlayerInfo player, List<int> points)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendGetPlayerCard(int playerId)
		{
			throw new NotImplementedException();
		}
		public void SendPlayerCardSlot(PlayerInfo info, List<UsersCardInfo> cardslots)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerCardSlot(PlayerInfo player, UsersCardInfo card)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerCardSoul(PlayerInfo player, bool isSoul, int soul)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendGetCard(PlayerInfo info, UsersCardInfo card)
		{
			throw new NotImplementedException();
		}
		public void SendPlayerCardInfo(CardInventory bag, int[] updatedSlots)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdatePetInfo(PlayerInfo info, UsersPetinfo[] pets)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateUserPet(PetInventory bag, int[] slots)
		{
			throw new NotImplementedException();
		}
		public void SendWaitingRoom(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateRoomList(List<BaseRoom> room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSceneAddPlayer(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomCreate(BaseRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomLoginResult(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPlayerAdd(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPairUpStart(BaseRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomChange(BaseRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isBind, int MinValid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendFusionResult(GamePlayer player, bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, ItemInfo item)
		{
			throw new NotImplementedException();
		}
		public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAddFriend(PlayerInfo user, int relation, bool state)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendFriendRemove(int FriendID)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendFriendState(int playerID, int state, byte typeVip, int viplevel)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateAllData(PlayerInfo player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendGetSpree(PlayerInfo player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateUpCount(PlayerInfo player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerRefreshTotem(PlayerInfo player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendLabyrinthUpdataInfo(int ID)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerFigSpiritinit(int ID, List<UserGemStone> gems)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerFigSpiritUp(int ID, UserGemStone gem, bool isUp, bool isMaxLevel, bool isFall, int num, int dir)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendTrusteeshipStart(int ID)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] quests)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendIDNumberCheck(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAASState(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAASInfoSet(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int ID)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int ID)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendContinuation(GamePlayer player, HotSpringRoomInfo hotSpringRoomInfo)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendInsufficientMoney(GamePlayer player, int type)
		{
			throw new NotImplementedException();
		}

        public GSPacketIn SendOpenChickenBox(int ID)//box
        {
            throw new NotImplementedException();
        }
	}
}
