using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json.Linq;
using Collection;
using Model;

namespace Plugin
{
	public class Plugin
	{
		public static string ProjectPath { get; set; }
		public static string ViewsPath { get; set; }
		public static string OutputFilePath { get; set; }

		[CommandMethod("InitiateReadProject")]
		public void InitiateReadProject()
		{
            Reader Reader = new Reader();
            Builder Builder = new Builder();
            Formatter Formatter = new Formatter();
            Writer Writer = new Writer();

            Entities Entities = new Entities();
            Site Site = new Site();

            JObject SiteJson = new JObject();
            JObject ProjectProperties = new JObject();

			Reader.ReadProject(ref ProjectProperties, ProjectPath, Entities);

			Builder.Build(Entities, Site);

			Formatter.Format(ProjectProperties, Site, SiteJson);

			OutputFilePath = System.IO.Path.GetDirectoryName(ProjectPath) + "\\" + "Site.json";

			Writer.Write(SiteJson, OutputFilePath);

			System.Windows.MessageBox.Show("Data Extracted Successfully");
		}

		[CommandMethod("InitiateReadView")]
		public void InitiateReadView()
		{
            Reader Reader = new Reader();
            Builder Builder = new Builder();
            Formatter Formatter = new Formatter();
            Writer Writer = new Writer();

            Entities Entities = new Entities();
            Site Site = new Site();

            JObject SiteJson = new JObject();
            JObject ProjectProperties = new JObject();

            Reader.ReadViews(ref ProjectProperties, ProjectPath, ViewsPath, Entities);

			Builder.Build(Entities, Site);

			Formatter.Format(ProjectProperties, Site, SiteJson);

			OutputFilePath = System.IO.Path.GetDirectoryName(ProjectPath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(ViewsPath) + ".json";

			Writer.Write(SiteJson, OutputFilePath);

			System.Windows.MessageBox.Show("Data Extracted Successfully");
		}
	}
}