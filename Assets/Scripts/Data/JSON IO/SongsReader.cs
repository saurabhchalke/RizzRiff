using System.IO;
using UnityEngine;

public class SongsReader {
    string _songsListPath = "data";

    public SongDataList ReadSongs() {
        // Try to load from persistentDataPath first
        string persistentDataPath = Path.Combine(Application.persistentDataPath, _songsListPath + ".json");
        if (File.Exists(persistentDataPath)) {
            string jsonData = File.ReadAllText(persistentDataPath);
            SongDataList songDataList = JsonUtility.FromJson<SongDataList>(jsonData);
            return songDataList;
        }

        // Fallback to Resources if not found in persistentDataPath
        TextAsset jsonDataAsset = Resources.Load<TextAsset>(_songsListPath);
        if (jsonDataAsset != null) {
            SongDataList songDataList = JsonUtility.FromJson<SongDataList>(jsonDataAsset.text);
            return songDataList;
        }

        Debug.LogError("SongsReader: JSON data file not found in Resources or persistent data path.");
        return null;
    }

    public void PrintData(SongDataList songDataList) {
        foreach (SongData songData in songDataList.songData) {
            Debug.Log(songData.title);
            Debug.Log(songData.artist);
            Debug.Log(songData.path);
            Debug.Log(songData.duration);
            foreach (SongLevel songLevel in songData.levels) {
                Debug.Log(songLevel.level);
                Debug.Log(songLevel.path);

                foreach (LevelRanking ranking in songLevel.levelRanking) {
                    Debug.Log(ranking.username);
                    Debug.Log(ranking.score);
                    Debug.Log(ranking.max_combo);
                }
            }
        }
    }
}