namespace Component
{
	public class Opening : ACADObject
	{
		public string ShapeType { get; set; }
		public string LineTypeID { get; set; }

		public string WallId { get; set; }

		public double Width { get; set; }
		public double Height { get; set; }
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public Point Normal { get; set; }
		public double Offset { get; set; }

		public string CollisionType { get; set; }


		public Opening() : base() { }
	}
}
