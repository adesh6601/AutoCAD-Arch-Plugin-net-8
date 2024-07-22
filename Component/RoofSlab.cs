using System.Collections.Generic;

namespace Component
{
	public class RoofSlab : ACADObject
	{
		public double LowPoint { get; set; }
		public double HighPoint { get; set; }

		public Point Location { get; set; }

		public double HorizontalOffset { get; set; }
		public double InteriorFaceOffset { get; set; }

		public Point Normal { get; set; }
		public double Rotation { get; set; }
		public double Slop { get; set; }

		public List<List<List<double>>> Holes { get; set; }
		public List<List<double>> SlabLoop { get; set; }

		public string Style { get; set; }


		public RoofSlab() : base() { }
	}
}
