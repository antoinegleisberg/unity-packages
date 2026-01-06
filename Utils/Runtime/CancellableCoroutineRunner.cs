using System;
using System.Collections;
using UnityEngine;


namespace antoinegleisberg.Utils
{
    public class CancellableCoroutineRunner
    {
        private MonoBehaviour _coroutineRunner;
        private event Action _onCancel;

        public CancellableCoroutineRunner(MonoBehaviour coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public IEnumerator RunCancellableCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = _coroutineRunner.StartCoroutine(routine);
            Action onCancel = () =>
            {
                if (coroutine != null)
                {
                    _coroutineRunner.StopCoroutine(coroutine);
                }
            };

            AddOnCancelListener(onCancel);
            yield return coroutine;
            RemoveOnCancelListener(onCancel);
        }

        public void AddOnCancelListener(Action action)
        {
            _onCancel += action;
        }

        public void RemoveOnCancelListener(Action action)
        {
            _onCancel -= action;
        }

        public void Cancel()
        {
            _onCancel?.Invoke();
        }
    }
}
