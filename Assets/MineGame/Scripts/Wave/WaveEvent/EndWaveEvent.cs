using System;
using System.Linq;

[Serializable]
public class EndWaveEvent : WaveEvent
{
    public override void StartEvent(StateEvent owner)
    {
        onEvent.Components.OfType<IEvent>().FirstOrDefault().Execute(owner);
    }
}
