using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class NoteDetector : MonoBehaviour
{
    [System.Serializable]
    public class GuitarString
    {
        public string name;
        public float openFrequency;
    }

    public GuitarString[] guitarStrings = new GuitarString[]
    {
        new GuitarString { name = "E", openFrequency = 329.63f },
        new GuitarString { name = "B", openFrequency = 246.94f },
        new GuitarString { name = "G", openFrequency = 196.00f },
        new GuitarString { name = "D", openFrequency = 146.83f },
        new GuitarString { name = "A", openFrequency = 110.00f },
        new GuitarString { name = "E", openFrequency = 82.41f }
    };

    private PitchDetector pitchDetector;
    private int bufferSize = 4096;
    private int sampleRate;
    private float[] audioBuffer;
    private AudioClip microphoneClip;
    private bool isListening = false;

    [Range(0f, 1f)]
    public float confidenceThreshold = 0.9f;
    public float detectionCooldown = 0.1f;
    private float lastDetectionTime;

    [Header("Simulation Settings")]
    [SerializeField] private bool enableSimulation = false;
    [SerializeField] private float simulationInterval = 1f;
    private float lastSimulationTime;

    [Header("Microphone Settings")]
    [SerializeField] private string selectedMicrophone;
    [SerializeField] private float volumeThreshold = 0.01f;

    public Dictionary<int, string> noteNames = new Dictionary<int, string>
    {
        {0, "C"}, {1, "C#"}, {2, "D"}, {3, "D#"}, {4, "E"}, {5, "F"},
        {6, "F#"}, {7, "G"}, {8, "G#"}, {9, "A"}, {10, "A#"}, {11, "B"}
    };

    public delegate void NoteDetectedHandler(int stringIndex, int fret, string noteName);
    public event NoteDetectedHandler OnNoteDetected;

    private void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        audioBuffer = new float[bufferSize];
        pitchDetector = new PitchDetector(bufferSize, sampleRate);

        if (!enableSimulation)
        {
            ListAvailableMicrophones();
            StartListening();
        }
    }

    private void ListAvailableMicrophones()
    {
        Debug.Log("Available Microphones:");
        foreach (string device in Microphone.devices)
        {
            Debug.Log("- " + device);
        }
    }

    [Button("Start Listening")]
    private void StartListening()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected!");
            return;
        }

        selectedMicrophone = string.IsNullOrEmpty(selectedMicrophone) ? Microphone.devices[0] : selectedMicrophone;
        Debug.Log($"Starting microphone: {selectedMicrophone}");

        microphoneClip = Microphone.Start(selectedMicrophone, true, 1, sampleRate);
        if (microphoneClip == null)
        {
            Debug.LogError("Failed to start microphone!");
            return;
        }

        isListening = true;
        Debug.Log("Microphone started successfully.");
    }

    [Button("Stop Listening")]
    private void StopListening()
    {
        if (isListening)
        {
            Microphone.End(selectedMicrophone);
            isListening = false;
            Debug.Log("Microphone stopped.");
        }
    }

    private void Update()
    {
        if (enableSimulation)
        {
            SimulateNoteDetection();
        }
        else if (isListening && Time.time - lastDetectionTime > detectionCooldown)
        {
            int pos = Microphone.GetPosition(selectedMicrophone);
            if (pos < bufferSize) return;

            microphoneClip.GetData(audioBuffer, pos - bufferSize);

            if (IsAudioLoudEnough(audioBuffer))
            {
                float frequency = pitchDetector.DetectPitch(audioBuffer);

                if (frequency > 0)
                {
                    Debug.Log($"Detected frequency: {frequency} Hz");
                    DetectNote(frequency);
                }
            }
        }
    }

    private bool IsAudioLoudEnough(float[] audioData)
    {
        float sum = 0f;
        for (int i = 0; i < audioData.Length; i++)
        {
            sum += Mathf.Abs(audioData[i]);
        }
        float average = sum / audioData.Length;
        return average > volumeThreshold;
    }

    [Button("Simulate Note Detection")]
    private void SimulateNoteDetection()
    {
        if (Time.time - lastSimulationTime < simulationInterval) return;

        lastSimulationTime = Time.time;
        int randomString = Random.Range(0, guitarStrings.Length);
        int randomFret = Random.Range(0, 24);  // Assuming a 24-fret guitar
        float randomFrequency = guitarStrings[randomString].openFrequency * Mathf.Pow(2, randomFret / 12f);

        DetectNote(randomFrequency);
    }

    private void DetectNote(float frequency)
    {
        for (int i = 0; i < guitarStrings.Length; i++)
        {
            float openFreq = guitarStrings[i].openFrequency;
            int fret = Mathf.RoundToInt(12 * Mathf.Log(frequency / openFreq, 2));

            if (fret >= 0 && fret <= 24)  // Assuming a 24-fret guitar
            {
                float expectedFreq = openFreq * Mathf.Pow(2, fret / 12f);
                float cents = 1200 * Mathf.Log(frequency / expectedFreq, 2);

                if (Mathf.Abs(cents) < 50)  // Within 50 cents of the expected frequency
                {
                    int noteIndex = (fret + GetNoteIndex(guitarStrings[i].name)) % 12;
                    string noteName = noteNames[noteIndex];

                    Debug.Log($"Detected: String {i + 1}, Fret {fret}, Note {noteName}");
                    OnNoteDetected?.Invoke(i, fret, noteName);
                    lastDetectionTime = Time.time;
                    return;
                }
            }
        }
    }

    private int GetNoteIndex(string noteName)
    {
        return System.Array.IndexOf(new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }, noteName);
    }

    private void OnDisable()
    {
        StopListening();
    }
}