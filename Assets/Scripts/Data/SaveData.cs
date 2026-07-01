[System.Serializable]
public class SaveData
{
    public int highScore;

    public float musicVolume;
    public float sfxVolume;

    public int totalCoins;

    public SaveData()
    {
        highScore = 0;
        totalCoins = 0;
        musicVolume = 1f; 
        sfxVolume = 1f;   
    }
}