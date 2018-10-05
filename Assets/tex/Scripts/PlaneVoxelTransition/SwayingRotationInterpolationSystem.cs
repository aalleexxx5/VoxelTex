using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Quaternion = UnityEngine.Quaternion;

namespace tex
{
    [BurstCompile]
    public class SwayingRotationInterpolationSystem : JobComponentSystem
    {
        struct SwayingRotationJob : IJobProcessComponentData<SwayingRotation, Rotation, DelayableInterpolation>
        {
            public void Execute(ref SwayingRotation sway, ref Rotation rot, ref DelayableInterpolation interpolation)
            {
                if (interpolation.Value<0.5)
                {
                    var otherInterpolation = (float) math.sin(interpolation.Value * math.PI );
                    rot.Value = Quaternion.Lerp(sway.startRotation, sway.sway, otherInterpolation);
                }
                else
                {
                    var otherInterpolation = (float) (1f- math.cos((interpolation.Value-0.5f) * math.PI));
                    rot.Value = Quaternion.Lerp(sway.sway, sway.endRotation, otherInterpolation);
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            base.OnUpdate(inputDeps);
            var swayingRotationJob = new SwayingRotationJob();
            return swayingRotationJob.Schedule(this, inputDeps);
        }
    }
}