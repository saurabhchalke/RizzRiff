using UnityEngine;
using TMPro;
using NaughtyAttributes;
using System.Collections;
using DG.Tweening;

public class GuitarTuner : MonoBehaviour
{
    [System.Serializable]
    public class StringInfo
    {
        public string name;
        public float frequency;
        public GameObject stringObject;
        public StringVibration vibration;
    }

    public StringInfo[] strings = new StringInfo[]
    {
        new StringInfo { name = "E", frequency = 329.63f },
        new StringInfo { name = "B", frequency = 246.94f },
        new StringInfo { name = "G", frequency = 196.00f },
        new StringInfo { name = "D", frequency = 146.83f },
        new StringInfo { name = "A", frequency = 110.00f },
        new StringInfo { name = "E", frequency = 82.41f }
    };

    public GameObject dialPrefab;
    public AudioClip tuneSuccessClip;
    public float tuningThreshold = 0.5f;
    public float dialMoveSpeed = 2f;

    public Material defaultMaterial;
    public Material activeMaterial;
    public Material tunedMaterial;

    public Transform topLeftCorner;
    public Transform topRightCorner;
    public Transform bottomLeftCorner;
    public Transform bottomRightCorner;

    public Transform dialPivot;

    private int currentStringIndex = 0;
    private GameObject currentDial;
    private TextMeshProUGUI dialText;
    private AudioSource audioSource;
    private Vector3 fretboardDirection;
    private float dialYOffset;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        InitializeStrings();
        CalculateFretboardDirection();
        if (dialPrefab != null) dialPrefab.SetActive(false);
        dialYOffset = dialPrefab.transform.position.y - dialPivot.position.y;

        StartTuningProcess();
    }

    private void CalculateFretboardDirection()
    {
        Vector3 topEdge = topRightCorner.position - topLeftCorner.position;
        Vector3 bottomEdge = bottomRightCorner.position - bottomLeftCorner.position;
        fretboardDirection = ((topEdge + bottomEdge) / 2f).normalized;
    }

    private void InitializeStrings()
    {
        foreach (var stringInfo in strings)
        {
            if (stringInfo.stringObject != null)
            {
                stringInfo.vibration = stringInfo.stringObject.GetComponent<StringVibration>();
                stringInfo.stringObject.GetComponent<Renderer>().material = defaultMaterial;
            }
        }
    }

    private void StartTuningProcess()
    {
        currentStringIndex = 0;
        CreateDialForCurrentString();
    }

    private void CreateDialForCurrentString()
    {
        if (currentDial != null)
        {
            Destroy(currentDial);
        }

        currentDial = Instantiate(dialPrefab, transform);
        currentDial.SetActive(true);
        dialText = currentDial.GetComponentInChildren<TextMeshProUGUI>();

        float t = (float)currentStringIndex / (strings.Length - 1);
        Vector3 stringPosition = Vector3.Lerp(bottomLeftCorner.position, topLeftCorner.position, t);
        currentDial.transform.position = new Vector3(currentDial.transform.position.x, stringPosition.y, currentDial.transform.position.z);

        UpdateDialPosition(0);
        UpdateStringMaterial(strings[currentStringIndex], activeMaterial);
        UpdateDialText(strings[currentStringIndex].name);
    }

    private void UpdateDialPosition(float tuningValue)
    {
        int reversedIndex = strings.Length - 1 - currentStringIndex;
        float normalizedPosition = Mathf.InverseLerp(-12f, 12f, tuningValue);
        Vector3 startPoint = Vector3.Lerp(bottomLeftCorner.position, topLeftCorner.position, (float)reversedIndex / (strings.Length - 1));
        Vector3 endPoint = startPoint + fretboardDirection * Vector3.Distance(topLeftCorner.position, topRightCorner.position);
        Vector3 targetPosition = Vector3.Lerp(startPoint, endPoint, normalizedPosition);
        targetPosition.y = strings[currentStringIndex].stringObject.transform.position.y + dialYOffset;
        currentDial.transform.DOMove(targetPosition, 1f / dialMoveSpeed).SetEase(Ease.InOutQuad);
    }

    private void UpdateDialText(string text)
    {
        dialText.text = text;
    }

    private void UpdateStringMaterial(StringInfo stringInfo, Material material)
    {
        if (stringInfo.stringObject != null)
        {
            stringInfo.stringObject.GetComponent<Renderer>().material = material;
        }
    }

    public void ProcessFrequency(float detectedFrequency)
    {
        if (currentStringIndex >= strings.Length) return;

        StringInfo currentString = strings[currentStringIndex];
        float centsOff = 1200 * Mathf.Log(detectedFrequency / currentString.frequency, 2);
        float tuningValue = Mathf.Clamp(centsOff / 100, -12f, 12f);

        UpdateDialPosition(tuningValue);
        UpdateDialText(tuningValue.ToString("F0"));

        if (currentString.vibration != null)
        {
            currentString.vibration.TriggerVibration();
        }

        if (Mathf.Abs(tuningValue) <= tuningThreshold)
        {
            StartCoroutine(StringTuned());
        }
    }

    private IEnumerator StringTuned()
    {
        StringInfo currentString = strings[currentStringIndex];
        UpdateStringMaterial(currentString, tunedMaterial);
        UpdateDialText(":)");

        audioSource.PlayOneShot(tuneSuccessClip);

        yield return new WaitForSeconds(1f);

        currentStringIndex++;
        if (currentStringIndex < strings.Length)
        {
            CreateDialForCurrentString();
        }
        else
        {
            Debug.Log("All strings are tuned!");
        }
    }
}