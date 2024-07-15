using Component;

namespace Collection
{
	public class Entities
	{
		public List<CurtainWallLayout> CurtainWallLayouts { get; set; } = new List<CurtainWallLayout>();
		public List<CurtainWallUnit> CurtainWallUnits { get; set; } = new List<CurtainWallUnit>();

		public List<Door> Doors { get; set; } = new List<Door>();
		public List<Opening> Openings { get; set; } = new List<Opening>();

		public List<Wall> Walls { get; set; } = new List<Wall>();
		public List<Window> Windows { get; set; } = new List<Window>();

		public List<WindowAssembly> WindowAssemblies { get; set; } = new List<WindowAssembly>();
		public List<WindowAssembly> WindowAssembliesAsWall { get; set; } = new List<WindowAssembly>();
		
		public List<Slab> Slabs { get; set; } = new List<Slab>();
		public List<RoofSlab> RoofSlabs { get; set; } = new List<RoofSlab>();

		public List<Space> Spaces { get; set; } = new List<Space>();
		public List<Zone> Zones { get; set; } = new List<Zone>();
	}
}
