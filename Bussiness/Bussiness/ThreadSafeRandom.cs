using System;
using System.Threading;
namespace Bussiness
{
	public class ThreadSafeRandom
	{
		private static Random randomStatic = new Random();
		private Random random = new Random();
		public static int NextStatic()
		{
			Random obj;
			Monitor.Enter(obj = ThreadSafeRandom.randomStatic);
			int result;
			try
			{
				result = ThreadSafeRandom.randomStatic.Next();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static int NextStatic(int maxValue)
		{
			Random obj;
			Monitor.Enter(obj = ThreadSafeRandom.randomStatic);
			int result;
			try
			{
				result = ThreadSafeRandom.randomStatic.Next(maxValue);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static int NextStatic(int minValue, int maxValue)
		{
			Random obj;
			Monitor.Enter(obj = ThreadSafeRandom.randomStatic);
			int result;
			try
			{
				result = ThreadSafeRandom.randomStatic.Next(minValue, maxValue);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static void NextStatic(byte[] keys)
		{
			Random obj;
			Monitor.Enter(obj = ThreadSafeRandom.randomStatic);
			try
			{
				ThreadSafeRandom.randomStatic.NextBytes(keys);
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public int Next()
		{
			Random obj;
			Monitor.Enter(obj = this.random);
			int result;
			try
			{
				result = this.random.Next();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public int Next(int maxValue)
		{
			Random obj;
			Monitor.Enter(obj = this.random);
			int result;
			try
			{
				result = this.random.Next(maxValue);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public int Next(int minValue, int maxValue)
		{
			Random obj;
			Monitor.Enter(obj = this.random);
			int result;
			try
			{
				result = this.random.Next(minValue, maxValue);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
	}
}
