using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GT_MP_Basic_Map_Editor.Server.Models;

namespace GT_MP_Basic_Map_Editor.Server.Services
{
    class MapService
    {
        public static void SaveMap(Map track, string FileName)
        {
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(Map));
            if (!System.IO.Directory.Exists("Maps"))
            {
                System.IO.Directory.CreateDirectory("Maps");
                API.shared.consoleOutput("Created Maps Folder at Root Directory");
            }
            System.IO.FileStream file = System.IO.File.Create("Maps/" + FileName + ".xml");

            writer.Serialize(file, track);
            file.Close();
        }

        public static Map LoadMapFromFile(string FileName)
        {
            if (!System.IO.Directory.Exists("Maps"))
            {
                System.IO.Directory.CreateDirectory("Maps");
                API.shared.consoleOutput("Created Maps Folder at Root Directory");
            }
            if (!System.IO.File.Exists($"Maps/{FileName}.xml"))
            {
                API.shared.consoleOutput(LogCat.Error, $"MapService: File {FileName} Not Found");
                return null;
            }
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Map));
            System.IO.StreamReader file = new System.IO.StreamReader($"Maps/{FileName}.xml");
            Map map = (Map)reader.Deserialize(file);
            file.Close();
            if (map == null)
            {
                API.shared.consoleOutput(LogCat.Info, $"MapService: {FileName} Parsing failed..");
            }
            else
            {
                API.shared.consoleOutput(LogCat.Info, "MapService: Map '" + map.Name + "' loaded..");
            }
            return map;
        }
    }
}
