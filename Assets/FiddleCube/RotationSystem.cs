using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FiddleCube
{
    public class RotationSystem : JobComponentSystem
    {
        [BurstCompile]
        struct RotationJob : IJobProcessComponentData<Rotation, EulerRotation, RotationSpeed>
        {
            public float deltaTime;

            public void Execute(ref Rotation rotation, ref EulerRotation euler, ref RotationSpeed speed)
            {
                euler.x += math.mul(speed.Value, deltaTime);
                rotation.Value = Quaternion.Euler(euler.x, euler.y, euler.z);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            RotationJob rotateJob = new RotationJob
            {
                deltaTime = Time.deltaTime
            };

            JobHandle rotationHandle = rotateJob.Schedule(this,inputDeps);
            return rotationHandle;
        }
    }
}