using System.Collections;
using System.Collections.Generic;
using tex.Background;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MovementPattern
{
	Center,
	Random,
	Left,
	Right,
	Up,
	Down
}

public class Background : MonoBehaviour
{

	public GameObject Hex;
	public int Width;
	public int Height;
	public float Space;
	public float MinWidth;
	public float MaxWidth;
	public float Duration;
	public MovementPattern Pattern;
	[Range(1,10)]
	public int repeats;
	
	private EntityManager entityManager;

	private float largeRadiusX;
	private float largeRadiusY;
	private float smallRadiusX;
	private float smallRadiusY;
	private float sideLengthX;
	private float sideLengthY;
	
	// Use this for initialization
	private void OnValidate()
	{
		Width = math.max(1, Width);
		Height = math.max(1, Height);
	}
	

	void Start ()
	{
		entityManager = World.Active.GetOrCreateManager<EntityManager>();
		// Half the diameter
		largeRadiusX = 0.5f * Hex.transform.localScale.x;
		largeRadiusY = 0.5f * Hex.transform.localScale.y;
		
		sideLengthX = largeRadiusX;
		sideLengthY = largeRadiusY;
		
		smallRadiusX = (math.sqrt(3f)/2f) * largeRadiusX;
		smallRadiusY = (math.sqrt(3f)/2f) * largeRadiusY;
		
		createHexes();
	}

	private void createHexes()
	{
		Vector3 right = Hex.transform.right;
		Vector3 up = Hex.transform.up;
		Vector3 forward = Hex.transform.forward;

		var smallRight = right * smallRadiusX;
		var largeUp = up * largeRadiusY;
		var stepUp = up * (largeRadiusY+sideLengthY/2);
		
		NativeArray<Entity> entities = new NativeArray<Entity>(Width*Height,Allocator.Temp);
		entityManager.Instantiate(Hex, entities);
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				Vector3 locationX = x * (smallRight*2) + x * Space * smallRight;
				if (y%2==1)
				{
					locationX += smallRight;
				}
				Vector3 locationY = y * stepUp + y * Space * (up * smallRadiusY);
				
				
				entityManager.AddComponentData(entities[x+Width*y],new Position{Value = locationX + locationY + Hex.transform.localToWorldMatrix.MultiplyPoint(Vector3.zero)});
				entityManager.AddComponentData(entities[x+Width*y],new Scale());
				entityManager.AddComponentData(entities[x+Width*y],new ScaleInterpolation{StartScale = new float3(Hex.transform.localScale.x,Hex.transform.localScale.y,MinWidth), EndScale = new float3(Hex.transform.localScale.x,Hex.transform.localScale.y,MaxWidth)});

				entityManager.AddComponentData(entities[x+Width*y],new ContinualInterpolation{Duration =  Duration, ValueData = getTime(x,y)});
			}
		}
		entities.Dispose();
	}

	private float getTime(int x, int y)
	{
		if (Pattern == MovementPattern.Center)
		{
			var center = new float2(Width/2f,Height/2f);
			var maxDist = math.distance(center,float2.zero);
			var timeCentered = (maxDist - math.distance(center, new float2(x,y))) * repeats / maxDist %1f;
			return timeCentered;
		}else if (Pattern == MovementPattern.Random)
		{
			return Random.Range(0f, 1f);
		}else if (Pattern == MovementPattern.Left)
		{
			return (float) x * repeats / Width % 1f;
		}
		else if (Pattern == MovementPattern.Right)
		{
			return (float)(Width - x) * repeats / Width % 1f;
		}
		else if (Pattern == MovementPattern.Down)
		{
			return (float) y * repeats / Height % 1f;
		}else if (Pattern == MovementPattern.Up)
		{
			return (float)(Height - y)*repeats / Height % 1f;
		}

		return 0;

	}

	// Update is called once per frame
	void Update () {
		
	}
}
