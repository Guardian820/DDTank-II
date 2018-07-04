using log4net;
using Road.Base.Packets;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Base.Packets
{
    public class StreamProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly BaseClient m_client;
        private FSM send_fsm;
        private FSM receive_fsm;
        private SocketAsyncEventArgs send_event;
        protected byte[] m_tcpSendBuffer;
        protected Queue m_tcpQueue;
        protected bool m_sendingTcp;
        protected int m_firstPkgOffset = 0;
        protected int m_sendBufferLength = 0;
        public static byte[] KEY = new byte[]
		{
			174,
			191,
			86,
			120,
			171,
			205,
			239,
			241
		};
        public StreamProcessor(BaseClient client)
        {
            this.m_client = client;
            this.m_client.resetKey();
            this.m_tcpSendBuffer = client.SendBuffer;
            this.m_tcpQueue = new Queue(256);
            this.send_event = new SocketAsyncEventArgs();
            this.send_event.UserToken = this;
            this.send_event.Completed += new EventHandler<SocketAsyncEventArgs>(StreamProcessor.AsyncTcpSendCallback);
            this.send_event.SetBuffer(this.m_tcpSendBuffer, 0, 0);
            this.send_fsm = new FSM(2059198199, 1501, "send_fsm");
            this.receive_fsm = new FSM(2059198199, 1501, "receive_fsm");
        }
        public void SetFsm(int adder, int muliter)
        {
            this.send_fsm.Setup(adder, muliter);
            this.receive_fsm.Setup(adder, muliter);
        }
        public void SendTCP(GSPacketIn packet)
        {
            packet.WriteHeader();
            packet.Offset = 0;
            if (this.m_client.Socket.Connected)
            {
                try
                {
                    Statistics.BytesOut += (long)packet.Length;
                    Statistics.PacketsOut += 1L;
                    if (StreamProcessor.log.IsDebugEnabled)
                    {
                        StreamProcessor.log.Debug(Marshal.ToHexDump(string.Format("Send Pkg to {0} :", this.m_client.TcpEndpoint), packet.Buffer, 0, packet.Length));
                    }
                    object syncRoot;
                    Monitor.Enter(syncRoot = this.m_tcpQueue.SyncRoot);
                    try
                    {
                        this.m_tcpQueue.Enqueue(packet);
                        if (this.m_sendingTcp)
                        {
                            return;
                        }
                        this.m_sendingTcp = true;
                    }
                    finally
                    {
                        Monitor.Exit(syncRoot);
                    }
                    if (this.m_client.AsyncPostSend)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(StreamProcessor.AsyncSendTcpImp), this);
                    }
                    else
                    {
                        StreamProcessor.AsyncTcpSendCallback(this, this.send_event);
                    }
                }
                catch (Exception ex)
                {
                    StreamProcessor.log.Error("SendTCP", ex);
                    StreamProcessor.log.WarnFormat("It seems <{0}> went linkdead. Closing connection. (SendTCP, {1}: {2})", this.m_client, ex.GetType(), ex.Message);
                    this.m_client.Disconnect();
                }
            }
        }
        private static void AsyncSendTcpImp(object state)
        {
            StreamProcessor streamProcessor = state as StreamProcessor;
            BaseClient client = streamProcessor.m_client;
            try
            {
                StreamProcessor.AsyncTcpSendCallback(streamProcessor, streamProcessor.send_event);
            }
            catch (Exception exception)
            {
                StreamProcessor.log.Error("AsyncSendTcpImp", exception);
                client.Disconnect();
            }
        }
        private static void AsyncTcpSendCallback(object sender, SocketAsyncEventArgs e)
        {
            StreamProcessor streamProcessor = (StreamProcessor)e.UserToken;
            BaseClient client = streamProcessor.m_client;
            try
            {
                Queue tcpQueue = streamProcessor.m_tcpQueue;
                if (tcpQueue != null && client.Socket.Connected)
                {
                    int bytesTransferred = e.BytesTransferred;
                    byte[] tcpSendBuffer = streamProcessor.m_tcpSendBuffer;
                    int num = 0;
                    if (bytesTransferred != e.Count)
                    {
                        if (streamProcessor.m_sendBufferLength > bytesTransferred)
                        {
                            num = streamProcessor.m_sendBufferLength - bytesTransferred;
                            Array.Copy(tcpSendBuffer, bytesTransferred, tcpSendBuffer, 0, num);
                        }
                    }
                    e.SetBuffer(0, 0);
                    int num2 = streamProcessor.m_firstPkgOffset;
                    object syncRoot;
                    Monitor.Enter(syncRoot = tcpQueue.SyncRoot);
                    try
                    {
                        if (tcpQueue.Count > 0)
                        {
                            do
                            {
                                PacketIn packetIn = (PacketIn)tcpQueue.Peek();
                                byte[] array = null;
                                int num3;
                                if (client.Encryted)
                                {
                                    array = StreamProcessor.cloneArrary(client.SEND_KEY, 8);
                                    num3 = packetIn.CopyTo(tcpSendBuffer, num, num2, ref array);
                                }
                                else
                                {
                                    num3 = packetIn.CopyTo(tcpSendBuffer, num, num2);
                                }
                                num2 += num3;
                                num += num3;
                                if (packetIn.Length <= num2)
                                {
                                    if (array != null && array != client.SEND_KEY)
                                    {
                                        client.SEND_KEY = StreamProcessor.cloneArrary(array, 8);
                                    }
                                    tcpQueue.Dequeue();
                                    num2 = 0;
                                    if (client.Encryted)
                                    {
                                        streamProcessor.send_fsm.UpdateState();
                                        packetIn.isSended = true;
                                    }
                                }
                                if (tcpSendBuffer.Length == num)
                                {
                                    break;
                                }
                            }
                            while (tcpQueue.Count > 0);
                        }
                        streamProcessor.m_firstPkgOffset = num2;
                        if (num <= 0)
                        {
                            streamProcessor.m_sendingTcp = false;
                            return;
                        }
                    }
                    finally
                    {
                        Monitor.Exit(syncRoot);
                    }
                    streamProcessor.m_sendBufferLength = num;
                    e.SetBuffer(0, num);
                    if (!client.SendAsync(e))
                    {
                        StreamProcessor.AsyncTcpSendCallback(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                StreamProcessor.log.Error("AsyncTcpSendCallback", ex);
                StreamProcessor.log.WarnFormat("It seems <{0}> went linkdead. Closing connection. (SendTCP, {1}: {2})", client, ex.GetType(), ex.Message);
                client.Disconnect();
            }
        }
        public void ReceiveBytes(int numBytes)
        {
            Monitor.Enter(this);
            try
            {
                byte[] packetBuf = this.m_client.PacketBuf;
                int num = this.m_client.PacketBufSize + numBytes;
                if (num < 20)
                {
                    this.m_client.PacketBufSize = num;
                }
                else
                {
                    this.m_client.PacketBufSize = 0;
                    int num2 = 0;
                    int num3;
                    int num5;
                    while (true)
                    {
                        num3 = 0;
                        if (this.m_client.Encryted)
                        {
                            int count = this.receive_fsm.count;
                            byte[] param = StreamProcessor.cloneArrary(this.m_client.RECEIVE_KEY, 8);
                            while (num2 + 4 < num)
                            {
                                byte[] array = StreamProcessor.decryptBytes(packetBuf, num2, 8, param);
                                int num4 = ((int)array[0] << 8) + (int)array[1];
                                if (num4 == 29099)
                                {
                                    num3 = ((int)array[2] << 8) + (int)array[3];
                                    break;
                                }
                                num2++;
                            }
                        }
                        else
                        {
                            while (num2 + 4 < num)
                            {
                                int num4 = ((int)packetBuf[num2] << 8) + (int)packetBuf[num2 + 1];
                                if (num4 == 29099)
                                {
                                    num3 = ((int)packetBuf[num2 + 2] << 8) + (int)packetBuf[num2 + 3];
                                    break;
                                }
                                num2++;
                            }
                        }
                        if ((num3 != 0 && num3 < 20) || num3 > 16869)
                        {
                            break;
                        }
                        num5 = num - num2;
                        if (num5 < num3 || num3 == 0)
                        {
                            goto Block_11;
                        }
                        GSPacketIn gSPacketIn = new GSPacketIn(new byte[16869], 16869);
                        if (this.m_client.Encryted)
                        {
                            gSPacketIn.CopyFrom3(packetBuf, num2, 0, num3, this.m_client.RECEIVE_KEY);
                        }
                        else
                        {
                            gSPacketIn.CopyFrom(packetBuf, num2, 0, num3);
                        }
                        gSPacketIn.ReadHeader();
                        StreamProcessor.log.Debug(Marshal.ToHexDump("Recieve Packet:", gSPacketIn.Buffer, 0, num3));
                        try
                        {
                            this.m_client.OnRecvPacket(gSPacketIn);
                        }
                        catch (Exception exception)
                        {
                            if (StreamProcessor.log.IsErrorEnabled)
                            {
                                StreamProcessor.log.Error("HandlePacket(pak)", exception);
                            }
                        }
                        num2 += num3;
                        if (num - 1 <= num2)
                        {
                            goto IL_332;
                        }
                    }
                    StreamProcessor.log.Error(string.Concat(new object[]
					{
						"packetLength:",
						num3,
						",GSPacketIn.HDR_SIZE:",
						20,
						",offset:",
						num2,
						",bufferSize:",
						num,
						",numBytes:",
						numBytes
					}));
                    StreamProcessor.log.ErrorFormat("Err pkg from {0}:", this.m_client.TcpEndpoint);
                    StreamProcessor.log.Error(Marshal.ToHexDump("===> error buffer", packetBuf));
                    this.m_client.PacketBufSize = 0;
                    if (this.m_client.Strict)
                    {
                        this.m_client.Disconnect();
                        goto IL_228;
                    }
                    goto IL_228;
                Block_11:
                    Array.Copy(packetBuf, num2, packetBuf, 0, num5);
                    this.m_client.PacketBufSize = num5;
                IL_332:
                    if (num - 1 == num2)
                    {
                        packetBuf[0] = packetBuf[num2];
                        this.m_client.PacketBufSize = 1;
                    }
                IL_228: ;
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
        public static byte[] cloneArrary(byte[] arr, int length = 8)
        {
            byte[] array = new byte[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = arr[i];
            }
            return array;
        }
        public static string PrintArray(byte[] arr, int length = 8)
        {
            byte[] array = new byte[length];
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = 0; i < length; i++)
            {
                stringBuilder.AppendFormat("{0} ", arr[i]);
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
        public static string PrintArray(byte[] arr, int first, int length)
        {
            byte[] array = new byte[length];
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = first; i < first + length; i++)
            {
                stringBuilder.AppendFormat("{0} ", arr[i]);
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }
        public static byte[] decryptBytes(byte[] param1, int curOffset, int param2, byte[] param3)
        {
            byte[] array = new byte[param2];
            for (int i = 0; i < param2; i++)
            {
                array[i] = param1[i];
            }
            for (int i = 0; i < param2; i++)
            {
                if (i > 0)
                {
                    param3[i % 8] = (byte)((int)(param3[i % 8] + param1[curOffset + i - 1]) ^ i);
                    array[i] = (byte)(param1[curOffset + i] - param1[curOffset + i - 1] ^ param3[i % 8]);
                }
                else
                {
                    array[0] = (byte)(param1[curOffset] ^ param3[0]);
                }
            }
            return array;
        }
        public void Dispose()
        {
            this.send_event.Dispose();
            this.m_tcpQueue.Clear();
        }
    }
}
