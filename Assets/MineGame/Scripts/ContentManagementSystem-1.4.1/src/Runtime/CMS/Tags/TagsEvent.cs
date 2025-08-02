using System;
using UnityEngine;

[Serializable]
public class TagEventEndSpawnWave : EntityComponentDefinition, IEvent
{
    public static event Action EndWave;

    public void Execute(StateEvent owner)
    {
        owner.monoBehaviour.StopCoroutine(owner.coroutine);
        EndWave?.Invoke();
    }
    public static void ClearEvent() 
    {
        EndWave = null;
    }
}


public class StateEvent
{
    public Coroutine coroutine;
    public MonoBehaviour monoBehaviour;

}