using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace tex.Background
{
    public class ContinualInterpolationSystem : JobComponentSystem
    {
        public struct ContinualInterpolationJob : IJobProcessComponentData<ContinualInterpolation>
        {
            public float deltaTime;
            public void Execute(ref ContinualInterpolation data)
            {
                data.ValueData += deltaTime/data.Duration;
                if (data.ValueData > 1f)
                {
                    data.ValueData = -1f;
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new ContinualInterpolationJob
            {
                deltaTime =  Time.deltaTime
            };
            return job.Schedule(this, inputDeps);
        }
    }
}