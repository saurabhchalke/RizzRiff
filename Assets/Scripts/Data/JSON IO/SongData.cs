using System;
using System.Collections.Generic;

[Serializable]
public class SongData {
    public string title;
    public string artist;
    public string path;
    public int duration;
    public SongLevel[] levels;

    public SongData(string title, string artist, string path, int duration, SongLevel[] levels) {
        this.title = title;
        this.artist = artist;
        this.path = path;
        this.duration = duration;
        this.levels = levels;
    }
}


[Serializable]
public class SongLevel {
    public string level;
    public string path;
    public List<LevelRanking> levelRanking;

    public SongLevel(string level, string path, List<LevelRanking> levelRanking = null){
        this.level = level;
        this.path = path;
        this.levelRanking = levelRanking ?? new List<LevelRanking>();
    }

    public void AddRanking(LevelRanking ranking) {
        if (!levelRanking.Exists(r => r.username == ranking.username)) levelRanking.Add(ranking);
        else levelRanking = UpdateRanking(levelRanking, ranking);

        levelRanking.Sort((a, b) => b.score.CompareTo(a.score));
        if (levelRanking.Count > 5) levelRanking.RemoveAt(levelRanking.Count - 1);
    }

    List<LevelRanking> UpdateRanking(List<LevelRanking> rankings, LevelRanking newRanking) {
        foreach (LevelRanking ranking in rankings) {
            if (ranking.username == newRanking.username) {
                if (newRanking.score <= ranking.score) break;
                ranking.score = newRanking.score;
                ranking.max_combo = newRanking.max_combo;
            }
        }
        return rankings;
    }
}

[Serializable]
public class LevelRanking {
    public string username;
    public int score;
    public int max_combo;

    public LevelRanking(string username, int score, int max_combo) {
        this.username = username;
        this.score = score;
        this.max_combo = max_combo;
    }
}

[Serializable]
public class LevelRankingArrayWrapper {
    public LevelRanking[] levelRankingArray;
}

[Serializable]
public class SongDataList {
    public List<SongData> songData;

    public SongDataList() {
        songData = new List<SongData>();
    }
}

[Serializable]
public class SongBeat {
    public float time;
    public string locationY;
    public string locationX;
    public string hit;

    public SongBeat(float time, string locationY, string locationX, string hit) {
        this.time = time;
        this.locationY = locationY;
        this.locationX = locationX;
        this.hit = hit;
    }
}

[Serializable]
public class SongLevelConfiguration {
    public List<SongBeat> beatData;

    public SongLevelConfiguration() {
        beatData = new List<SongBeat>();
    }
}
