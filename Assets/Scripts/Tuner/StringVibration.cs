using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class StringVibration : MonoBehaviour
{
    [Header("Vibration Settings")]
    [Range(0.01f, 1f)]
    public float vibrationIntensity = 0.1f;

    [Range(0.1f, 5f)]
    public float vibrationDuration = 1f;

    public AnimationCurve vibrationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector3 originalPosition;

    void Start()
    {
        // Store the original position of the string
        originalPosition = transform.localPosition;
    }

    [Button("Trigger Vibration")]
    public void TriggerVibration()
    {
        StopAllCoroutines();
        StartCoroutine(VibrateString());
    }

    private IEnumerator VibrateString()
    {
        float elapsedTime = 0f;

        while (elapsedTime < vibrationDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / vibrationDuration;
            float curveValue = vibrationCurve.Evaluate(normalizedTime);

            // Apply the vibration based on the intensity and the curve value
            float displacement = Mathf.Sin(elapsedTime * Mathf.PI * 2f * 10f) * vibrationIntensity * curveValue;
            transform.localPosition = originalPosition + new Vector3(0f, 0f, displacement);

            yield return null;
        }

        // Reset the string to its original position
        transform.localPosition = originalPosition;
    }
}