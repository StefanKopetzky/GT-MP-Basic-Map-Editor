/// <reference path="../types-gt-mp/index.d.ts" />

var EditEnabled = false;
var ObjEditorOnUpdate = null;
var ShiftPressed = false;
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName != "EditorStatus")
        return;
    EditEnabled = args[0];
    if (args[0]) {
        API.createMenulessInstructionalButton(0, "N0", "Switch Mode");

        API.createMenulessInstructionalButton(1, "N8", "X-Axis+");
        API.createMenulessInstructionalButton(2, "N2", "X-Axis-");

        API.createMenulessInstructionalButton(3, "N4", "Y-Axis+");
        API.createMenulessInstructionalButton(4, "N6", "Y-Axis-");

        API.createMenulessInstructionalButton(5, "N+", "Z-Axis+");
        API.createMenulessInstructionalButton(6, "N-", "Z-Axis-");
    } else {
        API.deleteAllMenulessInstructionalButtons();
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
    if (e.KeyCode == Keys.NumPad0) { // Switch Position / Rotation Mode
        API.triggerServerEvent("NumPad0");
        LockModeSwitchKey = true;
    }
    if (e.KeyCode == Keys.NumPad2) { // X-
        API.triggerServerEvent("NumPad2", ShiftPressed);
    }
    if (e.KeyCode == Keys.NumPad4) { // Y+
        API.triggerServerEvent("NumPad4", ShiftPressed);
    }
    if (e.KeyCode == Keys.NumPad6) { // Y-
        API.triggerServerEvent("NumPad6", ShiftPressed);
    }
    if (e.KeyCode == Keys.NumPad8) { // X+
        API.triggerServerEvent("NumPad8", ShiftPressed);
    }
    if (e.KeyCode == Keys.Add) {  // Z+
        API.triggerServerEvent("KeyAdd", ShiftPressed);
    }
    if (e.KeyCode == Keys.Subtract) { // Z-
        API.triggerServerEvent("KeySubtract", ShiftPressed);
    }
});

API.onKeyUp.connect(function (sender, e) {
    if (e.KeyCode == Keys.NumPad0) {
        LockModeSwitchKey = false;
    }
    if (e.KeyCode == Keys.RShiftKey || e.KeyCode == Keys.ShiftKey) {
        ShiftPressed = false;
    }
});