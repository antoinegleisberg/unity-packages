using antoinegleisberg.Animation;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace antoinegleisberg.UI
{
    public class Toggle : MonoBehaviour
    {
        [SerializeField] private Button _toggleContainer;
        [SerializeField] private Image _toggleIndicator;
        [SerializeField] private float _toggleOffPosition;
        [SerializeField] private float _toggleOnPosition;
        [SerializeField] private float _toggleSwitchDuration;

        public Action<bool> OnToggleSwitch;

        public bool TogglePosition => _togglePosition;

        private bool _togglePosition;
            
        private void Awake()
        {
            _toggleContainer.onClick.AddListener(() => OnToggleSwitched());
        }

        private void OnDestroy()
        {
            _toggleContainer.onClick.RemoveListener(() => OnToggleSwitched());
        }

        private void OnToggleSwitched()
        {
            StartCoroutine(SwitchToggleAnimation());
            _togglePosition = !_togglePosition;
            OnToggleSwitch?.Invoke(_togglePosition);
        }

        private IEnumerator SwitchToggleAnimation()
        {
            RectTransform rt = _toggleIndicator.GetComponent<RectTransform>();
            Vector3 start = rt.anchoredPosition;
            Vector3 end = rt.anchoredPosition;
            start.x = _togglePosition ? _toggleOnPosition : _toggleOffPosition;
            end.x = _togglePosition ? _toggleOffPosition : _toggleOnPosition;
            yield return StartCoroutine(rt.Slide(start, end, _toggleSwitchDuration));
        }
    }
}
