using System;
using UnityEngine;

[Serializable]
public class Event
{
    [SerializeReference, SubclassSelector]
    public WaveEvent waveEvent;
    public bool endOfWave;
    public float eventTime;

    [NonSerialized] public bool isWave = false;
}
