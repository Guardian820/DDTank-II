using Game.Logic.Phy.Maps;
using System;
using System.Drawing;
namespace Game.Logic.Phy.Object
{
	public class Physics
	{
		protected int m_id;
		protected Map m_map;
		protected int m_x;
		protected int m_y;
		protected Rectangle m_rect;
		protected Rectangle m_rectBomb;
		protected bool m_isLiving;
		protected bool m_isMoving;
		public int Id
		{
			get
			{
				return this.m_id;
			}
		}
		public Rectangle Bound
		{
			get
			{
				return this.m_rect;
			}
		}
		public Rectangle Bound1
		{
			get
			{
				return this.m_rectBomb;
			}
		}
		public bool IsMoving
		{
			get
			{
				return this.m_isMoving;
			}
		}
		public bool IsLiving
		{
			get
			{
				return this.m_isLiving;
			}
			set
			{
				this.m_isLiving = value;
			}
		}
		public virtual int X
		{
			get
			{
				return this.m_x;
			}
		}
		public virtual int Y
		{
			get
			{
				return this.m_y;
			}
		}
		public Physics(int id)
		{
			this.m_id = id;
			this.m_rect = new Rectangle(-5, -5, 10, 10);
			this.m_rectBomb = new Rectangle(0, 0, 0, 0);
			this.m_isLiving = true;
		}
		public virtual Point GetCollidePoint()
		{
			return new Point(this.X, this.Y);
		}
		public void SetRect(int x, int y, int width, int height)
		{
			this.m_rect.X = x;
			this.m_rect.Y = y;
			this.m_rect.Width = width;
			this.m_rect.Height = height;
		}
		public void SetRectBomb(int x, int y, int width, int height)
		{
			this.m_rectBomb.X = x;
			this.m_rectBomb.Y = y;
			this.m_rectBomb.Width = width;
			this.m_rectBomb.Height = height;
		}
		public virtual void SetXY(int x, int y)
		{
			this.m_x = x;
			this.m_y = y;
		}
		public void SetXY(Point p)
		{
			this.SetXY(p.X, p.Y);
		}
		public virtual void SetMap(Map map)
		{
			this.m_map = map;
		}
		public virtual void StartMoving()
		{
			if (this.m_map != null)
			{
				this.m_isMoving = true;
			}
		}
		public virtual void StopMoving()
		{
			this.m_isMoving = false;
		}
		public virtual void CollidedByObject(Physics phy)
		{
		}
		public virtual void Die()
		{
			this.StopMoving();
			this.m_isLiving = false;
		}
		public double Distance(int x, int y)
		{
			return Math.Sqrt((double)((this.m_x - x) * (this.m_x - x) + (this.m_y - y) * (this.m_y - y)));
		}
		public static int PointToLine(int x1, int y1, int x2, int y2, int px, int py)
		{
			int num = y1 - y2;
			int num2 = x2 - x1;
			int num3 = x1 * y2 - x2 * y1;
			return (int)((double)Math.Abs(num * px + num2 * py + num3) / Math.Sqrt((double)(num * num + num2 * num2)));
		}
		public virtual void PrepareNewTurn()
		{
		}
		public virtual void Dispose()
		{
			if (this.m_map != null)
			{
				this.m_map.RemovePhysical(this);
			}
		}
	}
}
