using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestarter : MonoBehaviour
{
    void Update()
    {
        // Check if the Y button is pressed on the controller
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            Debug.Log("Restarting current scene...");
            RestartCurrentScene();
        }
    }

    void RestartCurrentScene()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);
    }
}