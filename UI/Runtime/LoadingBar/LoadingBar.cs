using UnityEngine;

namespace antoinegleisberg.UI.LoadingBar
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _progressBar;
    
        public void UpdateProgress(float progress)
        {
            Vector3 scale = _progressBar.localScale;
            scale.x = progress;
            _progressBar.localScale = scale;
        }
    }
}
