using System;
using System.Drawing;
namespace Game.Logic.Phy.Maths
{
	public static class PointHelper
	{
		public static Point Normalize(this Point point, int len)
		{
			double num = point.Length();
			return new Point((int)((double)(point.X * len) / num), (int)((double)(point.Y * len) / num));
		}
		public static double Length(this Point point)
		{
			return Math.Sqrt((double)(point.X * point.X + point.Y * point.Y));
		}
		public static double Distance(this Point point, Point target)
		{
			int num = point.X - target.X;
			int num2 = point.Y - target.Y;
			return Math.Sqrt((double)(num * num + num2 * num2));
		}
		public static double Distance(this Point point, int tx, int ty)
		{
			int num = point.X - tx;
			int num2 = point.Y - ty;
			return Math.Sqrt((double)(num * num + num2 * num2));
		}
		public static PointF Normalize(this PointF point, float len)
		{
			double num = Math.Sqrt((double)(point.X * point.X + point.Y * point.Y));
			return new PointF((float)((double)(point.X * len) / num), (float)((double)(point.Y * len) / num));
		}
		public static double Length(this PointF point)
		{
			return Math.Sqrt((double)(point.X * point.X + point.Y * point.Y));
		}
	}
}
