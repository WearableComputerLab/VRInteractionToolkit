using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class dependenciesManager : MonoBehaviour {
    // Start is called before the first frame update

    public enum STEAMVR_VERSIONS{None, SteamVR_Legacy, SteamVR_2};
    public STEAMVR_VERSIONS steamVR_Version;


    void Start() {
        oldVersion = steamVR_Version;
    }

    private bool Contains(string[] strArr) {
        foreach (string str in strArr) {
            if (str == steamVR_Version.ToString()) {
                return true;
            }
        }
        return false;
    }

    private STEAMVR_VERSIONS oldVersion;

    // Update is called once per frame
    void Update() {
        if (oldVersion != steamVR_Version) {
            oldVersion = steamVR_Version;
            string[] split = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).ToString().Split(';');
            if (!Contains(split)) {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, steamVR_Version.ToString());
                Debug.Log("<color=red>Setting project settings to: </color>" + steamVR_Version);
                if (steamVR_Version == STEAMVR_VERSIONS.SteamVR_2) {
                    Debug.Log("<color=blue>We recommend using the SteamVR legacy input system with 3DUITK. </color>");
                } else if (steamVR_Version == STEAMVR_VERSIONS.SteamVR_2) {
                    Debug.Log("<color=blue>Note: You're running on SteamVR 2+ input system now. </color>");
                    Debug.Log("<color=blue>Please re-import the SteamVR 2 CameraRig into scenes. </color>");
                } else if (steamVR_Version == STEAMVR_VERSIONS.None) {
                    Debug.Log("<color=blue>SteamVR support has been disabled. </color>");
                    Debug.Log("<color=blue>Drag your tracked object/controller into the trackedObj inspector parameter for cross-platform compatibility. </color>");
                }
            }
        }
    }
}
