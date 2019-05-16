// Copyright 2016 Guavaman Enterprises. All rights reserved.
using UnityEditor;
using System;
using System.Collections.Generic;
using Rewired.Editor.Libraries.SimpleJson;

namespace Rewired.Dev.Editor {

    using UnityEngine;

    [InitializeOnLoad]
    public class UnityInputManagerTool : UnityEditor.Editor {

        const string jsonFileGUID = "54319329ff63f5c41be629b5386e7291";
        const string ppKey_Installed = "Rewired_StandaloneUnityElementIdentifier_Installed";
        const string ppKey_DontAskInstall = "Rewired_StandaloneUnityElementIdentifier_DontAskInstall";

        private static bool _checked = false;

        static UnityInputManagerTool() {
            EditorApplication.update += OnUpdate; // register for update event, not in batch mode
        }

        private static void OnUpdate() {
            if(EditorApplication.isPlaying) return;
            if(EditorApplication.isCompiling) return; // do not run the updates until compiling is finished
            
            if(!_checked) { // check only once per compile/session
                CheckInputManagerEntries(false);
                _checked = true;
            }
        }

        [MenuItem("Window/Rewired/Unity Joystick Element Identifier/Run Setup")]
        public static void RunSetup() {
            CheckInputManagerEntries(true);
        }

        [MenuItem("Window/Rewired/Unity Joystick Element Identifier/Clear Preferences")]
        public static void ClearEditorPrefs() {
            EditorPrefs.DeleteKey(ppKey_Installed);
            EditorPrefs.DeleteKey(ppKey_DontAskInstall);
        }

        [MenuItem("Window/Rewired/Unity Joystick Element Identifier/Uninstall")]
        public static void Uninstall() {
            UninstallNow();
        }

        private static void CheckInputManagerEntries(bool force) {
            if(!force && IsInstalled()) return;
            if(AskInstall()) {
                Install(); 
            }
        }

        private static void Install() {
            // Load JSON Input Manager asset
            TextAsset settings;
            try {
                settings = (TextAsset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(jsonFileGUID), typeof(TextAsset));
            } catch {
                Debug.LogError("A required asset was not found at the expected GUID! Please delete and reinstall this tool.");
                return;
            }

            if(InputManagerEntryInstaller.ImportFromJSON(settings) == 0) {
                Debug.Log("Input Manager entries installed successfully!");
                if(EditorUtility.DisplayDialog("Installation Successful", "The input manager entries have been installed.\n\nThis tool was provided by Guavaman Enterprises, developer of Rewired Advanced Input for Unity. For a complete, robust input solution that solves many problems and limitations with Unity's input system, please check out Rewired. A fully-functional free trial is available on the website. Thanks!", "Show me the Rewired Website", "Close")) {
                    Application.OpenURL("http://guavaman.com/rewired");
                }
                EditorPrefs.SetBool(ppKey_Installed, true);
            } else {
                Debug.LogError("Input Manager entries could not be installed!");
            }
        }

        private static void UninstallNow() {
            try {

                // Load JSON Input Manager asset
                TextAsset settings;
                try {
                    settings = (TextAsset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(jsonFileGUID), typeof(TextAsset));
                } catch {
                    Debug.LogError("A required asset was not found at the expected GUID! Please delete and reinstall this tool.");
                    return;
                }

                int result = InputManagerEntryInstaller.RemoveFromJSON(settings);
                if(result >= 0) {
                    string msg = string.Empty;
                    if(result == 0) {
                        msg = "The input manager entries have been removed.";
                    }
                    if(EditorUtility.DisplayDialog("Uninstallation Successful", msg + "You should now delete this tool from your project.\n\nThis tool was provided by Guavaman Enterprises, developer of Rewired Advanced Input for Unity. For a complete, robust input solution that solves many problems and limitations with Unity's input system, please check out Rewired. A fully-functional free trial is available on the website. Thanks!", "Show me the Rewired Website", "Close")) {
                        Application.OpenURL("http://guavaman.com/rewired");
                    }
                    EditorPrefs.SetBool(ppKey_Installed, true);
                } else {
                    Debug.LogError("Input Manager entries could not be removed!");
                }

            } catch {
            } finally {
                ClearEditorPrefs();
            }
        }

        private static bool IsInstalled() {
            if(!EditorPrefs.HasKey(ppKey_Installed)) return false;
            return EditorPrefs.GetBool(ppKey_Installed);
        }

        private static bool AskInstall() {
            if(EditorPrefs.HasKey(ppKey_DontAskInstall) && EditorPrefs.GetBool(ppKey_DontAskInstall)) return false;

            int result = EditorUtility.DisplayDialogComplex("Install Input Manager Entries", "The Rewired Unity Joystick Element Identifier tool must add entries to the Unity Input Manager in order to function. They can be uninstalled from the menu when you are finished with this tool. Would you like to install these now?", "Yes", "No", "No and don't ask me again");
            if(result == 0) {
                return true;
            } else if(result == 1) {
                return false;
            } else if(result == 2) {
                EditorPrefs.SetBool(ppKey_DontAskInstall, true);
                return false;
            }
            return false;
        }

        private static class InputManagerEntryInstaller {

            private static int removedCount = 0;
            private static int replacedCount = 0;
            private static List<string> names;

            public static int ImportFromJSON(TextAsset settings) {
                if(settings == null) return -1;

                removedCount = 0;
                replacedCount = 0;
                names = null;

                try {

                    SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
                    SerializedProperty axes = serializedObject.FindProperty("m_Axes");
                    serializedObject.Update();

                    // Get list of names for speed
                    names = GetNames(axes);

                    JsonObject data = SimpleJson.DeserializeObject<JsonObject>(settings.text);
                    JsonArray axisData = (JsonArray)data["m_Axes"];

                    for(int i = 0; i < axisData.Count; i++) {
                        JsonObject j = (JsonObject)axisData[i];

                        InputAxis axis = new InputAxis() {
                            name = (string)j["m_Name"],
                            descriptiveName = (string)j["descriptiveName"],
                            descriptiveNegativeName = (string)j["descriptiveNegativeName"],
                            negativeButton = (string)j["negativeButton"],
                            positiveButton = (string)j["positiveButton"],
                            altNegativeButton = (string)j["altNegativeButton"],
                            altPositiveButton = (string)j["altPositiveButton"],
                            gravity = (float)(long)j["gravity"],
                            dead = (float)(long)j["dead"],
                            sensitivity = (float)(long)j["sensitivity"],
                            snap = ((int)(long)j["snap"] == 1),
                            invert = ((int)(long)j["invert"] == 1),
                            type = (AxisType)((int)(long)j["type"]),
                            axis = (int)(long)j["axis"],
                            joyNum = (int)(long)j["joyNum"],
                        };

                        // Add or replace axis in input manager
                        AddAxis(axis, axes);
                    }

                    // Save changes
                    if(removedCount > 0 || replacedCount > 0) {
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        AssetDatabase.SaveAssets();

                        if(removedCount > 0) Debug.Log("Added " + removedCount + " entries to Unity input manager.");
                        if(replacedCount > 0) Debug.Log("Updated " + replacedCount + " entries in Unity input manager.");
                    }

                } catch(System.Exception ex) {
                    Debug.Log(ex);
                    return -1;
                }

                return 0;
            }

            public static int RemoveFromJSON(TextAsset settings) {
                if(settings == null) return -1;

                removedCount = 0;
                names = null;

                try {

                    SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
                    SerializedProperty axes = serializedObject.FindProperty("m_Axes");
                    serializedObject.Update();

                    // Get list of names for speed
                    names = GetNames(axes);

                    JsonObject data = SimpleJson.DeserializeObject<JsonObject>(settings.text);
                    JsonArray axisData = (JsonArray)data["m_Axes"];

                    for(int i = 0; i < axisData.Count; i++) {
                        JsonObject jsonAxis = (JsonObject)axisData[i];

                        string jsonName = (string)jsonAxis["m_Name"];
                        if(string.IsNullOrEmpty(jsonName)) continue;

                        for(int j = names.Count - 1; j >= 0; j--) {
                            if(!names[j].Equals(jsonName, StringComparison.OrdinalIgnoreCase)) continue;
                            axes.DeleteArrayElementAtIndex(j);
                            names.RemoveAt(j);
                            removedCount++;
                            break;
                        }
                    }

                    // Save changes
                    if(removedCount > 0) {
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        AssetDatabase.SaveAssets();
                        Debug.Log("Removed " + removedCount + " entries from Unity input manager.");
                    } else {
                        Debug.Log("No input manager entries found to remove.");
                        return 1;
                    }

                } catch(System.Exception ex) {
                    Debug.Log(ex);
                    return -1;
                }

                return 0;
            }

            private static void AddAxis(InputAxis axis, SerializedProperty axes) {
                if(axis == null || axes == null) return;

                int index = names.IndexOf(axis.name);
                if(index >= 0) { // already in name list
                    ReplaceAxis(axis, axes.GetArrayElementAtIndex(index), axes); // replace it
                    return;
                }

                // Add new axis
                axes.InsertArrayElementAtIndex(axes.arraySize);
                SerializedProperty axisProperty = axes.GetArrayElementAtIndex(axes.arraySize - 1);
                CopyToAxis(axis, axisProperty, axes);
                removedCount += 1;
            }

            private static void ReplaceAxis(InputAxis axis, SerializedProperty axisProperty, SerializedProperty axes) {
                if(axisProperty == null) return;
                if(CopyToAxis(axis, axisProperty, axes)) replacedCount += 1;
            }

            private static bool CopyToAxis(InputAxis axis, SerializedProperty axisProperty, SerializedProperty axes) {
                bool replaced = false;
                replaced |= ReplaceChildProperty(axisProperty, "m_Name", axis.name);
                replaced |= ReplaceChildProperty(axisProperty, "descriptiveName", axis.descriptiveName);
                replaced |= ReplaceChildProperty(axisProperty, "descriptiveNegativeName", axis.descriptiveNegativeName);
                replaced |= ReplaceChildProperty(axisProperty, "negativeButton", axis.negativeButton);
                replaced |= ReplaceChildProperty(axisProperty, "positiveButton", axis.positiveButton);
                replaced |= ReplaceChildProperty(axisProperty, "altNegativeButton", axis.altNegativeButton);
                replaced |= ReplaceChildProperty(axisProperty, "altPositiveButton", axis.altPositiveButton);
                replaced |= ReplaceChildProperty(axisProperty, "gravity", axis.gravity);
                replaced |= ReplaceChildProperty(axisProperty, "dead", axis.dead);
                replaced |= ReplaceChildProperty(axisProperty, "sensitivity", axis.sensitivity);
                replaced |= ReplaceChildProperty(axisProperty, "snap", axis.snap);
                replaced |= ReplaceChildProperty(axisProperty, "invert", axis.invert);
                replaced |= ReplaceChildProperty(axisProperty, "type", (int)axis.type);
                replaced |= ReplaceChildProperty(axisProperty, "axis", axis.axis);
                replaced |= ReplaceChildProperty(axisProperty, "joyNum", axis.joyNum);
                return replaced;
            }

            private static bool ContainsAxis(string axisName, SerializedProperty axes) {
                if(axes == null || !axes.isArray) return false;
                int count = axes.arraySize;
                for(int i = 0; i < count; i++) {
                    SerializedProperty p = axes.GetArrayElementAtIndex(i);
                    if(p.FindPropertyRelative("m_Name").stringValue == axisName) return true;
                }
                return false;
            }

            private static SerializedProperty GetAxis(string axisName, SerializedProperty axes) {
                if(axes == null || !axes.isArray) return null;
                int count = axes.arraySize;
                for(int i = 0; i < count; i++) {
                    SerializedProperty p = axes.GetArrayElementAtIndex(i);
                    if(p.FindPropertyRelative("m_Name").stringValue == axisName) return p;
                }
                return null;
            }

            private static SerializedProperty GetChildProperty(SerializedProperty parent, string name) {
                if(parent == null) return null;
                return parent.FindPropertyRelative(name);
            }

            private static bool ReplaceChildProperty(SerializedProperty parent, string childPropertyName, string value) {
                SerializedProperty sp = parent.FindPropertyRelative(childPropertyName);
                if(sp.stringValue == value) return false;
                sp.stringValue = value;
                return true;
            }
            private static bool ReplaceChildProperty(SerializedProperty parent, string childPropertyName, int value) {
                SerializedProperty sp = parent.FindPropertyRelative(childPropertyName);
                if(sp.intValue == value) return false;
                sp.intValue = value;
                return true;
            }
            private static bool ReplaceChildProperty(SerializedProperty parent, string childPropertyName, bool value) {
                SerializedProperty sp = parent.FindPropertyRelative(childPropertyName);
                if(sp.boolValue == value) return false;
                sp.boolValue = value;
                return true;
            }
            private static bool ReplaceChildProperty(SerializedProperty parent, string childPropertyName, float value) {
                SerializedProperty sp = parent.FindPropertyRelative(childPropertyName);
                if(sp.floatValue == value) return false;
                sp.floatValue = value;
                return true;
            }

            private static List<string> GetNames(SerializedProperty axes) {
                if(axes == null || !axes.isArray) return null;

                int count = axes.arraySize;
                names = new List<string>(count);

                for(int i = 0; i < count; i++) {
                    SerializedProperty p = axes.GetArrayElementAtIndex(i);
                    names.Add(p.FindPropertyRelative("m_Name").stringValue);
                }
                return names;
            }

            private enum AxisType {
                KeyOrMouseButton = 0,
                MouseMovement = 1,
                JoystickAxis = 2
            };

            private class InputAxis {
                public string name;
                public string descriptiveName;
                public string descriptiveNegativeName;
                public string negativeButton;
                public string positiveButton;
                public string altNegativeButton;
                public string altPositiveButton;

                public float gravity;
                public float dead;
                public float sensitivity;

                public bool snap = false;
                public bool invert = false;

                public AxisType type;

                public int axis;
                public int joyNum;
            }
        }
    }
}

