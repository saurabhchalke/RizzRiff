using UnityEngine;
using System.Collections.Generic;

public class FretboardMovement : MonoBehaviour
{
    public GameObject topLeftCorner;
    public GameObject topRightCorner;
    public GameObject bottomLeftCorner;
    public GameObject bottomRightCorner;

    public GameObject fretPrefab;
    public GameObject beatFretPrefab;
    public float moveSpeed = 5f;
    public string fretTag = "Fret";
    public int desiredFretCount = 5;
    public bool enableLogging = false;

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
    }

    void PopulateFretboard()
    {
        int fretCount = Mathf.FloorToInt(FretboardLength / FretSpacing);

        for (int i = 0; i <= fretCount; i++)
        {
            Vector3 fretPosition = StartPoint + MovementDirection * (i * FretSpacing);
            SpawnFretSet(fretPosition, globalFretNumber++);
        }
    }

    void Update()
    {
        MoveFrets();
    }

    void MoveFrets()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(fretTag))
            {
                child.Translate(MovementDirection * moveSpeed * Time.deltaTime, Space.World);

                if (Vector3.Dot(child.position - EndPoint, MovementDirection) > 0)
                {
                    Vector3 offset = child.position - EndPoint;
                    child.position = StartPoint + Vector3.Project(offset, MovementDirection);

                    FretData fretData = child.GetComponent<FretData>();
                    if (fretData == null)
                    {
                        Debug.LogWarning($"FretData component missing on {child.name}. Adding new FretData.");
                        fretData = child.gameObject.AddComponent<FretData>();
                    }

                    beatCount++;
                    if (beatCount == 4)
                    {
                        fretData.fretNumber = globalFretNumber++;
                        beatCount = 0;
                    }

                    if (enableLogging)
                    {
                        Debug.Log($"Fret wrapped around: Updated to fret number {fretData.fretNumber}");
                    }
                }

                FretData existingFretData = child.GetComponent<FretData>();
                if (existingFretData != null)
                {
                    int fretNumber = existingFretData.fretNumber;
                    if (!activeFrets.Contains(fretNumber))
                    {
                        activeFrets.Add(fretNumber);
                        if (enableLogging)
                        {
                            Debug.Log($"Fret {fretNumber} is now active on the fretboard.");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"FretData component missing on {child.name}. Child's position: {child.position}. Prefab: {child.name}");
                }
            }
        }
    }

    void SpawnFretSet(Vector3 position, int fretNumber)
    {
        if (enableLogging)
        {
            Debug.Log($"Spawning main fret {fretNumber} at position {position}");
        }
        SpawnFret(fretPrefab, position, $"MainFret_{fretNumber}", fretNumber);

        for (int i = 1; i <= 3; i++)
        {
            Vector3 beatPosition = position + MovementDirection * BeatFretSpacing * i;
            if (enableLogging)
            {
                Debug.Log($"Spawning beat fret {fretNumber} at position {beatPosition}");
            }
            SpawnFret(beatFretPrefab, beatPosition, $"BeatFret_{fretNumber}_{i}", fretNumber);
        }
    }

    void SpawnFret(GameObject prefab, Vector3 position, string namePrefix, int fretNumber)
    {
        GameObject fret = Instantiate(prefab, position, fretRotation, transform);
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
            Debug.Log($"Spawned {fret.name} at {position} with fretNumber {fretNumber}");
        }
    }

    public Vector3 GetLastFretPosition()
    {
        return EndPoint - MovementDirection * FretSpacing;
    }
}