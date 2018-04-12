/// <reference path="../types-gt-mp/index.d.ts" />
var json = null;
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "CloseMenus") {
        API.closeAllMenus();
    }
    if (eventName == "OpenEditorMenu") {
        API.closeAllMenus();
        json = JSON.parse(args[0]);
        var menu = API.createMenu(json["Title"], json["SubTitle"], 0, 0, 6);
        for (var i = 0; i < json["Items"].length; i++) {
            var MenuObj = json["Items"][i];
            var NewItem = API.createMenuItem(MenuObj["Title"], MenuObj["Description"]);
            if (MenuObj["RightLabel"] != "") {
                NewItem.SetRightLabel(MenuObj["RightLabel"]);
            }
            menu.AddItem(NewItem);
        }
        menu.OnItemSelect.connect(function (sender, selectedItem, index) {
            if (json["Items"][index]["SelectEvent"] != "") {
                var input = "";
                if (json["Items"][index]["UserInput"]) {
                    input = API.getUserInput(json["Items"][index]["DefaultUserInput"], 200);
                }
                if (json["Items"][index]["SelectEventArg"] != "") {
                    API.triggerServerEvent(json["Items"][index]["SelectEvent"], json["Items"][index]["SelectEventArg"], input);
                }
                else {
                    API.triggerServerEvent(json["Items"][index]["SelectEvent"], input);
                }
            }
        });
        menu.Visible = true;
    }
});
API.onKeyDown.connect(function (sender, e) {
    if (e.KeyCode == Keys.F2) {
        API.triggerServerEvent("F2KeyPressed");
    }
});
//# sourceMappingURL=ObjEditorMenus.js.map