using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using antoinegleisberg.Animation;

namespace antoinegleisberg.UI
{
    [RequireComponent(typeof(UIHoverDetector))]
    public class PopUpTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject _popUpTarget;
        [SerializeField] public float FadeOutDuration;

        private UIHoverDetector _uiHoverDetector;
        
        private Coroutine _fadeOutCoroutine;

        private void Awake()
        {
            _uiHoverDetector = GetComponent<UIHoverDetector>();
            _uiHoverDetector.OnHover += OnHover;
            _popUpTarget.SetActive(false);
        }

        private void OnDestroy()
        {
            _uiHoverDetector.OnHover -= OnHover;
        }

        private void OnHover(bool hover)
        {
            if (hover)
            {
                if (_fadeOutCoroutine != null)
                {
                    StopCoroutine(_fadeOutCoroutine);
                    _fadeOutCoroutine = null;
                }
                StartCoroutine(_popUpTarget.transform.FadeInGameObjectAndChildren(0, this));
                _popUpTarget.SetActive(true);
            }
            else
            {
                StartCoroutine(FadeOut());
            }
        }

        private IEnumerator FadeOut()
        {
            if (_popUpTarget.TryGetComponent(out Image image))
            {
                yield return StartCoroutine(image.transform.FadeOutGameObjectAndChildren(FadeOutDuration, this));
            }
            _popUpTarget.SetActive(false);
        }
    }
}
