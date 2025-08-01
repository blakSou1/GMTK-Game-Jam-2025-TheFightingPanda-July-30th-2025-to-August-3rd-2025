using System;
using System.Collections.Generic;
using System.Linq;
using Runtime;
using UnityEngine;

public class CMSEntityPfb : MonoBehaviour
{
    //[SerializeField]//[HideInInspector]
    private string idCMS;

    public string GetId() => idCMS;

    [SerializeReference, SubclassSelector]
    public List<EntityComponentDefinition> Components;
    private List<IFixedUpdate> ComponentsRuntimeUpdate;
    private List<IAnimationEvent> ComponentsAnimEvent;
    private List<IEnableUpdate> ComponentsEnableUpdate;
    [HideInInspector] public State state;
    public static event Action<int> IsDamage;
    private bool isStartUpdate = true;

    public CMSEntity AsEntity()
    {
        return CMS.Get<CMSEntity>(GetId());
    }

    public T As<T>() where T : EntityComponentDefinition, new()
    {
        return AsEntity().Get<T>();
    }

    public virtual Sprite GetSprite()
    {
        if (Components == null) return null;

        foreach (var component in Components)
        {
            // Use this to fetch sprite data from your different view variations
            switch (component)
            {
                case TagSprite tagSprite when tagSprite.sprite != null:
                    return tagSprite.sprite;
            }
        }

        return null;
    }

    private void OnEnable()
    {
        if (isStartUpdate)
            StartUpdate();

        foreach (IEnableUpdate enableUpdate in ComponentsEnableUpdate)
            enableUpdate.OnEnable(state);
    }

    private void StartUpdate()
    {
        DeepCopyComponents();

        state = new State
        {
            anim = GetComponent<Animator>(),
            gameObjectC = gameObject,
            cms = this,
            rb = GetComponent<Rigidbody2D>(),
            distancePlayer = Components
                .OfType<DistancePlayer>()
                .FirstOrDefault()
        };

        ComponentsEnableUpdate = Components?
        .Where(c => c is IEnableUpdate)
        .Cast<IEnableUpdate>()
        .ToList() ?? new List<IEnableUpdate>();

        List<IStartUpdate> startUpdates = Components?
        .Where(c => c is IStartUpdate)
        .Cast<IStartUpdate>()
        .ToList() ?? new List<IStartUpdate>();
        foreach (IStartUpdate startUpdate in startUpdates)
            startUpdate.Start(state);

        ComponentsRuntimeUpdate = Components?
        .Where(c => c is IFixedUpdate)
        .Cast<IFixedUpdate>()
        .ToList() ?? new List<IFixedUpdate>();

        ComponentsAnimEvent = Components?
        .Where(c => c is IAnimationEvent)
        .Cast<IAnimationEvent>()
        .ToList() ?? new List<IAnimationEvent>();

        isStartUpdate = false;
    }
    private void FixedUpdate()
    {
        foreach (IFixedUpdate updateComponent in ComponentsRuntimeUpdate)
            updateComponent.Execute(state);
    }

    public void AnimEvent(string val)
    {
        state.AnimEvent = val;
        foreach (IAnimationEvent updateComponent in ComponentsAnimEvent)
            updateComponent.Event(state);
    }

    public static void TriggerScoreChanged(int newScore)
    {
        IsDamage?.Invoke(newScore);
    }
    public void DeepCopyComponents()
    {
        Components = Components.Select(component =>
            component is IRuntimeStats
                ? component.DeepCopy()
                : component
        ).ToList();
    }
}