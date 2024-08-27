using UnityEngine;

public class LevelConfReader {
    string _songsListPath = "Music/Songs/";

    public SongLevelConfiguration ReadConf() {
        string songLevelPath = PlayerPrefs.GetString("song.level.path");

        // Load the JSON file from the Resources folder
        TextAsset jsonData = Resources.Load<TextAsset>(_songsListPath + songLevelPath);

        if (jsonData != null) {
            SongLevelConfiguration songLevelConfiguration = JsonUtility.FromJson<SongLevelConfiguration>(jsonData.text);
            return songLevelConfiguration;
        }

        Debug.LogError("LevelConfReader: JSON data file not found in Resources.");
        return null;
    }

    public void SetData(SongLevelConfiguration songLevelConfiguration) {
        Debug.Log(songLevelConfiguration.beatData);
        for (int i = 0; i < songLevelConfiguration.beatData.Count; i++) {
            Debug.Log(songLevelConfiguration.beatData[i].time);
            Debug.Log(songLevelConfiguration.beatData[i].locationY);
            Debug.Log(songLevelConfiguration.beatData[i].locationX);
            Debug.Log(songLevelConfiguration.beatData[i].hit);
        }
    }
}
