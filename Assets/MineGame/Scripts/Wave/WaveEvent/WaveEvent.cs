using System;

[Serializable]
public abstract class WaveEvent
{
    public abstract void StartEvent(StateEvent owner);
    public CMSEntityPfb onEvent;
}
