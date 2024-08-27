using System.Collections;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour {
    public delegate void GameFinished();
    public static event GameFinished OnGameFinished;

    [SerializeField] TMP_Text _scoreText, _comboText, _doubleText;
    int score = -10;
    int combo = -1;
    int maxCombo = -10;

    void Start() {
        UpdateScore();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) IncreaseScore();
        if (Input.GetKeyDown(KeyCode.DownArrow)) FinishGame();
    }

    void OnEnable() => OnGameFinished += GoToRanking;

    void OnDisable() => OnGameFinished -= GoToRanking;

    void UpdateScore() {
        if (_doubleText.IsActive()) score +=10;

        _scoreText.text = (score += 10).ToString();
        _comboText.text = (++combo).ToString();
        if (combo != 0 && combo % 10 == 0) StartCoroutine(ShowDouble());
    }

    void IncreaseScore() {
        UpdateScore();
    }

    //void DecreaseScore() { if (score > 0) UpdateScore(); }

    void GoToRanking() {
        if (combo > maxCombo) maxCombo = combo;
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("max_combo", maxCombo);

        SCManager.instance.LoadScene("Ranking");
        AudioManager.instance.UnloadSong();
    }

    public static void FinishGame() {
        AudioManager.instance.PlaySFX("victory");
        OnGameFinished.Invoke();
    }

    IEnumerator ShowDouble() {
        if (_doubleText.enabled) yield return null;
        _doubleText.gameObject.SetActive(true);
        yield return new WaitForSeconds(10);
        _doubleText.gameObject.SetActive(false);
    }

    void ResetCombo() {
        if (combo > maxCombo) maxCombo = combo;
        combo = 0;
        _comboText.text = combo.ToString();
    }

    public int GetCombo() => combo;
}