using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace antoinegleisberg.UI.ColorSetters
{
    public class ButtonColorSetter : MonoBehaviour
    {
        private TextMeshProUGUI _buttonText;
        private Button _button;
        
        [SerializeField] private ScriptableColor _textColor;
        [SerializeField] private ScriptableColor _buttonNormalColor;
        [SerializeField] private ScriptableColor _buttonHighlightedColor;
        [SerializeField] private ScriptableColor _buttonPressedColor;
        [SerializeField] private ScriptableColor _buttonSelectedColor;
        [SerializeField] private ScriptableColor _buttonDisabledColor;


        private void Awake()
        {
            SetColors();
        }

#if UNITY_EDITOR
        private void Update()
        {
            SetColors();
        }
#endif

        private void OnValidate()
        {
            SetColors();
        }

        private void SetColors()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
            if (_buttonText == null)
            {
                _buttonText = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (_buttonText != null && _textColor != null)
            {
                _buttonText.color = _textColor.Color;
            }

            if (_button == null || _button.transition != Selectable.Transition.ColorTint)
            {
                return;
            }
            ColorBlock colors = _button.colors;
            if (_buttonNormalColor != null)
            {
                colors.normalColor = _buttonNormalColor.Color;
            }
            if (_buttonHighlightedColor != null)
            {
                colors.highlightedColor = _buttonHighlightedColor.Color;
            }
            if (_buttonPressedColor != null)
            {
                colors.pressedColor = _buttonPressedColor.Color;
            }
            if (_buttonSelectedColor != null)
            {
                colors.selectedColor = _buttonSelectedColor.Color;
            }
            if (_buttonDisabledColor != null)
            {
                colors.disabledColor = _buttonDisabledColor.Color;
            }
            _button.colors = colors;
        }
    }
}
