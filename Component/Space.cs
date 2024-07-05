using System.Collections.Generic;

namespace Component
{
	public class Space : ACADObject
	{
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public List<Wall> Walls { get; set; }
		public List<List<List<double>>> Surfaces { get; set; }
		public List<List<List<double>>> TranslatedSurfaces { get; set; }


        public Space() : base() { }
	}
}
