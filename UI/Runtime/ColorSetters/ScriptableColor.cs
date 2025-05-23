using UnityEngine;

namespace antoinegleisberg.HOA.UI.ColorSetters
{
    [CreateAssetMenu(fileName = "Color", menuName = "ScriptableObject/Color")]
    public class ScriptableColor : ScriptableObject
    {
        [SerializeField] private Color _color;

        public Color Color => _color;
    }
}
