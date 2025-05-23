using TMPro;
using UnityEngine;

namespace antoinegleisberg.HOA.UI.ColorSetters
{
    [ExecuteAlways]
    public class TextColorSetter : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        [SerializeField] private ScriptableColor _color;

        private void Awake()
        {
            SetColor();
        }

#if UNITY_EDITOR
        // Careful about performance here: if this becomes an issue, I may have to move this from update to a manual button
        private void Update()
        {
            SetColor();
        }
#endif

        private void SetColor()
        {
            if (_text == null)
            {
                _text = GetComponent<TextMeshProUGUI>();
            }

            if (_text == null || _color == null)
            {
                return;
            }

            Color color = _color.Color;
            color.a = _text.color.a;
            _text.color = color;
        }
    }
}
