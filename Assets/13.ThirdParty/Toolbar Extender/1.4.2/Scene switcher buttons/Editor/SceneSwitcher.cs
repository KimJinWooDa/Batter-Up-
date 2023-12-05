using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

namespace UnityToolbarExtender.Examples
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }

    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        static Dictionary<string, string> userScenes = new Dictionary<string, string>
        {
            { "진우", "Test1" },
            { "예진", "Test2" },
            { "지원", "Test3" },
            { "지수", "Test4" },
            { "준영", "Test5" },
        };

        static string selectedUser;

        static SceneSwitchLeftButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("StartUp", "StartUp Scene")))
            {
                SceneHelper.OpenScene("StartUp");
            }

            if (GUILayout.Button(new GUIContent("MainMenu", "MainMenu Scene")))
            {
                SceneHelper.OpenScene("MainMenu");
            }

            if (GUILayout.Button(new GUIContent("Lobby", "Lobby Scene")))
            {
                SceneHelper.OpenScene("Lobby");
            }

            if (GUILayout.Button(new GUIContent("MainGame", "MainGame Scene")))
            {
                SceneHelper.OpenScene("MainGame");
            }

            
            if (GUILayout.Button(selectedUser ?? "사용자 선택", EditorStyles.popup))
            {
                var menu = new GenericMenu();
                foreach (var user in userScenes.Keys)
                {
                    menu.AddItem(new GUIContent(user), false, OnUserSelected, user);
                }

                menu.ShowAsContext();
            }

            string sceneName = selectedUser != null ? userScenes[selectedUser] : "씬 선택";
            if (!string.IsNullOrEmpty(selectedUser) && GUILayout.Button(new GUIContent(sceneName, "Open User Scene")))
            {
                SceneHelper.OpenScene(userScenes[selectedUser]);
            }
        }

        static void OnUserSelected(object user)
        {
            selectedUser = (string)user;
        }
    }

    static class SceneHelper
    {
        public static void OpenScene(string name)
        {
            var saved = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (saved)
            {
                _ = EditorSceneManager.OpenScene($"Assets/01.Scenes/{name}.unity");
            }
        }
    }
}