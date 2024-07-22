namespace Component
{
	public class WindowAssembly : AcadObject
	{
		public int CellCount { get; set; }

		public string Description { get; set; }
		
		public string WallId { get; set; }

		public double Length { get; set; }
		public double Height { get; set; }
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public Point Normal { get; set; }
		public double Offset { get; set; }

		public string CollisionType { get; set; }

		public string Style { get; set; }

		public string MaterialName { get; set; }

		public WindowAssembly() : base() { }
	}
}
