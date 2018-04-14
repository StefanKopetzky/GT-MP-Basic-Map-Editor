/// <reference path="../types-gt-mp/index.d.ts" />
var EditEnabled = false;
var ObjEditorOnUpdate = null;
var ShiftPressed = false;
var AltPressed = false;
var xPlusPressed = false;
var xMinusPressed = false;
var yPlusPressed = false;
var yMinusPressed = false;
var zPlusPressed = false;
var zMinusPressed = false;
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName != "EditorStatus")
        return;
    EditEnabled = args[0];
    if (args[0]) {
        API.createMenulessInstructionalButton(0, "N0", "Switch Mode");
        API.createMenulessInstructionalButton(1, "N8", "X-Axis+");
        API.createMenulessInstructionalButton(2, "N5", "X-Axis-");
        API.createMenulessInstructionalButton(3, "N4", "Y-Axis+");
        API.createMenulessInstructionalButton(4, "N6", "Y-Axis-");
        API.createMenulessInstructionalButton(5, "N+", "Z-Axis+");
        API.createMenulessInstructionalButton(6, "N-", "Z-Axis-");
        API.createMenulessInstructionalButton(7, 19, "Slower Movement");
        API.createMenulessInstructionalButton(8, 21, "Faster Movement");
    }
    else {
        API.deleteAllMenulessInstructionalButtons();
    }
    if (EditEnabled) {
        if (ObjEditorOnUpdate != null) {
            ObjEditorOnUpdate.disconnect();
            ObjEditorOnUpdate = null;
        }
        ObjEditorOnUpdate = API.onUpdate.connect(function () {
            if (xPlusPressed) {
                API.triggerServerEvent("NumPad8", ShiftPressed, AltPressed);
            }
            if (xMinusPressed) {
                API.triggerServerEvent("NumPad5", ShiftPressed, AltPressed);
            }
            if (yPlusPressed) {
                API.triggerServerEvent("NumPad4", ShiftPressed, AltPressed);
            }
            if (yMinusPressed) {
                API.triggerServerEvent("NumPad6", ShiftPressed, AltPressed);
            }
            if (zPlusPressed) {
                API.triggerServerEvent("KeyAdd", ShiftPressed, AltPressed);
            }
            if (zMinusPressed) {
                API.triggerServerEvent("KeySubtract", ShiftPressed, AltPressed);
            }
        });
    }
    else {
        if (ObjEditorOnUpdate != null) {
            ObjEditorOnUpdate.disconnect();
            ObjEditorOnUpdate = null;
        }
    }
});
var LockModeSwitchKey = false;
API.onKeyDown.connect(function (sender, e) {
    if (!EditEnabled)
        return;
    if (LockModeSwitchKey)
        return;
    if (e.KeyCode == Keys.RShiftKey || e.KeyCode == Keys.ShiftKey) {
        ShiftPressed = true;
    }
    if (e.KeyCode == Keys.NumPad0) {
        API.triggerServerEvent("NumPad0");
        LockModeSwitchKey = true;
    }
    if (e.KeyCode == Keys.NumPad5) {
        xMinusPressed = true;
    }
    if (e.KeyCode == Keys.NumPad4) {
        yPlusPressed = true;
    }
    if (e.KeyCode == Keys.NumPad6) {
        yMinusPressed = true;
    }
    if (e.KeyCode == Keys.NumPad8) {
        xPlusPressed = true;
    }
    if (e.KeyCode == Keys.Add) {
        zPlusPressed = true;
    }
    if (e.KeyCode == Keys.Subtract) {
        zMinusPressed = true;
    }
    if (e.KeyCode === Keys.Menu || e.KeyCode === Keys.RMenu) {
        AltPressed = true;
    }
});
API.onKeyUp.connect(function (sender, e) {
    if (e.KeyCode == Keys.NumPad0) {
        LockModeSwitchKey = false;
    }
    if (e.KeyCode == Keys.RShiftKey || e.KeyCode == Keys.ShiftKey) {
        ShiftPressed = false;
    }
    if (e.KeyCode == Keys.NumPad5) {
        xMinusPressed = false;
    }
    if (e.KeyCode == Keys.NumPad4) {
        yPlusPressed = false;
    }
    if (e.KeyCode == Keys.NumPad6) {
        yMinusPressed = false;
    }
    if (e.KeyCode == Keys.NumPad8) {
        xPlusPressed = false;
    }
    if (e.KeyCode == Keys.Add) {
        zPlusPressed = false;
    }
    if (e.KeyCode == Keys.Subtract) {
        zMinusPressed = false;
    }
    if (e.KeyCode === Keys.Menu || e.KeyCode === Keys.RMenu) {
        AltPressed = false;
    }
});
//# sourceMappingURL=ObjEditor.js.map