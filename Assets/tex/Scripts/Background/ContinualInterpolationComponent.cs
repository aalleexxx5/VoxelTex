using System;
using Unity.Entities;
using Unity.Mathematics;

namespace tex.Background
{
    [Serializable]
    public struct ContinualInterpolation : IComponentData
    {
        public float ValueData;
        public float Value => math.abs(ValueData);

        public float Duration;
    }
    public class ContinualInterpolationComponent
    {
        
    }
}