using System.Diagnostics;
using Autodesk.AutoCAD.Runtime;
using Collection;
using Model;
using Newtonsoft.Json.Linq;

namespace Plugin
{
    public class Plugin
    {
        public static string ProjectPath = "C:\\Users\\Adesh Lad\\Documents\\Autodesk\\My Projects\\Sample Project 2025\\Sample Project.apj";

        Reader Reader = new Reader();
        Builder Builder = new Builder();
		Formatter Formatter = new Formatter();
        Writer Writer = new Writer();

        Entities Entities = new Entities();
        Site Site = new Site();
        JObject SiteJson = new JObject();

        JObject ProjectProperties = new JObject();

        [CommandMethod("Initiate")]
		public void InitiateDataCollection()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Reader.ReadDWGFile(ref ProjectProperties, ProjectPath, Entities);

			Builder.Build(Entities, Site);

			Formatter.Format(ProjectProperties, Site, SiteJson);

            Writer.Write(SiteJson, "F:\\adesh_workspace\\SH-repos\\AutoCAD-Arch-plugin-repo\\Site25.json");

            stopwatch.Stop();
            
            // Log or display the elapsed time
            Writer.WriteToFile("timeTaken.txt", $"Time taken to execute InitiateDataCollection: {stopwatch.ElapsedMilliseconds} ms");
        }
	}
}
