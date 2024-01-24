using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class EditorPlayModeSettingsToggle
{
    private static bool enterPlayModeOptionsEnabled;
    private static EnterPlayModeOptions enterPlayModeOptions;

    static EditorPlayModeSettingsToggle()
    {
        enterPlayModeOptionsEnabled = EditorSettings.enterPlayModeOptionsEnabled;
        enterPlayModeOptions = EditorSettings.enterPlayModeOptions;

        ToolbarExtender.LeftToolbarGUI.Add(ShowToggle);
    }

    private static void ShowToggle()
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        
        GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
        toggleStyle.normal.textColor = toggleStyle.onNormal.textColor = enterPlayModeOptionsEnabled ? Color.red : Color.green;
        toggleStyle.hover.textColor = toggleStyle.onHover.textColor = enterPlayModeOptionsEnabled ? Color.red : Color.green;
        toggleStyle.active.textColor = toggleStyle.onActive.textColor = enterPlayModeOptionsEnabled ? Color.red : Color.green;
        toggleStyle.focused.textColor = toggleStyle.onFocused.textColor = enterPlayModeOptionsEnabled ? Color.red : Color.green;

        enterPlayModeOptionsEnabled = GUILayout.Toggle(enterPlayModeOptionsEnabled, "Play Mode Options Enable", toggleStyle);
        //enterPlayModeOptions = (EnterPlayModeOptions)EditorGUILayout.EnumPopup(enterPlayModeOptions);
        
        GUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            EditorSettings.enterPlayModeOptionsEnabled = enterPlayModeOptionsEnabled;
            EditorSettings.enterPlayModeOptions = enterPlayModeOptions;
        }
    }
}