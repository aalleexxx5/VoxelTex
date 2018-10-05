using System;
using Unity.Entities;

namespace tex
{
    [Serializable]
    public struct DelayableInterpolation : IComponentData
    {
        public float StartDelaySeconds;
        public float FinishDelaySeconds;
        public float DurationSeconds;
        public float Value;
        public byte IsFinished;
    }
    
    public class DelayableInterpolatingComponent : ComponentDataWrapper<DelayableInterpolation>{}
}