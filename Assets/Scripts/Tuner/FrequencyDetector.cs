using UnityEngine;
using NaughtyAttributes;

public class FrequencyDetector : MonoBehaviour
{
    public GuitarTuner guitarTuner;
    public NoteDetector noteDetector;
    
    [Header("Simulation Settings")]
    [SerializeField] private bool enableRandomNoteGeneration = false;
    [SerializeField] private float randomNoteInterval = 1f;
    
    [Header("Test Settings")]
    public bool ignorePitchDetection = false;
    public bool mockPlayer = false;
    [Range(0f, 1f)]
    public float correctPlayProbability = 0.8f;

    private PitchDetectionMock pitchDetectionMock;

    private void Start()
    {
        if (noteDetector == null)
        {
            noteDetector = gameObject.AddComponent<NoteDetector>();
        }

        noteDetector.OnNoteDetected += HandleNoteDetected;
        
        pitchDetectionMock = gameObject.AddComponent<PitchDetectionMock>();
        pitchDetectionMock.correctPlayProbability = correctPlayProbability;
    }

    private void HandleNoteDetected(int stringIndex, int fret, string noteName)
    {
        Debug.Log($"Detected note: String {stringIndex + 1}, Fret {fret}, Note {noteName}");
        
        if (guitarTuner != null && guitarTuner.enabled)
        {
            float detectedFrequency = noteDetector.guitarStrings[stringIndex].openFrequency * Mathf.Pow(2, fret / 12f);
            guitarTuner.ProcessFrequency(detectedFrequency);
        }

        if (ignorePitchDetection)
        {
            Debug.Log("Ignoring pitch detection as per settings");
        }
        else if (mockPlayer)
        {
            bool isCorrect = pitchDetectionMock.SimulateCorrectPlay();
            Debug.Log($"Mock player played note: {(isCorrect ? "Correct" : "Incorrect")}");
        }
    }

    private void Update()
    {
        if (enableRandomNoteGeneration)
        {
            GenerateRandomNotes();
        }
    }

    private void GenerateRandomNotes()
    {
        if (Time.time % randomNoteInterval < Time.deltaTime)
        {
            SimulateNoteDetection();
        }
    }

    [Button("Simulate Note Detection")]
    public void SimulateNoteDetection()
    {
        int randomString = Random.Range(0, noteDetector.guitarStrings.Length);
        int randomFret = Random.Range(0, 24);
        string randomNote = noteDetector.noteNames[Random.Range(0, 12)];
        
        HandleNoteDetected(randomString, randomFret, randomNote);
    }

    private void OnDisable()
    {
        if (noteDetector != null)
        {
            noteDetector.OnNoteDetected -= HandleNoteDetected;
        }
    }
}