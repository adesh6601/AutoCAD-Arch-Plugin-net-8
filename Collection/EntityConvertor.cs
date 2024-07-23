using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.Aec.Arch.Geometry;
using Autodesk.Aec.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Collection
{
	public static class EntityConvertor
	{
		public static object ConvertEntity(object entity, string entityType, double unitConversionFactor)
		{
			if (entityType == "wall")
			{
				Wall wall = (Wall)entity;
				Component.Wall convertedWall = new Component.Wall();

				ConvertWall(wall, convertedWall, unitConversionFactor);
				return convertedWall;
			}

			if (entityType == "curtainWallLayout")
			{
				CurtainWallLayout curtainWallLayout = (CurtainWallLayout)entity;
				Component.CurtainWallLayout convertedCurtainWall = new Component.CurtainWallLayout();

				ConvertCurtainWallLayout(curtainWallLayout, convertedCurtainWall, unitConversionFactor);
				return convertedCurtainWall;
			}

			if (entityType == "curtainWallUnit")
			{
				CurtainWallUnit curtainWallUnit = (CurtainWallUnit)entity;
				Component.CurtainWallUnit convertedCurtainWallUnit = new Component.CurtainWallUnit();

				ConvertCurtainWallUnit(curtainWallUnit, convertedCurtainWallUnit, unitConversionFactor);
				return convertedCurtainWallUnit;
			}

			if (entityType == "window")
			{
				Window window = (Window)entity;
				Component.Window convertedWindow = new Component.Window();

				ConvertWindow(window, convertedWindow, unitConversionFactor);
				return convertedWindow;
			}

			if (entityType == "windowAssembly")
			{
				WindowAssembly windowAssembly = (WindowAssembly)entity;
				Component.WindowAssembly convertedWindowAssembly = new Component.WindowAssembly();

				ConvertWindowAssembly(windowAssembly, convertedWindowAssembly, unitConversionFactor);
				return convertedWindowAssembly;
			}

			if (entityType == "door")
			{
				Door door = (Door)entity;
				Component.Door convertedDoor = new Component.Door();

				ConvertDoor(door, convertedDoor, unitConversionFactor);
				return convertedDoor;
			}

			if (entityType == "opening")
			{
				Opening opening = (Opening)entity;
				Component.Opening convertedOpening = new Component.Opening();

				ConvertOpening(opening, convertedOpening, unitConversionFactor);
				return convertedOpening;
			}

			if (entityType == "slab")
			{
				Slab slab = (Slab)entity;
				Component.Slab convertedSlab = new Component.Slab();

				ConvertSlab(slab, convertedSlab, unitConversionFactor);
				return convertedSlab;
			}

			if (entityType == "roofSlab")
			{
				RoofSlab slab = (RoofSlab)entity;
				Component.RoofSlab convertedSlab = new Component.RoofSlab();

				ConvertRoofSlab(slab, convertedSlab, unitConversionFactor);
				return convertedSlab;
			}

			if (entityType == "space")
			{
				Space space = (Space)entity;

				if (space.Surfaces.Count < 2)
				{
					return null;
				}

				Component.Space convertedSpace = new Component.Space();

				ConvertSpace(space, convertedSpace, unitConversionFactor);
				return convertedSpace;
			}

			if (entityType == "zone")
			{
				Zone zone = (Zone)entity;
				Component.Zone convertedZone = new Component.Zone();

				ConvertZone(zone, convertedZone, unitConversionFactor);
				return convertedZone;
			}

			return null;
		}

		private static void ConvertWall(Wall wall, Component.Wall convertedWall, double unitConversionFactor)
		{
			convertedWall.DisplayName = wall.DisplayName;

			convertedWall.ObjectId = wall.ObjectId.ToString();

			convertedWall.Bounds.Add(new Component.Point(wall.Bounds.Value.MaxPoint.X / unitConversionFactor, wall.Bounds.Value.MaxPoint.Y / unitConversionFactor, wall.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedWall.Bounds.Add(new Component.Point(wall.Bounds.Value.MinPoint.X / unitConversionFactor, wall.Bounds.Value.MinPoint.Y / unitConversionFactor, wall.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedWall.Description = wall.Description;

			convertedWall.Length = wall.Length / unitConversionFactor;
			convertedWall.Width = wall.Width / unitConversionFactor;
			convertedWall.BaseHeight = wall.BaseHeight / unitConversionFactor;

			convertedWall.StartPoint = new Component.Point(wall.StartPoint.X / unitConversionFactor, wall.StartPoint.Y / unitConversionFactor, wall.StartPoint.Z / unitConversionFactor);
			convertedWall.MidPoint = new Component.Point(wall.MidPoint.X / unitConversionFactor, wall.MidPoint.Y / unitConversionFactor, wall.MidPoint.Z / unitConversionFactor);
			convertedWall.EndPoint = new Component.Point(wall.EndPoint.X / unitConversionFactor, wall.EndPoint.Y / unitConversionFactor, wall.EndPoint.Z / unitConversionFactor);

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

		private static void ConvertCurtainWallLayout(CurtainWallLayout curtainWall, Component.CurtainWallLayout convertedCurtainWall, double unitConversionFactor)
		{
			convertedCurtainWall.DisplayName = curtainWall.DisplayName;

			convertedCurtainWall.ObjectId = curtainWall.ObjectId.ToString();

			convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MaxPoint.X / unitConversionFactor, curtainWall.Bounds.Value.MaxPoint.Y / unitConversionFactor, curtainWall.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MinPoint.X / unitConversionFactor, curtainWall.Bounds.Value.MinPoint.Y / unitConversionFactor, curtainWall.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedCurtainWall.CellCount = curtainWall.CellCount;

			convertedCurtainWall.Description = curtainWall.Description;

			convertedCurtainWall.Length = curtainWall.Length / unitConversionFactor;

			convertedCurtainWall.BaseHeight = curtainWall.BaseHeight / unitConversionFactor;

			convertedCurtainWall.StartPoint = new Component.Point(curtainWall.StartPoint.X / unitConversionFactor, curtainWall.StartPoint.Y / unitConversionFactor, curtainWall.StartPoint.Z / unitConversionFactor);
			convertedCurtainWall.EndPoint = new Component.Point(curtainWall.EndPoint.X / unitConversionFactor, curtainWall.EndPoint.Y / unitConversionFactor, curtainWall.EndPoint.Z / unitConversionFactor);

			convertedCurtainWall.CollisionType = curtainWall.CollisionType.ToString();

			try
			{
				var arc = (Arc)curtainWall.BaseCurve;

				Component.WallTypes.CurtainArcWall CurtainArcWallObject = new Component.WallTypes.CurtainArcWall();

				Component.Point Center = new Component.Point(arc.Center.X / unitConversionFactor, arc.Center.Y / unitConversionFactor, arc.Center.Z / unitConversionFactor);

				CurtainArcWallObject.Center = Center;
				CurtainArcWallObject.Radius = arc.Radius / unitConversionFactor;

				CurtainArcWallObject.StartAngle = arc.StartAngle;
				CurtainArcWallObject.EndAngle = arc.EndAngle;

				CurtainArcWallObject.Xaxis = new Component.Point(arc.Ecs.CoordinateSystem3d.Xaxis.X, arc.Ecs.CoordinateSystem3d.Xaxis.Y, arc.Ecs.CoordinateSystem3d.Xaxis.Z);
				CurtainArcWallObject.Yaxis = new Component.Point(arc.Ecs.CoordinateSystem3d.Yaxis.X, arc.Ecs.CoordinateSystem3d.Yaxis.Y, arc.Ecs.CoordinateSystem3d.Yaxis.Z);

				convertedCurtainWall.ArcWallObject = CurtainArcWallObject;
			}
			catch (Exception ex)
			{
				convertedCurtainWall.ArcWallObject = null;
			}
		}

		private static void ConvertCurtainWallUnit(CurtainWallUnit curtainWall, Component.CurtainWallUnit convertedCurtainWall, double unitConversionFactor)
		{
			convertedCurtainWall.DisplayName = curtainWall.DisplayName;

			convertedCurtainWall.ObjectId = curtainWall.ObjectId.ToString();

			convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MaxPoint.X / unitConversionFactor, curtainWall.Bounds.Value.MaxPoint.Y / unitConversionFactor, curtainWall.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MinPoint.X / unitConversionFactor, curtainWall.Bounds.Value.MinPoint.Y / unitConversionFactor, curtainWall.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedCurtainWall.CellCount = curtainWall.CellCount;

			convertedCurtainWall.Description = curtainWall.Description;

			convertedCurtainWall.Length = curtainWall.Length / unitConversionFactor;

			convertedCurtainWall.BaseHeight = curtainWall.Height / unitConversionFactor;

			convertedCurtainWall.StartPoint = new Component.Point(curtainWall.StartPoint.X / unitConversionFactor, curtainWall.StartPoint.Y / unitConversionFactor, curtainWall.StartPoint.Z / unitConversionFactor);
			convertedCurtainWall.EndPoint = new Component.Point(curtainWall.EndPoint.X / unitConversionFactor, curtainWall.EndPoint.Y / unitConversionFactor, curtainWall.EndPoint.Z / unitConversionFactor);

			convertedCurtainWall.CollisionType = curtainWall.CollisionType.ToString();
		}

		private static void ConvertWindow(Window window, Component.Window convertedWindow, double unitConversionFactor)
		{
			convertedWindow.DisplayName = window.DisplayName;

			convertedWindow.ObjectId = window.ObjectId.ToString();

			convertedWindow.Bounds.Add(new Component.Point(window.Bounds.Value.MaxPoint.X / unitConversionFactor, window.Bounds.Value.MaxPoint.Y / unitConversionFactor, window.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedWindow.Bounds.Add(new Component.Point(window.Bounds.Value.MinPoint.X / unitConversionFactor, window.Bounds.Value.MinPoint.Y / unitConversionFactor, window.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedWindow.Description = window.Description;

			convertedWindow.Width = window.Width / unitConversionFactor;
			convertedWindow.Height = window.Height / unitConversionFactor;
			convertedWindow.Area = window.Area / unitConversionFactor;

			convertedWindow.StartPoint = new Component.Point(window.StartPoint.X / unitConversionFactor, window.StartPoint.Y / unitConversionFactor, window.StartPoint.Z / unitConversionFactor);
			convertedWindow.EndPoint = new Component.Point(window.EndPoint.X / unitConversionFactor, window.EndPoint.Y / unitConversionFactor, window.EndPoint.Z / unitConversionFactor);

			convertedWindow.Altitude = window.StartPoint.Z / unitConversionFactor;
			convertedWindow.Normal = new Component.Point(window.Normal.X, window.Normal.Y, window.Normal.Z);

			convertedWindow.CollisionType = window.CollisionType.ToString();
		}

		private static void ConvertWindowAssembly(WindowAssembly windowAssembly, Component.WindowAssembly convertedWindowAssembly, double unitConversionFactor)
		{
			convertedWindowAssembly.DisplayName = windowAssembly.DisplayName;

			convertedWindowAssembly.ObjectId = windowAssembly.ObjectId.ToString();

			convertedWindowAssembly.Bounds.Add(new Component.Point(windowAssembly.Bounds.Value.MaxPoint.X / unitConversionFactor, windowAssembly.Bounds.Value.MaxPoint.Y / unitConversionFactor, windowAssembly.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedWindowAssembly.Bounds.Add(new Component.Point(windowAssembly.Bounds.Value.MinPoint.X / unitConversionFactor, windowAssembly.Bounds.Value.MinPoint.Y / unitConversionFactor, windowAssembly.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedWindowAssembly.CellCount = windowAssembly.CellCount;

			convertedWindowAssembly.Description = windowAssembly.Description;

			convertedWindowAssembly.Length = windowAssembly.Length / unitConversionFactor;
			convertedWindowAssembly.Height = windowAssembly.Height / unitConversionFactor;
			convertedWindowAssembly.Area = windowAssembly.Area / unitConversionFactor;

			convertedWindowAssembly.StartPoint = new Component.Point(windowAssembly.StartPoint.X / unitConversionFactor, windowAssembly.StartPoint.Y / unitConversionFactor, windowAssembly.StartPoint.Z / unitConversionFactor);
			convertedWindowAssembly.EndPoint = new Component.Point(windowAssembly.EndPoint.X / unitConversionFactor, windowAssembly.EndPoint.Y / unitConversionFactor, windowAssembly.EndPoint.Z / unitConversionFactor);

			convertedWindowAssembly.Normal = new Component.Point(windowAssembly.Normal.X, windowAssembly.Normal.Y, windowAssembly.Normal.Z);

			convertedWindowAssembly.CollisionType = windowAssembly.CollisionType.ToString();
		}

		private static void ConvertDoor(Door door, Component.Door convertedDoor, double unitConversionFactor)
		{
			convertedDoor.DisplayName = door.DisplayName;

			convertedDoor.ObjectId = door.ObjectId.ToString();

			convertedDoor.Bounds.Add(new Component.Point(door.Bounds.Value.MaxPoint.X / unitConversionFactor, door.Bounds.Value.MaxPoint.Y / unitConversionFactor, door.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedDoor.Bounds.Add(new Component.Point(door.Bounds.Value.MinPoint.X / unitConversionFactor, door.Bounds.Value.MinPoint.Y / unitConversionFactor, door.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedDoor.Description = door.Description;

			convertedDoor.Width = door.Width / unitConversionFactor;
			convertedDoor.Height = door.Height / unitConversionFactor;
			convertedDoor.Area = door.Area / unitConversionFactor;

			convertedDoor.StartPoint = new Component.Point(door.StartPoint.X / unitConversionFactor, door.StartPoint.Y / unitConversionFactor, door.StartPoint.Z / unitConversionFactor);
			convertedDoor.EndPoint = new Component.Point(door.EndPoint.X / unitConversionFactor, door.EndPoint.Y / unitConversionFactor, door.EndPoint.Z / unitConversionFactor);

			convertedDoor.Normal = new Component.Point(door.Normal.X, door.Normal.Y, door.Normal.Z);

			convertedDoor.CollisionType = door.CollisionType.ToString();
		}

		private static void ConvertOpening(Opening opening, Component.Opening convertedOpening, double unitConversionFactor)
		{
			convertedOpening.DisplayName = opening.DisplayName;

			convertedOpening.ObjectId = opening.ObjectId.ToString();

			convertedOpening.Bounds.Add(new Component.Point(opening.Bounds.Value.MaxPoint.X / unitConversionFactor, opening.Bounds.Value.MaxPoint.Y / unitConversionFactor, opening.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedOpening.Bounds.Add(new Component.Point(opening.Bounds.Value.MinPoint.X / unitConversionFactor, opening.Bounds.Value.MinPoint.Y / unitConversionFactor, opening.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedOpening.ShapeType = opening.ShapeType.ToString();
			convertedOpening.LineTypeID = opening.LinetypeId.ToString();

			convertedOpening.Width = opening.Width / unitConversionFactor;
			convertedOpening.Height = opening.Height / unitConversionFactor;
			convertedOpening.Area = opening.Area / unitConversionFactor;

			convertedOpening.StartPoint = new Component.Point(opening.StartPoint.X / unitConversionFactor, opening.StartPoint.Y / unitConversionFactor, opening.StartPoint.Z / unitConversionFactor);
			convertedOpening.EndPoint = new Component.Point(opening.EndPoint.X / unitConversionFactor, opening.EndPoint.Y / unitConversionFactor, opening.EndPoint.Z / unitConversionFactor);

			convertedOpening.Normal = new Component.Point(opening.Normal.X, opening.Normal.Y, opening.Normal.Z);

			convertedOpening.CollisionType = opening.CollisionType.ToString();
		}

		private static void ConvertSlab(Slab slab, Component.Slab convertedSlab, double unitConversionFactor)
		{
			convertedSlab.DisplayName = slab.DisplayName;

			convertedSlab.ObjectId = slab.ObjectId.ToString().Substring(1, slab.ObjectId.ToString().Length - 2);

			convertedSlab.LowPoint = slab.LowPoint / unitConversionFactor;
			convertedSlab.HighPoint = slab.HighPoint / unitConversionFactor;

			convertedSlab.Location = new Component.Point(slab.Location.X / unitConversionFactor, slab.Location.Y / unitConversionFactor, slab.Location.Z / unitConversionFactor);

			convertedSlab.HorizontalOffset = slab.HorizontalOffset / unitConversionFactor;
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
					point.Add(translatedVertices.X / unitConversionFactor);
					point.Add(translatedVertices.Y / unitConversionFactor);
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

		private static void ConvertRoofSlab(RoofSlab slab, Component.RoofSlab convertedSlab, double unitConversionFactor)
		{
			convertedSlab.DisplayName = slab.DisplayName;

			convertedSlab.ObjectId = slab.ObjectId.ToString().Substring(1, slab.ObjectId.ToString().Length - 2);

			convertedSlab.LowPoint = slab.LowPoint / unitConversionFactor;
			convertedSlab.HighPoint = slab.HighPoint / unitConversionFactor;

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

					point.Add(translatedVertices.X / unitConversionFactor);
					point.Add(translatedVertices.Y / unitConversionFactor);

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

		private static void ConvertSpace(Space space, Component.Space convertedSpace, double unitConversionFactor)
		{
			convertedSpace.DisplayName = space.DisplayName;

			convertedSpace.ObjectId = space.ObjectId.ToString();

			convertedSpace.Bounds.Add(new Component.Point(space.Bounds.Value.MaxPoint.X / unitConversionFactor, space.Bounds.Value.MaxPoint.Y / unitConversionFactor, space.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedSpace.Bounds.Add(new Component.Point(space.Bounds.Value.MinPoint.X / unitConversionFactor, space.Bounds.Value.MinPoint.Y / unitConversionFactor, space.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedSpace.Area = space.Area / unitConversionFactor;

			convertedSpace.StartPoint = new Component.Point(space.StartPoint.X / unitConversionFactor, space.StartPoint.Y / unitConversionFactor, space.StartPoint.Z / unitConversionFactor);
			convertedSpace.EndPoint = new Component.Point(space.EndPoint.X / unitConversionFactor, space.EndPoint.Y / unitConversionFactor, space.EndPoint.Z / unitConversionFactor);

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

				startPoint.Add(translatedVertices.X / unitConversionFactor);
				startPoint.Add(translatedVertices.Y / unitConversionFactor);

				EndPoint.Add(translatedEndVertices.X / unitConversionFactor);
				EndPoint.Add(translatedEndVertices.Y / unitConversionFactor);

				surfaceLoop.Add(startPoint);
				surfaceLoop.Add(EndPoint);

				surface.Add(surfaceLoop);
			}

			convertedSpace.Surfaces = surface;
		}

		private static void ConvertZone(Zone zone, Component.Zone convertedZone, double unitConversionFactor)
		{
			convertedZone.DisplayName = zone.DisplayName;

			convertedZone.ObjectId = zone.ObjectId.ToString();

			convertedZone.Bounds.Add(new Component.Point(zone.Bounds.Value.MaxPoint.X / unitConversionFactor, zone.Bounds.Value.MaxPoint.Y / unitConversionFactor, zone.Bounds.Value.MaxPoint.Z / unitConversionFactor));
			convertedZone.Bounds.Add(new Component.Point(zone.Bounds.Value.MinPoint.X / unitConversionFactor, zone.Bounds.Value.MinPoint.Y / unitConversionFactor, zone.Bounds.Value.MinPoint.Z / unitConversionFactor));

			convertedZone.Area = zone.Area / unitConversionFactor;

			convertedZone.StartPoint = new Component.Point(zone.StartPoint.X / unitConversionFactor, zone.StartPoint.Y / unitConversionFactor, zone.StartPoint.Z / unitConversionFactor);
			convertedZone.EndPoint = new Component.Point(zone.EndPoint.X / unitConversionFactor, zone.EndPoint.Y / unitConversionFactor, zone.EndPoint.Z / unitConversionFactor);

			convertedZone.TotalNumberOfSpaces = zone.TotalNumberOfSpaces;
			convertedZone.TotalNumberOfZones = zone.TotalNumberOfZones;
		}

		public static void ConvertWindowAssemblyAsWallToWall(Object oldWindowAssemblyAsWall, Component.Wall wall)
		{
			Component.WindowAssembly windowAssemblyAsWall = (Component.WindowAssembly)oldWindowAssemblyAsWall;

			wall.DisplayName = windowAssemblyAsWall.DisplayName;

			wall.ObjectId = windowAssemblyAsWall.ObjectId.ToString();

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

		private static string CheckSegmentType(Wall wall)
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
