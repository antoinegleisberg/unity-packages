using UnityEngine;

namespace antoinegleisberg.Utils
{
    public class CoroutineRunnerMB : MonoBehaviour { }

    public static class CoroutineRunner
    {
        private static CoroutineRunnerMB _runner;

        public static CoroutineRunnerMB GetRunner()
        {
            if (_runner == null)
            {
                _runner = new GameObject("CoroutineRunner").AddComponent<CoroutineRunnerMB>();
            }
            return _runner;
        }
    }
}
