using UnityEngine;

namespace antoinegleisberg.StateMachine
{
    public abstract class BaseState<T> where T : MonoBehaviour
    {
        public abstract void EnterState(T t);

        public abstract void UpdateState(T t);

        public abstract void ExitState(T t);
    }
}
