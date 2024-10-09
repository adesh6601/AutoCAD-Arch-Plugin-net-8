using System.Collections.Specialized;
using System.Xml;
using Newtonsoft.Json.Linq;
using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.Aec.DatabaseServices;
using Autodesk.Aec.Project;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.InteropHelpers;
using Collection;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

public class Reader
{

    private EntityConvertor Convertor = new EntityConvertor();

    private Project Project;
    private ProjectFile ProjectFile { get; set; }
    private ProjectFile[] ProjectFiles;

    private Document Document;
    private dynamic OpenedDoc;
    private Database Database;

    private Transaction Txn;
    private BlockTableRecord BlockTableRecord;
    private StringCollection xRefs = new StringCollection();

    private UnitsValue Unit { get; set; }
    private double UnitConversionFactor { get; set; }

    private bool IsxRefFormConstruct { get; set; }

    private Dictionary<string, HashSet<string>> DivisionsAndLevels = new Dictionary<string, HashSet<string>>();

	const string Level = "Level";
    const string Division = "Division";
    const string Window = "window";
    const string WindowAssembly = "windowAssembly";
    const string Opening = "opening";
    const string Door = "door";
    const string CurtainWallLayout = "curtainWallLayout";
    const string CurtainWallUnit = "curtainWallUnit";
    const string Slab = "slab";
    const string RoofSlab = "roofSlab";
    const string Space = "space";
    const string Zone = "zone";
    const string Wall = "wall";

    public void ReadProject(ref JObject ProjectProperties, string projectPath, Entities entities)
	{
		OpenProject(projectPath);
		SetProjectFiles();

		IsxRefFormConstruct = false;

		foreach (ProjectFile projectFile in ProjectFiles)
		{
			if (projectFile.Name == "")
			{
				continue;
			}
			DivisionsAndLevels.Clear();

			SetDivisionsAndLevels(projectFile.FileFullPath);

			ReadEntitiesForProject(projectFile, entities);
			xRefs.Clear();
			SetXRefs(projectFile);

			foreach (string xRef in xRefs)
			{
				//Need to Check work for all the cases.
				SetProjectFile(projectPath, xRef);

				if (ProjectFile == null) continue;

				ReadEntitiesForProject(ProjectFile, entities);
				ReSetProjectFile();
			}

		}
		ProjectProperties = GetProjectProperties(projectPath);
	}

	public void ReadViews(ref JObject ProjectProperties, string projectPath, string viewPath, Entities entities)
	{
		OpenProject(projectPath);
		SetViewFiles();

		foreach (ProjectFile projectFile in ProjectFiles)
		{
			if (projectFile.Name == "" || !projectFile.DrawingFullPath.Equals(viewPath, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			StringCollection xRefsForView = new StringCollection();

			xRefs.Clear();
			SetXRefs(projectFile);

			foreach (string item in xRefs)
			{
				xRefsForView.Add(item);
			}

			foreach (string xRef in xRefsForView)
			{
				xRefs.Clear();
				IsxRefFormConstruct = true;

				//Need to Check work for all the cases.
				SetProjectFile(projectPath, xRef);

				if (ProjectFile == null)
				{
					continue;
				}

				ReadEntitiesForViews(ProjectFile, entities);

				SetXRefs(ProjectFile);
				ReSetProjectFile();

				if (xRefs.Count == 0)
				{
					continue;
				}

				foreach (string subxRef in xRefs)
				{
					IsxRefFormConstruct = false;
					//Need to Check work for all the cases.
					SetProjectFile(projectPath, subxRef);

					if (ProjectFile == null) continue;

					ReadEntitiesForProject(ProjectFile, entities);
					ReSetProjectFile();
				}
			}
		}
		ProjectProperties = GetProjectProperties(projectPath);
	}

	public JObject GetProjectProperties(string projectPath)
	{
		JObject projectInformation = new JObject();

		if (!File.Exists(projectPath))
			return projectInformation;

		SetUnitConversionFactor();

		using (XmlReader reader = XmlReader.Create(projectPath))
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == Level)
				{
					if (!projectInformation.ContainsKey(Level))
					{
						projectInformation[Level] = new JObject();
					}

					JObject keyValuePairs = (JObject)projectInformation[Level];
					JObject levelProperties = new JObject
					{
						["Height"] = Convert.ToString(Convert.ToInt64(reader.GetAttribute("Height")) / UnitConversionFactor),
						["Elevation"] = Convert.ToString(Convert.ToInt64(reader.GetAttribute("Elevation")) / UnitConversionFactor),
					};

					keyValuePairs[reader.GetAttribute("Id")] = levelProperties;
				}

				if (reader.NodeType == XmlNodeType.Element && reader.Name == Division)
				{
					if (!projectInformation.ContainsKey(Division))
					{
						projectInformation[Division] = new JObject();
					}

					JObject keyValuePairs = (JObject)projectInformation[Division];
					JObject divisionProperties = new JObject
					{
						["ScheduleId"] = reader.GetAttribute("ScheduleId"),
						["Description"] = reader.GetAttribute("Description"),
						["Name"] = reader.GetAttribute("Name")
					};

					keyValuePairs[reader.GetAttribute("Id")] = divisionProperties;
				}
			}
		}

		return projectInformation;
	}

	public void SetProjectFile(string projectPath, string xRef)
	{
		string path = GetTructedPath(xRef);
		string directoryPath = Path.GetDirectoryName(projectPath);
		path = Path.Combine(directoryPath, path);

		ProjectFile[] files;

		if (IsxRefFormConstruct)
		{
			files = Project.GetConstructs();
		}
		else
		{
			files = Project.GetElements();
		}

		foreach (var file in files)
		{
			if (file.Name == "")
			{
				continue;
			}

			//Need to Implement the logic for Furniture Removal
			if (file.DrawingFullPath.IndexOf("Furniture", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				continue;
			}

			if (string.Equals(file.DrawingFullPath, path, StringComparison.OrdinalIgnoreCase))
			{
				ProjectFile = file;
				return;
			}
		}
	}

	public string GetTructedPath(string path)
	{
		int startIndex = path.IndexOf("Elements");

		// Check if the keyword is found in the string
		if (startIndex != -1)
		{
			// Extract the substring starting from the keyword
			string result = path.Substring(startIndex);
			return result;
		}

		startIndex = path.IndexOf("Constructs");

		// Check if the keyword is found in the string
		if (startIndex != -1)
		{
			// Extract the substring starting from the keyword
			string result = path.Substring(startIndex);
			return result;
		}

		return "";
	}

	public void ReSetProjectFile()
	{
		ProjectFile = null;
	}

	public void ReadEntitiesForProject(ProjectFile projectFile, Entities entities)
	{
		OpenFileInApp(projectFile);
		SetDatabase();
        SetTransaction();
		SetBlockTableRecorde();
		SetUnit();
		SetUnitConversionFactor();

		foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId objectId in BlockTableRecord)
		{
			object entity = GetEntity(objectId);
			if (entity == null)
			{
				continue;
			}

			string entityType = GetEntityType(entity);
			if (entityType == null || entityType == "")
			{
				continue;
			}

			object convertedEntity = Convertor.ConvertEntity(entity, entityType, UnitConversionFactor);
			if (convertedEntity == null)
			{
				continue;
			}

			AddEntityPosition(entityType, convertedEntity);
			AddEntityMaterial(entity, entityType, convertedEntity);
			AddEntityStyle(entity, entityType, convertedEntity);

			if (entityType == Opening || entityType == Door || entityType == Window || entityType == WindowAssembly)
			{
				AddEntityAnchor(entity, ref entityType, convertedEntity);
			}

			AddEntityToEntites(entityType, convertedEntity, entities);
		}
		ResetTransaction();
		CloseFileInApp();
	}

	public void ReadEntitiesForViews(ProjectFile projectFile, Entities entities)
	{
		DivisionsAndLevels.Clear();
		SetDivisionsAndLevels(projectFile.FileFullPath);

		if (DivisionsAndLevels.Count == 0) return;

		OpenFileInApp(projectFile);

		SetDatabase();
		SetTransaction();
		SetBlockTableRecorde();
		SetUnit();
		SetUnitConversionFactor();

		foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId objectId in BlockTableRecord)
		{
			object entity = GetEntity(objectId);
			if (entity == null)
			{
				continue;
			}

			string entityType = GetEntityType(entity);
			if (entityType == null || entityType == "")
			{
				continue;
			}

			object convertedEntity = Convertor.ConvertEntity(entity, entityType, UnitConversionFactor);
			if (convertedEntity == null)
			{
				continue;
			}

			AddEntityPosition(entityType, convertedEntity);
			AddEntityMaterial(entity, entityType, convertedEntity);
			AddEntityStyle(entity, entityType, convertedEntity);

			if (entityType == Opening || entityType == Door || entityType == Window || entityType == WindowAssembly)
			{
				AddEntityAnchor(entity, ref entityType, convertedEntity);
			}

			AddEntityToEntites(entityType, convertedEntity, entities);
		}
		ResetTransaction();
		CloseFileInApp();
	}

	public void OpenProject(string projectPath)
	{
		try
		{
			ProjectBaseServices projectBaseServices = ProjectBaseServices.Service;
			ProjectBaseManager projectManager = projectBaseServices.ProjectManager;
			Project = projectManager.OpenProject(OpenMode.ForRead, projectPath);
		}
		catch (Exception ex)
		{
			//System.Windows.MessageBox.Show(ex.Message);
		}

	}

	public void SetProjectFiles()
	{
		ProjectFiles = Project.GetConstructs();
	}

	public void SetViewFiles()
	{
		ProjectFiles = Project.GetViews();
	}

	public void SetDivisionsAndLevels(string filePath)
	{
		if (!File.Exists(filePath))
		{
			return;
		}

		using (XmlReader reader = XmlReader.Create(filePath))
		{
			while (reader.Read())
			{
				if (reader.NodeType != XmlNodeType.Element || reader.Name != "Cell")
				{
					continue;
				}

				string division = reader.GetAttribute(Division);
				string level = reader.GetAttribute(Level);

				if (!DivisionsAndLevels.ContainsKey(division))
				{
					DivisionsAndLevels[division] = new HashSet<string>();
				}

				DivisionsAndLevels[division].Add(level);
			}
		}

		Dictionary<string, HashSet<string>> UpdatedDivisionsAndLevels = new Dictionary<string, HashSet<string>>();
		foreach (string division in DivisionsAndLevels.Keys)
		{
			int currentMinLevel = 999999999;
			foreach (string level in DivisionsAndLevels[division])
			{
				currentMinLevel = Math.Min(currentMinLevel, int.Parse(level));
			}

			if (currentMinLevel != 999999999)
			{
				UpdatedDivisionsAndLevels[division] = new HashSet<string> { currentMinLevel.ToString() };
			}
		}

		DivisionsAndLevels = UpdatedDivisionsAndLevels;
	}

	public void SetXRefs(ProjectFile file)
	{
		if (file == null || !file.DwgExists)
		{
			return;
		}

		string dwgFullPath = file.DrawingFullPath;
		Database dwgDatabase = GetDbForFile(dwgFullPath);

		if (dwgDatabase == null)
		{
			return;
		}

		Autodesk.AutoCAD.DatabaseServices.ObjectId symTbId = dwgDatabase.BlockTableId;
		using (Transaction trans = dwgDatabase.TransactionManager.StartTransaction())
		{
			BlockTable bt = trans.GetObject(symTbId, OpenMode.ForRead, false) as BlockTable;
			foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId recId in bt)
			{
				BlockTableRecord btr = trans.GetObject(recId, OpenMode.ForRead) as BlockTableRecord;
				if (!btr.IsFromExternalReference)
					continue;

				xRefs.Add(btr.PathName);
			}
		}
	}

	public Database GetDbForFile(string dwgFullPath)
	{
		DocumentCollection docs = Application.DocumentManager;
		Document doc = null;

		foreach (Document elem in docs)
		{
			if (IsSamePath(elem.Database.Filename, dwgFullPath))
			{
				doc = elem;
				break;
			}
		}

		if (doc != null)
		{
			return doc.Database;
		}

		Database db = new Database(false, true);
		db.ReadDwgFile(dwgFullPath, FileShare.Read, false, null);
		db.ResolveXrefs(false, true);
		return db;
	}

	public bool IsSamePath(string path1, string path2)
	{
		return string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase);
	}

	public void OpenFileInApp(ProjectFile file)
	{
		string dwgFullPath = file.DrawingFullPath;
		try
		{
			dynamic acadApp = ComInterop.GetActiveAcadApp();
			if (System.IO.File.Exists(dwgFullPath))
			{
				OpenedDoc = acadApp.Documents.Open(dwgFullPath);
                SetDocument(file);
			}
		}
		catch (Autodesk.AutoCAD.Runtime.Exception ex)
		{
			Console.WriteLine($"Error opening DWG file: {ex.Message}");
		}
	}

	public void SetDocument(ProjectFile file)
	{
		string dwgFullPath = file.DrawingFullPath;

		DocumentCollection documentManager = Application.DocumentManager;
		foreach (Document document in documentManager)
		{
			if (document.Name != dwgFullPath)
			{
				continue;
			}

			Document = document;
			return;
		}
	}

	public void SetDatabase()
	{
		Database = Document.Database;
	}

	public void SetTransaction()
	{
		Txn = Database.TransactionManager.StartTransaction();
	}

	public void SetBlockTableRecorde()
	{
		BlockTableRecord = Txn.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(Database), OpenMode.ForRead) as BlockTableRecord;
	}

	public void SetUnit()
	{
		Unit = Database.Insunits;
	}

	public void SetUnitConversionFactor()
	{
		if (Unit is UnitsValue.Inches)
		{
			UnitConversionFactor = 12.00;
			return;
		}

		if (Unit is UnitsValue.Meters)
		{
			UnitConversionFactor = 0.3048;
			return;
		}

		if (Unit is UnitsValue.Millimeters)
		{
			UnitConversionFactor = 304.8;
			return;
		}

		if (Unit is UnitsValue.Feet)
		{
			UnitConversionFactor = 1.0;
		}
	}

	public dynamic GetEntity(Autodesk.AutoCAD.DatabaseServices.ObjectId objectId)
	{
		return Txn.GetObject(objectId, OpenMode.ForRead);
	}

	public string GetEntityType(object entity)
	{
		if (entity is Wall) return Wall;

		if (entity is CurtainWallLayout) return CurtainWallLayout;

		if (entity is CurtainWallUnit) return CurtainWallUnit;

		if (entity is Window) return Window;

		if (entity is WindowAssembly) return WindowAssembly;

		if (entity is Door) return Door;

		if (entity is Opening) return Opening;

		if (entity is Slab) return Slab;

		if (entity is RoofSlab) return RoofSlab;

		if (entity is Space) return Space;

		if (entity is Zone) return Zone;

		return null;
	}

	public void AddEntityPosition(string entityType, object entity)
	{
		if (entityType == Wall)
		{
			Component.Wall wall = (Component.Wall)entity;
			wall.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == CurtainWallLayout)
		{
			Component.CurtainWallLayout curtainWall = (Component.CurtainWallLayout)entity;
			curtainWall.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == CurtainWallUnit)
		{
			Component.CurtainWallUnit curtainWallUnit = (Component.CurtainWallUnit)entity;
			curtainWallUnit.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == Window)
		{
			Component.Window window = (Component.Window)entity;
			window.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == WindowAssembly)
		{
			Component.WindowAssembly windowAssembly = (Component.WindowAssembly)entity;
			windowAssembly.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == Door)
		{
			Component.Door door = (Component.Door)entity;
			door.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == Opening)
		{
			Component.Opening opening = (Component.Opening)entity;
			opening.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == Slab)
		{
			Component.Slab slab = (Component.Slab)entity;
			slab.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == RoofSlab)
		{
			Component.RoofSlab roofSlab = (Component.RoofSlab)entity;
			roofSlab.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == Space)
		{
			Component.Space space = (Component.Space)entity;
			space.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

			return;
		}

		if (entityType == Zone)
		{
			Component.Zone zone = (Component.Zone)entity;
			zone.DivisionsAndLevels = new Dictionary<string, HashSet<string>>(DivisionsAndLevels);

		}
	}

	public void AddEntityMaterial(object entity, string entityType, object convertedEntity)
	{
		if (entityType == Wall)
		{
			Wall wall = (Wall)entity;
			Component.Wall convertedWall = (Component.Wall)convertedEntity;

			Material material = Txn.GetObject(wall.MaterialId, OpenMode.ForRead) as Material;

			convertedWall.MaterialName = material.Name;

			return;
		}

		if (entityType == CurtainWallLayout)
		{
			CurtainWallLayout curtainWallLayout = (CurtainWallLayout)entity;
			Component.CurtainWallLayout convertedCurtainWall = (Component.CurtainWallLayout)convertedEntity;

			Material material = Txn.GetObject(curtainWallLayout.MaterialId, OpenMode.ForRead) as Material;

			convertedCurtainWall.MaterialName = material.Name;

			return;
		}

		if (entityType == CurtainWallUnit)
		{
			CurtainWallUnit curtainWallLayout = (CurtainWallUnit)entity;
			Component.CurtainWallUnit convertedCurtainWallUnit = (Component.CurtainWallUnit)convertedEntity;

			Material material = Txn.GetObject(curtainWallLayout.MaterialId, OpenMode.ForRead) as Material;

			convertedCurtainWallUnit.MaterialName = material.Name;

			return;
		}

		if (entityType == Window)
		{
			Window window = (Window)entity;
			Component.Window convertedWindow = (Component.Window)convertedEntity;

			Material material = Txn.GetObject(window.MaterialId, OpenMode.ForRead) as Material;

			convertedWindow.MaterialName = material.Name;

			return;
		}

		if (entityType == WindowAssembly)
		{
			WindowAssembly windowAssembly = (WindowAssembly)entity;
			Component.WindowAssembly convertedWindowAssembly = (Component.WindowAssembly)convertedEntity;

			Material material = Txn.GetObject(windowAssembly.MaterialId, OpenMode.ForRead) as Material;

			convertedWindowAssembly.MaterialName = material.Name;

			return;
		}

		if (entityType == Door)
		{
			Door door = (Door)entity;
			Component.Door convertedDoor = (Component.Door)convertedEntity;

			Material material = Txn.GetObject(door.MaterialId, OpenMode.ForRead) as Material;

			convertedDoor.MaterialName = material.Name;
		}
	}

	public void AddEntityStyle(object entity, string entityType, object convertedEntity)
	{
		if (entityType == Wall)
		{
			Wall wall = (Wall)entity;
			Component.Wall convertedWall = (Component.Wall)convertedEntity;

			WallStyle wallStyle = Txn.GetObject(wall.StyleId, OpenMode.ForRead) as WallStyle;
			convertedWall.Style = wallStyle.Name;

			return;
		}

		if (entityType == CurtainWallLayout)
		{
			CurtainWallLayout curtainWallLayout = (CurtainWallLayout)entity;
			Component.CurtainWallLayout convertedCurtainWallLayout = (Component.CurtainWallLayout)convertedEntity;

			CurtainWallLayoutStyle curtainWallLayoutStyle = Txn.GetObject(curtainWallLayout.StyleId, OpenMode.ForRead) as CurtainWallLayoutStyle;
			convertedCurtainWallLayout.Style = curtainWallLayoutStyle.Name;

			return;
		}

		if (entityType == CurtainWallUnit)
		{
			CurtainWallUnit curtainWallUnit = (CurtainWallUnit)entity;
			Component.CurtainWallUnit convertedCurtainWallUnit = (Component.CurtainWallUnit)convertedEntity;

			CurtainWallUnitStyle curtainWallUnitStyle = Txn.GetObject(curtainWallUnit.StyleId, OpenMode.ForRead) as CurtainWallUnitStyle;
			convertedCurtainWallUnit.Style = curtainWallUnitStyle.Name;

			return;
		}

		if (entityType == Window)
		{
			Window window = (Window)entity;
			Component.Window convertedWindow = (Component.Window)convertedEntity;

			WindowStyle windowStyle = Txn.GetObject(window.StyleId, OpenMode.ForRead) as WindowStyle;
			convertedWindow.Style = windowStyle.Name;

			return;
		}

		if (entityType == WindowAssembly)
		{
			WindowAssembly windowAssembly = (WindowAssembly)entity;
			Component.WindowAssembly convertedWindowAssembly = (Component.WindowAssembly)convertedEntity;

			WindowAssemblyStyle windowAssemblyStyle = Txn.GetObject(windowAssembly.StyleId, OpenMode.ForRead) as WindowAssemblyStyle;
			convertedWindowAssembly.Style = windowAssemblyStyle.Name;

			return;
		}

		if (entityType == Door)
		{
			Door door = (Door)entity;
			Component.Door convertedDoor = (Component.Door)convertedEntity;

			DoorStyle doorStyle = Txn.GetObject(door.StyleId, OpenMode.ForRead) as DoorStyle;
			convertedDoor.Style = doorStyle.Name;

			return;
		}

		if (entityType == Slab)
		{
			Slab slab = (Slab)entity;
			Component.Slab convertedSlab = (Component.Slab)convertedEntity;

			SlabStyle slabStyle = Txn.GetObject(slab.StyleId, OpenMode.ForRead) as SlabStyle;
			convertedSlab.Style = slabStyle.Name;

			return;
		}

		if (entityType == RoofSlab)
		{
			RoofSlab roofSlab = (RoofSlab)entity;
			Component.RoofSlab convertedRoofSlab = (Component.RoofSlab)convertedEntity;

			RoofSlabStyle roofSlabStyle = Txn.GetObject(roofSlab.StyleId, OpenMode.ForRead) as RoofSlabStyle;
			convertedRoofSlab.Style = roofSlabStyle.Name;

		}
	}

	public void AddEntityAnchor(object entity, ref string entityType, object convertedEntity)
	{
		if (entityType == Window)
		{
			Window window = (Window)entity;
			Component.Window convertedWindow = (Component.Window)convertedEntity;

			Autodesk.AutoCAD.DatabaseServices.DBObject anchorObject = null;

			if (window.IsAnchored)
			{
				anchorObject = Txn.GetObject(window.AnchorId, OpenMode.ForRead);
			}

			if (anchorObject is AnchorEntityToGridAssembly)
			{
				AnchorEntityToGridAssembly anchorEntity = (AnchorEntityToGridAssembly)Txn.GetObject(window.AnchorId, OpenMode.ForRead);

				if (window.AnchorId != null)
				{
					string wallObjectid = anchorEntity.SingleReferenceId.ToString();
					wallObjectid = wallObjectid.Substring(1, wallObjectid.Length - 2);

					convertedWindow.WallId = wallObjectid;
				}
			}

			else if (anchorObject is AnchorEntityToWall)
			{
				AnchorEntityToWall anchorEntity = (AnchorEntityToWall)Txn.GetObject(window.AnchorId, OpenMode.ForRead);
				AnchorOpeningBaseToWall anchorToWall = null;

				if (window.AnchorId != null)
				{
					if (window.IsAnchoredToWall)
					{
						anchorToWall = (AnchorOpeningBaseToWall)anchorEntity;

						string wallObjectid = anchorToWall.SingleReferenceId.ToString();
						wallObjectid = wallObjectid.Substring(1, wallObjectid.Length - 2);

						convertedWindow.WallId = wallObjectid;
					}
				}
			}
		}

		if (entityType == Door)
		{
			Door door = (Door)entity;
			Component.Door convertedDoor = (Component.Door)convertedEntity;

			Autodesk.AutoCAD.DatabaseServices.DBObject anchorObject = null;

			if (door.IsAnchored)
			{
				anchorObject = Txn.GetObject(door.AnchorId, OpenMode.ForRead);
			}

			if (anchorObject is AnchorEntityToGridAssembly)
			{
				AnchorEntityToGridAssembly anchorEntity = (AnchorEntityToGridAssembly)Txn.GetObject(door.AnchorId, OpenMode.ForRead);

				if (door.AnchorId != null)
				{
					string wallObjectid = anchorEntity.SingleReferenceId.ToString();
					wallObjectid = wallObjectid.Substring(1, wallObjectid.Length - 2);

					convertedDoor.WallId = wallObjectid;
				}
			}

			else if (anchorObject is AnchorEntityToWall)
			{
				AnchorEntityToWall anchorEntity = (AnchorEntityToWall)Txn.GetObject(door.AnchorId, OpenMode.ForRead);
				AnchorOpeningBaseToWall anchorToWall = null;

				if (door.AnchorId != null)
				{
					if (door.IsAnchoredToWall)
					{
						anchorToWall = (AnchorOpeningBaseToWall)anchorEntity;

						string wallObjectid = anchorToWall.SingleReferenceId.ToString();
						wallObjectid = wallObjectid.Substring(1, wallObjectid.Length - 2);

						convertedDoor.WallId = wallObjectid;
					}
				}
			}

			return;
		}

		if (entityType == Opening)
		{
			Opening opening = (Opening)entity;
			Component.Opening convertedOpening = (Component.Opening)convertedEntity;

			Autodesk.AutoCAD.DatabaseServices.DBObject anchorObject = null;

			if (opening.IsAnchored)
			{
				anchorObject = Txn.GetObject(opening.AnchorId, OpenMode.ForRead);
			}

			if (anchorObject is AnchorEntityToGridAssembly)
			{
				AnchorEntityToGridAssembly anchorEntity = (AnchorEntityToGridAssembly)Txn.GetObject(opening.AnchorId, OpenMode.ForRead);

				if (opening.AnchorId != null)
				{
					string wallObjectid = anchorEntity.SingleReferenceId.ToString();
					wallObjectid = wallObjectid.Substring(1, wallObjectid.Length - 2);

					convertedOpening.WallId = wallObjectid;

				}
			}

			else if (anchorObject is AnchorEntityToWall)
			{
				AnchorEntityToWall anchorEntity = (AnchorEntityToWall)Txn.GetObject(opening.AnchorId, OpenMode.ForRead);
				AnchorOpeningBaseToWall anchorToWall = null;

				if (opening.AnchorId != null)
				{
					if (opening.IsAnchoredToWall)
					{
						anchorToWall = (AnchorOpeningBaseToWall)anchorEntity;

						string wallObjectid = anchorToWall.SingleReferenceId.ToString();
						wallObjectid = wallObjectid.Substring(1, wallObjectid.Length - 2);

						convertedOpening.WallId = wallObjectid;
					}
				}
			}

			return;
		}

		if (entityType == WindowAssembly)
		{
			WindowAssembly windowAssembly = (WindowAssembly)entity;
			Component.WindowAssembly convertedWindowAssembly = (Component.WindowAssembly)convertedEntity;

			Autodesk.AutoCAD.DatabaseServices.DBObject anchorObject = null;

			if (windowAssembly.IsAnchored)
			{
				anchorObject = Txn.GetObject(windowAssembly.AnchorId, OpenMode.ForRead);
			}

			if (anchorObject is AnchorEntityToGridAssembly)
			{
				AnchorEntityToGridAssembly anchorEntity = (AnchorEntityToGridAssembly)Txn.GetObject(windowAssembly.AnchorId, OpenMode.ForRead);

				if (windowAssembly.AnchorId != null)
				{
					string wallObjectid = anchorEntity.SingleReferenceId.ToString();
					wallObjectid = wallObjectid.Substring(1, wallObjectid.Length - 2);

					convertedWindowAssembly.WallId = wallObjectid;
				}
			}

			else if (anchorObject is AnchorEntityToWall)
			{
				AnchorEntityToWall anchorEntity = (AnchorEntityToWall)Txn.GetObject(windowAssembly.AnchorId, OpenMode.ForRead);
				AnchorOpeningBaseToWall anchorToWall = null;

				if (windowAssembly.AnchorId != null)
				{
					if (windowAssembly.IsAnchoredToWall)
					{
						anchorToWall = (AnchorOpeningBaseToWall)anchorEntity;

						string wallObjectid = anchorToWall.SingleReferenceId.ToString();
						wallObjectid = wallObjectid.Substring(1, wallObjectid.Length - 2);

						convertedWindowAssembly.WallId = wallObjectid;
					}
				}
			}
			else if (anchorObject == null)
			{
				entityType = "windowAssemblyAsWall";
			}

		}
	}

	public void AddEntityToEntites(string entityType, object convertedEntity, Entities entities)
	{
		if (entityType == Wall)
		{
			entities.Walls.Add((Component.Wall)convertedEntity);
			return;
		}

		if (entityType == CurtainWallLayout)
		{
			entities.CurtainWallLayouts.Add((Component.CurtainWallLayout)convertedEntity);
			return;
		}

		if (entityType == CurtainWallUnit)
		{
			entities.CurtainWallUnits.Add((Component.CurtainWallUnit)convertedEntity);
			return;
		}

		if (entityType == Window)
		{
			entities.Windows.Add((Component.Window)convertedEntity);
			return;
		}

		if (entityType == WindowAssembly)
		{
			entities.WindowAssemblies.Add((Component.WindowAssembly)convertedEntity);
			return;
		}

		if (entityType == "windowAssemblyAsWall")
		{
			Component.Wall wall = new Component.Wall();
			Convertor.CovertWindowAssemblyAsWallToWall(convertedEntity, wall);
			entities.Walls.Add(wall);
			return;
		}

		if (entityType == Door)
		{
			entities.Doors.Add((Component.Door)convertedEntity);
			return;
		}

		if (entityType == Opening)
		{
			entities.Openings.Add((Component.Opening)convertedEntity);
			return;
		}

		if (entityType == Space)
		{
			entities.Spaces.Add((Component.Space)convertedEntity);
			return;
		}

		if (entityType == Slab)
		{
			entities.Slabs.Add((Component.Slab)convertedEntity);
			return;
		}

		if (entityType == RoofSlab)
		{
			entities.RoofSlabs.Add((Component.RoofSlab)convertedEntity);
			return;
		}

		if (entityType == Zone)
		{
			entities.Zones.Add((Component.Zone)convertedEntity);
		}
	}

	public void ResetTransaction()
	{
		Txn.Commit();
	}

	public void CloseFileInApp()
	{
         OpenedDoc.Close();
	}
}
