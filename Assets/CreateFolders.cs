using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public class CreateFolders : EditorWindow
{
    private static string projectName = "MyProject";

    [MenuItem("Assets/Create Default Folders")]
    private static void SetUpFolders()
    {
        CreateFolders window = ScriptableObject.CreateInstance<CreateFolders>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 150);
        window.ShowPopup();
    }
    

    private static void CreateAllFolders()
    {
        string root = $"Assets/{projectName}";

        List<string> folders = new List<string>
        {
            $"{root}/Art",
            $"{root}/Art/Materials",
            $"{root}/Art/Models",
            $"{root}/Art/Textures",

            $"{root}/Audio",
            $"{root}/Audio/Music",
            $"{root}/Audio/Sound",

            $"{root}/Code",
            $"{root}/Code/Scripts",
            $"{root}/Code/Scripts/Core",
            $"{root}/Code/Scripts/Systems",
            $"{root}/Code/Scripts/Entities",

            $"{root}/Code/Scripts/UI",
            $"{root}/Code/Scripts/Utilities",
            $"{root}/Code/Scripts/Events",
            $"{root}/Code/Scripts/Editor",
            $"{root}/Code/Scripts/ScriptableObjects",
            $"{root}/Code/Scripts/ScriptableObjects/Entities",


            $"{root}/Code/Shaders",

            $"{root}/Data",
            $"{root}/Data/Entities",
            $"{root}/Data/Entities/Player",
            $"{root}/Data/Entities/Enemies",
            $"{root}/Data/Entities/NPC",

            $"{root}/Docs",
            $"{root}/Docs/Design",
            $"{root}/Docs/ConceptArt",
            $"{root}/Docs/Marketing",
            $"{root}/Docs/Technical",
            $"{root}/Docs/Wiki",

            $"{root}/Level",
            $"{root}/Level/Prefabs",
            $"{root}/Level/Scenes",
            $"{root}/Level/UI"
        };

        foreach (string folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        AssetDatabase.Refresh();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Insert the Project name used as the root folder");
        projectName = EditorGUILayout.TextField("Project Name: ", projectName);

        GUILayout.Space(20);

        if (GUILayout.Button("Generate Folders"))
        {
            CreateAllFolders();
            Close();
        }
    }
}