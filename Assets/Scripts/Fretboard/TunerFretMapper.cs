using UnityEngine;
using System.Collections.Generic;

public class TunerFretMapper : MonoBehaviour
{
    public GameObject topLeftCorner;
    public GameObject topRightCorner;
    public GameObject bottomLeftCorner;
    public GameObject bottomRightCorner;

    public GameObject fretPrefab;
    public GameObject beatFretPrefab;
    public GameObject greenLinePrefab;
    public string fretTag = "Fret";
    public int desiredFretCount = 5;
    public bool enableLogging = false;
    public float fretOffset = 0.22f;

    public Vector3 StartPoint { get; private set; }
    public Vector3 EndPoint { get; private set; }
    public float FretboardLength { get; private set; }
    public float FretSpacing { get; private set; }
    public float BeatFretSpacing { get; private set; }
    public Vector3 MovementDirection { get; private set; }

    public HashSet<int> activeFrets = new HashSet<int>();
    private Quaternion fretRotation;
    private int globalFretNumber = 0;
    private int beatCount = 0;

    void Start()
    {
        if (!topLeftCorner || !topRightCorner || !bottomLeftCorner || !bottomRightCorner)
        {
            Debug.LogError("Please assign all 4 corner GameObjects!");
            return;
        }

        fretPrefab.SetActive(false);
        beatFretPrefab.SetActive(false);

        StartPoint = (topRightCorner.transform.position + bottomRightCorner.transform.position) / 2f;
        EndPoint = (topLeftCorner.transform.position + bottomLeftCorner.transform.position) / 2f;

        MovementDirection = (EndPoint - StartPoint).normalized;
        FretboardLength = Vector3.Distance(StartPoint, EndPoint);

        fretRotation = Quaternion.Euler(90, 0, 0);

        FretSpacing = FretboardLength / desiredFretCount;
        BeatFretSpacing = FretSpacing / 4f;

        PopulateFretboard();
        RemoveOuterFrets();
    }

    void PopulateFretboard()
    {
        int fretCount = Mathf.FloorToInt(FretboardLength / FretSpacing);

        for (int i = 0; i <= fretCount; i++)
        {
            Vector3 fretPosition = StartPoint + MovementDirection * (i * FretSpacing);
            SpawnFretSet(fretPosition, globalFretNumber++, i == fretCount / 2);
        }
    }

    void SpawnFretSet(Vector3 position, int fretNumber, bool isMiddleFret)
    {
        if (isMiddleFret)
        {
            SpawnFret(greenLinePrefab, position, "CenterGreenLine", fretNumber);
        }
        else
        {
            SpawnFret(fretPrefab, position, $"MainFret_{fretNumber}", fretNumber);
        }

        for (int i = 1; i <= 3; i++)
        {
            Vector3 beatPosition = position + MovementDirection * BeatFretSpacing * i;
            SpawnFret(beatFretPrefab, beatPosition, $"BeatFret_{fretNumber}_{i}", fretNumber);
        }
    }

    void SpawnFret(GameObject prefab, Vector3 position, string namePrefix, int fretNumber)
    {
        Vector3 offsetPosition = position + Vector3.up * fretOffset;
        GameObject fret = Instantiate(prefab, offsetPosition, fretRotation, transform);
        FretData fretData = fret.GetComponent<FretData>();

        if (fretData == null)
        {
            fretData = fret.AddComponent<FretData>();
        }

        fretData.fretNumber = fretNumber;
        fret.SetActive(true);
        fret.tag = fretTag;
        fret.name = namePrefix;

        if (enableLogging)
        {
            Debug.Log($"Spawned {fret.name} at {offsetPosition} with fretNumber {fretNumber}");
        }
    }

    void RemoveOuterFrets()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(fretTag))
            {
                if (Vector3.Dot(child.position - EndPoint, MovementDirection) > 0)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}