using UnityEngine;

[System.Serializable]
public class NoteData
{
    public int fretNumber;
    public int beatNumber;
    public float length;
    public int stringNumber;
    public int fretLocation;
    public int fingerUsed;

    public static class FingerUsed
    {
        public const int Open = 0;
        public const int Index = 1;
        public const int Middle = 2;
        public const int Ring = 3;
        public const int Pinky = 4;
    }
}