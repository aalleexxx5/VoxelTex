using System;
using Unity.Entities;

namespace FiddleCube
{
    [Serializable]
    public struct EulerRotation : IComponentData
    {
        public float x;
        public float y;
        public float z;
    }
    
    public class EulerRotationComponent:ComponentDataWrapper<EulerRotation> {}
}