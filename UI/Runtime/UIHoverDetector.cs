using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace antoinegleisberg.UI
{
    public class UIHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsHovered { get; private set; }

        public event Action<bool> OnHover;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
            OnHover?.Invoke(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
            OnHover?.Invoke(false);
        }
    }
}
