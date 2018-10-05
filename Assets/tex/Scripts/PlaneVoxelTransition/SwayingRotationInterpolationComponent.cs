using Unity.Entities;
using UnityEngine;

namespace tex
{
    public struct SwayingRotation : IComponentData
    {
        public Quaternion startRotation;
        public Quaternion endRotation;
        public Quaternion sway;
    }
    
    public class SwayingRotationInterpolationComponent : ComponentDataWrapper<SwayingRotation>
    {
        
    }
}