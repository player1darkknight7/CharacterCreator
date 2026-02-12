using System.IO;
using UnityEngine;

public static class PresetIO
{
    private static string FolderPath =>
        Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
            "CharacterCreatorPresets");

    public static string GetPath(string fileNameNoExt)
    {
        Directory.CreateDirectory(FolderPath);
        return Path.Combine(FolderPath, fileNameNoExt + ".json");
    }

    public static void Save(string fileNameNoExt, CharacterPreset preset)
    {
        string path = GetPath(fileNameNoExt);
        File.WriteAllText(path, JsonUtility.ToJson(preset, true));
        Debug.Log($"Saved preset: {path}");
    }

    public static CharacterPreset Load(string fileNameNoExt)
    {
        string path = GetPath(fileNameNoExt);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Preset not found: {path}");
            return null;
        }

        return JsonUtility.FromJson<CharacterPreset>(File.ReadAllText(path));
    }
}
