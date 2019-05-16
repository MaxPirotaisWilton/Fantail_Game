// Copyright 2016 Guavaman Enterprises. All rights reserved.
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Rewired.Dev.Tools {

    internal sealed class StandaloneUnityJoystickElementIdentifier : MonoBehaviour {

        private const int maxJoysticks = 11;
        private const int maxAxes = 28;
        private const int maxButtons = 20;
        private const int buttonKeyCodesStartingIndex = 350;
        private const string unityAxisPrefix = "RUJEE_";

        int currentJoyId = 1;
        GUIStyle textStyle;
        GUIStyle pageStyle;

        public void Start() {
            string[] joystickNames = Input.GetJoystickNames();
            string s = "Detected " + joystickNames.Length + " attached joysticks";

            if(joystickNames.Length > 0) s += ":\n";
            foreach(string name in joystickNames) {
                s += "\"" + name + "\"\n";
            }
            Debug.Log(s);

            if(!IsUnity5OrGreater()) {
                Debug.LogWarning("Detected Unity version: " + Application.unityVersion + ". This tool was designed for Unity 5+. You can use it in Unity 4.x, but it will not be able to detect button presses on Joysticks 4-8.");
            }
        }

        void Update() {
            // Change active joystick id
            if(Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) currentJoyId += 1;
            if(Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) currentJoyId -= 1;

            if(currentJoyId <= 0) currentJoyId = maxJoysticks;
            else if(currentJoyId > maxJoysticks) currentJoyId = 1;
        }

        public void OnGUI() {

            if(textStyle == null) {
                textStyle = new GUIStyle(GUI.skin.label);
                textStyle.padding = new RectOffset();
                textStyle.margin = new RectOffset();
                textStyle.fontSize = 14;
                pageStyle = new GUIStyle();
                pageStyle.padding = new RectOffset(15, 0, 15, 0);
            }

            GUILayout.BeginVertical(pageStyle);

            // Write info to screen

            Log("Rewired Unity Joystick Element Identifier:"); // clear buffer
            GUILayout.Space(15);

            string[] joystickNames = Input.GetJoystickNames();
            if(joystickNames.Length > 0) Log("Connected joysticks:");
            else Log("No joysticks detected.");
            for(int i = 0; i < joystickNames.Length; i++) {
                Log("[" + i + "] \"" + joystickNames[i] + "\"");
            }

            GUILayout.Space(10);

            // Display joystick elements on screen
            Log("Current Joystick: [" + (currentJoyId - 1) + "] " + ((currentJoyId - 1) < joystickNames.Length ? joystickNames[currentJoyId - 1] : "None"));
            Log("(Press + or - to change monitored joystick id.)");
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(200f));

            Log("Buttons:");
            for(int i = 0; i < maxButtons; i++) {
                string name = "Button " + i; // show in 0-based index
                bool value = GetButton(currentJoyId, i);
                string valueStr = value ? "PRESSED" : "";
                Log(name, valueStr);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(200f));

            Log("Axes:");
            for(int i = 0; i < maxAxes; i++) {
                string name;
                if(i == 0) name = "X Axis";
                else if(i == 1) name = "Y Axis";
                else {
                    name = "Axis " + (i + 1);
                }

                float value = GetAxis(currentJoyId, i);
                Log(name, value.ToString("f4"));
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public void OnDestroy() {
        }

        private float GetAxis(int joystickId, int axisIndex) {
            return Input.GetAxisRaw(unityAxisPrefix + "Joy" + joystickId + "Axis" + (axisIndex + 1));
        }

        private bool GetButton(int joystickId, int buttonIndex) {
            if(joystickId <= 8) {
                int startingIndex = buttonKeyCodesStartingIndex + ((joystickId - 1) * maxButtons);
                return Input.GetKey((KeyCode)(startingIndex + buttonIndex));
            } else {
                return Input.GetButton(unityAxisPrefix + "Joy" + joystickId + "Button" + buttonIndex);
            }
        }

        private void Log(object value) {
            GUILayout.Label(value.ToString(), textStyle);
        }
        private void Log(string key, object value) {
            GUILayout.Label(key + " = " + value.ToString(), textStyle);
        }

        private bool IsUnity5OrGreater() {
            string version = Application.unityVersion;
            string[] split = version.Split('.');
            try {
                return int.Parse(split[1]) > 4;
            } catch {
                return true;
            }
        }
    }
}
