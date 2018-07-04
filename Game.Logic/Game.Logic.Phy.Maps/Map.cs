using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
namespace Game.Logic.Phy.Maps
{
	public class Map
	{
		private MapInfo _info;
		private float _wind;
		private HashSet<Physics> _objects;
		protected Tile _layer1;
		protected Tile _layer2;
		protected Rectangle _bound;
		public float wind
		{
			get
			{
				return this._wind;
			}
			set
			{
				this._wind = value;
			}
		}
		public float gravity
		{
			get
			{
				return (float)this._info.Weight;
			}
		}
		public float airResistance
		{
			get
			{
				return (float)this._info.DragIndex;
			}
		}
		public Tile Ground
		{
			get
			{
				return this._layer1;
			}
		}
		public MapInfo Info
		{
			get
			{
				return this._info;
			}
		}
		public Rectangle Bound
		{
			get
			{
				return this._bound;
			}
		}
		public Map(MapInfo info, Tile layer1, Tile layer2)
		{
			this._info = info;
			this._objects = new HashSet<Physics>();
			this._layer1 = layer1;
			this._layer2 = layer2;
			if (this._layer1 != null)
			{
				this._bound = new Rectangle(0, 0, this._layer1.Width, this._layer1.Height);
				return;
			}
			this._bound = new Rectangle(0, 0, this._layer2.Width, this._layer2.Height);
		}
		public void Dig(int cx, int cy, Tile surface, Tile border)
		{
			if (this._layer1 != null)
			{
				this._layer1.Dig(cx, cy, surface, border);
			}
			if (this._layer2 != null)
			{
				this._layer2.Dig(cx, cy, surface, border);
			}
		}
		public bool IsEmpty(int x, int y)
		{
			return (this._layer1 == null || this._layer1.IsEmpty(x, y)) && (this._layer2 == null || this._layer2.IsEmpty(x, y));
		}
		public bool IsRectangleEmpty(Rectangle rect)
		{
			return (this._layer1 == null || this._layer1.IsRectangleEmptyQuick(rect)) && (this._layer2 == null || this._layer2.IsRectangleEmptyQuick(rect));
		}
		public Point FindYLineNotEmptyPoint(int x, int y, int h)
		{
			x = ((x < 0) ? 0 : ((x >= this._bound.Width) ? (this._bound.Width - 1) : x));
			y = ((y < 0) ? 0 : y);
			h = ((y + h >= this._bound.Height) ? (this._bound.Height - y - 1) : h);
			for (int i = 0; i < h; i++)
			{
				if (!this.IsEmpty(x - 1, y) || !this.IsEmpty(x + 1, y))
				{
					return new Point(x, y);
				}
				y++;
			}
			return Point.Empty;
		}
		public Point FindYLineNotEmptyPoint(int x, int y)
		{
			return this.FindYLineNotEmptyPoint(x, y, this._bound.Height);
		}
		public Point FindNextWalkPoint(int x, int y, int direction, int stepX, int stepY)
		{
			if (direction != 1 && direction != -1)
			{
				return Point.Empty;
			}
			int num = x + direction * stepX;
			if (num < 0 || num > this._bound.Width)
			{
				return Point.Empty;
			}
			Point point = this.FindYLineNotEmptyPoint(num, y - stepY - 1, stepY * 2 + 3);
			if (point != Point.Empty && Math.Abs(point.Y - y) > stepY)
			{
				point = Point.Empty;
			}
			return point;
		}
		public bool canMove(int x, int y)
		{
			return this.IsEmpty(x, y) && !this.IsOutMap(x, y);
		}
		public bool IsOutMap(int x, int y)
		{
			return x < 0 || x >= this._bound.Width || y >= this._bound.Height;
		}
		public void AddPhysical(Physics phy)
		{
			phy.SetMap(this);
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				this._objects.Add(phy);
			}
			finally
			{
				Monitor.Exit(objects);
			}
		}
		public void RemovePhysical(Physics phy)
		{
			phy.SetMap(null);
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				this._objects.Remove(phy);
			}
			finally
			{
				Monitor.Exit(objects);
			}
		}
		public List<Physics> GetAllPhysicalSafe()
		{
			List<Physics> list = new List<Physics>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					list.Add(current);
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<PhysicalObj> GetAllPhysicalObjSafe()
		{
			List<PhysicalObj> list = new List<PhysicalObj>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current is PhysicalObj)
					{
						list.Add(current as PhysicalObj);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public Physics[] FindPhysicalObjects(Rectangle rect, Physics except)
		{
			List<Physics> list = new List<Physics>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current.IsLiving && current != except)
					{
						Rectangle bound = current.Bound;
						Rectangle bound2 = current.Bound1;
						bound.Offset(current.X, current.Y);
						bound2.Offset(current.X, current.Y);
						if (bound.IntersectsWith(rect) || bound2.IntersectsWith(rect))
						{
							list.Add(current);
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list.ToArray();
		}
		public bool FindPlayers(Point p, int radius)
		{
			int num = 0;
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current is Player && current.IsLiving && (current as Player).BoundDistance(p) < (double)radius)
					{
						num++;
					}
					if (num >= 2)
					{
						return true;
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return false;
		}
		public List<Player> FindPlayers(int x, int y, int radius)
		{
			List<Player> list = new List<Player>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current is Player && current.IsLiving && current.Distance(x, y) < (double)radius)
					{
						list.Add(current as Player);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<Living> FindLivings(int x, int y, int radius)
		{
			List<Living> list = new List<Living>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current is Living && current.IsLiving && current.Distance(x, y) < (double)radius)
					{
						list.Add(current as Living);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<Living> FindPlayers(int fx, int tx, List<Player> exceptPlayers)
		{
			List<Living> list = new List<Living>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current is Player && current.IsLiving && current.X > fx && current.X < tx)
					{
						if (exceptPlayers != null)
						{
							using (List<Player>.Enumerator enumerator2 = exceptPlayers.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									Player current2 = enumerator2.Current;
									if (((Player)current).DefaultDelay != current2.DefaultDelay)
									{
										list.Add(current as Living);
									}
								}
								continue;
							}
						}
						list.Add(current as Living);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<Living> FindHitByHitPiont()
		{
			List<Living> list = new List<Living>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current is Living && current.IsLiving)
					{
						list.Add(current as Living);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<Living> FindHitByHitPiont(Point p, int radius)
		{
			List<Living> list = new List<Living>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current is Living && current.IsLiving && (current as Living).BoundDistance(p) < (double)radius)
					{
						list.Add(current as Living);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public Living FindNearestEnemy(int x, int y, double maxdistance, Living except)
		{
			Living result = null;
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics current in this._objects)
				{
					if (current is Living && current != except && current.IsLiving && ((Living)current).Team != except.Team)
					{
						double num = current.Distance(x, y);
						if (num < maxdistance)
						{
							result = (current as Living);
							maxdistance = num;
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return result;
		}
		public void Dispose()
		{
			foreach (Physics current in this._objects)
			{
				current.Dispose();
			}
		}
		public Map Clone()
		{
			Tile layer = (this._layer1 != null) ? this._layer1.Clone() : null;
			Tile layer2 = (this._layer2 != null) ? this._layer2.Clone() : null;
			return new Map(this._info, layer, layer2);
		}

        public List<NormalNpc> FindNpcIds(int fx, int tx, List<NormalNpc> exceptPlayers)
        {
            List<NormalNpc> list = new List<NormalNpc>();
            lock (_objects)
            {
                foreach (Physics phy in _objects)
                {
                    if (phy is NormalNpc && phy.IsLiving && phy.X > fx && phy.X < tx)
                    {
                        if (exceptPlayers != null)
                        {
                            foreach (NormalNpc player in exceptPlayers)
                            {
                                //if (((NormalNpc)phy).DefaultDelay != player.DefaultDelay)
                                //{
                                list.Add(phy as NormalNpc);
                                //}
                            }
                        }
                        else
                        {
                            list.Add(phy as NormalNpc);
                        }
                    }
                }
            }
            return list;
        }
	}
}
