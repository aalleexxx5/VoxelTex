using System;
using Unity.Entities;
using Unity.Mathematics;

namespace tex.Background
{
    [Serializable]
    public struct ScaleInterpolation : IComponentData
    {
        public float3 StartScale;
        public float3 EndScale;
    }
    public class ScaleInterpolationComponent
    {
        
    }
}