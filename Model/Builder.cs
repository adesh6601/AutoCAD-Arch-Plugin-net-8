using Collection;
using Component;

namespace Model
{
    public class Builder
    {
        public void Build(Entities entities, Site site)
        {
            AddCurtainWallLayoutToBuilding(entities, site);
            AddCurtainWallUnitToBuilding(entities, site);

            AddDoorsToBuilding(entities, site);
            AddOpeningsToBuilding(entities, site);
            AddWallsToBuilding(entities, site);
            AddWindowsToBuilding(entities, site);

            AddWindowAssembliesToBuilding(entities, site);
            AddWindowAssembliesAsWallsToBuilding(entities, site);

            AddSlabsToBuilding(entities, site);
            AddRoofSlabsToBuilding(entities, site);

            AddSpacesToBuilding(entities, site);
            AddZonesToBuilding(entities, site);
        }

        private void AddCurtainWallLayoutToBuilding(Entities entities, Site site)
        {
            foreach (CurtainWallLayout curtainWallLayout in entities.CurtainWallLayouts)
            {
                foreach (string buildingId in curtainWallLayout.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = curtainWallLayout.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].CurtainWallLayouts.Add(curtainWallLayout);
                    }
                }
            }
        }

        private void AddCurtainWallUnitToBuilding(Entities entities, Site site)
        {
            foreach (CurtainWallUnit curtainWallUnit in entities.CurtainWallUnits)
            {
                foreach (string buildingId in curtainWallUnit.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = curtainWallUnit.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].CurtainWallUnits.Add(curtainWallUnit);
                    }
                }
            }
        }

        private void AddDoorsToBuilding(Entities entities, Site site)
        {
            foreach (Door door in entities.Doors)
            {
                foreach (string buildingId in door.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = door.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].Doors.Add(door);
                    }
                }
            }
        }

        private void AddOpeningsToBuilding(Entities entities, Site site)
        {
            foreach (Opening opening in entities.Openings)
            {
                foreach (string buildingId in opening.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = opening.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].Openings.Add(opening);
                    }
                }
            }
        }

        private void AddWallsToBuilding(Entities entities, Site site)
        {
            foreach (Wall wall in entities.Walls)
            {
                foreach (string buildingId in wall.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = wall.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].Walls.Add(wall);
                    }
                }
            }
        }

        private void AddWindowsToBuilding(Entities entities, Site site)
        {
            foreach (Window window in entities.Windows)
            {
                foreach (string buildingId in window.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = window.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].Windows.Add(window);
                    }
                }
            }
        }

        private void AddWindowAssembliesToBuilding(Entities entities, Site site)
        {
            foreach (WindowAssembly windowAssembly in entities.WindowAssemblies)
            {
                foreach (string buildingId in windowAssembly.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = windowAssembly.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].WindowAssemblies.Add(windowAssembly);
                    }
                }
            }
        }

        private void AddWindowAssembliesAsWallsToBuilding(Entities entities, Site site)
        {
            foreach (WindowAssembly windowAssembly in entities.WindowAssembliesAsWall)
            {
                foreach (string buildingId in windowAssembly.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = windowAssembly.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].WindowAssembliesAsWall.Add(windowAssembly);
                    }
                }
            }
        }

        private void AddSlabsToBuilding(Entities entities, Site site)
        {
            foreach (Slab slab in entities.Slabs)
            {
                foreach (string buildingId in slab.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = slab.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].Slabs.Add(slab);
                    }
                }
            }
        }

        private void AddRoofSlabsToBuilding(Entities entities, Site site)
        {
            foreach (RoofSlab roofSlab in entities.RoofSlabs)
            {
                foreach (string buildingId in roofSlab.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = roofSlab.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].RoofSlabs.Add(roofSlab);
                    }
                }
            }
        }

        private void AddSpacesToBuilding(Entities entities, Site site)
        {
            foreach (Space space in entities.Spaces)
            {
                foreach (string buildingId in space.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = space.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].Spaces.Add(space);
                    }
                }
            }
        }

        private void AddZonesToBuilding(Entities entities, Site site)
        {
            foreach (Zone zone in entities.Zones)
            {
                foreach (string buildingId in zone.DivisionsAndLevels.Keys)
                {
                    if (!site.Buildings.ContainsKey(buildingId))
                    {
                        site.Buildings[buildingId] = new Building();
                    }

                    HashSet<string> floorIds = zone.DivisionsAndLevels[buildingId];

                    foreach (string floorId in floorIds)
                    {
                        if (!site.Buildings[buildingId].Floors.ContainsKey(floorId))
                        {
                            site.Buildings[buildingId].Floors[floorId] = new Floor();
                        }

                        site.Buildings[buildingId].Floors[floorId].Zones.Add(zone);
                    }
                }
            }
        }
    }
}
