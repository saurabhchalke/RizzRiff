using UnityEngine;
using System.Collections.Generic;

public class FretMapper : MonoBehaviour
{
    public GameObject topLeftCorner; // Top-left corner of the fretboard
    public GameObject topRightCorner; // Top-right corner of the fretboard
    public GameObject bottomLeftCorner; // Bottom-left corner of the fretboard
    public GameObject bottomRightCorner; // Bottom-right corner of the fretboard

    public GameObject fretPrefab; // Main fret prefab
    public GameObject beatFretPrefab; // Beat fret prefab
    public float moveSpeed = 5f; // Speed of fret movement
    public string fretTag = "Fret"; // Tag to identify frets
    public int desiredFretCount = 5; // Number of main frets
    public bool enableLogging = false; // Flag to enable/disable logging

    public Vector3 StartPoint { get; private set; }
    public Vector3 EndPoint { get; private set; }
    public float FretboardLength { get; private set; }
    public float FretSpacing { get; private set; }
    public float BeatFretSpacing { get; private set; }
    public Vector3 MovementDirection { get; private set; }

    public HashSet<int> activeFrets = new HashSet<int>();
    private Quaternion fretRotation;
    private int globalFretNumber = 0; // Global counter for fret numbers
    private int beatCount = 0; // Counter to track beats within a fret

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

    void RemoveOuterFrets()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(fretTag))
            {
                // If fret passes the end point, hide it instead of moving it back to the start
                if (Vector3.Dot(child.position - EndPoint, MovementDirection) > 0)
                {
                    // Hide the fret by disabling the GameObject or setting it inactive
                    child.gameObject.SetActive(false);

                    // Optionally, update FretData or perform any necessary operations before hiding
                    FretData fretData = child.GetComponent<FretData>();
                    if (fretData == null)
                    {
                        Debug.LogWarning($"FretData component missing on {child.name}. Adding new FretData.");
                        fretData = child.gameObject.AddComponent<FretData>();
                    }

                    // You may still want to update the fret number for tracking purposes
                    beatCount++;
                    if (beatCount == 4)
                    {
                        fretData.fretNumber = globalFretNumber++;
                        beatCount = 0;
                    }

                    Debug.Log($"Fret hidden: {child.name} with fret number {fretData.fretNumber} is now inactive.");
                }
            }
        }
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

    void SpawnFretSet(Vector3 position, int fretNumber)
    {
        Debug.Log($"Spawning main fret {fretNumber} at position {position}");
        SpawnFret(fretPrefab, position, $"MainFret_{fretNumber}", fretNumber);

        for (int i = 1; i <= 3; i++)
        {
            Vector3 beatPosition = position + MovementDirection * BeatFretSpacing * i;
            Debug.Log($"Spawning beat fret {fretNumber} at position {beatPosition}");
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

        Debug.Log($"Spawned {fret.name} at {position} with fretNumber {fretNumber}");
    }
}