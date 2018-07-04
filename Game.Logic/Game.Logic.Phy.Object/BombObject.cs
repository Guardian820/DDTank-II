using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using System;
using System.Drawing;
namespace Game.Logic.Phy.Object
{
	public class BombObject : Physics
	{
		private float m_mass;
		private float m_gravityFactor;
		private float m_windFactor;
		private float m_airResitFactor;
		private EulerVector m_vx;
		private EulerVector m_vy;
		private float m_arf;
		private float m_gf;
		private float m_wf;
		public float vX
		{
			get
			{
				return this.m_vx.x1;
			}
		}
		public float vY
		{
			get
			{
				return this.m_vy.x1;
			}
		}
		public float Arf
		{
			get
			{
				return this.m_arf;
			}
		}
		public float Gf
		{
			get
			{
				return this.m_gf;
			}
		}
		public float Wf
		{
			get
			{
				return this.m_wf;
			}
		}
		public BombObject(int id, float mass, float gravityFactor, float windFactor, float airResitFactor) : base(id)
		{
			this.m_mass = mass;
			this.m_gravityFactor = gravityFactor;
			this.m_windFactor = windFactor;
			this.m_airResitFactor = airResitFactor;
			this.m_vx = new EulerVector(0, 0, 0f);
			this.m_vy = new EulerVector(0, 0, 0f);
			this.m_rect = new Rectangle(-3, -3, 6, 6);
		}
		public void setSpeedXY(int vx, int vy)
		{
			this.m_vx.x1 = (float)vx;
			this.m_vy.x1 = (float)vy;
		}
		public override void SetXY(int x, int y)
		{
			base.SetXY(x, y);
			this.m_vx.x0 = (float)x;
			this.m_vy.x0 = (float)y;
		}
		public override void SetMap(Map map)
		{
			base.SetMap(map);
			this.UpdateAGW();
		}
		protected void UpdateForceFactor(float air, float gravity, float wind)
		{
			this.m_airResitFactor = air;
			this.m_gravityFactor = gravity;
			this.m_windFactor = wind;
			this.UpdateAGW();
		}
		private void UpdateAGW()
		{
			if (this.m_map != null)
			{
				this.m_arf = this.m_map.airResistance * this.m_airResitFactor;
				this.m_gf = this.m_map.gravity * this.m_gravityFactor * this.m_mass;
				this.m_wf = this.m_map.wind * this.m_windFactor;
			}
		}
		protected Point CompleteNextMovePoint(float dt)
		{
			this.m_vx.ComputeOneEulerStep(this.m_mass, this.m_arf, this.m_wf, dt);
			this.m_vy.ComputeOneEulerStep(this.m_mass, this.m_arf, this.m_gf, dt);
			return new Point((int)this.m_vx.x0, (int)this.m_vy.x0);
		}
		public void MoveTo(int px, int py)
		{
			if (px != this.m_x || py != this.m_y)
			{
				int num = px - this.m_x;
				int num2 = py - this.m_y;
				bool flag;
				int num3;
				int num4;
				if (Math.Abs(num) > Math.Abs(num2))
				{
					flag = true;
					num3 = Math.Abs(num);
					num4 = num / num3;
				}
				else
				{
					flag = false;
					num3 = Math.Abs(num2);
					num4 = num2 / num3;
				}
				Point point = new Point(this.m_x, this.m_y);
				for (int i = 1; i <= num3; i += 3)
				{
					if (flag)
					{
						point = this.GetNextPointByX(this.m_x, px, this.m_y, py, this.m_x + i * num4);
					}
					else
					{
						point = this.GetNextPointByY(this.m_x, px, this.m_y, py, this.m_y + i * num4);
					}
					Rectangle rect = this.m_rect;
					rect.Offset(point.X, point.Y);
					Physics[] array = this.m_map.FindPhysicalObjects(rect, this);
					if (array.Length > 0)
					{
						base.SetXY(point.X, point.Y);
						this.CollideObjects(array);
					}
					else
					{
						if (!this.m_map.IsRectangleEmpty(rect))
						{
							base.SetXY(point.X, point.Y);
							this.CollideGround();
						}
						else
						{
							if (this.m_map.IsOutMap(point.X, point.Y))
							{
								base.SetXY(point.X, point.Y);
								this.FlyoutMap();
							}
						}
					}
					if (!this.m_isLiving || !this.m_isMoving)
					{
						return;
					}
				}
				base.SetXY(px, py);
			}
		}
		protected virtual void CollideObjects(Physics[] list)
		{
		}
		protected virtual void CollideGround()
		{
			this.StopMoving();
		}
		protected virtual void FlyoutMap()
		{
			this.StopMoving();
			if (this.m_isLiving)
			{
				this.Die();
			}
		}
		private Point GetNextPointByX(int x1, int x2, int y1, int y2, int x)
		{
			if (x2 == x1)
			{
				return new Point(x, y1);
			}
			return new Point(x, (x - x1) * (y2 - y1) / (x2 - x1) + y1);
		}
		private Point GetNextPointByY(int x1, int x2, int y1, int y2, int y)
		{
			if (y2 == y1)
			{
				return new Point(x1, y);
			}
			return new Point((y - y1) * (x2 - x1) / (y2 - y1) + x1, y);
		}
	}
}
