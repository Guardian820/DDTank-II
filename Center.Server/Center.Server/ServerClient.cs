// Type: Center.Server.ServerClient
// Assembly: Center.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: D:\Server 4.0 fix bug\Azo\Center\Center.Server.dll

using Bussiness;
using Bussiness.Protocol;
using Center.Server.Managers;
using Game.Base;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Center.Server
{
    public class ServerClient : BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private RSACryptoServiceProvider _rsa;
        private CenterServer _svr;
        public bool NeedSyncMacroDrop;

        public ServerInfo Info { get; set; }

        static ServerClient()
        {
        }

        public ServerClient(CenterServer svr)
            : base(new byte[8192], new byte[8192])
        {
            this._svr = svr;
        }

        protected override void OnConnect()
        {
            base.OnConnect();
            this._rsa = new RSACryptoServiceProvider();
            RSAParameters rsaParameters = this._rsa.ExportParameters(false);
            this.SendRSAKey(rsaParameters.Modulus, rsaParameters.Exponent);
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            this._rsa = (RSACryptoServiceProvider)null;
            List<Player> serverPlayers = LoginMgr.GetServerPlayers(this);
            LoginMgr.RemovePlayer(serverPlayers);
            this.SendUserOffline(serverPlayers);
            if (this.Info == null)
                return;
            this.Info.State = 1;
            this.Info.Online = 0;
            this.Info = (ServerInfo)null;
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            Console.Write("ServerClient: code: " + pkg.Code.ToString() + "\n");
            switch (pkg.Code)
            {
                case (short)165:
                    this.HandleFriendState(pkg);
                    break;
                case (short)166:
                    this.HandleFirendResponse(pkg);
                    break;
                case (short)178:
                    this.HandleMacroDrop(pkg);
                    break;
                case (short)240:
                    this.HandleIPAndPort(pkg);
                    break;
                case (short)128:
                    this.HandleConsortiaResponse(pkg);
                    break;
                case (short)130:
                    this.HandleConsortiaCreate(pkg);
                    break;
                case (short)156:
                    this.HandleConsortiaOffer(pkg);
                    break;
                case (short)158:
                    this.HandleConsortiaFight(pkg);
                    break;
                case (short)160:
                    this.HandleBuyBadge(pkg);
                    break;
                case (short)72:
                    this.HandleBigBugle(pkg);
                    break;
                case (short)117:
                    this.HandleMailResponse(pkg);
                    break;
                case (short)1:
                    this.HandleLogin(pkg);
                    break;
                case (short)3:
                    this.HandleUserLogin(pkg);
                    break;
                case (short)4:
                    this.HandleUserOffline(pkg);
                    break;
                case (short)5:
                    this.HandleUserOnline(pkg);
                    break;
                case (short)6:
                    this.HandleQuestUserState(pkg);
                    break;
                case (short)10:
                    this.HandkeItemStrengthen(pkg);
                    break;
                case (short)11:
                    this.HandleReload(pkg);
                    break;
                case (short)12:
                    this.HandlePing(pkg);
                    break;
                case (short)13:
                    this.HandleUpdatePlayerState(pkg);
                    break;
                case (short)14:
                    this.HandleMarryRoomInfoToPlayer(pkg);
                    break;
                case (short)15:
                    this.HandleShutdown(pkg);
                    break;
                case (short)19:
                    this.HandleChatScene(pkg);
                    break;
                case (short)37:
                    this.HandleChatPersonal(pkg);
                    break;
            }
        }

        public void HandleLogin(GSPacketIn pkg)
        {
            string[] strArray = Encoding.UTF8.GetString(this._rsa.Decrypt(pkg.ReadBytes(), false)).Split(new char[1]
      {
        ','
      });
            if (strArray.Length == 2)
            {
                this._rsa = (RSACryptoServiceProvider)null;
                int id = int.Parse(strArray[0]);
                this.Info = ServerMgr.GetServerInfo(id);
                if (this.Info == null || this.Info.State != 1)
                {
                    ServerClient.log.ErrorFormat("Error Login Packet from {0} want to login serverid:{1}", (object)this.TcpEndpoint, (object)id);
                    this.Disconnect();
                }
                else
                {
                    this.Strict = false;
                    CenterServer.Instance.SendConfigState();
                    this.Info.Online = 0;
                    this.Info.State = 2;
                }
            }
            else
            {
                ServerClient.log.ErrorFormat("Error Login Packet from {0}", (object)this.TcpEndpoint);
                this.Disconnect();
            }
        }

        public void HandleIPAndPort(GSPacketIn pkg)
        {
        }

        private void HandleUserLogin(GSPacketIn pkg)
        {
            int num = pkg.ReadInt();
            if (LoginMgr.TryLoginPlayer(num, this))
                this.SendAllowUserLogin(num, true);
            else
                this.SendAllowUserLogin(num, false);
        }

        private void HandleUserOnline(GSPacketIn pkg)
        {
            int num = pkg.ReadInt();
            for (int index = 0; index < num; ++index)
            {
                int id = pkg.ReadInt();
                pkg.ReadInt();
                LoginMgr.PlayerLogined(id, this);
            }
            this._svr.SendToALL(pkg, this);
        }

        private void HandleUserOffline(GSPacketIn pkg)
        {
            List<int> list = new List<int>();
            int num = pkg.ReadInt();
            for (int index = 0; index < num; ++index)
            {
                int id = pkg.ReadInt();
                pkg.ReadInt();
                LoginMgr.PlayerLoginOut(id, this);
            }
            this._svr.SendToALL(pkg);
        }

        private void HandleUserPrivateMsg(GSPacketIn pkg, int playerid)
        {
            ServerClient serverClient = LoginMgr.GetServerClient(playerid);
            if (serverClient == null)
                return;
            serverClient.SendTCP(pkg);
        }

        public void HandleUserPublicMsg(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleQuestUserState(GSPacketIn pkg)
        {
            int num = pkg.ReadInt();
            if (LoginMgr.GetServerClient(num) == null)
                this.SendUserState(num, false);
            else
                this.SendUserState(num, true);
        }

        public void HandlePing(GSPacketIn pkg)
        {
            this.Info.Online = pkg.ReadInt();
            this.Info.State = ServerMgr.GetState(this.Info.Online, this.Info.Total);
        }

        public void HandleChatPersonal(GSPacketIn pkg)
        {
            ServerClient serverClient = LoginMgr.GetServerClient(pkg.ReadInt());
            if (serverClient != null)
            {
                serverClient.SendTCP(pkg);
            }
            else
            {
                int clientId = pkg.ClientID;
                string str = pkg.ReadString();
                GSPacketIn pkg1 = new GSPacketIn((short)38);
                pkg1.WriteInt(1);
                pkg1.WriteInt(clientId);
                pkg1.WriteString(str);
                this.SendTCP(pkg1);
            }
        }

        public void HandleBigBugle(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleFriendState(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleFirendResponse(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleMailResponse(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();
            this.HandleUserPrivateMsg(pkg, playerid);
        }

        public void HandleReload(GSPacketIn pkg)
        {
            eReloadType eReloadType = (eReloadType)pkg.ReadInt();
            int num = pkg.ReadInt();
            bool flag = pkg.ReadBoolean();
            Console.WriteLine((string)(object)num + (object)" " + ((object)eReloadType).ToString() + " is reload " + (string)(flag ? (object)"succeed!" : (object)"fail"));
        }

        public void HandleChatScene(GSPacketIn pkg)
        {
            if ((int)pkg.ReadByte() != 3)
                return;
            this.HandleChatConsortia(pkg);
        }

        public void HandleChatConsortia(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleConsortiaResponse(GSPacketIn pkg)
        {
            switch (pkg.ReadByte())
            {
                default:
                    this._svr.SendToALL(pkg, (ServerClient)null);
                    break;
            }
        }

        public void HandleConsortiaOffer(GSPacketIn pkg)
        {
            pkg.ReadInt();
            pkg.ReadInt();
            pkg.ReadInt();
        }

        public void HandleBuyBadge(GSPacketIn pkg)
        {
            pkg.ReadInt();
            this._svr.SendToALL(pkg, (ServerClient)null);
        }

        public void HandleConsortiaCreate(GSPacketIn pkg)
        {
            pkg.ReadInt();
            pkg.ReadInt();
            this._svr.SendToALL(pkg, (ServerClient)null);
        }

        public void HandleConsortiaUpGrade(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleConsortiaFight(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg);
        }

        public void HandkeItemStrengthen(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleUpdatePlayerState(GSPacketIn pkg)
        {
            Player player = LoginMgr.GetPlayer(pkg.ReadInt());
            if (player == null || player.CurrentServer == null)
                return;
            player.CurrentServer.SendTCP(pkg);
        }

        public void HandleMarryRoomInfoToPlayer(GSPacketIn pkg)
        {
            Player player = LoginMgr.GetPlayer(pkg.ReadInt());
            if (player == null || player.CurrentServer == null)
                return;
            player.CurrentServer.SendTCP(pkg);
        }

        public void HandleShutdown(GSPacketIn pkg)
        {
            int num = pkg.ReadInt();
            if (pkg.ReadBoolean())
                Console.WriteLine((string)(object)num + (object)"  begin stoping !");
            else
                Console.WriteLine((string)(object)num + (object)"  is stoped !");
        }

        public void HandleMacroDrop(GSPacketIn pkg)
        {
            Dictionary<int, int> temp = new Dictionary<int, int>();
            int num1 = pkg.ReadInt();
            for (int index = 0; index < num1; ++index)
            {
                int key = pkg.ReadInt();
                int num2 = pkg.ReadInt();
                temp.Add(key, num2);
            }
            MacroDropMgr.DropNotice(temp);
            this.NeedSyncMacroDrop = true;
        }

        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn pkg = new GSPacketIn((short)0);
            pkg.Write(m);
            pkg.Write(e);
            this.SendTCP(pkg);
        }

        public void SendAllowUserLogin(int playerid, bool allow)
        {
            GSPacketIn pkg = new GSPacketIn((short)3);
            pkg.WriteInt(playerid);
            pkg.WriteBoolean(allow);
            this.SendTCP(pkg);
        }

        public void SendKitoffUser(int playerid)
        {
            this.SendKitoffUser(playerid, LanguageMgr.GetTranslation("Center.Server.SendKitoffUser", new object[0]));
        }

        public void SendKitoffUser(int playerid, string msg)
        {
            GSPacketIn pkg = new GSPacketIn((short)2);
            pkg.WriteInt(playerid);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendUserOffline(List<Player> users)
        {
            int num = 0;
            while (num < users.Count)
            {
                int val = num + 100 > users.Count ? users.Count - num : 100;
                GSPacketIn pkg = new GSPacketIn((short)4);
                pkg.WriteInt(val);
                for (int index = num; index < num + val; ++index)
                {
                    pkg.WriteInt(users[index].Id);
                    pkg.WriteInt(0);
                }
                this.SendTCP(pkg);
                this._svr.SendToALL(pkg, this);
                num += 100;
            }
        }

        public void SendUserState(int player, bool state)
        {
            GSPacketIn pkg = new GSPacketIn((short)6, player);
            pkg.WriteBoolean(state);
            this.SendTCP(pkg);
        }

        public void SendChargeMoney(int player, string chargeID)
        {
            GSPacketIn pkg = new GSPacketIn((short)9, player);
            pkg.WriteString(chargeID);
            this.SendTCP(pkg);
        }

        public void SendASS(bool state)
        {
            GSPacketIn pkg = new GSPacketIn((short)7);
            pkg.WriteBoolean(state);
            this.SendTCP(pkg);
        }
    }
}
