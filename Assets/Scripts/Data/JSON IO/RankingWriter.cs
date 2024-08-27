using System.IO;
using UnityEngine;

public class RankingWriter {
    string _songsListPath = "data.json";
    string _dataFilePath;

    public RankingWriter() {
        string persistentDataPath = Application.persistentDataPath;
        _dataFilePath = Path.Combine(persistentDataPath, _songsListPath);
    }

    public void UpdateRanking() {
        SongsReader songsReader = new();
        SongDataList songDataList = songsReader.ReadSongs() ?? new SongDataList();
        string username = PlayerPrefs.GetString("username", "Anonymous");
        int score = PlayerPrefs.GetInt("score", 0);
        int max_combo = PlayerPrefs.GetInt("max_combo", 0);

        bool rankingUpdated = false;

        foreach (SongData songData in songDataList.songData) {
            if (songData.title == PlayerPrefs.GetString("song.title")) {
                foreach (SongLevel songLevel in songData.levels) {
                    if (songLevel.path == PlayerPrefs.GetString("song.level.path")) {
                        songLevel.AddRanking(new LevelRanking(username, score, max_combo));
                        PlayerPrefs.SetString("song.level.rankings", JsonUtility.ToJson(new LevelRankingArrayWrapper { levelRankingArray = songLevel.levelRanking.ToArray() }));
                        rankingUpdated = true;
                        break;
                    }
                }
            }
            if (rankingUpdated) break;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(_dataFilePath));
        string jsonData = JsonUtility.ToJson(songDataList, true);
        File.WriteAllText(_dataFilePath, jsonData);
    }
}