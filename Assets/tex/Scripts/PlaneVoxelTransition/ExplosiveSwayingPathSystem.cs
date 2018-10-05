using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace tex
{
    public class ExplosiveSwayingPathSystem : JobComponentSystem
    {
        [BurstCompile]
        struct SwayingPathJob : IJobProcessComponentData<Position, SwayingPath, DelayableInterpolation>
        {

            public void Execute(ref Position pos, ref SwayingPath swayingPath, ref DelayableInterpolation interpolation)
            {
                float3 current;
                if (interpolation.Value<0.5f)
                {
                    var otherInterpolation = (float) math.sin(interpolation.Value * math.PI ); 
                    current = math.lerp(swayingPath.StartPoint, swayingPath.StartPoint + swayingPath.Sway,
                        otherInterpolation);
                }else
                {
                    var otherInterpolation = (float) (1f- math.cos((interpolation.Value-0.5f) * math.PI));
                    current = math.lerp(swayingPath.StartPoint + swayingPath.Sway, swayingPath.EndPonint,
                        otherInterpolation);
                }

                pos.Value = current;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var swayingPathJob = new SwayingPathJob();
            var swayHandle = swayingPathJob.Schedule(this, inputDeps);
            return swayHandle;
        }
    }
}