using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct RotationSpeed : IComponentData
{
    public float Value;
}

public class RotationSpeedComponent : ComponentDataWrapper<RotationSpeed>{}
