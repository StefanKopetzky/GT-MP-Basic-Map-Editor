using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using GT_MP_Basic_Map_Editor.Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace GT_MP_Basic_Map_Editor.Server.Services
{
    internal class ObjectBuilder : Script
    {
        public static ModelList ObjectModels = null;

        public ObjectBuilder()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            if (!System.IO.Directory.Exists("MapEditorSettings"))
            {
                System.IO.Directory.CreateDirectory("MapEditorSettings");
            }
            if (System.IO.File.Exists("MapEditorSettings/ModelList.xml"))
            {
                System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(ModelList));
                System.IO.StreamReader file = new System.IO.StreamReader("MapEditorSettings/ModelList.xml");
                ObjectModels = (ModelList)reader.Deserialize(file);
                file.Close();
                if (ObjectModels == null)
                {
                    API.consoleOutput(LogCat.Error, "Parsing of ModelList.xml failed..");
                    ObjectModels = new ModelList();
                }
                else
                {
                    API.consoleOutput(LogCat.Info, "Parsing of ModelList.xml succeed..");
                }
            }
            else
            {
                API.consoleOutput(LogCat.Info, "MapEditorSettings/ModelList.xml does not exist.. Skip..");
                ObjectModels = new ModelList();
            }
        }

        public static Dictionary<int, TextLabel> Labels = new Dictionary<int, TextLabel>();
        public static int NextObjId = 1;

        [Command("addobject")]
        public void CreateObject(Client player, int objectHash)
        {
            AddObjectCmd(player, objectHash);
        }

        public static void AddObjectCmd(Client player, int objectHash)
        {
            var obj = API.shared.createObject(objectHash, player.position - new Vector3(0, 0, 1), player.rotation);
            var label = API.shared.createTextLabel("Object ID: ~b~" + NextObjId + "~n~~w~Placed by ~y~" + player.name, player.position + new Vector3(0, 0, 1), 20f, 0.5f, true);
            API.shared.attachEntityToEntity(label, obj, "", new Vector3(0, 0, 1), new Vector3());
            Labels.Add(NextObjId, label);
            Handler.MainHandler.Objects.Add(NextObjId, obj);
            EnableObjectEditor(player, obj);
            NextObjId++;
        }

        [Command("deleteobject")]
        public void DeleteObject(Client player, int objId)
        {
            DeleteObjectCmd(player, objId);
        }

        public static void DeleteObjectCmd(Client player, int objId)
        {
            if (Handler.MainHandler.Objects.ContainsKey(objId))
            {
                API.shared.deleteEntity(Handler.MainHandler.Objects[objId]);
                Handler.MainHandler.Objects.Remove(objId);
                API.shared.deleteEntity(Labels[objId]);
                Labels.Remove(objId);
                player.sendNotification("", $"~g~Object ID: {objId} removed..");
            }
            else
            {
                player.sendNotification("", $"~r~Object ID: {objId} not exist..");
            }
        }

        [Command("addobjectcategory")]
        public void AddObjectCategory(Client player, string name)
        {
            if (ObjectModels.Categories.FirstOrDefault(x => x.Name == name) != null)
            {
                API.sendNotificationToPlayer(player, $"Category ~y~{name} ~w~already exists..");
                return;
            }
            ObjectModels.Categories.Add(new ModelCategory(name));
            API.sendNotificationToPlayer(player, $"Category ~y~{name} ~w~added..");
        }

        [Command("addobjecttocategory", GreedyArg = true)]
        public void AddObjectToCategory(Client player, string category, int modelHash, string objectName)
        {
            if (ObjectModels.Categories.FirstOrDefault(x => x.Name == category) == null)
            {
                API.sendNotificationToPlayer(player, $"Category ~y~{category} ~w~does not exist..");
                return;
            }
            ObjectModels.Categories.First(x => x.Name == category).Models.Add(new ModelInformation
            {
                ModelHash = modelHash,
                Name = objectName
            });
        }

        [Command("savemodellist")]
        public void SaveModelList(Client player)
        {
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(ModelList));
            System.IO.FileStream file = System.IO.File.Create("MapEditorSettings/ModelList.xml");
            writer.Serialize(file, ObjectModels);
            file.Close();
            API.sendNotificationToPlayer(player, "ModelList saved..");
        }

        [Command("editobject")]
        public void EditObject(Client player, int id)
        {
            if (Handler.MainHandler.CurrentMap == null)
                return;
            EditObjectCmd(player, id);
        }

        public static void EditObjectCmd(Client player, int objectId)
        {
            EnableObjectEditor(player, Handler.MainHandler.Objects[objectId]);
            API.shared.sendNotificationToPlayer(player, "Edit Object ID: ~b~" + objectId);
        }

        public static void CopyObject(Client player, int objectId)
        {
            if (!Handler.MainHandler.Objects.ContainsKey(objectId))
                return;
            var objToCopy = Handler.MainHandler.Objects[objectId];
            var obj = API.shared.createObject(objToCopy.model, objToCopy.position, objToCopy.rotation);
            var label = API.shared.createTextLabel("Object ID: ~b~" + NextObjId + "~n~~w~Placed by ~y~" + player.name, player.position + new Vector3(0, 0, 1), 20f, 0.5f, true);
            API.shared.attachEntityToEntity(label, obj, "", new Vector3(0, 0, 1), new Vector3());
            Labels.Add(NextObjId, label);
            Handler.MainHandler.Objects.Add(NextObjId, obj);
            EnableObjectEditor(player, obj);
            API.shared.sendNotificationToPlayer(player, "Edit Object ID: ~b~" + NextObjId);
            NextObjId++;
        }

        [Command("exiteditor")]
        public void ExitObjectEditor(Client player)
        {
            DisableObjectEditor(player);
            player.sendChatMessage("Disabled Edit Object");
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (!player.hasData("ObjEditor"))
                return;
            var CurrentObject = player.GetObject();
            double multiplier = 1;
            switch (eventName)
            {
                case "NumPad0": // Switch Position / Rotation Mode
                    if (player.PositionMode())
                    {
                        player.ActivateRotationMode();
                        return;
                    }
                    player.ActivatePositionMode();
                    break;

                case "NumPad2": // X-
                    if ((bool)arguments[0]) { multiplier = 2; }
                    if (player.PositionMode())
                    {
                        API.setEntityPosition(CurrentObject, CurrentObject.position - new Vector3(0.1 * multiplier, 0, 0));
                        break;
                    }
                    API.setEntityRotation(CurrentObject, CurrentObject.rotation - new Vector3(0.2 * multiplier, 0, 0));
                    break;

                case "NumPad4": // Y+
                    if ((bool)arguments[0]) { multiplier = 2; }
                    if (player.PositionMode())
                    {
                        API.setEntityPosition(CurrentObject, CurrentObject.position + new Vector3(0, 0.1 * multiplier, 0));
                        break;
                    }
                    API.setEntityRotation(CurrentObject, CurrentObject.rotation + new Vector3(0, 0.2 * multiplier, 0));
                    break;

                case "NumPad6": // Y-
                    if ((bool)arguments[0]) { multiplier = 2; }
                    if (player.PositionMode())
                    {
                        API.setEntityPosition(CurrentObject, CurrentObject.position - new Vector3(0, 0.1 * multiplier, 0));
                        break;
                    }
                    API.setEntityRotation(CurrentObject, CurrentObject.rotation - new Vector3(0, 0.2 * multiplier, 0));
                    break;

                case "NumPad8": // X+
                    if ((bool)arguments[0]) { multiplier = 2; }
                    if (player.PositionMode())
                    {
                        API.setEntityPosition(CurrentObject, CurrentObject.position + new Vector3(0.1 * multiplier, 0, 0));
                        break;
                    }
                    API.setEntityRotation(CurrentObject, CurrentObject.rotation + new Vector3(0.2 * multiplier, 0, 0));
                    break;

                case "KeyAdd": // Z+
                    if ((bool)arguments[0]) { multiplier = 2; }
                    if (player.PositionMode())
                    {
                        API.setEntityPosition(CurrentObject, CurrentObject.position + new Vector3(0, 0, 0.05 * multiplier));
                        break;
                    }
                    API.setEntityRotation(CurrentObject, CurrentObject.rotation + new Vector3(0, 0, 0.5 * multiplier));
                    break;

                case "KeySubtract": // Z-
                    if ((bool)arguments[0]) { multiplier = 2; }
                    if (player.PositionMode())
                    {
                        API.setEntityPosition(CurrentObject, CurrentObject.position - new Vector3(0, 0, 0.05 * multiplier));
                        break;
                    }
                    API.setEntityRotation(CurrentObject, CurrentObject.rotation - new Vector3(0, 0, 0.5 * multiplier));
                    break;
            }
        }

        public static void EnableObjectEditor(Client player, GrandTheftMultiplayer.Server.Elements.Object obj)
        {
            player.setData("ObjEditor", obj);
            player.triggerEvent("EditorStatus", true);
        }

        public static void DisableObjectEditor(Client player)
        {
            player.resetData("ObjEditor");
            player.triggerEvent("EditorStatus", false);
        }

        public static List<MenuItem> GetEditorMenuItems()
        {
            var menuitems = new List<MenuItem>
            {
                new MenuItem("Placed Objects"){
                    SelectEvent = "showplacedobjects"
                },
                new MenuItem("Create Object from Gallery"){
                    SelectEvent = "showobjectgallery"
                },
                new MenuItem("Create Object from Hash"){
                    UserInput = true,
                    DefaultUserInput = "ObjectHash",
                    SelectEvent = "createobjectfromhash"
                },
                new MenuItem("Help")
                {
                    SelectEvent = "objecteditorhelp"
                },
                new MenuItem("~o~Exit Object Editor")
                {
                    SelectEvent = "exitobjecteditor"
                }
            };
            return menuitems;
        }
    }

    public static class ClientExtension
    {
        #region Extension

        public static GrandTheftMultiplayer.Server.Elements.Object GetObject(this Client player)
        {
            if (!player.hasData("ObjEditor"))
                return null;
            return (GrandTheftMultiplayer.Server.Elements.Object)player.getData("ObjEditor");
        }

        public static bool PositionMode(this Client player)
        {
            if (!player.hasData("rotmode"))
                return true;
            return false;
        }

        public static void ActivatePositionMode(this Client player)
        {
            player.resetData("rotmode");
            player.sendNotification("", "PositionMode");
        }

        public static void ActivateRotationMode(this Client player)
        {
            player.setData("rotmode", true);
            player.sendNotification("", "RotationMode");
        }

        #endregion Extension
    }
}