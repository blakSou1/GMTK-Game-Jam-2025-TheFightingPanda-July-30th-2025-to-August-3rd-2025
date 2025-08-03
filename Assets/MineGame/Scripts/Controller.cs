using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller controller;
    [SerializeField] private CanvasGroup canvasGroupWin;
    public CanvasGroup canvasGroupExemple;
    
    public List<CMSEntityPfb> boost;
    public List<GameObject> BoostPoint = new();
    public CanvasGroup canvasGroupboost;
    
    private List<GameObject> boostIn = new();

    public List<Wave> wave;
    private SpawnerController spawnerController;
    [SerializeField] private GeneralButton buttonNextWave;
    public static GeneralButton ButtonNextWave;
    private int indexWave = 0;
    private Wave wav;

    public float times = .5f;

    private void Start()
    {
        canvasGroupboost.alpha = 0;
        canvasGroupboost.gameObject.SetActive(false);

        controller = this;

        TagEventEndSpawnWave.EndWave += () => EndWave();

        spawnerController = SpawnerController.spawnerController;

        ButtonNextWave = buttonNextWave;
        buttonNextWave.onClick.AddListener(NextWave);

        buttonNextWave.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        TagEventEndSpawnWave.ClearEvent();
        //TagEventEndSpawnWave.EndWave -= () => EndWave();
        StopAllCoroutines();
    }

    private void EndWave()
    {
        foreach (GameObject transform in BoostPoint)
            boostIn.Add(Instantiate(boost[Random.Range(0, boost.Count)].gameObject, transform.transform));

        controller.StartCoroutine(UpdatePanel(canvasGroupboost));

        ButtonNextWave.gameObject.SetActive(true);
    }
    
    private void NextWave()
    {
        StartCoroutine(UpdatePanel(canvasGroupboost));

        if (indexWave + 1 == wave.Count)
        {
            StartCoroutine(controller.UpdatePanel(canvasGroupWin));
            buttonNextWave.gameObject.SetActive(false);
            return;
        }

        foreach (var i in boostIn)
            Destroy(i);

        spawnerController.EndSpawnWoln(wav);

        indexWave++;

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
