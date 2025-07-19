namespace CYA_Adventure_Game_Engine
{
    public interface IInstantiable<T>
        where T : IInstantiable<T>, new()
    {
        T Make();
    }
}
