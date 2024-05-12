using System;
using UnityEngine;


namespace antoinegleisberg.Animation
{
    [Serializable]
    public struct AnimationFrame
    {
        public Sprite Sprite;
        public bool FlipX;
        public bool FlipY;
    }
}
