using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public FretboardStringPositioner stringPositioner;
    public FretboardMovement fretboardMovement;
    public string notesDirectoryPrefix = "Notes/";
    public float fallSpeed = 10f;
    public bool enableRandomNoteGeneration = false;
    public float randomNoteInterval = 1f;
    public Transform nutTransform;
    public Transform destroyBound;

    public Material indexFingerMaterial;
    public Material middleFingerMaterial;
    public Material ringFingerMaterial;
    public Material pinkyFingerMaterial;
    public Material openStringMaterial;
    public Material correctNoteMaterial;
    public Material incorrectNoteMaterial;

    public bool ignorePitchDetection = false;
    public bool mockPlayer = false;
    [Range(0f, 1f)]
    public float correctPlayProbability = 0.8f;

    [Range(0f, 0.5f)]
    public float timingThreshold = 0.1f;

    public GameObject fretLocationIndicatorPrefab;
    public float indicatorOffset = 2.5f;
    [Range(0.1f, 2f)]
    public float indicatorScaleMultiplier = 0.5f;

    public bool loopMode = false;
    public string[] availableScales = { "default", "a-major", "c-major" };
    public int currentScaleIndex = 1;

    private Dictionary<int, List<NoteData>> notesByFret = new Dictionary<int, List<NoteData>>();
    private HashSet<int> instantiatedFrets = new HashSet<int>();
    private float nextNoteTime = 0f;
    private float noteHitX;

    private PitchDetectionMock pitchDetectionMock;
    private List<NoteData> allNotes = new List<NoteData>();
    private int currentNoteIndex = 0;

    void Start()
    {
        if (!enableRandomNoteGeneration)
        {
            LoadNotesFromResources(notesDirectoryPrefix + availableScales[currentScaleIndex]);
        }
        noteHitX = nutTransform.position.x;
        pitchDetectionMock = gameObject.AddComponent<PitchDetectionMock>();
        pitchDetectionMock.correctPlayProbability = correctPlayProbability;
    }

    void Update()
    {
        if (enableRandomNoteGeneration)
        {
            GenerateRandomNotes();
        }
        else
        {
            foreach (int activeFret in fretboardMovement.activeFrets)
            {
                if (!instantiatedFrets.Contains(activeFret) && notesByFret.ContainsKey(activeFret))
                {
                    instantiatedFrets.Add(activeFret);
                    foreach (NoteData noteData in notesByFret[activeFret])
                    {
                        SpawnNoteAtPosition(noteData);
                    }
                }
            }

            if (loopMode && allNotes.Count > 0 && !fretboardMovement.activeFrets.Contains(allNotes[currentNoteIndex].fretNumber))
            {
                SpawnNextNote();
            }
        }

        CheckNoteHits();
        UpdateNoteEmissions();
    }

    void CheckNoteHits()
    {
        NoteBehaviour[] activeNotes = FindObjectsOfType<NoteBehaviour>();
        foreach (NoteBehaviour note in activeNotes)
        {
            if (note == null || note.isPlayed)
            {
                continue;
            }

            float noteX = note.transform.position.x;
            float distanceToHitX = Mathf.Abs(noteX - noteHitX);

            if (distanceToHitX <= timingThreshold)
            {
                Debug.Log($"Note hit at {Time.time}");
                if (ignorePitchDetection)
                {
                    note.MissNote();
                }
                else if (mockPlayer)
                {
                    bool isCorrect = pitchDetectionMock.SimulateCorrectPlay();
                    Debug.Log($"Mock player played note: {isCorrect}");
                    note.PlayNote(isCorrect);
                }
            }
            else if (noteX < noteHitX - timingThreshold)
            {
                Debug.Log($"Note missed at {Time.time}");
                note.MissNote();
            }
        }
    }

    void UpdateNoteEmissions()
    {
        NoteBehaviour[] activeNotes = FindObjectsOfType<NoteBehaviour>();
        foreach (NoteBehaviour note in activeNotes)
        {
            if (note == null) continue;

            float distanceToLastFret = Vector3.Distance(note.transform.position, fretboardMovement.GetLastFretPosition());
            if (distanceToLastFret <= fretboardMovement.FretSpacing)
            {
                note.EnableEmission();
            }
            else
            {
                note.DisableEmission();
            }
        }
    }

    void GenerateRandomNotes()
    {
        if (Time.time >= nextNoteTime)
        {
            int randomStringIndex = Random.Range(0, 6);
            int randomFret = Random.Range(0, fretboardMovement.desiredFretCount);
            int randomLength = Random.Range(1, 3);
            int randomFretLocation = Random.Range(0, 12);
            int randomFingerUsed = Random.Range(0, 5);

            NoteData noteData = new NoteData
            {
                fretNumber = randomFret,
                beatNumber = Random.Range(0, 4),
                length = randomLength,
                stringNumber = randomStringIndex,
                fretLocation = randomFretLocation,
                fingerUsed = randomFingerUsed
            };

            SpawnNoteAtPosition(noteData);

            nextNoteTime = Time.time + randomNoteInterval;
        }
    }

    void LoadNotesFromResources(string fileName)
    {
        TextAsset notesFile = Resources.Load<TextAsset>(fileName);
        if (notesFile == null)
        {
            Debug.LogError($"Notes file '{fileName}' not found in Resources!");
            return;
        }

        notesByFret.Clear();
        allNotes.Clear();
        currentNoteIndex = 0;

        using (StringReader reader = new StringReader(notesFile.text))
        {
            string line;
            bool isFirstLine = true;
            while ((line = reader.ReadLine()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] parts = line.Split(',');

                if (parts.Length != 6)
                {
                    Debug.LogWarning($"Skipping invalid line: {line}");
                    continue;
                }

                if (int.TryParse(parts[0], out int fretNumber) &&
                    int.TryParse(parts[1], out int beatNumber) &&
                    float.TryParse(parts[2], out float length) &&
                    int.TryParse(parts[3], out int stringNumber) &&
                    int.TryParse(parts[4], out int fretLocation) &&
                    int.TryParse(parts[5], out int fingerUsed))
                {
                    NoteData note = new NoteData
                    {
                        fretNumber = fretNumber,
                        beatNumber = beatNumber,
                        length = length,
                        stringNumber = stringNumber,
                        fretLocation = fretLocation,
                        fingerUsed = fingerUsed
                    };

                    if (!notesByFret.ContainsKey(fretNumber))
                    {
                        notesByFret[fretNumber] = new List<NoteData>();
                    }
                    notesByFret[fretNumber].Add(note);
                    allNotes.Add(note);
                }
                else
                {
                    Debug.LogWarning($"Skipping line with invalid data: {line}");
                }
            }
        }

        allNotes = allNotes.OrderBy(n => n.fretNumber).ToList();
    }

    void SpawnNoteAtPosition(NoteData noteData)
    {
        GameObject targetString = null;

        switch (noteData.stringNumber)
        {
            case 0:
                targetString = stringPositioner.eString;
                break;
            case 1:
                targetString = stringPositioner.BString;
                break;
            case 2:
                targetString = stringPositioner.GString;
                break;
            case 3:
                targetString = stringPositioner.DString;
                break;
            case 4:
                targetString = stringPositioner.AString;
                break;
            case 5:
                targetString = stringPositioner.EString;
                break;
        }

        float yPosition = targetString.transform.position.y;
        float zPosition = targetString.transform.position.z;
        float xPosition = fretboardMovement.topRightCorner.transform.position.x;

        Vector3 notePosition = new Vector3(xPosition, yPosition, zPosition);
        GameObject note = Instantiate(notePrefab, notePosition, Quaternion.identity, transform);

        note.transform.localScale = new Vector3(fretboardMovement.BeatFretSpacing * noteData.length, note.transform.localScale.y, note.transform.localScale.z);
        note.SetActive(true);

        ApplyMaterialBasedOnFinger(note, noteData.fingerUsed);

        NoteMover noteMover = note.AddComponent<NoteMover>();
        noteMover.Init(fretboardMovement.moveSpeed, fretboardMovement.MovementDirection, fretboardMovement.StartPoint, destroyBound.position);

        NoteBehaviour noteBehaviour = note.AddComponent<NoteBehaviour>();
        noteBehaviour.fallSpeed = fallSpeed;
        noteBehaviour.correctNoteMaterial = correctNoteMaterial;
        noteBehaviour.incorrectNoteMaterial = incorrectNoteMaterial;

        GameObject indicator = Instantiate(fretLocationIndicatorPrefab, note.transform.position, Quaternion.identity);

        IndicatorFollower follower = indicator.AddComponent<IndicatorFollower>();
        follower.targetNote = note.transform;
        follower.offset = new Vector3(0, indicatorOffset, 0);
        follower.scaleMultiplier = indicatorScaleMultiplier;

        FretLocationIndicator fretIndicator = indicator.GetComponent<FretLocationIndicator>();
        fretIndicator.SetFretNumber(noteData.fretLocation);
        fretIndicator.SetColor(note.GetComponent<Renderer>().material.color);

        noteBehaviour.SetFretLocationIndicator(fretIndicator);
    }

    void ApplyMaterialBasedOnFinger(GameObject note, int fingerUsed)
    {
        Renderer renderer = note.GetComponent<Renderer>();
        if (renderer != null)
        {
            switch (fingerUsed)
            {
                case NoteData.FingerUsed.Open:
                    renderer.material = openStringMaterial;
                    break;
                case NoteData.FingerUsed.Index:
                    renderer.material = indexFingerMaterial;
                    break;
                case NoteData.FingerUsed.Middle:
                    renderer.material = middleFingerMaterial;
                    break;
                case NoteData.FingerUsed.Ring:
                    renderer.material = ringFingerMaterial;
                    break;
                case NoteData.FingerUsed.Pinky:
                    renderer.material = pinkyFingerMaterial;
                    break;
                default:
                    Debug.LogWarning($"Unknown finger used: {fingerUsed}");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Note object does not have a Renderer component");
        }
    }

    void SpawnNextNote()
    {
        if (allNotes.Count == 0) return;

        NoteData nextNote = allNotes[currentNoteIndex];
        SpawnNoteAtPosition(nextNote);

        currentNoteIndex = (currentNoteIndex + 1) % allNotes.Count;
        if (currentNoteIndex == 0 && !loopMode)
        {
            LoadNextScale();
        }
    }

    void LoadNextScale()
    {
        currentScaleIndex = (currentScaleIndex + 1) % availableScales.Length;
        LoadNotesFromResources(availableScales[currentScaleIndex]);
    }
}