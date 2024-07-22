using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Component;
using Model;

public class LevelFormatter
{
    readonly Dictionary<string, bool>  UniqueIds = new Dictionary<string, bool>();
    Dictionary<int, Point> UniqueVerticesDictionary = new Dictionary<int, Point>();
    Dictionary<string, string> ObjectIdTojsonIdDictionary = new Dictionary<string, string>();
    Dictionary<string, Vertex> Vertices = new Dictionary<string, Vertex>();
    Dictionary<string, Line2D> Lines = new Dictionary<string, Line2D>();
    Dictionary<string, Holes> Holes = new Dictionary<string, Holes>();
    Dictionary<string, Zone> Zones = new Dictionary<string, Zone>();
    Dictionary<string, Slab> Slabs = new Dictionary<string, Slab>();
    Dictionary<string, RoofSlab> RoofSlabs = new Dictionary<string, RoofSlab>();
    Dictionary<string, Areas> areas = new Dictionary<string, Areas>();

    private Dictionary<string, Layer> layers = new Dictionary<string, Layer>();

    public Dictionary<string, Layer> Layers
    {
        get { return layers; }
        set { layers = value; }
    }
    private Model.Floor Floor;

    const string Vertex = "Vertex";
    public LevelFormatter(Model.Floor floor)
    {
        this.Floor = floor;

        UniqueVertices();
        GetLinesDictionary();
        MakeHole();
        MakeSlabs();
        MakeRoofSlabs();
        MakeLayer();
        // makeZones();
        //MakeStructuralMembers(structuralMemberDictionary);
    }
    public static double PolygonArea(List<List<double>> vertices)
    {
        int n = vertices.Count;
        if (n < 3)
            return 0; // Not a valid polygon

        // Append the first vertex to the end to close the polygon
        vertices.Add(vertices[0]);

        // Calculate the area using the Shoelace formula
        double area = 0;
        for (int i = 0; i < n; i++)
        {
            area += (vertices[i][0] * vertices[i + 1][1]) - (vertices[i + 1][0] * vertices[i][1]);
        }

        // Take the absolute value and divide by 2
        area = Math.Abs(area) / 2;
        return area;
    }
    public bool IsUniquePoint(Point InPoint, Dictionary<int, Point> uniqueVertices)
    {
        foreach (var Point in uniqueVertices.Values)
        {
            if (Math.Abs(Point.X - InPoint.X) <= 0.1 && Math.Abs(Point.Y - InPoint.Y) <= 0.1)
            { return false; }
        }
        return true;
    }
    public string GenerateId()
    {
        int length = 10;
        string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
        Random random = new Random();
        char[] result = new char[length];
        int maxAttempts = 1000; // Define maximum attempts

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            for (int i = 0; i < length; i++)
            {
                result[i] = characters[random.Next(characters.Length)];
            }

            string randomString = new string(result);
            if (!UniqueIds.ContainsKey(randomString))
            {
                UniqueIds.Add(randomString, true);
                return randomString;
            }
        }

        throw new Exception("Unable to generate a unique ID after maximum attempts.");
    }
    public List<string> GetVerticesOfLine(List<Point> inLine)
    {
        List<string> vertexList = new List<string>();
        foreach (var vertex in Vertices)
        {
            if (Math.Abs(vertex.Value.x - inLine[0].X) < 0.1 && Math.Abs(vertex.Value.y - inLine[0].Y) < 0.1)
            {
                vertexList.Add(vertex.Key);
                continue;
            }
            if (Math.Abs(vertex.Value.x - inLine[1].X) < 0.1 && Math.Abs(vertex.Value.y - inLine[1].Y) < 0.1)
            {
                vertexList.Add(vertex.Key);
            }

        }
        return vertexList;
    }
    public static bool IsStartingVertex(List<Point> listOfVertices)
    {
        if (listOfVertices[0].X < listOfVertices[1].X && Math.Abs(listOfVertices[0].X - listOfVertices[1].X) > 0.1)
        {
            return true;
        }
        else if (listOfVertices[0].X > listOfVertices[1].X && Math.Abs(listOfVertices[0].X - listOfVertices[1].X) > 0.1)
        {
            return false;
        }
        else
        {
            if (listOfVertices[0].Y < listOfVertices[1].Y)
            {
                return true;
            }
            return false;
        }

    }
    public double GetLength(List<Point> inPoint)
    {
        var length = Math.Pow(Math.Pow((inPoint[1].X - inPoint[0].X), 2) + Math.Pow((inPoint[1].Y - inPoint[0].Y), 2), 0.5);
        return length;
    }
    public void UniqueVertices()
    {
        int IndexOfUniqueVertces = 1;
        int addUniqueVertex = 1;

        foreach (var pair in Floor.Walls)
        {
            Point startPoint = new Point(pair.StartPoint.X, pair.StartPoint.Y, pair.StartPoint.Z);
            Point endPoint = new Point(pair.EndPoint.X, pair.EndPoint.Y, pair.EndPoint.Z);
            if (IsUniquePoint(startPoint, UniqueVerticesDictionary) && addUniqueVertex == 1)
            {
                UniqueVerticesDictionary.Add(IndexOfUniqueVertces, startPoint);
                IndexOfUniqueVertces++;
                var startVertex = new Vertex();
                string uniqueId = GenerateId();
                startVertex.type = Vertex;
                startVertex.x = pair.StartPoint.X;
                startVertex.y = pair.StartPoint.Y;
                startVertex.Lines = new List<string>();

                Vertices.Add(uniqueId, startVertex);

            }
            if (IsUniquePoint(endPoint, UniqueVerticesDictionary) && addUniqueVertex == 1)
            {

                UniqueVerticesDictionary.Add(IndexOfUniqueVertces, endPoint);
                IndexOfUniqueVertces++;
                var endVertex = new Vertex();
                string uniqueId = GenerateId();
                endVertex.type = Vertex;
                endVertex.x = pair.EndPoint.X;
                endVertex.y = pair.EndPoint.Y;
                endVertex.Lines = new List<string>();

                Vertices.Add(uniqueId, endVertex);

            }

        }

        foreach (var pair in Floor.CurtainWallLayouts)
        {
            Point startPoint = new Point(pair.StartPoint.X, pair.StartPoint.Y, pair.StartPoint.Z);
            Point endPoint = new Point(pair.EndPoint.X, pair.EndPoint.Y, pair.EndPoint.Z);
            if (IsUniquePoint(startPoint, UniqueVerticesDictionary) && addUniqueVertex == 1)
            {
                UniqueVerticesDictionary.Add(IndexOfUniqueVertces, startPoint);
                IndexOfUniqueVertces++;
                var startVertex = new Vertex();
                string uniqueId = GenerateId();
                startVertex.type = Vertex;
                startVertex.x = pair.StartPoint.X;
                startVertex.y = pair.StartPoint.Y;
                startVertex.Lines = new List<string>();

                Vertices.Add(uniqueId, startVertex);

            }
            if (IsUniquePoint(endPoint, UniqueVerticesDictionary) && addUniqueVertex == 1)
            {

                UniqueVerticesDictionary.Add(IndexOfUniqueVertces, endPoint);
                IndexOfUniqueVertces++;

                var endVertex = new Vertex();
                string uniqueId = GenerateId();
                endVertex.type = Vertex;
                endVertex.x = pair.EndPoint.X;
                endVertex.y = pair.EndPoint.Y;
                endVertex.Lines = new List<string>();

                Vertices.Add(uniqueId, endVertex);

            }

        }

        foreach (var pair in Floor.CurtainWallUnits)
        {
            Point startPoint = new Point(pair.StartPoint.X, pair.StartPoint.Y, pair.StartPoint.Z);
            Point endPoint = new Point(pair.EndPoint.X, pair.EndPoint.Y, pair.EndPoint.Z);
            if (IsUniquePoint(startPoint, UniqueVerticesDictionary) && addUniqueVertex == 1)
            {
                UniqueVerticesDictionary.Add(IndexOfUniqueVertces, startPoint);
                IndexOfUniqueVertces++;
                var startVertex = new Vertex();
                string uniqueId = GenerateId();
                startVertex.type = Vertex;
                startVertex.x = pair.StartPoint.X;
                startVertex.y = pair.StartPoint.Y;
                startVertex.Lines = new List<string>();

                Vertices.Add(uniqueId, startVertex);

            }
            if (IsUniquePoint(endPoint, UniqueVerticesDictionary) && addUniqueVertex == 1)
            {
                UniqueVerticesDictionary.Add(IndexOfUniqueVertces, endPoint);
                IndexOfUniqueVertces++;
                var endVertex = new Vertex();
                string uniqueId = GenerateId();
                endVertex.type = Vertex;
                endVertex.x = pair.EndPoint.X;
                endVertex.y = pair.EndPoint.Y;
                endVertex.Lines = new List<string>();

                Vertices.Add(uniqueId, endVertex);

            }

        }
    }
    public void GetLinesDictionary()
    {
        foreach (var line in Floor.Walls)
        {
            var l1 = new Line2D();
            string uniqueId = GenerateId();
            l1.type = line.DisplayName;
            string newJson = @"{
                        ""style"":"""",
                        ""length"": 0,
                        ""height"":0,
                        ""baseOffset"":0,
                        ""thickness"": 0
                      }";
            var initialJson = JObject.Parse(newJson);
            initialJson["length"] = line.Length;
            initialJson["height"] = line.BaseHeight;
            initialJson["thickness"] = line.Width;// Set the desired length value here
            initialJson["style"] = line.Style;
            initialJson["baseOffset"] = line.StartPoint.Z;

            l1.ArcWall = line.ArcWallObject;
            string modifiedJson = initialJson.ToString();
            l1.properties = JObject.Parse(modifiedJson);
            List<Point> points = new List<Point>();
            Point startPoint = new Point(line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z);
            Point endPoint = new Point(line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z);
            points.Add(startPoint);
            points.Add(endPoint);


            if (GetVerticesOfLine(points).Count == 2)
            {
                l1.vertices = GetVerticesOfLine(points);
                l1.holes = new List<string>();
                string key = line.ObjectId.ToString().Substring(1, line.ObjectId.ToString().Length - 2).ToString();
                if (!ObjectIdTojsonIdDictionary.ContainsKey(key)) { ObjectIdTojsonIdDictionary.Add(key, uniqueId); }
                Lines.Add(uniqueId, l1);

                if (l1.vertices != null)
                {
                    foreach (var vertexId in l1.vertices)
                    {
                        Vertices[vertexId].Lines.Add(uniqueId);
                    }
                }
            }


        }

        foreach (var line in Floor.CurtainWallLayouts)
        {
            var l1 = new Line2D();
            string uniqueId = GenerateId();
            l1.type = line.DisplayName;
            string newJson = @"{
                        ""style"":"""",
                        ""length"": 0,
                        ""height"":0,
                        ""baseOffset"":0,
                        ""thickness"": 0.1236666666666668,
                    }";
            var initialJson = JObject.Parse(newJson);
            initialJson["length"] = line.Length;
            initialJson["height"] = line.BaseHeight;
            //initialJson["thickness"] = line.Width;// Set the desired length value here
            initialJson["style"] = line.Style;
            initialJson["baseOffset"] = line.StartPoint.Z;
            l1.curatainArcWall = line.ArcWallObject;
            string modifiedJson = initialJson.ToString();
            l1.properties = JObject.Parse(modifiedJson);
            List<Point> points = new List<Point>();
            Point startPoint = new Point(line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z);
            Point endPoint = new Point(line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z);
            points.Add(startPoint);
            points.Add(endPoint);

            if (GetVerticesOfLine(points).Count == 2)
            {
                l1.vertices = GetVerticesOfLine(points);
                l1.holes = new List<string>();
                string key = line.ObjectId.ToString().Substring(1, line.ObjectId.ToString().Length - 2).ToString();
                if (!ObjectIdTojsonIdDictionary.ContainsKey(key)) { ObjectIdTojsonIdDictionary.Add(key, uniqueId); }
                Lines.Add(GenerateId(), l1);

                if (l1.vertices != null)
                {
                    foreach (var vertexId in l1.vertices)
                    {
                        Vertices[vertexId].Lines.Add(uniqueId);
                    }
                }
            }

        }

        foreach (var line in Floor.CurtainWallUnits)
        {
            var l1 = new Line2D();
            string uniqueId = GenerateId();
            l1.type = line.DisplayName;
            string newJson = @"{                        
                        ""style"":"""",
                        ""length"": 300,
                        ""thickness"": 0.1236666666666668,
                        ""height"":0,
                        ""baseOffset"":0,
                    }";
            var initialJson = JObject.Parse(newJson);
            initialJson["length"] = line.Length;
            initialJson["height"] = line.BaseHeight;
            //initialJson["thickness"] = line.Width;// Set the desired length value here
            initialJson["style"] = line.Style;
            initialJson["baseOffset"] = line.StartPoint.Z;
            string modifiedJson = initialJson.ToString();
            l1.properties = JObject.Parse(modifiedJson);

            List<Point> points = new List<Point>();
            Point startPoint = new Point(line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z);
            Point endPoint = new Point(line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z);
            points.Add(startPoint);
            points.Add(endPoint);

            if (GetVerticesOfLine(points).Count == 2)
            {
                l1.vertices = GetVerticesOfLine(points);
                l1.holes = new List<string>();
                string key = line.ObjectId.ToString().Substring(1, line.ObjectId.ToString().Length - 2).ToString();
                if (!ObjectIdTojsonIdDictionary.ContainsKey(key)) { ObjectIdTojsonIdDictionary.Add(key, uniqueId); }

                Lines.Add(GenerateId(), l1);

                if (l1.vertices != null)
                {
                    foreach (var vertexId in l1.vertices)
                    {
                        Vertices[vertexId].Lines.Add(uniqueId);
                    }
                }
            }

        }

    }
    public void MakeHole()
    {
        foreach (var window in Floor.WindowAssemblies)
        {
            if (window.WallId != null && ObjectIdTojsonIdDictionary.ContainsKey(window.WallId))
            {
                var line = Lines[ObjectIdTojsonIdDictionary[window.WallId]];
                Point point1 = new Point(Vertices[line.vertices[0]].x, Vertices[line.vertices[0]].y, Vertices[line.vertices[0]].z);
                Point point2 = new Point(Vertices[line.vertices[1]].x, Vertices[line.vertices[1]].y, Vertices[line.vertices[1]].z);


                Holes windowHole = new Holes();
                windowHole.line = ObjectIdTojsonIdDictionary[window.WallId];
                string uniqueId = GenerateId();
                Lines[windowHole.line].holes.Add(uniqueId);
                windowHole.type = window.DisplayName;
                string newJson1 = @"{
                        ""width"": 0,
                        ""height"": 0,
                        ""altitude"": 0,
                        ""thickness"": 0,
                        ""style"":"""",
                        ""frameWidth"":"""",
                        ""frameDepth"":"""",
                        ""glassThickness"":""""
                        }";
                var initialJson = JObject.Parse(newJson1);

                initialJson["width"] = window.Length;
                initialJson["height"] = window.Height;
                initialJson["type"] = window.DisplayName;
                initialJson["style"] = window.Style;
                initialJson["altitude"] = window.StartPoint.Z;

                string modifiedJson = initialJson.ToString();
                windowHole.properties = JObject.Parse(modifiedJson);
                Point startWindow = new Point(window.StartPoint.X, window.StartPoint.Y, 0.0);
                Point endWindow = new Point(window.EndPoint.X, window.EndPoint.Y, 0.0);
                windowHole.startPoint = startWindow;
                windowHole.endPoint = endWindow;
                List<Point> points = new List<Point>();
                points.Add(startWindow);
                points.Add(endWindow);


                Point windowStartingVertex = new Point();
                Point wallStartingVertex = new Point();

                if (IsStartingVertex(points))
                {
                    windowStartingVertex = startWindow;
                }
                else
                {
                    windowStartingVertex = endWindow;
                }
                points.Clear();
                points.Add(point1);
                points.Add(point2);

                if (IsStartingVertex(points))
                {
                    wallStartingVertex = point1;
                }
                else
                {
                    wallStartingVertex = point2;
                }
                points.Clear();
                points.Add(wallStartingVertex);
                points.Add(windowStartingVertex);

                List<Point> wallLength = new List<Point>();
                wallLength.Add(point2);
                wallLength.Add(point1);
                //double offset = (GetLength(points) + (window.Width / 2)) / GetLength(wallLength);
                double offset = (GetLength(points)) / GetLength(wallLength);

                windowHole.offset = offset;

                Normal normal1 = new Normal();
                normal1.x = window.Normal.X;
                normal1.y = window.Normal.Y;
                normal1.z = window.Normal.Z;
                windowHole.normal = normal1;

                if (!ObjectIdTojsonIdDictionary.ContainsKey(window.ObjectId.ToString())) { ObjectIdTojsonIdDictionary.Add(window.ObjectId.ToString(), uniqueId); }
                Holes.Add(uniqueId, windowHole);
            }



        }

        foreach (var window in Floor.Windows)
        {
            if (ObjectIdTojsonIdDictionary.ContainsKey(window.WallId))
            {
                var line = Lines[ObjectIdTojsonIdDictionary[window.WallId]];
                Point point1 = new Point(Vertices[line.vertices[0]].x, Vertices[line.vertices[0]].y, Vertices[line.vertices[0]].z);
                Point point2 = new Point(Vertices[line.vertices[1]].x, Vertices[line.vertices[1]].y, Vertices[line.vertices[1]].z);

                Holes windowHole = new Holes();
                string uniqueId = GenerateId();
                windowHole.line = ObjectIdTojsonIdDictionary[window.WallId];
                Lines[windowHole.line].holes.Add(uniqueId);

                windowHole.type = window.DisplayName;
                string newJson1 = @"{
                        ""width"": 0.18,
                        ""height"": 0.35,
                        ""altitude"": 0.25,
                        ""thickness"": 0.6,
                        ""style"":"""",
                        ""frameWidth"":"""",
                        ""frameDepth"":"""",
                        ""glassThickness"":""""
                        }";
                var initialJson = JObject.Parse(newJson1);
                //initialJson["altitude"] = window.StartPoint.Z;
                initialJson["width"] = window.Width;
                initialJson["height"] = window.Height;
                initialJson["style"] = window.Style;
                initialJson["altitude"] = window.StartPoint.Z;

                string modifiedJson = initialJson.ToString();
                windowHole.properties = JObject.Parse(modifiedJson);

                Point startWindow = new Point(window.StartPoint.X, window.StartPoint.Y, 0.0);
                Point endWindow = new Point(window.EndPoint.X, window.EndPoint.Y, 0.0);
                windowHole.startPoint = startWindow;
                windowHole.endPoint = endWindow;
                List<Point> points = new List<Point>();
                points.Add(startWindow);
                points.Add(endWindow);


                Point windowStartingVertex = new Point();
                Point wallStartingVertex = new Point();

                if (IsStartingVertex(points))
                {
                    windowStartingVertex = startWindow;
                }
                else
                {
                    windowStartingVertex = endWindow;
                }
                points.Clear();
                points.Add(point1);
                points.Add(point2);

                if (IsStartingVertex(points))
                {
                    wallStartingVertex = point1;
                }
                else
                {
                    wallStartingVertex = point2;
                }
                points.Clear();
                points.Add(wallStartingVertex);
                points.Add(windowStartingVertex);

                List<Point> wallLength = new List<Point>();
                wallLength.Add(point2);
                wallLength.Add(point1);

                double offset = (GetLength(points) + (window.Width / 2)) / GetLength(wallLength);
                //double offset = (GetLength(points)) / GetLength(wallLength);

                windowHole.offset = offset;

                Normal normal1 = new Normal();
                normal1.x = window.Normal.X;
                normal1.y = window.Normal.Y;
                normal1.z = window.Normal.Z;
                windowHole.normal = normal1;

                Holes.Add(uniqueId, windowHole);
            }
        }

        foreach (var door in Floor.Doors)
        {
            Line2D line = null;
            string lineId = null;
            if (door.WallId != null && ObjectIdTojsonIdDictionary.ContainsKey(door.WallId))
            {
                //if door is directly attached wall
                if (Lines.ContainsKey(ObjectIdTojsonIdDictionary[door.WallId]))
                {
                    line = Lines[ObjectIdTojsonIdDictionary[door.WallId]];
                    lineId = ObjectIdTojsonIdDictionary[door.WallId];
                }
                else
                {
                    //if door is  attached door/window assembly and it is attached to wall
                    foreach (var doorwindowAssembly in Floor.WindowAssemblies)
                    {
                        if (door.WallId == doorwindowAssembly.ObjectId)
                        {
                            line = Lines[ObjectIdTojsonIdDictionary[doorwindowAssembly.WallId]];
                            lineId = ObjectIdTojsonIdDictionary[doorwindowAssembly.WallId];
                            break;
                        }
                    }
                }
                if (line == null)
                {
                // To check door has wall or not
                    continue;
                }
                Point point1 = new Point(Vertices[line.vertices[0]].x, Vertices[line.vertices[0]].y, Vertices[line.vertices[0]].z);
                Point point2 = new Point(Vertices[line.vertices[1]].x, Vertices[line.vertices[1]].y, Vertices[line.vertices[1]].z);


                Holes hole = new Holes();
                string uniqueId = GenerateId();
                hole.line = lineId;
                Lines[hole.line].holes.Add(uniqueId);
                hole.type = door.DisplayName;
                string newJson = @"{
                        ""width"": 0.18,
                        ""height"": 0.35,
                        ""altitude"": 0,
                        ""thickness"": 0.1016,
                        ""style"":""""
                        }";
                var initialJson = JObject.Parse(newJson);
                //initialJson["altitude"] = door.StartPoint.Z;
                initialJson["width"] = door.Width;
                initialJson["height"] = door.Height;
                initialJson["style"] = door.Style;

                // Set the desired length value here
                string modifiedJson = initialJson.ToString();
                hole.properties = JObject.Parse(modifiedJson);
                Point startWindow = new Point(door.StartPoint.X, door.StartPoint.Y, 0.0);
                Point endWindow = new Point(door.EndPoint.X, door.EndPoint.Y, 0.0);
                List<Point> points = new List<Point>();
                points.Add(startWindow);
                points.Add(endWindow);
                hole.startPoint = startWindow;
                hole.endPoint = endWindow;
                Point windowStartingVertex = new Point();
                Point wallStartingVertex = new Point();

                if (IsStartingVertex(points))
                {
                    windowStartingVertex = startWindow;
                }
                else
                {
                    windowStartingVertex = endWindow;
                }
                points.Clear();
                points.Add(point1);
                points.Add(point2);

                if (IsStartingVertex(points))
                {
                    wallStartingVertex = point1;
                }
                else
                {
                    wallStartingVertex = point2;
                }
                points.Clear();
                points.Add(wallStartingVertex);
                points.Add(windowStartingVertex);

                List<Point> wallLength = new List<Point>();
                wallLength.Add(point2);
                wallLength.Add(point1);

                double offset = (GetLength(points) + (door.Width / 2)) / GetLength(wallLength);
                hole.offset = offset;

                Normal normal = new Normal();
                normal.x = door.Normal.X;
                normal.y = door.Normal.Y;
                normal.z = door.Normal.Z;
                hole.normal = normal;

                Holes.Add(uniqueId, hole);
            }
        }

        foreach (var door in Floor.Openings)
        {
            if (ObjectIdTojsonIdDictionary.ContainsKey(door.WallId))
            {
                var line = Lines[ObjectIdTojsonIdDictionary[door.WallId]];
                Point point1 = new Point(Vertices[line.vertices[0]].x, Vertices[line.vertices[0]].y, Vertices[line.vertices[0]].z);
                Point point2 = new Point(Vertices[line.vertices[1]].x, Vertices[line.vertices[1]].y, Vertices[line.vertices[1]].z);

                Holes hole = new Holes();
                string uniqueId = GenerateId();
                hole.line = ObjectIdTojsonIdDictionary[door.WallId];
                Lines[hole.line].holes.Add(uniqueId);
                hole.type = door.DisplayName;

                string newJson = @"{
                        ""width"": 0.18,
                        ""height"": 0.35,
                        ""altitude"": 0,
                        ""thickness"": 0.1016,
                        ""style"":""""
                        }";
                var initialJson = JObject.Parse(newJson);
                initialJson["width"] = door.Width;
                initialJson["height"] = door.Height;
                initialJson["style"] = "Opening";

                string modifiedJson = initialJson.ToString();
                hole.properties = JObject.Parse(modifiedJson);
                Point startWindow = new Point(door.StartPoint.X, door.StartPoint.Y, 0.0);
                Point endWindow = new Point(door.EndPoint.X, door.EndPoint.Y, 0.0);
                List<Point> points = new List<Point>();
                points.Add(startWindow);
                points.Add(endWindow);
                hole.startPoint = startWindow;
                hole.endPoint = endWindow;
                Point windowStartingVertex = new Point();
                Point wallStartingVertex = new Point();

                if (IsStartingVertex(points))
                {
                    windowStartingVertex = startWindow;
                }
                else
                {
                    windowStartingVertex = endWindow;
                }
                points.Clear();
                points.Add(point1);
                points.Add(point2);

                if (IsStartingVertex(points))
                {
                    wallStartingVertex = point1;
                }
                else
                {
                    wallStartingVertex = point2;
                }
                points.Clear();
                points.Add(wallStartingVertex);
                points.Add(windowStartingVertex);

                List<Point> wallLength = new List<Point>();
                wallLength.Add(point2);
                wallLength.Add(point1);

                double offset = (GetLength(points) + (door.Width / 2)) / GetLength(wallLength);
                //double offset = (GetLength(points)) / GetLength(wallLength);
                hole.offset = offset;

                Normal normal = new Normal();
                normal.x = door.Normal.X;
                normal.y = door.Normal.Y;
                normal.z = door.Normal.Z;
                hole.normal = normal;

                Holes.Add(uniqueId, hole);
            }

        }
    }
    public void MakeArea(List<List<double>> vertices, int count, string spaceId)
    {
        Areas area = new Areas();
        string uniqueId = GenerateId();
        area.type = "area";
        area.name = "Space " + count;

        List<List<double>> verticeOfArea = new List<List<double>>();
        List<Point> verticeOfPolygoan = new List<Point>();
        foreach (var vertex in vertices)
        {
            List<double> WallVertices = new List<double>();
            //listOfVertices.Add(vertex);
            WallVertices.Add(vertex[0]);
            WallVertices.Add(vertex[1]);
            verticeOfArea.Add(WallVertices);
            Point point = new Point(vertex[0], vertex[1], 0.0);
            verticeOfPolygoan.Add(point);
        }

        if (verticeOfArea.Count != 0) { verticeOfArea.RemoveAt(verticeOfArea.Count - 1); }

        float[][][] polygon = new float[1][][];
        polygon[0] = FluidPoint.FluidPointCalculator.ConvertPolygonToFloatArray(verticeOfPolygoan);
        float[] fluidPointArray = FluidPoint.FluidPointCalculator.GetPolyLabel(polygon);

        area.fluidPoint = new Dictionary<string, double>();
        area.fluidPoint.Add("x", fluidPointArray[0]);
        area.fluidPoint.Add("y", fluidPointArray[1]);

        areas.Add(uniqueId, area);
    }
    public Layer MakeLayer()
    {
        var layer1 = new Layer();
        layer1.id = "layer-1";
        layer1.altitude = 0;
        layer1.vertices = Vertices;
        layer1.elevation = 0;
        //layer1.zones = zones;
        List<List<List<double>>> rooms = new List<List<List<double>>>();

        List<string> spaceIdList = new List<string>();
        foreach (var kvp in Floor.Spaces)
        {
            List<List<double>> room = new List<List<double>>();
            Dictionary<int, Point> uniqueRoomVertices = new Dictionary<int, Point>();
            var surfaces = kvp.Surfaces;
            int index = 0;

            foreach (var surface in surfaces)
            {
                var wall = surface;

                Point startPoint = new Point(wall[0][0], wall[0][1], 0.0);
                Point endPoint = new Point(wall[1][0], wall[1][1], 0.0);

                if (IsUniquePoint(startPoint, uniqueRoomVertices))
                {
                    List<double> points = new List<double>();
                    points.Add(wall[0][0]);
                    points.Add(wall[0][1]);
                    uniqueRoomVertices.Add(index++, startPoint);
                    room.Add(points);
                }
                if (IsUniquePoint(endPoint, uniqueRoomVertices))
                {
                    List<double> points = new List<double>();
                    points.Add(wall[1][0]);
                    points.Add(wall[1][1]);
                    uniqueRoomVertices.Add(index++, endPoint);
                    room.Add(points);
                }
            }
            spaceIdList.Add(kvp.ObjectId.ToString().Substring(1, kvp.ObjectId.ToString().Length - 2));
            rooms.Add(room);
        }

        int count = 101;
        int spaceCount = 0;
        foreach (var loop in rooms)
        {
            MakeArea(loop, count++, spaceIdList[spaceCount]);
            spaceCount++;
        }

        layer1.lines = Lines;
        layer1.holes = Holes;
        layer1.areas = areas;
        layer1.slabs = Slabs;
        layer1.roofSlabs = RoofSlabs;
        //layer1.structuralMembers = structuralMembers;

        layers.Add("layer-1", layer1);
        return layer1;
    }
    public void MakeSlabs()
    {
        foreach (var slab in Floor.Slabs)
        {
            Slab slabObject = new Slab();
            slabObject.lowPoint = slab.LowPoint;
            slabObject.highPoint = slab.HighPoint;
            slabObject.slabLoop = slab.SlabLoop;
            slabObject.holes = slab.Holes;

            Slabs.Add(GenerateId(), slabObject);
        }

    }
    public void MakeRoofSlabs()
    {
        foreach (var slab in Floor.RoofSlabs)
        {
            RoofSlab slabObject = new RoofSlab();
            slabObject.lowPoint = slab.LowPoint;
            slabObject.highPoint = slab.HighPoint;
            slabObject.slabLoop = slab.SlabLoop;
            slabObject.holes = slab.Holes;

            RoofSlabs.Add(GenerateId(), slabObject);
        }

    }

}


#region MaterialLibrary
public class MaterialLibrary
{
    public ExposedWall Exposed_Wall { get; set; }
    public PartitionWall Partition_Wall { get; set; }
    public GlassWall Glass_Wall { get; set; }
    public Floor Floor { get; set; }
    public Roof Roof { get; set; }
    public Ceiling Ceiling { get; set; }
}

public class ExposedWall
{
    public string uValueUnit { get; set; }
    public double absorptivity { get; set; }
    public string materialAssembly { get; set; }
    public string wallGroup { get; set; }
    public string thicknessUnit { get; set; }
    public double total_Thickness { get; set; }
    public double uValue { get; set; }
    public double transmissivity { get; set; }
    public int colorAdjustmentFactor { get; set; }
}

public class PartitionWall
{
    public string materialAssembly { get; set; }
    public string infoText { get; set; }
    public string total_Thickness { get; set; }
    public string uValue { get; set; }
    public string transmissivity { get; set; }
    public string absorptivity { get; set; }
    public string uValueUnit { get; set; }
    public string thicknessUnit { get; set; }
}

public class GlassWall
{
    public string uValueUnit { get; set; }
    public double infiltrationThickness { get; set; }
    public List<int> color { get; set; }
    public string absorptivity { get; set; }
    public double shadingCoefficient { get; set; }
    public string materialAssembly { get; set; }
    public string thicknessUnit { get; set; }
    public string total_Thickness { get; set; }
    public string infoText { get; set; }
    public string uValue { get; set; }
    public string transmissivity { get; set; }
}


public class Roof
{
    public string uValueUnit { get; set; }
    public int f { get; set; }
    public double absorptivity { get; set; }
    public double uValueSolarLoad { get; set; }
    public string materialAssembly { get; set; }
    public int roofNo { get; set; }
    public string thicknessUnit { get; set; }
    public double total_Thickness { get; set; }
    public string infoText { get; set; }
    public double uValue { get; set; }
    public double transmissivity { get; set; }
    public int colorAdjustmentFactor { get; set; }
}

public class Ceiling
{
    public string uValueUnit { get; set; }
    public int f { get; set; }
    public double absorptivity { get; set; }
    public double uValueSolarLoad { get; set; }
    public string materialAssembly { get; set; }
    public int roofNo { get; set; }
    public string thicknessUnit { get; set; }
    public double total_Thickness { get; set; }
    public string infoText { get; set; }
    public double uValue { get; set; }
    public double transmissivity { get; set; }
    public int colorAdjustmentFactor { get; set; }
}

public class CeilingMatProperties
{
    public string name { get; set; }
    public string unit { get; set; }
    public string type { get; set; }
    public double area { get; set; }
    public RoofCeilingMaterialProperties materialProperties { get; set; }
}
#endregion

#region Layers & Others
public class Layer
{
    public string id { get; set; }
    public double altitude { get; set; }
    public double elevation { get; set; }
    public Dictionary<string, Vertex> vertices { get; set; }
    public Dictionary<string, Line2D> lines { get; set; }
    public Dictionary<string, Holes> holes { get; set; }
    public Dictionary<string, Areas> areas { get; set; }
    public Dictionary<string, Zone> zones { get; set; }
    public Dictionary<string, Slab> slabs { get; set; }
    public Dictionary<string, RoofSlab> roofSlabs { get; set; }

    //public Dictionary<string, StructuralMember> structuralMembers { get; set; }
    /*public Dictionary<string, FloorMat> floorMat { get; set; }
    public Dictionary<string, CeilingMat> ceilingMat { get; set; }*/
}

public class Normal
{
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }
}
#endregion

#region Vertices
public class Vertex
{
    public string type { get; set; }
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }
    public List<string> Lines { get; set; }
}

#endregion

#region Lines
public class Line2D
{
    public string type { get; set; }
    public JObject properties { get; set; }
    public List<string> vertices { get; set; }
    public List<string> holes { get; set; }
    public WallTypes.CurtainArcWall curatainArcWall { get; set; }

    public WallTypes.ArcWall ArcWall { get; set; }

}

public class LineProperties
{
    public double length { get; set; }
    public double thickness { get; set; }
    public bool userSetPartition { get; set; }
}
#endregion

#region Areas
public class Areas
{
    public string type { get; set; }
    public string name { get; set; }
    public Dictionary<string, double> fluidPoint { get; set; }
}

#endregion

#region Holes
public class Holes
{
    public string type { get; set; }
    public JObject properties { get; set; }
    public double offset { get; set; }
    public string line { get; set; }
    //public string mountType { get; set; }
    //public string diffuserType { get; set; }
    public Normal normal { get; set; }

    public Point startPoint { get; set; }

    public Point endPoint { get; set; }
}

public class DoorProperties 
{
    public double width { get; set; }
    public double height { get; set; }
    public double thickness { get; set; }
    public double altitude { get; set; }
    public bool flip { get; set; }
    public bool infiltration { get; set; }
}

public class WindowProperties 
{
    public double width { get; set; }
    public double height { get; set; }
    public double altitude { get; set; }
    public double thickness { get; set; }
    public bool infiltration { get; set; }
    public MaterialProperties materialProperties { get; set; }
}


#endregion

#region Slabs
public class Slab
{
    public double lowPoint { get; set; }
    public double highPoint { get; set; }
    public List<List<double>> slabLoop { get; set; }
    public List<List<List<double>>> holes { get; set; }
}

public class RoofSlab
{
    public double lowPoint { get; set; }
    public double highPoint { get; set; }
    public List<List<double>> slabLoop { get; set; }
    public List<List<List<double>>> holes { get; set; }
}
#endregion

#region Material Properties
public class MaterialProperties
{
    private string materialAssembly;
    private double total_Thickness;
    private double uValue;
    private string uValueUnit = "Btu/(hr-ft²-°F)";
    private double transmissivity;
    private double absorptivity;
    private string thicknessUnit = "in";
}

public class RoofCeilingMaterialProperties : MaterialProperties
{
    private double uValueSolarLoad;
    private double colorAdjustmentFactor;
    private double f;
}

public class GlassMaterialProperties : MaterialProperties
{
    private double shadingCoefficient;
    private List<int> color;
}

#endregion
