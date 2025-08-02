using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButtonScene : MonoBehaviour
{
    public void RestartCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        SceneManager.LoadScene(currentSceneIndex);
    }
}
