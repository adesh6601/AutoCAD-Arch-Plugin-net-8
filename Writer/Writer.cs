using Newtonsoft.Json.Linq;

public class Writer
{
    public Writer() { }

    public void Write(JObject site, string filePath)
    {
        string siteJson = ConvertJObjectToString(site);
        WriteToFile(filePath, siteJson);
    }
    public string ConvertJObjectToString(JObject jObject)
    {
        // Convert JObject to a formatted (indented) JSON string
        return jObject.ToString(Newtonsoft.Json.Formatting.Indented);
    }

    public void WriteToFile(string filePath, string text)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(text);
            }
            Console.WriteLine("Text written to file successfully.");
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("An error occurred while writing to the file: " + ex.Message);
        }
    }
}
