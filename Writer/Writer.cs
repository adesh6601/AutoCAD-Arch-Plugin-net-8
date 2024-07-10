﻿using System.Reflection;
using Newtonsoft.Json.Linq;

public class Writer
{
	public void Write(JObject site, string filePath)
	{
		string siteJson = ConvertJObjectToString(site);
		WriteToFile(filePath, siteJson);
	}

	public string ConvertJObjectToString(JObject jObject)
	{
		return jObject.ToString(Newtonsoft.Json.Formatting.None);
	}

	public void WriteToFile(string filePath, string text)
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
