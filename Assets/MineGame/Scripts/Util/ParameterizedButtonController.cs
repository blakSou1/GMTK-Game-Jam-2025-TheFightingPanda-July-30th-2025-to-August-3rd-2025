using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GeneralButton))]
public class ParameterizedButtonController : MonoBehaviour
{
    public CanvasGroup[] parameter; // Параметр, который можно указать в Inspector

    void Start()
    {
        foreach(var i in parameter)
            GetComponent<GeneralButton>().onClick.AddListener(() => StartCoroutine(Controller.controller.UpdatePanel(i)));
    }
}