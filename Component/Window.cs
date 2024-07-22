namespace Component
{
	public class Window : AcadObject
	{
		public string Description { get; set; }

		public string WallId { get; set; }

		public double Width { get; set; }
		public double Height { get; set; }
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public double Altitude { get; set; }
		public Point Normal { get; set; }
		public double Offset { get; set; }

		public string CollisionType { get; set; }

		public string Style { get; set; }

		public string MaterialName { get; set; }

		public Window() : base() { }
	}
}
