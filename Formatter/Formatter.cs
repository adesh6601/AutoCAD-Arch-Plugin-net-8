using Newtonsoft.Json.Linq;
using Model;

public class Formatter
{
    public void Format(JObject ProjectProperties, Site site, JObject buildingJson)
    {
      
        AddSiteProperties(site, buildingJson , ProjectProperties);
        AddSite(buildingJson);
        AddBuildings(site, buildingJson);
    }
   
    public void AddSiteProperties(Site site, JObject buildingJson, JObject ProjectProperties)
    {
        buildingJson["siteProperties"] = ProjectProperties;
    }

    public void AddSite(JObject buildingJson)
    {
        buildingJson["site"] = new JObject();
    }

    public void AddBuildings(Site site, JObject buildingJson)
    {
        buildingJson["site"]["Buildings"] = new JObject();

        foreach (string buildingId in site.Buildings.Keys)
        {
            buildingJson["site"]["Buildings"][buildingId] = new JObject();

            buildingJson["site"]["Buildings"][buildingId]["units"] = "ft";

            buildingJson["site"]["Buildings"][buildingId]["Levels"] = new JObject();

            foreach (string floorId in site.Buildings[buildingId].Floors.Keys)
            {
                LevelFormatter floorObject = new LevelFormatter(site.Buildings[buildingId].Floors[floorId]);
                JObject layerJObject = JObject.FromObject(floorObject.layers["layer-1"]);

                // Add elevation and altitude properties
                var keyValuePairs = buildingJson["siteProperties"]["Level"][floorId];
                layerJObject["id"] = floorId;
                layerJObject["elevation"] = double.Parse(keyValuePairs["Elevation"].ToString());
                layerJObject["altitude"] = double.Parse(keyValuePairs["Height"].ToString());

                // Add the updated layerJObject to the Levels object in the JSON
                buildingJson["site"]["Buildings"][buildingId]["Levels"][floorId] = layerJObject;

            }

        }
    }   
}
