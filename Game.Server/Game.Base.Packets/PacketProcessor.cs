using Game.Base.Events;
using Game.Server;
using Game.Server.Packets.Client;
using log4net;
using System;
using System.Reflection;
using System.Threading;
namespace Game.Base.Packets
{
	public class PacketProcessor
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected IPacketHandler m_activePacketHandler;
		protected int m_handlerThreadID;
		protected GameClient m_client;
		protected static readonly IPacketHandler[] m_packetHandlers = new IPacketHandler[256];
		public PacketProcessor(GameClient client)
		{
			this.m_client = client;
		}
		public void HandlePacket(GSPacketIn packet)
		{
			int code = (int)packet.Code;
			Statistics.BytesIn += (long)packet.Length;
			Statistics.PacketsIn += 1L;
			IPacketHandler packetHandler = null;

            if (code < PacketProcessor.m_packetHandlers.Length)
            {
                packetHandler = PacketProcessor.m_packetHandlers[code];
                Console.WriteLine(string.Concat(new object[]
				{
					"ClientID: " + packet.ClientID,
					" received code: ",
					code,
					" <",
					string.Format("0x{0:x}", code),
					">"
				}));
                Console.WriteLine(" ==>" + packetHandler.ToString());
            }

			if (code < PacketProcessor.m_packetHandlers.Length)
			{
				packetHandler = PacketProcessor.m_packetHandlers[code];
				try
				{
					packetHandler.ToString();
					goto IL_157;
				}
				catch
				{
					Console.WriteLine("______________ERROR______________");
					Console.WriteLine(string.Concat(new object[]
					{
						"___ Received code: ",
						code,
						" <",
						string.Format("0x{0:x}", code),
						"> ____"
					}));
					Console.WriteLine("_________________________________");
					goto IL_157;
				}
			}
			if (PacketProcessor.log.IsErrorEnabled)
			{
				PacketProcessor.log.ErrorFormat("Received packet code is outside of m_packetHandlers array bounds! " + this.m_client.ToString(), new object[0]);
				PacketProcessor.log.Error(Marshal.ToHexDump(string.Format("===> <{2}> Packet 0x{0:X2} (0x{1:X2}) length: {3} (ThreadId={4})", new object[]
				{
					code,
					code ^ 168,
					this.m_client.TcpEndpoint,
					packet.Length,
					Thread.CurrentThread.ManagedThreadId
				}), packet.Buffer));
			}
			IL_157:
			if (packetHandler != null)
			{
				long num = (long)Environment.TickCount;
				try
				{
					if (this.m_client != null && packet != null && this.m_client.TcpEndpoint != "not connected")
					{
						packetHandler.HandlePacket(this.m_client, packet);
					}
				}
				catch (Exception exception)
				{
					if (PacketProcessor.log.IsErrorEnabled)
					{
						string tcpEndpoint = this.m_client.TcpEndpoint;
						PacketProcessor.log.Error(string.Concat(new string[]
						{
							"Error while processing packet (handler=",
							packetHandler.GetType().FullName,
							"  client: ",
							tcpEndpoint,
							")"
						}), exception);
						PacketProcessor.log.Error(Marshal.ToHexDump("Package Buffer:", packet.Buffer, 0, packet.Length));
					}
				}
				long num2 = (long)Environment.TickCount - num;
				this.m_activePacketHandler = null;
				if (PacketProcessor.log.IsDebugEnabled)
				{
					PacketProcessor.log.Debug("Package process Time:" + num2 + "ms!");
				}
				if (num2 > 1000L)
				{
					string tcpEndpoint2 = this.m_client.TcpEndpoint;
					if (PacketProcessor.log.IsWarnEnabled)
					{
						PacketProcessor.log.Warn(string.Concat(new object[]
						{
							"(",
							tcpEndpoint2,
							") Handle packet Thread ",
							Thread.CurrentThread.ManagedThreadId,
							" ",
							packetHandler,
							" took ",
							num2,
							"ms!"
						}));
					}
				}
			}
		}
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
		{
			Array.Clear(PacketProcessor.m_packetHandlers, 0, PacketProcessor.m_packetHandlers.Length);
			int num = PacketProcessor.SearchPacketHandlers("v168", Assembly.GetAssembly(typeof(GameServer)));
			if (PacketProcessor.log.IsInfoEnabled)
			{
				PacketProcessor.log.Info("PacketProcessor: Loaded " + num + " handlers from GameServer Assembly!");
			}
		}
		public static void RegisterPacketHandler(int packetCode, IPacketHandler handler)
		{
			PacketProcessor.m_packetHandlers[packetCode] = handler;
		}
		protected static int SearchPacketHandlers(string version, Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsClass && type.GetInterface("Game.Server.Packets.Client.IPacketHandler") != null)
				{
					PacketHandlerAttribute[] array = (PacketHandlerAttribute[])type.GetCustomAttributes(typeof(PacketHandlerAttribute), true);
					if (array.Length > 0)
					{
						num++;
						PacketProcessor.RegisterPacketHandler(array[0].Code, (IPacketHandler)Activator.CreateInstance(type));
					}
				}
			}
			return num;
		}
	}
}
