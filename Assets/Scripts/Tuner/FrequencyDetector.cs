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

    private PitchDetector pitchDetector;
    private int bufferSize = 4096;
    private int sampleRate;
    private float[] audioBuffer;
    private AudioClip microphoneClip;
    private bool isListening = false;

    private PitchDetectionMock pitchDetectionMock;

    private void Start()
    {
        if (noteDetector == null)
        {
            noteDetector = gameObject.AddComponent<NoteDetector>();
        }

        noteDetector.OnNoteDetected += HandleNoteDetected;

        sampleRate = AudioSettings.outputSampleRate;
        audioBuffer = new float[bufferSize];
        pitchDetector = new PitchDetector(bufferSize, sampleRate);

        pitchDetectionMock = gameObject.AddComponent<PitchDetectionMock>();
        pitchDetectionMock.correctPlayProbability = correctPlayProbability;

        StartListening();
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
        else if (isListening)
        {
            DetectPitch();
        }
    }

    private void GenerateRandomNotes()
    {
        if (Time.time % randomNoteInterval < Time.deltaTime)
        {
            SimulateNoteDetection();
        }
    }

    private void DetectPitch()
    {
        int pos = Microphone.GetPosition(null);
        if (pos < bufferSize) return;

        microphoneClip.GetData(audioBuffer, pos - bufferSize);
        float frequency = pitchDetector.DetectPitch(audioBuffer);

        if (frequency > 0)
        {
            Debug.Log($"Detected frequency: {frequency} Hz");
            noteDetector.DetectNote(frequency);
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

    private void StartListening()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected!");
            return;
        }

        microphoneClip = Microphone.Start(null, true, 1, sampleRate);
        isListening = true;
    }

    private void StopListening()
    {
        if (isListening)
        {
            Microphone.End(null);
            isListening = false;
        }
    }

    private void OnDisable()
    {
        if (noteDetector != null)
        {
            noteDetector.OnNoteDetected -= HandleNoteDetected;
        }
        StopListening();
    }
}