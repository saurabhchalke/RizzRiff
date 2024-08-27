using UnityEngine;
using TMPro;
using DG.Tweening;

public class NoteResultDisplay : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public float movementDistance = 50f;
    public float animationDuration = 0.5f;
    public float fadeDuration = 0.3f;

    private static readonly string[] positiveResults = { "Perfect", "Nice", "Awesome" };

    public void DisplayResult(bool isCorrect)
    {
        string text = isCorrect ? GetRandomPositiveResult() : "Miss";
        resultText.text = text;
        resultText.color = isCorrect ? Color.green : Color.red;

        Vector3 movement = isCorrect ? Vector3.up : Vector3.down;
        
        // Reset position and alpha
        resultText.transform.localPosition = Vector3.zero;
        resultText.alpha = 1f;

        // Animate
        Sequence sequence = DOTween.Sequence();
        sequence.Append(resultText.transform.DOLocalMove(movement * movementDistance, animationDuration).SetEase(Ease.OutCubic));
        sequence.Join(resultText.DOFade(0f, fadeDuration).SetDelay(animationDuration - fadeDuration));
    }

    private string GetRandomPositiveResult()
    {
        return positiveResults[Random.Range(0, positiveResults.Length)];
    }
}