using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.StateMachine
{
    public class StateMachine<T> where T : MonoBehaviour
    {
        private Stack<BaseState<T>> _stateStack;
        private T _parentObject;

        public StateMachine(T parentObject, BaseState<T> startState)
        {
            _parentObject = parentObject;
            _stateStack = new Stack<BaseState<T>>();
            _stateStack.Push(startState);

            _parentObject.StartCoroutine(Start());
        }

        public void PushState(BaseState<T> newState)
        {
            _stateStack.Peek().ExitState(_parentObject);
            _stateStack.Push(newState);
            _stateStack.Peek().EnterState(_parentObject);
        }

        public void SwitchState(BaseState<T> newState)
        {
            _stateStack.Peek().ExitState(_parentObject);
            _stateStack.Pop();
            _stateStack.Push(newState);
            _stateStack.Peek().EnterState(_parentObject);
        }

        public void PopState()
        {
            _stateStack.Peek().ExitState(_parentObject);
            _stateStack.Pop();
            _stateStack.Peek().EnterState(_parentObject);
        }
        
        private IEnumerator Start()
        {
            yield return null;
            _stateStack.Peek().EnterState(_parentObject);

            _parentObject.StartCoroutine(UpdateCoroutine());
        }
        
        private IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                _stateStack.Peek().UpdateState(_parentObject);
                yield return null;
            }
        }
    }
}
