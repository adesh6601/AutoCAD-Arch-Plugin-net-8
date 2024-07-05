namespace Model
{
	public class Building
	{
		public Dictionary<string, Floor> Floors { get; set; } = new Dictionary<string, Floor>();
	}
}
