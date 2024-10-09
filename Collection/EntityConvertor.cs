using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.Aec.Arch.Geometry;
using Autodesk.Aec.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Collection
{
	public class EntityConvertor
	{
		private double UnitConversionFactor;

		public object ConvertEntity(object entity, string entityType, double unitConversionFactor)
		{

            UnitConversionFactor = unitConversionFactor;

			if (entityType == "wall")
			{
				Wall wall = (Wall)entity;
				Component.Wall convertedWall = new Component.Wall();

				ConvertWall(wall, convertedWall);
				return convertedWall;
			}

			if (entityType == "curtainWallLayout")
			{
				CurtainWallLayout curtainWallLayout = (CurtainWallLayout)entity;
				Component.CurtainWallLayout convertedCurtainWall = new Component.CurtainWallLayout();

				ConvertCurtainWallLayout(curtainWallLayout, convertedCurtainWall);
				return convertedCurtainWall;
			}

			if (entityType == "curtainWallUnit")
			{
				CurtainWallUnit curtainWallUnit = (CurtainWallUnit)entity;
				Component.CurtainWallUnit convertedCurtainWallUnit = new Component.CurtainWallUnit();

				ConvertCurtainWallUnit(curtainWallUnit, convertedCurtainWallUnit);
				return convertedCurtainWallUnit;
			}

			if (entityType == "window")
			{
				Window window = (Window)entity;
				Component.Window convertedWindow = new Component.Window();

				ConvertWindow(window, convertedWindow);
				return convertedWindow;
			}

			if (entityType == "windowAssembly")
			{
				WindowAssembly windowAssembly = (WindowAssembly)entity;
				Component.WindowAssembly convertedWindowAssembly = new Component.WindowAssembly();

				ConvertWindowAssembly(windowAssembly, convertedWindowAssembly);
				return convertedWindowAssembly;
			}

			if (entityType == "door")
			{
				Door door = (Door)entity;
				Component.Door convertedDoor = new Component.Door();

				ConvertDoor(door, convertedDoor);
				return convertedDoor;
			}

			if (entityType == "opening")
			{
				Opening opening = (Opening)entity;
				Component.Opening convertedOpening = new Component.Opening();

				ConvertOpening(opening, convertedOpening);
				return convertedOpening;
			}

			if (entityType == "slab")
			{
				Slab slab = (Slab)entity;
				Component.Slab convertedSlab = new Component.Slab();

				ConvertSlab(slab, convertedSlab);
				return convertedSlab;
			}

			if (entityType == "roofSlab")
			{
				RoofSlab slab = (RoofSlab)entity;
				Component.RoofSlab convertedSlab = new Component.RoofSlab();

				ConvertRoofSlab(slab, convertedSlab);
				return convertedSlab;
			}

			if (entityType == "space")
			{
				Space space = (Space)entity;

				if (space.Surfaces.Count < 2)
				{
#pragma warning disable CS8603 // Possible null reference return.
                    return null;
#pragma warning restore CS8603 // Possible null reference return.
                }

				Component.Space convertedSpace = new Component.Space();

				ConvertSpace(space, convertedSpace);
				return convertedSpace;
			}

			if (entityType == "zone")
			{
				Zone zone = (Zone)entity;
				Component.Zone convertedZone = new Component.Zone();

				ConvertZone(zone, convertedZone);
				return convertedZone;
			}

#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

		public void ConvertWall(Wall wall, Component.Wall convertedWall)
		{
			convertedWall.DisplayName = wall.DisplayName;

			convertedWall.ObjectId = wall.ObjectId.ToString();

			if(wall.Bounds.HasValue)
			{
                convertedWall.Bounds.Add(new Component.Point(wall.Bounds.Value.MaxPoint.X / UnitConversionFactor, wall.Bounds.Value.MaxPoint.Y / UnitConversionFactor, wall.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedWall.Bounds.Add(new Component.Point(wall.Bounds.Value.MinPoint.X / UnitConversionFactor, wall.Bounds.Value.MinPoint.Y / UnitConversionFactor, wall.Bounds.Value.MinPoint.Z / UnitConversionFactor));
            }

            convertedWall.Description = wall.Description;

			convertedWall.Length = wall.Length / UnitConversionFactor;
			convertedWall.Width = wall.Width / UnitConversionFactor;
			convertedWall.BaseHeight = wall.BaseHeight / UnitConversionFactor;


			convertedWall.StartPoint = new Component.Point(wall.StartPoint.X / UnitConversionFactor, wall.StartPoint.Y / UnitConversionFactor, wall.StartPoint.Z / UnitConversionFactor);
			convertedWall.MidPoint = new Component.Point(wall.MidPoint.X / UnitConversionFactor, wall.MidPoint.Y / UnitConversionFactor, wall.MidPoint.Z / UnitConversionFactor);
			convertedWall.EndPoint = new Component.Point(wall.EndPoint.X / UnitConversionFactor, wall.EndPoint.Y / UnitConversionFactor, wall.EndPoint.Z / UnitConversionFactor);


			convertedWall.CollisionType = wall.CollisionType.ToString();

			convertedWall.SegmentType = CheckSegmentType(wall);

			if (convertedWall.SegmentType == "Arc")
			{
				Component.WallTypes.ArcWall ArcWallObject = new Component.WallTypes.ArcWall();

				ArcWallObject.StartPoint = convertedWall.StartPoint;
				ArcWallObject.EndPoint = convertedWall.EndPoint;
				ArcWallObject.PointOnArc = convertedWall.MidPoint;
				convertedWall.ArcWallObject = ArcWallObject;
			}
		}

		public void ConvertCurtainWallLayout(CurtainWallLayout curtainWall, Component.CurtainWallLayout convertedCurtainWall)
		{
			convertedCurtainWall.DisplayName = curtainWall.DisplayName;

			convertedCurtainWall.ObjectId = curtainWall.ObjectId.ToString();

			if (curtainWall.Bounds.HasValue)
			{
                convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MaxPoint.X / UnitConversionFactor, curtainWall.Bounds.Value.MaxPoint.Y / UnitConversionFactor, curtainWall.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MinPoint.X / UnitConversionFactor, curtainWall.Bounds.Value.MinPoint.Y / UnitConversionFactor, curtainWall.Bounds.Value.MinPoint.Z / UnitConversionFactor));
            }

            convertedCurtainWall.CellCount = curtainWall.CellCount;

			convertedCurtainWall.Description = curtainWall.Description;

			convertedCurtainWall.Length = curtainWall.Length / UnitConversionFactor;

			convertedCurtainWall.BaseHeight = curtainWall.BaseHeight / UnitConversionFactor;

			convertedCurtainWall.StartPoint = new Component.Point(curtainWall.StartPoint.X / UnitConversionFactor, curtainWall.StartPoint.Y / UnitConversionFactor, curtainWall.StartPoint.Z / UnitConversionFactor);
			convertedCurtainWall.EndPoint = new Component.Point(curtainWall.EndPoint.X / UnitConversionFactor, curtainWall.EndPoint.Y / UnitConversionFactor, curtainWall.EndPoint.Z / UnitConversionFactor);

			convertedCurtainWall.CollisionType = curtainWall.CollisionType.ToString();
            try
            {
				var arc = (Arc)curtainWall.BaseCurve;

				Component.WallTypes.CurtainArcWall CurtainArcWallObject = new Component.WallTypes.CurtainArcWall();

				Component.Point Center = new Component.Point(arc.Center.X / UnitConversionFactor, arc.Center.Y / UnitConversionFactor, arc.Center.Z / UnitConversionFactor);

				CurtainArcWallObject.Center = Center;
				CurtainArcWallObject.Radius = arc.Radius / UnitConversionFactor;

				CurtainArcWallObject.StartAngle = arc.StartAngle;
				CurtainArcWallObject.EndAngle = arc.EndAngle;

				CurtainArcWallObject.Xaxis = new Component.Point(arc.Ecs.CoordinateSystem3d.Xaxis.X, arc.Ecs.CoordinateSystem3d.Xaxis.Y, arc.Ecs.CoordinateSystem3d.Xaxis.Z);
				CurtainArcWallObject.Yaxis = new Component.Point(arc.Ecs.CoordinateSystem3d.Yaxis.X, arc.Ecs.CoordinateSystem3d.Yaxis.Y, arc.Ecs.CoordinateSystem3d.Yaxis.Z);

				convertedCurtainWall.ArcWallObject = CurtainArcWallObject;
			}
			catch (Exception)
            {
				convertedCurtainWall.ArcWallObject = null;
			}
		}

		public void ConvertCurtainWallUnit(CurtainWallUnit curtainWall, Component.CurtainWallUnit convertedCurtainWall)
		{
			convertedCurtainWall.DisplayName = curtainWall.DisplayName;

			convertedCurtainWall.ObjectId = curtainWall.ObjectId.ToString();

			if(curtainWall.Bounds.HasValue)
			{
                convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MaxPoint.X / UnitConversionFactor, curtainWall.Bounds.Value.MaxPoint.Y / UnitConversionFactor, curtainWall.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MinPoint.X / UnitConversionFactor, curtainWall.Bounds.Value.MinPoint.Y / UnitConversionFactor, curtainWall.Bounds.Value.MinPoint.Z / UnitConversionFactor));
            }

            convertedCurtainWall.CellCount = curtainWall.CellCount;

			convertedCurtainWall.Description = curtainWall.Description;

			convertedCurtainWall.Length = curtainWall.Length / UnitConversionFactor;

			convertedCurtainWall.BaseHeight = curtainWall.Height / UnitConversionFactor;

			convertedCurtainWall.StartPoint = new Component.Point(curtainWall.StartPoint.X / UnitConversionFactor, curtainWall.StartPoint.Y / UnitConversionFactor, curtainWall.StartPoint.Z / UnitConversionFactor);
			convertedCurtainWall.EndPoint = new Component.Point(curtainWall.EndPoint.X / UnitConversionFactor, curtainWall.EndPoint.Y / UnitConversionFactor, curtainWall.EndPoint.Z / UnitConversionFactor);

			convertedCurtainWall.CollisionType = curtainWall.CollisionType.ToString();
		}

		public void ConvertWindow(Window window, Component.Window convertedWindow)
		{
			convertedWindow.DisplayName = window.DisplayName;

			convertedWindow.ObjectId = window.ObjectId.ToString();

			if(window.Bounds.HasValue)
			{
                convertedWindow.Bounds.Add(new Component.Point(window.Bounds.Value.MaxPoint.X / UnitConversionFactor, window.Bounds.Value.MaxPoint.Y / UnitConversionFactor, window.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedWindow.Bounds.Add(new Component.Point(window.Bounds.Value.MinPoint.X / UnitConversionFactor, window.Bounds.Value.MinPoint.Y / UnitConversionFactor, window.Bounds.Value.MinPoint.Z / UnitConversionFactor));
            }

            convertedWindow.Description = window.Description;

			convertedWindow.Width = window.Width / UnitConversionFactor;
			convertedWindow.Height = window.Height / UnitConversionFactor;
			convertedWindow.Area = window.Area / UnitConversionFactor;

			convertedWindow.StartPoint = new Component.Point(window.StartPoint.X / UnitConversionFactor, window.StartPoint.Y / UnitConversionFactor, window.StartPoint.Z / UnitConversionFactor);
			convertedWindow.EndPoint = new Component.Point(window.EndPoint.X / UnitConversionFactor, window.EndPoint.Y / UnitConversionFactor, window.EndPoint.Z / UnitConversionFactor);

			convertedWindow.Altitude = window.StartPoint.Z / UnitConversionFactor;
			convertedWindow.Normal = new Component.Point(window.Normal.X, window.Normal.Y, window.Normal.Z);

			convertedWindow.CollisionType = window.CollisionType.ToString();
		}

		public void ConvertWindowAssembly(WindowAssembly windowAssembly, Component.WindowAssembly convertedWindowAssembly)
		{
			convertedWindowAssembly.DisplayName = windowAssembly.DisplayName;

			convertedWindowAssembly.ObjectId = windowAssembly.ObjectId.ToString();

			if(windowAssembly.Bounds.HasValue)
			{
                convertedWindowAssembly.Bounds.Add(new Component.Point(windowAssembly.Bounds.Value.MaxPoint.X / UnitConversionFactor, windowAssembly.Bounds.Value.MaxPoint.Y / UnitConversionFactor, windowAssembly.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedWindowAssembly.Bounds.Add(new Component.Point(windowAssembly.Bounds.Value.MinPoint.X / UnitConversionFactor, windowAssembly.Bounds.Value.MinPoint.Y / UnitConversionFactor, windowAssembly.Bounds.Value.MinPoint.Z / UnitConversionFactor));
            }

            convertedWindowAssembly.CellCount = windowAssembly.CellCount;

			convertedWindowAssembly.Description = windowAssembly.Description;

			convertedWindowAssembly.Length = windowAssembly.Length / UnitConversionFactor;
			convertedWindowAssembly.Height = windowAssembly.Height / UnitConversionFactor;
			convertedWindowAssembly.Area = windowAssembly.Area / UnitConversionFactor;

			convertedWindowAssembly.StartPoint = new Component.Point(windowAssembly.StartPoint.X / UnitConversionFactor, windowAssembly.StartPoint.Y / UnitConversionFactor, windowAssembly.StartPoint.Z / UnitConversionFactor);
			convertedWindowAssembly.EndPoint = new Component.Point(windowAssembly.EndPoint.X / UnitConversionFactor, windowAssembly.EndPoint.Y / UnitConversionFactor, windowAssembly.EndPoint.Z / UnitConversionFactor);

			convertedWindowAssembly.Normal = new Component.Point(windowAssembly.Normal.X, windowAssembly.Normal.Y, windowAssembly.Normal.Z);

			convertedWindowAssembly.CollisionType = windowAssembly.CollisionType.ToString();
		}

		public void ConvertDoor(Door door, Component.Door convertedDoor)
		{
			convertedDoor.DisplayName = door.DisplayName;

			convertedDoor.ObjectId = door.ObjectId.ToString();

			if(door.Bounds.HasValue)
			{
                convertedDoor.Bounds.Add(new Component.Point(door.Bounds.Value.MaxPoint.X / UnitConversionFactor, door.Bounds.Value.MaxPoint.Y / UnitConversionFactor, door.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedDoor.Bounds.Add(new Component.Point(door.Bounds.Value.MinPoint.X / UnitConversionFactor, door.Bounds.Value.MinPoint.Y / UnitConversionFactor, door.Bounds.Value.MinPoint.Z / UnitConversionFactor));
            }

            convertedDoor.Description = door.Description;

			convertedDoor.Width = door.Width / UnitConversionFactor;
			convertedDoor.Height = door.Height / UnitConversionFactor;
			convertedDoor.Area = door.Area / UnitConversionFactor;

			convertedDoor.StartPoint = new Component.Point(door.StartPoint.X / UnitConversionFactor, door.StartPoint.Y / UnitConversionFactor, door.StartPoint.Z / UnitConversionFactor);
			convertedDoor.EndPoint = new Component.Point(door.EndPoint.X / UnitConversionFactor, door.EndPoint.Y / UnitConversionFactor, door.EndPoint.Z / UnitConversionFactor);

			convertedDoor.Normal = new Component.Point(door.Normal.X, door.Normal.Y, door.Normal.Z);

			convertedDoor.CollisionType = door.CollisionType.ToString();
		}

		public void ConvertOpening(Opening opening, Component.Opening convertedOpening)
		{
			convertedOpening.DisplayName = opening.DisplayName;

			convertedOpening.ObjectId = opening.ObjectId.ToString();

			if(opening.Bounds.HasValue)
			{
                convertedOpening.Bounds.Add(new Component.Point(opening.Bounds.Value.MaxPoint.X / UnitConversionFactor, opening.Bounds.Value.MaxPoint.Y / UnitConversionFactor, opening.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedOpening.Bounds.Add(new Component.Point(opening.Bounds.Value.MinPoint.X / UnitConversionFactor, opening.Bounds.Value.MinPoint.Y / UnitConversionFactor, opening.Bounds.Value.MinPoint.Z / UnitConversionFactor));
            }

            convertedOpening.ShapeType = opening.ShapeType.ToString();
			convertedOpening.LineTypeID = opening.LinetypeId.ToString();

			convertedOpening.Width = opening.Width / UnitConversionFactor;
			convertedOpening.Height = opening.Height / UnitConversionFactor;
			convertedOpening.Area = opening.Area / UnitConversionFactor;

			convertedOpening.StartPoint = new Component.Point(opening.StartPoint.X / UnitConversionFactor, opening.StartPoint.Y / UnitConversionFactor, opening.StartPoint.Z / UnitConversionFactor);
			convertedOpening.EndPoint = new Component.Point(opening.EndPoint.X / UnitConversionFactor, opening.EndPoint.Y / UnitConversionFactor, opening.EndPoint.Z / UnitConversionFactor);

			convertedOpening.Normal = new Component.Point(opening.Normal.X, opening.Normal.Y, opening.Normal.Z);

			convertedOpening.CollisionType = opening.CollisionType.ToString();
		}

		public void ConvertSlab(Slab slab, Component.Slab convertedSlab)
		{
			convertedSlab.DisplayName = slab.DisplayName;

			convertedSlab.ObjectId = slab.ObjectId.ToString().Substring(1, slab.ObjectId.ToString().Length - 2);

			convertedSlab.LowPoint = slab.LowPoint / UnitConversionFactor;
			convertedSlab.HighPoint = slab.HighPoint / UnitConversionFactor;

			convertedSlab.Location = new Component.Point(slab.Location.X / UnitConversionFactor, slab.Location.Y / UnitConversionFactor, slab.Location.Z / UnitConversionFactor);

			convertedSlab.HorizontalOffset = slab.HorizontalOffset / UnitConversionFactor;
			convertedSlab.InteriorFaceOffset = slab.InteriorFaceOffset;

			convertedSlab.Normal = new Component.Point(slab.Normal.X, slab.Normal.Y, slab.Normal.Z);
			convertedSlab.Rotation = slab.Rotation;
			convertedSlab.Slop = slab.Slope;

			List<List<List<double>>> loops = new List<List<List<double>>>();
			double area = double.MinValue;
			List<List<double>> slabLoop = new List<List<double>>();

			foreach (SlabLoop loop in slab.Face.Loops)
			{
				List<List<double>> loopVertices = new List<List<double>>();

				foreach (CompoundCurve2dVertex vertex in loop.Vertices)
				{
					Point3d translatedVertices = new Point3d(vertex.Point.X, vertex.Point.Y, 0.0);
					translatedVertices = translatedVertices.TransformBy(slab.Ecs);
					List<double> point = new List<double>();
					point.Add(translatedVertices.X / UnitConversionFactor);
					point.Add(translatedVertices.Y / UnitConversionFactor);
					loopVertices.Add(point);
				}

				if (area < Math.Abs(loop.Area))
				{
					if (!loops.Contains(slabLoop) && slabLoop.Count != 0)
					{
						loops.Add(slabLoop);
					}
					slabLoop = loopVertices;
					area = Math.Abs(loop.Area);
				}
				else
				{
					loops.Add(loopVertices);
				}
			}
			loops.Remove(slabLoop);

			convertedSlab.Holes = loops;
			convertedSlab.SlabLoop = slabLoop;
		}

		public void ConvertRoofSlab(RoofSlab slab, Component.RoofSlab convertedSlab)
		{
			convertedSlab.DisplayName = slab.DisplayName;

			convertedSlab.ObjectId = slab.ObjectId.ToString().Substring(1, slab.ObjectId.ToString().Length - 2);

			convertedSlab.LowPoint = slab.LowPoint / UnitConversionFactor;
			convertedSlab.HighPoint = slab.HighPoint / UnitConversionFactor;

			convertedSlab.Location = new Component.Point(slab.Location.X, slab.Location.Y, slab.Location.Z);

			convertedSlab.HorizontalOffset = slab.HorizontalOffset;
			convertedSlab.InteriorFaceOffset = slab.InteriorFaceOffset;

			convertedSlab.Normal = new Component.Point(slab.Normal.X, slab.Normal.Y, slab.Normal.Z);
			convertedSlab.Rotation = slab.Rotation;
			convertedSlab.Slop = slab.Slope;

			double area = double.MinValue;
			List<List<List<double>>> loops = new List<List<List<double>>>();
			List<List<double>> slabLoop = new List<List<double>>();

			foreach (SlabLoop loop in slab.Face.Loops)
			{
				List<List<double>> loopVertices = new List<List<double>>();

				foreach (CompoundCurve2dVertex vertex in loop.Vertices)
				{
					Point3d translatedVertices = new Point3d(vertex.Point.X, vertex.Point.Y, 0.0);
					translatedVertices = translatedVertices.TransformBy(slab.Ecs);
					List<double> point = new List<double>();

					point.Add(translatedVertices.X / UnitConversionFactor);
					point.Add(translatedVertices.Y / UnitConversionFactor);

					loopVertices.Add(point);
				}
				if (area < Math.Abs(loop.Area))
				{
					if (!loops.Contains(slabLoop) && slabLoop.Count != 0)
					{
						loops.Add(slabLoop);
					}
					slabLoop = loopVertices;
					area = Math.Abs(loop.Area);
				}
				else
				{
					loops.Add(loopVertices);
				}
			}
			loops.Remove(slabLoop);

			convertedSlab.Holes = loops;
			convertedSlab.SlabLoop = slabLoop;
		}

		public void ConvertSpace(Space space, Component.Space convertedSpace)
		{
			convertedSpace.DisplayName = space.DisplayName;

			convertedSpace.ObjectId = space.ObjectId.ToString();

			if(space.Bounds.HasValue)
			{
                convertedSpace.Bounds.Add(new Component.Point(space.Bounds.Value.MaxPoint.X / UnitConversionFactor, space.Bounds.Value.MaxPoint.Y / UnitConversionFactor, space.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedSpace.Bounds.Add(new Component.Point(space.Bounds.Value.MinPoint.X / UnitConversionFactor, space.Bounds.Value.MinPoint.Y / UnitConversionFactor, space.Bounds.Value.MinPoint.Z / UnitConversionFactor));

            }

            convertedSpace.Area = space.Area / UnitConversionFactor;

			convertedSpace.StartPoint = new Component.Point(space.StartPoint.X / UnitConversionFactor, space.StartPoint.Y / UnitConversionFactor, space.StartPoint.Z / UnitConversionFactor);
			convertedSpace.EndPoint = new Component.Point(space.EndPoint.X / UnitConversionFactor, space.EndPoint.Y / UnitConversionFactor, space.EndPoint.Z / UnitConversionFactor);

			List<List<List<double>>> surface = new List<List<List<double>>>();
			foreach (var entity in space.Surfaces)
			{
				List<List<double>> surfaceLoop = new List<List<double>>();
				SpaceSurface wall = (SpaceSurface)entity;

				Point3d translatedVertices = new Point3d(wall.StartPoint.X, wall.StartPoint.Y, 0.0);
				Point3d translatedEndVertices = new Point3d(wall.EndPoint.X, wall.EndPoint.Y, 0.0);
				translatedVertices = translatedVertices.TransformBy(space.Ecs);
				translatedEndVertices = translatedEndVertices.TransformBy(space.Ecs);

				List<double> startPoint = new List<double>();
				List<double> EndPoint = new List<double>();

				startPoint.Add(translatedVertices.X / UnitConversionFactor);
				startPoint.Add(translatedVertices.Y / UnitConversionFactor);

				EndPoint.Add(translatedEndVertices.X / UnitConversionFactor);
				EndPoint.Add(translatedEndVertices.Y / UnitConversionFactor);

				surfaceLoop.Add(startPoint);
				surfaceLoop.Add(EndPoint);

				surface.Add(surfaceLoop);
			}

			convertedSpace.Surfaces = surface;
		}

		public void ConvertZone(Zone zone, Component.Zone convertedZone)
		{
			convertedZone.DisplayName = zone.DisplayName;

			convertedZone.ObjectId = zone.ObjectId.ToString();

			if(zone.Bounds.HasValue)
			{
                convertedZone.Bounds.Add(new Component.Point(zone.Bounds.Value.MaxPoint.X / UnitConversionFactor, zone.Bounds.Value.MaxPoint.Y / UnitConversionFactor, zone.Bounds.Value.MaxPoint.Z / UnitConversionFactor));
                convertedZone.Bounds.Add(new Component.Point(zone.Bounds.Value.MinPoint.X / UnitConversionFactor, zone.Bounds.Value.MinPoint.Y / UnitConversionFactor, zone.Bounds.Value.MinPoint.Z / UnitConversionFactor));

            }

            convertedZone.Area = zone.Area / UnitConversionFactor;

			convertedZone.StartPoint = new Component.Point(zone.StartPoint.X / UnitConversionFactor, zone.StartPoint.Y / UnitConversionFactor, zone.StartPoint.Z / UnitConversionFactor);
			convertedZone.EndPoint = new Component.Point(zone.EndPoint.X / UnitConversionFactor, zone.EndPoint.Y / UnitConversionFactor, zone.EndPoint.Z / UnitConversionFactor);

			convertedZone.TotalNumberOfSpaces = zone.TotalNumberOfSpaces;
			convertedZone.TotalNumberOfZones = zone.TotalNumberOfZones;
		}

		public void CovertWindowAssemblyAsWallToWall(Object oldWindowAssemblyAsWall, Component.Wall wall)
		{
			Component.WindowAssembly windowAssemblyAsWall = (Component.WindowAssembly)oldWindowAssemblyAsWall;

            if (windowAssemblyAsWall != null )
            {
                wall.DisplayName = windowAssemblyAsWall.DisplayName;

                if (windowAssemblyAsWall.ObjectId != null) { wall.ObjectId = windowAssemblyAsWall.ObjectId.ToString(); }
                
                wall.DivisionsAndLevels = windowAssemblyAsWall.DivisionsAndLevels;

                wall.Bounds = windowAssemblyAsWall.Bounds;

                wall.Description = windowAssemblyAsWall.Description;

                wall.Length = windowAssemblyAsWall.Length;
                wall.BaseHeight = windowAssemblyAsWall.Height;
					
                wall.StartPoint = new Component.Point(windowAssemblyAsWall.StartPoint.X, windowAssemblyAsWall.StartPoint.Y, windowAssemblyAsWall.StartPoint.Z);
                wall.EndPoint = new Component.Point(windowAssemblyAsWall.EndPoint.X, windowAssemblyAsWall.EndPoint.Y, windowAssemblyAsWall.EndPoint.Z);

                wall.CollisionType = windowAssemblyAsWall.CollisionType.ToString();

                wall.Style = windowAssemblyAsWall.Style;

                wall.MaterialName = windowAssemblyAsWall.MaterialName;
            }

			
		}

		public static string CheckSegmentType(Wall wall)
		{
			Curve3d curveType = wall.BaseCurve(true);

			if (curveType is CircularArc3d)
			{
				return "Arc";
			}
			else if (curveType is LineSegment3d)
			{
				return "Line";
			}
			return "";
		}
	}
}
