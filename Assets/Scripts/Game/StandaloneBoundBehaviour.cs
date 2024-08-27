using System.Collections;
using UnityEngine;

public class StandaloneBoundBehaviour : MonoBehaviour
{
    [SerializeField] GameObject _scoreCanvas, _loseCanvas;
    int _boxesLost = 0;

    void FixedUpdate()
    {
        if (_scoreCanvas.GetComponent<Score>().GetCombo() != 0) _boxesLost = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            if (++_boxesLost == int.MaxValue)
            {
                StartCoroutine(Lose()); // Changed the boxes lost to int.MaxValue from 5, so the player can't lose
            }
            _scoreCanvas.SendMessage("ResetCombo");
            other.transform.parent.SendMessage("DestroyBox");
        }
    }

    IEnumerator Lose()
    {
        GameObject boxesSpawn = GameObject.Find("BoxesSpawn");
        boxesSpawn.SendMessage("StopSpawn");
        boxesSpawn.BroadcastMessage("DestroyBox");
        AudioManager.instance.PlaySFX("gameOver");
        _loseCanvas.SetActive(true);
        yield return new WaitForSeconds(2);
        SCManager.instance.LoadScene("Menu");
    }
}
