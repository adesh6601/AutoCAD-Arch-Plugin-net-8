﻿namespace Component
{
	public class AcadObject
	{
		public string? DisplayName { get; set; }

		public string? ObjectId { get; set; }

		public Dictionary<string, HashSet<string>> DivisionsAndLevels { get; set; } = new Dictionary<string, HashSet<string>>();

		public List<Point> Bounds { get; set; } = new List<Point>();
	}
}
