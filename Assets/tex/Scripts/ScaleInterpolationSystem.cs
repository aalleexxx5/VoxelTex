using System;
using tex.Background;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace tex
{
    public class ScaleInterpolationSystem : JobComponentSystem
    {
        public struct ScaleInterpolationJob : IJobProcessComponentData<Scale, ScaleInterpolation, ContinualInterpolation>
        {
            public void Execute(ref Scale scale, ref ScaleInterpolation scaleInterpolation, ref ContinualInterpolation interpolation)
            {
                
                var otherInterpolation = (float) math.sin(interpolation.Value * math.PI ); 
                //var otherInterpolation = interpolation.Value; 
                scale.Value = math.lerp(scaleInterpolation.StartScale, scaleInterpolation.EndScale,
                    otherInterpolation);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new ScaleInterpolationJob().Schedule(this, inputDeps);
        }
    }
}