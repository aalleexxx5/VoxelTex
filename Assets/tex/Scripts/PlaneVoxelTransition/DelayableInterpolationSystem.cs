using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace tex
{
    public class DelayableInterpolationSystem : JobComponentSystem
    {
        
        [BurstCompile]
        struct DelayableInterpolationJob : IJobProcessComponentData<DelayableInterpolation>
        {
            public float DeltaTime;
            public void Execute(ref DelayableInterpolation interpolation)
            {
                if (interpolation.IsFinished != 0) return;
                if (interpolation.StartDelaySeconds > 0)
                {
                    interpolation.StartDelaySeconds -= DeltaTime;
                }else if (interpolation.Value!=1)
                {
                    interpolation.Value += DeltaTime / interpolation.DurationSeconds;
                    if (interpolation.Value >1)
                    {
                        interpolation.Value = 1;
                    }
                }else
                {
                    interpolation.FinishDelaySeconds -= DeltaTime;
                    if (interpolation.FinishDelaySeconds<0)
                    {
                        interpolation.IsFinished = 1;
                    }
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            DelayableInterpolationJob job = new DelayableInterpolationJob
            {
                DeltaTime = Time.deltaTime
            };
            return job.Schedule(this, inputDeps);
        }
    }
}