using UnityEngine;

namespace antoinegleisberg.UI.ColorSetters
{
    [CreateAssetMenu(fileName = "Color", menuName = "ScriptableObject/Color")]
    public class ScriptableColor : ScriptableObject
    {
        [SerializeField] private Color _color;

        public Color Color => _color;

    }
}
