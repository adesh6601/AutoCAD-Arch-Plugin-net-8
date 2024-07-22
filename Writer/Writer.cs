using System.Reflection;
using Newtonsoft.Json.Linq;

public static class Writer
{
	public static void Write(JObject site, string filePath)
	{
		string siteJson = ConvertJObjectToString(site);
		WriteToFile(filePath, siteJson);
	}

	private static string ConvertJObjectToString(JObject jObject)
	{
		return jObject.ToString(Newtonsoft.Json.Formatting.None);
	}

	private static void WriteToFile(string filePath, string text)
	{
		try
		{
			using (StreamWriter writer = new StreamWriter(filePath))
			{
				writer.WriteLine(text);
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine("An error occurred while writing to the file: " + ex.Message);
		}
	}
}
