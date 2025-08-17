public interface IEvent
{
    public abstract void Execute(StateEvent owner);
}
