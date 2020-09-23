namespace Tac.Backend.Emit.Support
{
    public interface ITacMethod<Tin,Tout>
    {
        Tout Invoke(Tin @in);
    }

    //public interface ITacInCastMethod<T>
    //{
    //    T Invoke(ITacObject @in);
    //}

    //public interface ITacOutCastMethod<T>
    //{
    //    ITacObject Invoke(T @in);
    //}
}