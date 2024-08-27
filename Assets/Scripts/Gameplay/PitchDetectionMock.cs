using UnityEngine;

public class PitchDetectionMock : MonoBehaviour
{
    [Range(0f, 1f)]
    public float correctPlayProbability = 0.8f;

    public bool SimulateCorrectPlay()
    {
        return Random.value < correctPlayProbability;
    }
}