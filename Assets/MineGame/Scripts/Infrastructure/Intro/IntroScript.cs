using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    private AsyncOperation _loadedScene;
    private InputPlayer input;
    [SerializeField] private int numScene = 1;

    private void Start()
    {
        input = new();
        input.Enable();

        input.Player.Esc.started += i => _loadedScene.allowSceneActivation = true;
        _loadedScene = SceneManager.LoadSceneAsync(numScene, LoadSceneMode.Single);
        _loadedScene.allowSceneActivation = false;
    }
    private void OnDestroy()
    {
        input.Disable();
    }
    public void ChangeLoadingActive() => _loadedScene.allowSceneActivation = true;
}
