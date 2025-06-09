using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateDefaultFoldersScripts : MonoBehaviour
{
    [MenuItem("Assets/Create Default Folders")]
    private static void CreateDefaultFolders()
    {
        // Получаем путь к выбранной папке в Project Window
        string path = "Assets";
        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!AssetDatabase.IsValidFolder(path))
            {
                path = Path.GetDirectoryName(path);
            }
        }

        // Создаем папки
        CreateFolder(path, "Animations");
        CreateFolder(path, "Font");
        CreateFolder(path, "Prefs");
        CreateFolder(path, "Scenes");
        CreateFolder(path, "Scripts");
        CreateFolder(path, "Sprites");

        CreateAssemblyDefinition(Path.Combine(path ,"Scripts" ));
        // Обновляем Asset Database
        AssetDatabase.Refresh();
    }
    
    private static void CreateAssemblyDefinition(string path)
    {
        if (Directory.Exists(path))
        {
            string assemblyPath = Path.Combine(path, "Game.asmdef");
            if (!File.Exists(assemblyPath))
            {
                string assemblyContent = @"{
                    ""name"": ""Game""
                }";
                
                File.WriteAllText(assemblyPath, assemblyContent);
                Debug.Log("Created Game.asmdef in Scripts folder");
                AssetDatabase.Refresh();
            }
        }
    }

    private static void CreateFolder(string parentPath, string folderName)
    {
        string folderPath = Path.Combine(parentPath, folderName);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(parentPath, folderName);
            Debug.Log($"Created folder: {folderPath}");
        }
        else
        {
            Debug.Log($"Folder already exists: {folderPath}");
        }
    }

    // Добавляем возможность вызова по клику мыши
    [MenuItem("Assets/Create Default Folders", true)]
    private static bool ValidateCreateDefaultFolders()
    {
        // Проверяем, что выбран валидный объект (папка или ничего)
        return Selection.activeObject == null || 
               AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));
    }
}