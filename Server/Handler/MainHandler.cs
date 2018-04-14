using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GT_MP_Basic_Map_Editor.Server.Extensions;
using GT_MP_Basic_Map_Editor.Server.Models;
using GT_MP_Basic_Map_Editor.Server.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GT_MP_Basic_Map_Editor.Server.Handler
{
    class MainHandler : Script
    {
        public MainHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerConnected += API_onPlayerConnected;
        }

        private void API_onPlayerConnected(Client player)
        {
            API.sendChatMessageToPlayer(player, "~h~~b~The Basic Map Editor is running on this server.");
            API.sendChatMessageToPlayer(player, "Use the F2 key to open the Editor Menu");
        }
        public Dictionary<string, Action<object[]>> MenuItemActions = new Dictionary<string, Action<object[]>>();
        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "F2KeyPressed":
                    Menu menu = new Menu
                    {
                        Title = "Map Editor",
                        SubTitle = "by ~b~Neta"
                    };
                    if (CurrentMap == null)
                    {
                        menu.AddItem(new MenuItem("Create new Map")
                        {
                            SelectEvent = "createnewtrack",
                            UserInput = true,
                            DefaultUserInput = "Enter Map Name"
                        });
                        menu.AddItem(new MenuItem("Load Map")
                        {
                            SelectEvent = "loadtrackdialog"
                        });
                        menu.AddItem(new MenuItem("Load MapEditor Map", "~y~Guadmaz Map Editor")
                        {
                            SelectEvent = "loadmapeditordialog"
                        });
                    }
                    else
                    {
                        if (sender.GetObject() == null)
                        {
                            menu.AddItem(new MenuItem("Title", "", CurrentMap.Name)
                            {
                                SelectEvent = "settracktitle",
                                UserInput = true,
                                DefaultUserInput = "Enter new Map Title"
                            });
                            menu.AddItem(new MenuItem("Open object placer")
                            {
                                SelectEvent = "forceobjectmenu"
                            });
                            menu.AddItem(new MenuItem("~r~Cancel creation")
                            {
                                SelectEvent = "cancelcreationdialog",
                            });
                            menu.AddItem(new MenuItem("~b~Save Map")
                            {
                                SelectEvent = "savetrack"
                            });
                            menu.AddItem(new MenuItem("~b~Save as MapEditor Map", "~y~Guadmaz Map Editor")
                            {
                                SelectEvent = "savemapeditor"
                            });
                        }
                        else
                        {
                            menu.Items.AddRange(ObjectBuilder.GetEditorMenuItems()); // Menu Item functions not implemented ... todo
                        }
                    }
                    menu.Show(sender);
                    break;

                #region UnloadedTrackMenu
                case "createnewtrack":
                    {
                        if (CurrentMap != null) return;
                        if ((string)arguments[0] == "" || (string)arguments[0] == " " || (string)arguments[0] == "  " || (string)arguments[0] == "Enter Track Name")
                        {
                            API.sendNotificationToPlayer(sender, "~r~Name cannot be empty..");
                            return;
                        }
                        API.sendNotificationToAll("Map creation started. (~b~" + arguments[0] + "~w~)");
                        CurrentMap = new Map
                        {
                            Name = (string)arguments[0],
                            Author = sender.socialClubName
                        };
                        sender.CloseAllMenus();
                    }
                    break;
                case "loadtrack":
                    {
                        LoadMapCmd(sender, Convert.ToString(arguments[0]));
                        sender.CloseAllMenus();
                    }
                    break;
                case "loadmapeditormap":
                    {
                        LoadEditorMapCmd(sender, Convert.ToString(arguments[0]));
                        sender.CloseAllMenus();
                    }
                    break;
                case "loadtrackdialog":
                    {
                        var mapmenu = new Menu("Map List", "Select a Map");
                        if (!Directory.Exists("Maps"))
                        {
                            Directory.CreateDirectory("Maps");
                            API.shared.consoleOutput("Created Maps Folder at Root Directory");
                        }
                        var maps = Directory.GetFiles("Maps");
                        if (maps.Length <= 0)
                        {
                            mapmenu.AddItem(new MenuItem("No maps found..")
                            {
                                SelectEvent = "F2KeyPressed"
                            });
                        }
                        else
                        {
                            foreach (string map in maps)
                            {
                                var mapfile = map.Split('.');
                                mapmenu.AddItem(new MenuItem(mapfile[0])
                                {
                                    SelectEvent = "loadtrack",
                                    SelectEventArg = mapfile[0].Split('\\')[1]
                                });
                            }
                        }
                        mapmenu.Show(sender);
                    }
                    break;
                case "loadmapeditordialog":
                    {
                        var mapmenu = new Menu("MapEditor List", "Select a Map");
                        if (!Directory.Exists("MapEditorMaps"))
                        {
                            Directory.CreateDirectory("MapEditorMaps");
                            API.shared.consoleOutput("Created MapEditorMaps Folder at Root Directory");
                        }
                        var maps = Directory.GetFiles("MapEditorMaps");
                        if (maps.Length <= 0)
                        {
                            mapmenu.AddItem(new MenuItem("No maps found..")
                            {
                                SelectEvent = "F2KeyPressed"
                            });
                        }
                        else
                        {
                            foreach (string map in maps)
                            {
                                var mapfile = map.Split('.');
                                mapmenu.AddItem(new MenuItem(mapfile[0])
                                {
                                    SelectEvent = "loadmapeditormap",
                                    SelectEventArg = mapfile[0].Split('\\')[1]
                                });
                            }
                        }
                        mapmenu.Show(sender);
                    }
                    break;
                #endregion UnloadedTrackMenu
                #region LoadedTrackMenu
                case "forceobjectmenu":
                    {
                        var objmenu = new Menu("Object Placer");
                        objmenu.Items.AddRange(ObjectBuilder.GetEditorMenuItems());
                        objmenu.Show(sender);
                    }
                    break;
                case "savetrack":
                    {
                        API.sendNotificationToAll($"~o~{sender.name}~w~ saved the map as ~b~Maps/{CurrentMap.Name}.xml");
                        SaveRace(sender);
                        sender.CloseAllMenus();
                    }
                    break;
                case "savemapeditor":
                    {
                        API.sendNotificationToAll($"~o~{sender.name}~w~ saved the map as ~b~MapEditorMaps/{CurrentMap.Name}.xml");
                        SaveRace(sender, true);
                        sender.CloseAllMenus();
                    }
                    break;
                case "settracktitle":
                    {
                        CurrentMap.Name = (string)arguments[0];
                        sender.CloseAllMenus();
                        API.sendNotificationToAll($"~o~{sender.name}~w~ changed the map name to ~b~{CurrentMap.Name}");
                    }
                    break;
                case "cancelcreationdialog":
                    {
                        var cancelmenu = new Menu("Cancel Creation", "Do you want to cancel the creation?");
                        cancelmenu.AddItem(new MenuItem("No")
                        {
                            SelectEvent = "cancelcreation",
                            SelectEventArg = "0"
                        });
                        cancelmenu.AddItem(new MenuItem("~r~Yes")
                        {
                            SelectEvent = "cancelcreation",
                            SelectEventArg = "1"
                        });
                        cancelmenu.Show(sender);
                    }
                    break;
                case "cancelcreation":
                    {
                        if (Convert.ToInt32((string)arguments[0]) == 1)
                        {
                            ClearMap();
                            API.sendNotificationToAll($"Map cleared by ~b~{sender.name}");
                        }
                        sender.CloseAllMenus();
                    }
                    break;
                #endregion LoadedTrackMenu
                #region Object Editor
                case "showplacedobjects":
                    {
                        var placedobjmenu = new Menu("Placed Objects", "Select to Edit");
                        Objects.ToList().ForEach(obj =>
                        {
                            placedobjmenu.AddItem(new MenuItem($"ID: {obj.Key}", $"Model: {obj.Value.model}")
                            {
                                SelectEvent = "placedobjectdialog",
                                SelectEventArg = obj.Key.ToString()
                            });
                        });
                        if (placedobjmenu.Items.Count == 0)
                        {
                            placedobjmenu.AddItem(new MenuItem("No Objects placed..")
                            {
                                SelectEvent = "forceobjectmenu"
                            });
                        }
                        placedobjmenu.Show(sender);
                    }
                    break;
                case "placedobjectdialog":
                    {
                        int objid = Convert.ToInt32((string)arguments[0]);
                        var placedobjdialog = new Menu("Object ID: " + objid);
                        placedobjdialog.AddItem(new MenuItem("~b~Edit")
                        {
                            SelectEvent = "editobjectfrommenu",
                            SelectEventArg = objid.ToString()
                        });
                        placedobjdialog.AddItem(new MenuItem("~y~Copy")
                        {
                            SelectEvent = "copyobjectfrommenu",
                            SelectEventArg = objid.ToString()
                        });
                        placedobjdialog.AddItem(new MenuItem("~r~Delete")
                        {
                            SelectEvent = "deleteobjectfrommenu",
                            SelectEventArg = objid.ToString()
                        });
                        placedobjdialog.Show(sender);
                    }
                    break;
                case "editobjectfrommenu":
                    {
                        ObjectBuilder.EditObjectCmd(sender, Convert.ToInt32((string)arguments[0]));
                        sender.CloseAllMenus();
                    }
                    break;
                case "copyobjectfrommenu":
                    {
                        ObjectBuilder.CopyObject(sender, Convert.ToInt32((string)arguments[0]));
                        sender.CloseAllMenus();
                    }
                    break;
                case "deleteobjectfrommenu":
                    {
                        ObjectBuilder.DeleteObjectCmd(sender, Convert.ToInt32((string)arguments[0]));
                        sender.CloseAllMenus();
                    }
                    break;
                case "showobjectgallery":
                    {
                        var objectgallery = new Menu("Object Gallery", "Select a Category");
                        ObjectBuilder.ObjectModels.Categories.ForEach(category =>
                        {
                            objectgallery.AddItem(new MenuItem(category.Name, "", $"({category.Models.Count})")
                            {
                                SelectEvent = "showobjectgallerycategory",
                                SelectEventArg = category.Name
                            });
                        });
                        if (objectgallery.Items.Count == 0)
                        {
                            objectgallery.AddItem(new MenuItem("No Categories found..")
                            {
                                SelectEvent = "forceobjectmenu"
                            });
                        }
                        objectgallery.Show(sender);
                    }
                    break;
                case "showobjectgallerycategory":
                    {
                        string selectedcategory = (string)arguments[0];
                        var categorygallery = new Menu(selectedcategory);
                        ObjectBuilder.ObjectModels.Categories.First(x => x.Name == selectedcategory).Models.ForEach(model =>
                        {
                            categorygallery.AddItem(new MenuItem(model.Name, $"~b~Model Hash:~w~ {model.ModelHash}")
                            {
                                SelectEvent = "createobjectfromhash",
                                SelectEventArg = model.ModelHash.ToString()
                            });
                        });
                        categorygallery.Show(sender);
                    }
                    break;
                case "createobjectfromhash":
                    {
                        ObjectBuilder.AddObjectCmd(sender, Convert.ToInt32((string)arguments[0]));
                        sender.CloseAllMenus();
                    }
                    break;
                case "exitobjecteditor":
                    {
                        ObjectBuilder.DisableObjectEditor(sender);
                        API.sendNotificationToPlayer(sender, "Object Editor ~r~Disabled");
                        sender.CloseAllMenus();
                    }
                    break;
                case "objecteditorhelp":
                    {
                        API.sendChatMessageToPlayer(sender, "~o~========================================");
                        API.sendChatMessageToPlayer(sender, "~o~=========== ~y~Object Editor Help ~o~==========");
                        API.sendChatMessageToPlayer(sender, "~o~========================================");
                        API.sendChatMessageToPlayer(sender, "~b~NumPad8 ~w~|| ~g~X-Axis +");
                        API.sendChatMessageToPlayer(sender, "~b~NumPad2 ~w~|| ~g~X-Axis -");
                        API.sendChatMessageToPlayer(sender, "~b~NumPad4 ~w~|| ~g~Y-Axis +");
                        API.sendChatMessageToPlayer(sender, "~b~NumPad6 ~w~|| ~g~Y-Axis -");
                        API.sendChatMessageToPlayer(sender, "~b~NumPad+ ~w~|| ~g~Z-Axis +");
                        API.sendChatMessageToPlayer(sender, "~b~NumPad- ~w~|| ~g~Z-Axis -");
                        API.sendChatMessageToPlayer(sender, "~b~NumPad0 ~w~|| ~g~Switch Position/Rotation Mode");
                        sender.CloseAllMenus();
                    }
                    break;
                    #endregion Object Editor
            }
        }

        public static Map CurrentMap = null;
        public List<Blip> Blips = new List<Blip>();
        public List<Marker> Markers = new List<Marker>();
        public List<Vehicle> Vehicles = new List<Vehicle>();
        public static Dictionary<int, GrandTheftMultiplayer.Server.Elements.Object> Objects = new Dictionary<int, GrandTheftMultiplayer.Server.Elements.Object>();
        [Command("createmap", GreedyArg = true)]
        public void CreateRace(Client client, string raceName)
        {
            if (CurrentMap != null) return;
            API.sendChatMessageToPlayer(client, "Map creation started. (~b~" + raceName + "~w~)");
            CurrentMap = new Map
            {
                Name = raceName,
                Author = client.socialClubName
            };
        }

        [Command("loadmap")]
        public void LoadMapCmd(Client player, string filename)
        {
            Map map = MapService.LoadMapFromFile(filename);
            if (map == null)
            {
                player.sendChatMessage("~r~Error~w~: Map not found..");
                return;
            }
            CurrentMap = map;

            map.MapObjects.ForEach(obj =>
            {
                var cobj = API.createObject(obj.Model, obj.Position, obj.Rotation);
                var label = API.createTextLabel("Object ID: ~b~" + ObjectBuilder.NextObjId, player.position + new Vector3(0, 0, 1), 20f, 0.5f, true);
                API.attachEntityToEntity(label, cobj, "", new Vector3(0, 0, 1), new Vector3());
                ObjectBuilder.Labels.Add(ObjectBuilder.NextObjId, label);
                Objects.Add(ObjectBuilder.NextObjId, cobj);
                ObjectBuilder.NextObjId++;
            });
            CurrentMap.MapObjects.Clear();
            API.sendNotificationToAll($"Map ~b~{CurrentMap.Name} ~w~loaded by ~y~{player.name}");
        }

        [Command("loadeditormap")]
        public void LoadEditorMapCmd(Client player, string filename)
        {
            Map map = MapEditorService.LoadMapFromFile(filename);
            if (map == null)
            {
                player.sendChatMessage("~r~Error~w~: Map not found..");
                return;
            }
            CurrentMap = map;

            map.MapObjects.ForEach(obj =>
            {
                var cobj = API.createObject(obj.Model, obj.Position, obj.Rotation);
                var label = API.createTextLabel("Object ID: ~b~" + ObjectBuilder.NextObjId, player.position + new Vector3(0, 0, 1), 20f, 0.5f, true);
                API.attachEntityToEntity(label, cobj, "", new Vector3(0, 0, 1), new Vector3());
                ObjectBuilder.Labels.Add(ObjectBuilder.NextObjId, label);
                Objects.Add(ObjectBuilder.NextObjId, cobj);
                ObjectBuilder.NextObjId++;
            });
            CurrentMap.MapObjects.Clear();
            API.sendNotificationToAll($"Map ~b~{CurrentMap.Name} ~w~loaded by ~y~{player.name}");
        }

        public void LoadMap(Client player, string fileName)
        {
            Map map = MapService.LoadMapFromFile(fileName);
            if (map == null)
            {
                player.sendChatMessage("~r~Error~w~: Map not found..");
                return;
            }
            CurrentMap = map;

            map.MapObjects.ForEach(obj =>
            {
                var cobj = API.createObject(obj.Model, obj.Position, obj.Rotation);
                var label = API.createTextLabel("Object ID: ~b~" + ObjectBuilder.NextObjId, player.position + new Vector3(0, 0, 1), 30f, 1f, true);
                API.attachEntityToEntity(label, cobj, "", new Vector3(0, 0, 1), new Vector3());
                ObjectBuilder.Labels.Add(ObjectBuilder.NextObjId, label);
                Objects.Add(ObjectBuilder.NextObjId, cobj);
                ObjectBuilder.NextObjId++;
            });
            CurrentMap.MapObjects.Clear();
        }

        [Command("finishmap")]
        public void FinishMapCmd(Client client, string fileName)
        {
            if (CurrentMap == null) return;
            Objects.ToList().ForEach(obj =>
            {
                CurrentMap.MapObjects.Add(new MapObject
                {
                    Model = obj.Value.model,
                    Position = obj.Value.position,
                    Rotation = obj.Value.rotation
                });
            });
            MapService.SaveMap(CurrentMap, fileName);
            API.sendChatMessageToPlayer(client, "Map Saved at ~b~Maps/" + fileName + ".xml");

            ClearMap();
        }

        public void ClearMap()
        {
            Objects.ToList().ForEach(obj =>
            {
                API.deleteEntity(obj.Value);
            });
            Objects.Clear();
            ObjectBuilder.Labels.ToList().ForEach(lbl =>
            {
                API.deleteEntity(lbl.Value);
            });
            ObjectBuilder.Labels.Clear();
            ObjectBuilder.NextObjId = 1;
            CurrentMap = null;
        }

        public void SaveRace(Client player, bool mapEditorFile = false)
        {
            if (CurrentMap == null) return;
            Objects.ToList().ForEach(obj =>
            {
                CurrentMap.MapObjects.Add(new MapObject
                {
                    Model = obj.Value.model,
                    Position = obj.Value.position,
                    Rotation = obj.Value.rotation
                });
            });
            if (mapEditorFile)
            {
                MapEditorService.SaveMap(CurrentMap, CurrentMap.Name);
                API.sendChatMessageToPlayer(player, "Map Saved at ~b~MapEditorMaps/" + CurrentMap.Name + ".xml");
            }
            else
            {
                MapService.SaveMap(CurrentMap, CurrentMap.Name);
                API.sendChatMessageToPlayer(player, "Map Saved at ~b~Maps/" + CurrentMap.Name + ".xml");
            }

            Objects.ToList().ForEach(obj =>
            {
                API.deleteEntity(obj.Value);
            });
            Objects.Clear();
            ObjectBuilder.Labels.ToList().ForEach(lbl =>
            {
                API.deleteEntity(lbl.Value);
            });
            ObjectBuilder.Labels.Clear();
            ObjectBuilder.NextObjId = 1;
            CurrentMap = null;
        }
    }
}
