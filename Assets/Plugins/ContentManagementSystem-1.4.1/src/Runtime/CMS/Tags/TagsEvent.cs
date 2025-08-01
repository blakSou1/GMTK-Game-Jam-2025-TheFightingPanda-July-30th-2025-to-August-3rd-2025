using System;

[Serializable]
public class TagEventEndSpawnWave : EntityComponentDefinition, IEvent
{
    public static event Action EndWave;

    public void Execute(State owner)
    {
        EndWave?.Invoke();
    }
}
