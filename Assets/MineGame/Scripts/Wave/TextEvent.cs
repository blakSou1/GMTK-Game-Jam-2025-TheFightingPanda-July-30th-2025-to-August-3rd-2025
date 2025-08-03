using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class TextEvent : WaveEvent
{
    public TextMeshProUGUI prefabText;
    private TextMeshProUGUI text;

    public List<string> textSource;
    [SerializeField] private float fadeDuration = .3f;
    [SerializeField] private float displayDuration = .4f;
    private int index = 0;

    public override void StartEvent(StateEvent owner)
    {
        text = GameObject.Instantiate(prefabText, Controller.controller.canvasGroupExemple.transform);

        text.text = textSource[index];
        index++;

        onEvent.Components.OfType<IEvent>().FirstOrDefault().Execute(owner);
        Controller.controller.StartCoroutine(UpdateText());
    }

    private IEnumerator UpdateText()
    {
        CanvasGroup canvasGroup = text.GetComponent<CanvasGroup>();

        canvasGroup.gameObject.SetActive(true);

        float time = 0;
        float startPosition = 0;
        float endPosition = 1;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            time += Time.fixedDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(startPosition, endPosition, t);

            yield return new WaitForFixedUpdate();
        }
        canvasGroup.alpha = endPosition;

        yield return new WaitForSeconds(displayDuration);

        time = 0;
        startPosition = 1;
        endPosition = 0;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            time += Time.fixedDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(startPosition, endPosition, t);

            yield return new WaitForFixedUpdate();
        }
        canvasGroup.alpha = endPosition;

        canvasGroup.gameObject.SetActive(false);

        GameObject.Destroy(text);
    }
}
