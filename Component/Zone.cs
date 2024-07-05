using System.Collections.Generic;

namespace Component
{
	public class Zone : ACADObject
	{
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public int TotalNumberOfSpaces { get; set; }
		public int TotalNumberOfZones { get; set; }

		public List<Space> Spaces { get; set; }
		public List<string> SpaceIds { get; set; }
		public List<string> ZoneIds { get; set; }


		public Zone() : base() { }
	}
}
