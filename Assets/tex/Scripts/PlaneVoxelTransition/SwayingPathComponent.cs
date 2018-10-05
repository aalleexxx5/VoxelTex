using System;
using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;

namespace tex
{
    [Serializable]
    public struct SwayingPath : IComponentData
    {
        public float3 StartPoint;
        public float3 EndPonint;
        public float3 Sway;
    }
    
    public class SwayingPathComponent : ComponentDataWrapper<SwayingPath>
    { }
}