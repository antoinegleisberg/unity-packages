namespace antoinegleisberg.StateMachine
{
    public abstract class BaseState<T>
    {
        public abstract void EnterState(T t);

        public abstract void UpdateState(T t);

        public abstract void ExitState(T t);
    }
}
