using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandaloneBoxesPool : MonoBehaviour
{
    [SerializeField] GameObject[] _boxPrefabs;
    [SerializeField] float _boxSpeed = 20f;
    [SerializeField] float _rotationSpeed = 100f;
    [SerializeField] float _spawnDelay = 4.900012f;

    [SerializeField] bool _loopLevel = false;
    //[SerializeField] Transform GameObject;

    void Start()
    {
        //float distance = Vector3.Distance(transform.position, GameObject.position);
        //float time = distance / _boxSpeed;

        SongLevelConfiguration _levelConf = new LevelConfReader().ReadConf();
        AudioManager.instance.PlaySong();
        StartCoroutine(SpawnBoxes(_levelConf.beatData));

        if (_loopLevel) StartCoroutine(LoopLevel());
        else
            StartCoroutine(EndLevel());
    }

    IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(PlayerPrefs.GetInt("song.duration"));
        Score.FinishGame();
    }

    // Loop Level
    IEnumerator LoopLevel()
    {
        while (true) // This makes the loop infinite
        {
            yield return new WaitForSeconds(PlayerPrefs.GetInt("song.duration"));
            AudioManager.instance.PlaySong();
            StartCoroutine(SpawnBoxes(new LevelConfReader().ReadConf().beatData));
        }
    }

    IEnumerator SpawnBoxes(List<SongBeat> _beatData)
    {
        float oldTime = 0;
        foreach (SongBeat beat in _beatData)
        {
            if (beat.time <= _spawnDelay) continue;
            if (beat.time + _spawnDelay >= PlayerPrefs.GetInt("song.duration") - 1) break;
            yield return new WaitForSeconds(beat.time - _spawnDelay - oldTime);
            oldTime = beat.time - _spawnDelay;
            InstantiateBox(beat);
        }
    }

    float GetLocation(string location, string type)
    {
        // X Direction
        float LEFT_LOCATION = -0.45f;
        float RIGHT_LOCATION = 0.566112f;
        float CENTER_LOCATION = (-0.39f + 0.456112f) / 2;

        // Y Direction
        float TOP_LOCATION = 1.49f;
        float BOTTOM_LOCATION = 0.70f;

        switch (type)
        {
            case "locX":
                if (location == "Left") return LEFT_LOCATION;
                if (location == "Right") return RIGHT_LOCATION;
                if (location == "Middle") return CENTER_LOCATION;
                break;
            case "locY":
                if (location == "Top") return TOP_LOCATION;
                if (location == "Bottom") return BOTTOM_LOCATION;
                break;
        }
        return -1;
    }

    int GetHitArea(string location)
    {
        int rotation = 0;

        switch (location)
        {
            case "Left":
                rotation = 1;
                break;
            case "Bottom":
                rotation = 2;
                break;
            case "Right":
                rotation = 3;
                break;
        }
        return rotation;
    }

    void InstantiateBox(SongBeat beat)
    {
        float locationX = GetLocation(beat.locationX, "locX");
        float locationY = GetLocation(beat.locationY, "locY");
        int hitArea = GetHitArea(beat.hit);

        Vector3 spawnPosition = new(locationX, locationY, transform.position.z);

        //! The box model needs to be rotated 180 degrees on the Y axis
        Quaternion rotation = Quaternion.Euler(0, 180, 0);

        int boxIndex = Random.Range(0, _boxPrefabs.Length);

        GameObject box = Instantiate(_boxPrefabs[boxIndex], spawnPosition, rotation);
        box.active = false;
        box.transform.parent = transform;

        StartCoroutine(Rotate(box, hitArea));

        Rigidbody rb = box.GetComponent<Rigidbody>();
        rb.velocity = -Vector3.forward * _boxSpeed;
    }

    IEnumerator Rotate(GameObject box, int direction)
    {
        RotateHitbox(box, direction);
        GameObject arrow = box.transform.Find("Arrow").gameObject;

        Quaternion targetRotation = arrow.transform.rotation * Quaternion.Euler(0, 0, 90 * direction);

        // As long as the box's rotation is not close enough to the target rotation
        while (Quaternion.Angle(arrow.transform.rotation, targetRotation) > 0.1f)
        {
            // Rotate the box towards the target rotation at the _rotationSpeed
            arrow.transform.rotation = Quaternion.RotateTowards(arrow.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void RotateHitbox(GameObject box, int direction)
    {
        Transform hitbox = box.transform.Find("CorrectHitbox");
        if (hitbox == null) return;

        switch (direction)
        {
            case 1:
                hitbox.localPosition = new Vector3(0.001f, 0.002f, -0.004f);
                hitbox.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case 2:
                hitbox.localPosition = new Vector3(0f, 0.004f, -0.121f);
                break;
            case 3:
                hitbox.localPosition = new Vector3(0.689f, 0.002f, -0.004f);
                hitbox.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
        }
    }

    void StopSpawn() => StopAllCoroutines();
}
