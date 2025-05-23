using UnityEngine;
using UnityEngine.UI;

namespace antoinegleisberg.HOA.UI.ColorSetters
{
    public class ImageColorSetter : MonoBehaviour
    {
        private Image _image;
        [SerializeField] private ScriptableColor _color;

        private void Awake()
        {
            SetColor();
        }

#if UNITY_EDITOR
        private void Update()
        {
            SetColor();
        }
#endif

        private void OnValidate()
        {
            SetColor();
        }

        public void ChangeColor(ScriptableColor color)
        {
            _color = color;
            SetColor();
        }

        private void SetColor()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            if (_image == null)
            {
                return;
            }
            if (_color == null)
            {
                return;
            }

            float alpha = _image.color.a;
            Color color = _color.Color;
            color.a = alpha;
            _image.color = color;
        }
    }
}
