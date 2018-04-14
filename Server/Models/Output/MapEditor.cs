using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;

namespace GT_MP_Basic_Map_Editor.Server.Models.Output.MapEditor
{
    public class Map
    {
        public List<MapObject> Objects = new List<MapObject>();
    }

    public class MapObject
    {
        public string Type { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int Hash { get; set; }
        public bool Dynamic { get; set; }
        public Quaternion Quaternion { get; set; }
        public bool SirensActive { get; set; }
    }
}