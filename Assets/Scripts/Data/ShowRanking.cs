using System.Collections;
using TMPro;
using UnityEngine;

public class ShowRanking : MonoBehaviour {
    [SerializeField] GameObject _scoreContent;
    [SerializeField] GameObject _rankingContent;
    [SerializeField] GameObject _keyboard;
    [SerializeField] GameObject _inputText;
    bool _hasRegistered = false;

    void Awake() {
        SetUp();
    }

    void SetUp() {
        Transform scoreHeader = _scoreContent.transform.GetChild(0).transform;
        scoreHeader.GetChild(0).GetComponent<TMP_Text>().text = PlayerPrefs.GetString("song.title");
        scoreHeader.GetChild(1).GetComponent<TMP_Text>().text = PlayerPrefs.GetString("song.level");
        _scoreContent.transform.GetChild(1).GetComponent<TMP_Text>().text = $"SCORE: {PlayerPrefs.GetInt("score")}";
        _scoreContent.transform.GetChild(2).GetComponent<TMP_Text>().text = $"MAX COMBO: {PlayerPrefs.GetInt("max_combo")}";
        
        SetUpRankingList();
    }

    void SetUpRankingList() {
        LevelRankingArrayWrapper wrapper = JsonUtility.FromJson<LevelRankingArrayWrapper>(PlayerPrefs.GetString("song.level.rankings"));
        LevelRanking[] levelRankings = wrapper.levelRankingArray;

        for (int i = 0; i < levelRankings.Length; i++) {
            GameObject ranking = _rankingContent.transform.GetChild(i).gameObject;
            ranking.transform.SetParent(_rankingContent.transform, false);
            ranking.transform.GetChild(0).GetComponent<TMP_Text>().text = levelRankings[i].username;

            Transform details = ranking.transform.GetChild(1);
            details.GetChild(0).GetComponent<TMP_Text>().text = $"SCORE: {levelRankings[i].score}";
            details.GetChild(1).GetComponent<TMP_Text>().text = $"MAX COMBO: {levelRankings[i].max_combo}";
        }
    }

    void UpdateRanking() {
        RankingWriter rankingWriter = new();
        rankingWriter.UpdateRanking();
        SetUpRankingList();
    }

    public void OnClick() {
        _keyboard.SetActive(true);
    }

    public void Submit() {
        if (_hasRegistered) return;
        _keyboard.SetActive(false);
        string name = _inputText.GetComponent<TMP_Text>().text;
        PlayerPrefs.SetString("username", name);
        UpdateRanking();
        _hasRegistered = true;
    }
}
