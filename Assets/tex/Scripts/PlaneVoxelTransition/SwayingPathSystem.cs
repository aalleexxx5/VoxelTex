using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace tex
{
    /*public class SwayingPathSystem : JobComponentSystem
    {
        private float GlobalInterpolate = 0;
        [BurstCompile]
        struct SwayingPathJob : IJobProcessComponentData<Position, SwayingPath>
        {
            public float Interpolate;
            public void Execute(ref Position pos, ref SwayingPath swayingPath)
            {
                float3 current;
                if (Interpolate<0.5f)
                {
                    current = math.lerp(swayingPath.StartPoint, swayingPath.StartPoint + swayingPath.Sway + (swayingPath.EndPonint*0.25f),
                        Interpolate * 2);
                }else
                {
                    current = math.lerp(swayingPath.StartPoint + swayingPath.Sway + (swayingPath.EndPonint*0.25f), swayingPath.EndPonint,
                        (Interpolate - 0.5f) * 2);
                    //current = swayingPath.StartPoint + swayingPath.Sway * (swayingPath.EndPonint * Interpolate * 2);
                }

                pos.Value = current;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            GlobalInterpolate += Time.deltaTime;
            if (GlobalInterpolate > 1)
            {
                GlobalInterpolate = 0;
            }
            var swayingPathJob = new SwayingPathJob
            {
                Interpolate = GlobalInterpolate
            };
            var swayHandle = swayingPathJob.Schedule(this, inputDeps);
            return swayHandle;
        }
    }*/
}