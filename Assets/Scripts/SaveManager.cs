using System.IO;
using UnityEngine;

[System.Serializable]
public class GameProgress
{
    public int currentLevelId;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private GameProgress progress = new GameProgress();
    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "progress.json");
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(progress, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            progress = JsonUtility.FromJson<GameProgress>(json);
        }
        else
        {
            progress = new GameProgress { currentLevelId = 0 };
        }
    }

    public GameProgress GetProgress()
    {
        if (progress == null)
            LoadGame();
        return progress;
    }

    public void SetProgressLevel(int levelID)
    {
        progress.currentLevelId = levelID;
    }

    public void ResetProgress()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
        progress = new GameProgress { currentLevelId = 0 };
    }
}
