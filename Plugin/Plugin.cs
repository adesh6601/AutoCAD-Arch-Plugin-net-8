﻿using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json.Linq;
using Collection;
using Model;

namespace Plugin
{
	public class Plugin
	{
		public static string ProjectPath { get; set; } = "C:\\Users\\Adesh Lad\\Documents\\Autodesk\\My Projects\\Sample Project 2025\\Sample Project.apj";
		public static string ViewsPath { get; set; }
		public static string OutputFilePath { get; set; }

		Reader Reader;
		Formatter Formatter;

		Entities Entities;
		Site Site;

		JObject SiteJson;
		JObject ProjectProperties;


		[CommandMethod("InitiateReadProject")]
		public void InitiateReadProject()
		{
			Reader = new Reader();
			Formatter = new Formatter();

            Entities Entities = new Entities();
            Site Site = new Site();

            JObject SiteJson = new JObject();
            JObject ProjectProperties = new JObject();

			Reader.ReadProject(ref ProjectProperties, ProjectPath, Entities);

			Builder.Build(Entities, Site);

			Formatter.Format(ProjectProperties, Site, SiteJson);

			OutputFilePath += "\\" + "Site.json";

			Writer.Write(SiteJson, OutputFilePath);

			System.Windows.MessageBox.Show("Data Extracted Successfully");
		}

		[CommandMethod("InitiateReadView")]
		public void InitiateReadView()
		{
			Reader = new Reader();
			Formatter = new Formatter();
 
            Entities Entities = new Entities();
            Site Site = new Site();

            JObject SiteJson = new JObject();
            JObject ProjectProperties = new JObject();

            Reader.ReadViews(ref ProjectProperties, ProjectPath, ViewsPath, Entities);

			Builder.Build(Entities, Site);

			Formatter.Format(ProjectProperties, Site, SiteJson);

			OutputFilePath += "\\" + System.IO.Path.GetFileNameWithoutExtension(ViewsPath) + ".json";

			Writer.Write(SiteJson, OutputFilePath);

			System.Windows.MessageBox.Show("Data Extracted Successfully");
		}
	}
}
