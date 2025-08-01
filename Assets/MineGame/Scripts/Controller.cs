using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller controller;

    public List<Wave> wave;
    private SpawnerController spawnerController;
    [SerializeField] private GeneralButton buttonNextWave;
    public static GeneralButton ButtonNextWave;
    private int indexWave = 0;
    private Wave wav;

    public float times = .5f;

    [System.Obsolete]
    private void Start()
    {
        controller = this;

        TagEventEndSpawnWave.EndWave += () => ButtonNextWave.gameObject.SetActive(true);

        spawnerController = FindObjectOfType<SpawnerController>();

        ButtonNextWave = buttonNextWave;
        buttonNextWave.onClick.AddListener(NextWave);

        buttonNextWave.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        TagEventEndSpawnWave.EndWave -= () => ButtonNextWave.gameObject.SetActive(true);
    }

    private void NextWave()
    {
        spawnerController.EndSpawnWoln(wav);

        indexWave = wav.numberWave + 1;

        wav = wave[indexWave].DeepCopy();
        spawnerController.StartSpawmWave(wav);

        buttonNextWave.gameObject.SetActive(false);
    }

    public void StartRun()
    {
        wav = wave[indexWave].DeepCopy();
        spawnerController.StartSpawmWave(wav);
    }

    public IEnumerator UpdatePanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup.alpha == 0)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.interactable = true;
        }
        else
            canvasGroup.interactable = false;

        float time = 0;
        float startPosition = canvasGroup.alpha;
        float endPosition = startPosition == 1 ? 0 : 1;

        while (time < times)
        {
            float t = time / times;
            time += Time.fixedDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(startPosition, endPosition, t);

            yield return new WaitForFixedUpdate();
        }
        canvasGroup.alpha = endPosition;

        if (canvasGroup.alpha == 0) canvasGroup.gameObject.SetActive(false);
    }
}
