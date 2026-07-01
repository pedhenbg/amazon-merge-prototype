using UnityEngine;
using System.IO;
using System;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro crítico ao salvar o arquivo: {e.Message}");
        }
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            return new SaveData();
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

            return loadedData ?? new SaveData();
        }
        catch (Exception e)
        {
            Debug.LogError($"Save corrompido ou erro de leitura. Retornando save limpo: {e.Message}");
            return new SaveData();
        }
    }
}