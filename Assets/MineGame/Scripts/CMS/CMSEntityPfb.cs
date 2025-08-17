using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CMSEntityPfb : MonoBehaviour
{
    [SerializeReference, SubclassSelector]
    public List<EntityComponentDefinition> Components;
    
    [HideInInspector] public State state;

    public event Action<State> Update;
    public event Action<State> OnEnables;
    public event Action<State> AnimationEvent;

    private void Awake()
    {
        state = new State
        {
            audio = GetComponent<AudioSource>(),
            monoBehaviour = this,
            anim = GetComponent<Animator>(),
            gameObjectC = gameObject,
            cms = this,
            rb = GetComponent<Rigidbody2D>(),
            distancePlayer = Components
                .OfType<DistancePlayer>()
                .FirstOrDefault()
        };

        foreach (EntityComponentDefinition entity in Components)
            entity.Init(this);
    }
    private void OnEnable()
    {
        OnEnables?.Invoke(state);
    }
    private void OnDestroy()
    {
        foreach (EntityComponentDefinition entity in Components)
            entity.OnDestroy(this);
    }

    private void FixedUpdate()
    {
        Update?.Invoke(state);
    }

    public void AnimEvent(string val)
    {
        state.AnimEvent = val;
        AnimationEvent?.Invoke(state);
    }
}