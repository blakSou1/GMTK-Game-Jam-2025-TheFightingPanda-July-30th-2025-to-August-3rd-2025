using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GeneralButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool interactable = true;
    [SerializeField] private TextMeshProUGUI label;

    [SerializeField] private Color labelDefolt;
    [SerializeField] private Color labelHover;
    [SerializeField] private Color labelPressed;

    [SerializeField] private GameObject stateDefolt;
    [SerializeField] private GameObject stateHover;
    [SerializeField] private GameObject statePressed;

    public UnityEvent onClick;

    private void Awake()
    {
        if (label == null)
            label = GetComponentInChildren<TextMeshProUGUI>();
            
        if (interactable)
            State(stateDefolt);
        else
            State(stateHover);
    }
    private void OnDestroy()
    {
        State(stateDefolt);
    }

    public void UpdateInteractable(bool isStat)
    {
        interactable = isStat;
        if (interactable)
            State(stateDefolt);
        else
            State(stateHover);
    }
    public void UpdateText(string text)
    {
        label.text = text;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (interactable)
            State(stateHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(interactable)
            State(stateDefolt);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(interactable)
            onClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(interactable)
            State(statePressed);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(interactable)
            State(stateDefolt);
    }

    private void State(GameObject go)
    {
        stateDefolt.SetActive(stateDefolt == go);
        stateHover.SetActive(stateHover == go);
        statePressed.SetActive(statePressed == go);

        if (stateHover.activeSelf)
            label.color = labelHover;
        else if (statePressed.activeSelf)
            label.color = labelPressed;
        else
            label.color = labelDefolt;
    }
}