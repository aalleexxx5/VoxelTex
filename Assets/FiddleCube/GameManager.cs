using FiddleCube;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

	public GameObject cubePrefab;

	public int objectsToAdd;

	EntityManager manager;

	// Use this for initialization
	void Start ()
	{
		manager = World.Active.GetOrCreateManager<EntityManager>();
		addCubes(objectsToAdd);
	}

	// Update is called once per frame

	void Update () {
		if (Input.GetKeyDown("c"))
		{
			addCubes(objectsToAdd);
		}
	}

	private void addCubes(int amount)
	{
		
		NativeArray<Entity> entities = new NativeArray<Entity>(amount, Allocator.Temp);
		manager.Instantiate(cubePrefab, entities);
		
		
		for (int i = 0; i < amount; i++)
		{
			float zVal = Random.Range(50f, 200f);
			float xVal = Random.Range(6f, -6f);
			float yVal = Random.Range(3f, -3f);
			xVal *= 0.2f*zVal;
			yVal *= 0.2f*zVal;

			Quaternion rot = quaternion.Euler(0,0,0);
			
			manager.SetComponentData(entities[i],new Position{Value = new float3(xVal,yVal,zVal)});
			manager.SetComponentData(entities[i],new Rotation{Value = rot});
			manager.SetComponentData(entities[i],new EulerRotation{x = Random.Range(0,90f),y = Random.Range(0,90f),z = Random.Range(0,90f)});
			manager.SetComponentData(entities[i],new RotationSpeed{Value = Random.Range(5,50)});
		}
		entities.Dispose();
	}
}
