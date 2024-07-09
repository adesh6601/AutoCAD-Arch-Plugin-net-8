using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json.Linq;
using Collection;
using Model;

namespace Plugin
{
	public class Plugin
	{
		public static string ProjectPath { get; set; } = "C:\\Users\\Adesh Lad\\Documents\\Autodesk\\My Projects\\Sample Project 2024\\Sample Project.apj";
		public static string ViewsPath { get; set; }

		Reader Reader;
		Builder Builder;
		Formatter Formatter;
		Writer Writer;

		Entities Entities;
		Site Site;

		JObject SiteJson;
		JObject ProjectProperties;

		[CommandMethod("InitiateReadProject")]
		public void InitiateReadProject()
		{
			Reader = new Reader();
			Builder = new Builder();
			Formatter = new Formatter();
			Writer = new Writer();

			Entities = new Entities();
			Site = new Site();

			SiteJson = new JObject();
			ProjectProperties = new JObject();

			Reader.ReadProject(ref ProjectProperties, ProjectPath, Entities);

			Builder.Build(Entities, Site);

			Formatter.Format(ProjectProperties, Site, SiteJson);

			Writer.Write(SiteJson, "F:\\adesh_workspace\\SH-repos\\AutoCAD-Arch-plugin-repo\\SiteProject25.json");

			//MessageBox.Show("Data Extracted Successfully");
		}

		[CommandMethod("InitiateReadView")]
		public void InitiateReadView()
		{
			Reader = new Reader();
			Builder = new Builder();
			Formatter = new Formatter();
			Writer = new Writer();

			Entities = new Entities();
			Site = new Site();

			SiteJson = new JObject();
			ProjectProperties = new JObject();

			Reader.ReadViews(ref ProjectProperties, ProjectPath, ViewsPath, Entities);

			Builder.Build(Entities, Site);

			Formatter.Format(ProjectProperties, Site, SiteJson);

			Writer.Write(SiteJson, "F:\\adesh_workspace\\SH-repos\\AutoCAD-Arch-plugin-repo\\SiteView25.json");

			//MessageBox.Show("Data Extracted Successfully");
		}
	}
}
