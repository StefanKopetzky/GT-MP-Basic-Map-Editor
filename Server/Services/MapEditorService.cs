using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared.Math;
using GT_MP_Basic_Map_Editor.Server.Models;
using System;

namespace GT_MP_Basic_Map_Editor.Server.Services
{
    class MapEditorService
    {
        public static void SaveMap(Map track, string FileName)
        {
            Models.Output.MapEditor.Map newmap = new Models.Output.MapEditor.Map();

            track.MapObjects.ForEach(x =>
            {
                newmap.Objects.Add(new Models.Output.MapEditor.MapObject {
                    Type = "Prop",
                    Position = x.Position,
                    Rotation = x.Rotation,
                    Hash = x.Model,
                    Dynamic = false,
                    Quaternion = Vector3ToQuaternion(x.Rotation),
                    SirensActive = false
                });
            });

            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(Models.Output.MapEditor.Map));
            if (!System.IO.Directory.Exists("MapEditorMaps"))
            {
                System.IO.Directory.CreateDirectory("MapEditorMaps");
                API.shared.consoleOutput("Created Maps Folder at Root Directory");
            }
            System.IO.FileStream file = System.IO.File.Create("MapEditorMaps/" + FileName + ".xml");

            writer.Serialize(file, newmap);
            file.Close();
        }

        public static Map LoadMapFromFile(string FileName)
        {
            if (!System.IO.Directory.Exists("MapEditorMaps"))
            {
                System.IO.Directory.CreateDirectory("MapEditorMaps");
                API.shared.consoleOutput("Created MapEditorMaps Folder at Root Directory");
            }
            if (!System.IO.File.Exists($"MapEditorMaps/{FileName}.xml"))
            {
                API.shared.consoleOutput(LogCat.Error, $"MapEditorService: File {FileName} Not Found");
                return null;
            }
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Models.Output.MapEditor.Map));
            System.IO.StreamReader file = new System.IO.StreamReader($"MapEditorMaps/{FileName}.xml");
            Models.Output.MapEditor.Map map = (Models.Output.MapEditor.Map)reader.Deserialize(file);
            file.Close();
            if (map == null)
            {
                API.shared.consoleOutput(LogCat.Info, $"MapEditorService: {FileName} Parsing failed..");
            }
            else
            {
                API.shared.consoleOutput(LogCat.Info, "MapEditorService: Map loaded..");
            }

            Map exp = new Map();
            exp.Name = FileName;
            map.Objects.ForEach(x =>
            {
                exp.MapObjects.Add(new MapObject
                {
                    Model = x.Hash,
                    Position = x.Position,
                    Rotation = x.Rotation,
                    Frozen = !x.Dynamic
                });
            });

            return exp;
        }

        public static Quaternion Vector3ToQuaternion(Vector3 vector3)
        {
            var c1 = Math.Cos(vector3.X / 2);
            var c2 = Math.Cos(vector3.Y / 2);
            var c3 = Math.Cos(vector3.Z / 2);
            var s1 = Math.Sin(vector3.X / 2);
            var s2 = Math.Sin(vector3.Y / 2);
            var s3 = Math.Sin(vector3.Z / 2);
            var x = s1 * c2 * c3 + c1 * s2 * s3;
            var y = c1 * s2 * c3 - s1 * c2 * s3;
            var z = c1 * c2 * s3 + s1 * s2 * c3;
            var w = c1 * c2 * c3 - s1 * s2 * s3;

            return new Quaternion(x, y, z, w);
        }
    }
}
