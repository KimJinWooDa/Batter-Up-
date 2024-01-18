using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityToolbarExtender;

[InitializeOnLoad]
public class InitializeXREditorToggle : EditorWindow
{
    private static BuildTargetGroup selectedTargetGroup = BuildTargetGroup.Standalone;
    private static bool initManagerOnStart;


    static InitializeXREditorToggle()
    {
        XRGeneralSettings generalSettings = XRGeneralSettings.Instance;
        if (generalSettings != null)
        {
            initManagerOnStart = generalSettings.InitManagerOnStart;
        }
        
        ToolbarExtender.RightToolbarGUI.Add(ShowToggle);
    }

    private static void ShowToggle()
    {
        initManagerOnStart = GUILayout.Toggle(initManagerOnStart,
            "Initialize XR[Standalone]"); // + selectedTargetGroup

        if (GUI.changed)
        {
            XRGeneralSettings generalSettings = XRGeneralSettings.Instance;
            if (generalSettings != null)
            {
                generalSettings.InitManagerOnStart = initManagerOnStart;
                EditorUtility.SetDirty(generalSettings);
                AssetDatabase.SaveAssets();
            }
        }
    }
}