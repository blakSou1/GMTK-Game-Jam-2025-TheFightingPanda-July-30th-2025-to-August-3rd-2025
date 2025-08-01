using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GeneralButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
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
        State(stateDefolt);
    }
    private void OnDestroy()
    {
        State(stateDefolt);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        State(stateHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        State(stateDefolt);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        State(statePressed);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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