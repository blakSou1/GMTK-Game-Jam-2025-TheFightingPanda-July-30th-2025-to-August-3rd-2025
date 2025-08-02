public interface IEvent
{
    public abstract void Execute(StateEvent owner);
}
public interface IAtack
{
    public abstract void Execute(State owner);
}
public interface IDamage
{
    public abstract void Execute(State owner);
}
public interface IShoot
{
    public abstract void Execute(State owner);
}
public interface IMove
{
    public abstract void Execute(State owner);
}
public interface IFixedUpdate
{
    public abstract void Execute(State owner);
}
public interface IStartUpdate
{
    public abstract void Start(State owner);
}
public interface IEnableUpdate
{
    public abstract void OnEnable(State owner);
}
public interface IDisebleUpdate
{
    public abstract void OnDisable(State owner);
}

public interface IRuntimeStats
{
}

public interface IAnimationEvent
{
    public abstract void Event(State owner);
}

public interface IBoost
{
    public abstract void Boost(State owner);
}
