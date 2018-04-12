using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;

namespace GT_MP_Basic_Map_Editor.Server.Models
{
    public class Map
    {
        public string Name { get; set; }
        public string Author { get; set; }

        public List<MapObject> MapObjects { get; set; }

        public Map()
        {
            Name = "Unknown";
            Author = "Unknown";
            MapObjects = new List<MapObject>();
        }
    }

    public class MapObject
    {
        public int Model { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public bool Frozen { get; set; }

        public MapObject()
        {
            Model = 0;
            Position = new Vector3();
            Rotation = new Vector3();
            Frozen = true;
        }
    }
}