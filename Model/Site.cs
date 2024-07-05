namespace Model
{
    public class Site
    {
        public Dictionary<string, Building> Buildings { get; set; } = new Dictionary<string, Building>();
    }
}