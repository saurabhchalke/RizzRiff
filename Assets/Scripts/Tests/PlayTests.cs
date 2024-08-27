using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayTests {
    GameObject _fly, _logo, _audioManager;

    [UnityTest, Order(1)]
    public IEnumerator Start() {
        SceneManager.LoadScene("Menu");
        yield return null;
        _fly = GameObject.Find("fly");
        _logo = GameObject.Find("logo");
        _audioManager = GameObject.Find("AudioManager");
    }

    [Test, Order(2)]
    public void FlyExists() {
        Assert.IsNotNull(_fly);
    }

    [Test, Order(3)]
    public void GoalExists() {
        Assert.IsNotNull(_logo);
    }

    [Test, Order(4)]
    public void AudioManagerExists() {
        Assert.IsNotNull(_audioManager);
    }
}

