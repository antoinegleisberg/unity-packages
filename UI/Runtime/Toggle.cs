using antoinegleisberg.Animation;
using antoinegleisberg.HOA.UI.ColorSetters;
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

        [SerializeField] private ScriptableColor _colorOn;
        [SerializeField] private ScriptableColor _colorOff;

        public Action<bool> OnToggleSwitch;

        public bool TogglePosition { get; private set; }
            
        private void Awake()
        {
            _toggleContainer.onClick.AddListener(() => OnToggleSwitched());
        }

        private void OnDestroy()
        {
            _toggleContainer.onClick.RemoveListener(() => OnToggleSwitched());
        }

        public void SetValue(bool value)
        {
            TogglePosition = value;
            RectTransform rt = _toggleIndicator.GetComponent<RectTransform>();
            Vector3 end = rt.anchoredPosition;
            end.x = value ? _toggleOnPosition : _toggleOffPosition;
            rt.anchoredPosition = end;
            SwitchIndicatorColor();
        }

        private void OnToggleSwitched()
        {
            StartCoroutine(SwitchToggleAnimation(_toggleSwitchDuration));
            TogglePosition = !TogglePosition;
            SwitchIndicatorColor();
            OnToggleSwitch?.Invoke(TogglePosition);
        }

        private IEnumerator SwitchToggleAnimation(float duration)
        {
            RectTransform rt = _toggleIndicator.GetComponent<RectTransform>();
            Vector3 start = rt.anchoredPosition;
            Vector3 end = rt.anchoredPosition;
            start.x = TogglePosition ? _toggleOnPosition : _toggleOffPosition;
            end.x = TogglePosition ? _toggleOffPosition : _toggleOnPosition;
            yield return StartCoroutine(rt.Slide(start, end, duration));
        }

        private void SwitchIndicatorColor()
        {
            ScriptableColor targetColor;
            if (TogglePosition)
            {
                targetColor = _colorOn;
            }
            else
            {
                targetColor = _colorOff;
            }
            _toggleIndicator.GetComponent<ImageColorSetter>().ChangeColor(targetColor);
        }
    }
}
