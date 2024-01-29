using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityToolbarExtender;

[InitializeOnLoad]
public class InitializeXREditorToggle : EditorWindow
{
    private const string InitManagerPrefKey = "InitXRManagerOnStart";
    private static bool initManagerOnStart;

    static InitializeXREditorToggle()
    {
        initManagerOnStart = EditorPrefs.GetBool(InitManagerPrefKey, false);
        ToolbarExtender.RightToolbarGUI.Add(ShowToggle);
    }

    private static void ShowToggle()
    {
        bool newInitManagerOnStart = GUILayout.Toggle(initManagerOnStart, "Initialize XR[Standalone]");

        if (newInitManagerOnStart != initManagerOnStart)
        {
            initManagerOnStart = newInitManagerOnStart;
            EditorPrefs.SetBool(InitManagerPrefKey, initManagerOnStart);

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