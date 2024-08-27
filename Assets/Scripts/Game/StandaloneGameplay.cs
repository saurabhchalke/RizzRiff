using System.Collections;
using UnityEngine;

public class StandaloneGameplay : MonoBehaviour {
    private SongLevel[] _levels;

    void Awake() {
        SongDataList songDataList = new SongsReader().ReadSongs();
        SongData songData = songDataList.songData[0];
        Debug.Log($"Selecting {songData.title} by {songData.artist}");

        PlayerPrefs.SetString("song.title", songData.title);
        PlayerPrefs.SetString("song.path", songData.path);
        PlayerPrefs.SetInt("song.duration", songData.duration);
        _levels = songData.levels;

        SongLevel songLevel = _levels[0];

        PlayerPrefs.SetString("song.level", songLevel.level);
        PlayerPrefs.SetString("song.level.path", songLevel.path);
        PlayerPrefs.SetString("song.level.rankings", JsonUtility.ToJson(new LevelRankingArrayWrapper { levelRankingArray = songLevel.levelRanking.ToArray() }));

        Debug.Log($"Selected level {songLevel.level} with path {songLevel.path}");
    }
}
