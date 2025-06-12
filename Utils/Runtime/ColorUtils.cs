using System;
using UnityEngine;

namespace antoinegleisberg.Utils
{
    public static class ColorUtils
    {
        public static Color FromHex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out var color))
            {
                return color;
            }

            throw new ArgumentException($"Invalid hex string: {hex}");
        }
    }
}
