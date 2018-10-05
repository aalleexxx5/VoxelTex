using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using tex;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

public class PlaneVoxelizer : MonoBehaviour
{
    public GameObject VoxelPix;
    public GameObject SourcePlane;
    public GameObject DestinationPlane;
    public float SimultaniousRows;
    [Range(0f,0.1f)]
    public float ColumnDisolveSpeed;
    [Range(0.5f,60f)]
    public float DurationSeconds;
    
    public float EndPauseSeconds;
    public float StartPauseSeconds;
    public float SwayAmount;
    public int DownscaleMultiplyer;


    private Texture2D startTexture;
    private Texture2D endTexture;
    private Texture2D downscaledStart;
    private Texture2D downscaledEnd;
    private EntityManager entityManager;
    private Entity slowestEntity;
    private float longestDelay;
    private float delay = 2;


    private void OnValidate()
    {
        SimultaniousRows = math.max(0, SimultaniousRows);
        StartPauseSeconds = math.max(0, StartPauseSeconds);
        EndPauseSeconds = math.max(0, EndPauseSeconds);
        SwayAmount = math.max(0, SwayAmount);
        DownscaleMultiplyer = Util.NearestPowerOfTwo(DownscaleMultiplyer);
        DownscaleMultiplyer = math.max(2, DownscaleMultiplyer);
        DownscaleMultiplyer = math.min(SourcePlane.GetComponent<Renderer>().sharedMaterial.mainTexture.width/2,
            DownscaleMultiplyer);

    }

    void Start()
    {
        InitValues();
    }

    public void InitValues()
    {
        var startRenderer = SourcePlane.GetComponent<Renderer>();
        var endRenderer = DestinationPlane.GetComponent<Renderer>();
        startTexture = (Texture2D) startRenderer.material.mainTexture;
        endTexture = (Texture2D) endRenderer.material.mainTexture;
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        
        downscaledStart = Downscale(startTexture);
        downscaledEnd = Downscale(endTexture);
    }

    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            hideEndLocation();
            hideStartLocation();
            createCubesTransition();
        }

        if (Input.GetKeyDown("r"))
        {
            delay = 0;
            hideEndLocation();
            showStartLocation();
        }

        if (delay >= 2 && delay < 3)
        {
            delay = 3;
            showDownscaledStartLocation();
        }

        delay += Time.deltaTime;
        RemoveCubesWhenFinished();
    }

    private void showStartLocation()
    {
        SourcePlane.GetComponent<Renderer>().enabled = true;
        SourcePlane.GetComponent<Renderer>().material.mainTexture = startTexture;

    }

    private void showDownscaledStartLocation()
    {
        SourcePlane.GetComponent<Renderer>().material.mainTexture = downscaledStart;
    }

    private void hideStartLocation()
    {
        SourcePlane.GetComponent<Renderer>().enabled = false;
    }

    private void showEndLocation()
    {
        DestinationPlane.GetComponent<Renderer>().enabled = true;
        DestinationPlane.GetComponent<Renderer>().material.mainTexture = endTexture;
    }

    private void showDownscaledEndLocation()
    {
        DestinationPlane.GetComponent<Renderer>().material.mainTexture = downscaledEnd;
    }

    private void hideEndLocation()
    {
        DestinationPlane.GetComponent<Renderer>().enabled = false;
    }

    private void RemoveCubesWhenFinished()
    {
        if (!slowestEntity.Equals(Entity.Null))
        {
            if (entityManager.GetComponentData<DelayableInterpolation>(slowestEntity).IsFinished != 0)
            {
                var entities = entityManager.GetAllEntities();
                foreach (var entity in entities)
                {
                    if (entityManager.HasComponent<DelayableInterpolation>(entity))
                    {
                        entityManager.DestroyEntity(entity);
                    }
                }

                slowestEntity = Entity.Null;
                entities.Dispose();
            }
            else if (entityManager.GetComponentData<DelayableInterpolation>(slowestEntity).Value >= 1)
            {
                showEndLocation();
                showDownscaledEndLocation();
                delay = -(EndPauseSeconds+1);
            }
        }

        if (delay >=-1 && delay<0)
        {
            showEndLocation();
            delay = 3;
        }
    }

    //IDEA: 2D image pixelates. Each pixel becomes a cube.

    // Each cube takes random swoopy path to final destination.

    // Each cube rotates 90 degrees. New face is final pixelated image.

    // Join effect un-pixelates the final image.

    private void createCubesTransition()
    {
        if (startTexture.width == endTexture.width)
        {
            slowestEntity = Entity.Null;
            longestDelay = 0;
            var downscaledStartPixels = downscaledStart.GetPixels();

            downscaledEnd = Downscale(endTexture);
            var downscaledWidth = startTexture.width / DownscaleMultiplyer;
            var downscaledEndPixels = downscaledEnd.GetPixels();

            NativeArray<Entity> entities = new NativeArray<Entity>(downscaledStartPixels.Length, Allocator.Temp);
            entityManager.Instantiate(VoxelPix, entities);

            var cubeScale = 10f / downscaledWidth;

            for (int i = 0; i < downscaledStartPixels.Length; i++)
            {

                var elementX = i% downscaledWidth;
                var elementY = i / downscaledWidth;

                var startXIncrement = SourcePlane.transform.forward;
                var startYIncrement = SourcePlane.transform.right;
                var startZIncrement = SourcePlane.transform.up;

                // Array is looped through from bottom left, but plane will be rotated. NOTE: ElementX and y are swapped.
                var startX = startXIncrement*(downscaledWidth*cubeScale) - startXIncrement * elementY*cubeScale - startXIncrement * (cubeScale/2);
                var startY = startYIncrement*(downscaledWidth*cubeScale) - startYIncrement * elementX*cubeScale - startYIncrement * (cubeScale/2);
                
                var startZ = startZIncrement * (-cubeScale/2);
                var pos = startX + startY + startZ + SourcePlane.transform.localToWorldMatrix.MultiplyPoint(new Vector3(-5, 0, -5));
               
                
                var endXIncrement = DestinationPlane.transform.forward;
                var endYIncrement = DestinationPlane.transform.right;
                var endZIncrement = DestinationPlane.transform.up;
                
                //Same goes here
                var endX = endXIncrement*(downscaledWidth*cubeScale)-endXIncrement * elementY*cubeScale - endXIncrement * (cubeScale/2);
                var endY = endYIncrement*(downscaledWidth*cubeScale)-endYIncrement * elementX*cubeScale - endYIncrement * (cubeScale/2);
                
                var endZ = endZIncrement * (-cubeScale/2);
                var endPos = endX + endY + endZ + DestinationPlane.transform.localToWorldMatrix.MultiplyPoint(new Vector3(-5, 0, -5));

                
                
                entityManager.SetComponentData(entities[i], new Position {Value = pos});
                entityManager.SetComponentData(entities[i],
                    new SwayingPath
                    {
                        StartPoint = pos, EndPonint = endPos,
                        Sway = new float3(Random.Range(-SwayAmount, SwayAmount), Random.Range(-SwayAmount, SwayAmount),
                            Random.Range(-SwayAmount, SwayAmount))
                    });
                entityManager.SetComponentData(entities[i],
                    new ColorScheme {StartColor = downscaledStartPixels[i], EndColor = downscaledEndPixels[i]});

                var yDisolve = elementY * ((downscaledWidth / SimultaniousRows) * ColumnDisolveSpeed);
                var xDisolve = elementX * ColumnDisolveSpeed;
                var startDelay = StartPauseSeconds + xDisolve + yDisolve;
                if (startDelay > longestDelay)
                {
                    longestDelay = startDelay;
                    slowestEntity = entities[i];
                }
                entityManager.AddComponentData(entities[i], new SwayingRotation{startRotation = SourcePlane.transform.rotation, sway = Random.rotation, endRotation = DestinationPlane.transform.rotation});
                entityManager.AddComponentData(entities[i], new Scale {Value = new float3(cubeScale,cubeScale,cubeScale)});
                entityManager.AddComponentData(entities[i], new Rotation{Value = SourcePlane.transform.rotation});
                entityManager.AddComponentData(entities[i],
                    new DelayableInterpolation
                    {
                        StartDelaySeconds = startDelay, FinishDelaySeconds = 0,
                        DurationSeconds = DurationSeconds, IsFinished = 0, Value = 0
                    });
                
            }

            entities.Dispose();
        }
    }

    private Texture2D Downscale(Texture2D texture)
    {
        var res = Instantiate(texture);
        //var res = new Texture2D(texture.width, texture.height);
        //res.SetPixels(texture.GetPixels());
        res.Resize(texture.width / DownscaleMultiplyer, texture.width / DownscaleMultiplyer, TextureFormat.RGB24,
            false);
        res.SetPixels(DownscaleCenter(texture.GetPixels()));
        res.Apply(false);

        return res;
    }


    private Color[] DownscaleCenter(Color[] startColors)
    {
        Color[] endColors = new Color[(int) (startColors.Length / math.pow(DownscaleMultiplyer, 2))];
        var pixelsPrWidth = startTexture.width / DownscaleMultiplyer;
        var widthOfSmallPixel = DownscaleMultiplyer;
        var area = (int) math.pow(widthOfSmallPixel, 2);

        for (int i = 0; i < endColors.Length; i++)
        {
            int smallX = i % pixelsPrWidth;
            int smallY = i / pixelsPrWidth;
            endColors[i] =
                startColors[
                    smallX * widthOfSmallPixel + widthOfSmallPixel / 2 +
                    (smallY * widthOfSmallPixel + widthOfSmallPixel / 2) * startTexture.width];
        }

        return endColors;
    }

    private Color[] DownscaleAverage(Color[] startColors)
    {
        Color[] downscaledStartColor = new Color[startColors.Length / (int) math.pow(DownscaleMultiplyer, 2)];

        var pixelsPrWidth = startTexture.width / DownscaleMultiplyer;
        var widthOfSmallPixel = DownscaleMultiplyer;
        var area = (int) math.pow(widthOfSmallPixel, 2);

        float r, g, b, a;

        for (int i = 0; i < downscaledStartColor.Length; i++)
        {
            int smallX = i % pixelsPrWidth;
            int smallY = i / pixelsPrWidth;
            r = 0;
            g = 0;
            b = 0;
            a = 0;
            for (int areaY = 0; areaY < widthOfSmallPixel; areaY++)
            {
                for (int areaX = 0; areaX < widthOfSmallPixel; areaX++)
                {
                    var x = areaX + (smallX * widthOfSmallPixel);
                    var y = areaY + (smallY * widthOfSmallPixel);
                    var index = x + y * startTexture.width;
                    if (startColors.Length < index)
                    {
                        print(smallX + " : " + x + ", " + smallY + " : " + y);
                    }
                    else
                    {
                        var currentColor = startColors[x + y * (startTexture.width)];
                        r += currentColor.r;
                        g += currentColor.g;
                        b += currentColor.b;
                        a += currentColor.a;
                    }
                }
            }


            downscaledStartColor[i] = new Color(r / area, g / area,
                b / area, a / area);
        }

        return downscaledStartColor;
    }
}