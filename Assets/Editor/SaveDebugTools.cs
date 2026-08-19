using UnityEditor;
using UnityEngine;
using System.IO;

public static class SaveDebugTools
{
    [MenuItem("Tools/ Save System/ Delete Save File")]
    private static void DeleteSave()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted: " + path);
        }

        else
        {
            Debug.Log("No save file found.");
        }
    }

    [MenuItem("Tools/ Save System/ Open Save Folder")]
    private static void OpenSaveFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
