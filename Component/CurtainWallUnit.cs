namespace Component
{
	public class CurtainWallUnit : AcadObject
	{
		public int CellCount { get; set; }

		public string Description { get; set; }

		public double Length { get; set; }
		public double Width { get; set; }
		public double BaseHeight { get; set; }
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public double Rotation { get; set; }

		public string CollisionType { get; set; }

		public string Style { get; set; }

		public string MaterialName { get; set; }

		// public MaterialColor Ambient { get; set; }
		// public double ColorBleedScale { get; set; }
		// public double IndirectBumpScale { get; set; }
		// public double Luminance { get; set; }
		// public double ReflectanceScale { get; set; }
		// public double Reflectivity { get; set; }
		// public double SelfIllumination { get; set; }
		// public double Translucence { get; set; }
		// public double TransmittanceScale { get; set; }
		// public bool TwoSided { get; set; }


		public CurtainWallUnit() : base() { }
	}
}
