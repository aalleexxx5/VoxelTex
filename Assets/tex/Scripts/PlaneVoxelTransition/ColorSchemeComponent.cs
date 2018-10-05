using System;
using Unity.Entities;
using UnityEngine;

namespace tex
{
    [Serializable]
    public struct ColorScheme : IComponentData
    {
        public Color StartColor;
        public Color EndColor;
    }

    public class ColorSchemeComponent : ComponentDataWrapper<ColorScheme>
    {
        
    }
}