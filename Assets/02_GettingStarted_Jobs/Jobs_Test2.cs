using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;

public class Jobs_Test2 : MonoBehaviour
{
    [SerializeField] private bool useJobs1;
    [SerializeField] private bool useJobs2;
    [SerializeField] private Transform cubePrefab;
    private List<Cube> cubeList;
    public class Cube
    {
        public Transform transform;
        public float moveY;
    }

    private void Start()
    {
        cubeList = new List<Cube>();
        for (int i = 0; i < 1000; i++)
        {
            Transform cube = Instantiate(cubePrefab, new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f), 0), Quaternion.identity);
            cubeList.Add(new Cube { transform = cube, moveY = UnityEngine.Random.Range(1f, 2f)});
        }
    }

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs1)
        {
            NativeArray<float3> positionArray = new NativeArray<float3>(cubeList.Count, Allocator.TempJob);
            NativeArray<float> moveYArray = new NativeArray<float>(cubeList.Count, Allocator.TempJob);

            for (int i = 0; i < cubeList.Count; i++)
            {
                positionArray[i] = cubeList[i].transform.position;
                moveYArray[i] = cubeList[i].moveY;
            }

            ReallyToughParallelJob reallyToughParallelJob = new ReallyToughParallelJob
            {
                deltaTime = Time.deltaTime,
                positionArray = positionArray,
                moveYArray = moveYArray
            };

            JobHandle jobHandle = reallyToughParallelJob.Schedule(cubeList.Count, 100);
            jobHandle.Complete();

            for (int i = 0; i < cubeList.Count; i++)
            {
                cubeList[i].transform.position = positionArray[i];
                cubeList[i].moveY = moveYArray[i];
            }
            positionArray.Dispose();
            moveYArray.Dispose();
        }
        else if (useJobs2)
        {
            TransformAccessArray transformAccessArray = new TransformAccessArray(cubeList.Count);
            NativeArray<float> moveYArray = new NativeArray<float>(cubeList.Count, Allocator.TempJob);

            for (int i = 0; i < cubeList.Count; i++)
            {
                transformAccessArray.Add(cubeList[i].transform);
                moveYArray[i] = cubeList[i].moveY;
            }
            
            ReallyToughParallelJobTransforms reallyToughParallelJobTransforms = new ReallyToughParallelJobTransforms
            {
                deltaTime = Time.deltaTime,
                moveYArray = moveYArray
            };
            JobHandle jobHandle = reallyToughParallelJobTransforms.Schedule(transformAccessArray);
            jobHandle.Complete();

            for (int i = 0; i < cubeList.Count; i++)
            {
                cubeList[i].moveY = moveYArray[i];
            }
            transformAccessArray.Dispose();
            moveYArray.Dispose();
        }
        else
        {
            foreach (Cube cube in cubeList)
            {
                cube.transform.position += new Vector3(0, cube.moveY * Time.deltaTime);
                if (cube.transform.position.y > 5f)
                {
                    cube.moveY = -math.abs(cube.moveY);
                }
                if (cube.transform.position.y < -5f)
                {
                    cube.moveY = +math.abs(cube.moveY);
                }

                float value = 0f;
                for (int i = 0; i < 1000; i++)
                {
                    value = math.exp10(math.sqrt(value));
                }
            }
        }
        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }
}

[BurstCompile]
public struct ReallyToughParallelJob : IJobParallelFor
{
    public NativeArray<float3> positionArray;
    public NativeArray<float> moveYArray;
    public float deltaTime;

    public void Execute(int index)
    {
        positionArray[index] += new float3(0, moveYArray[index] * deltaTime, 0);
        if (positionArray[index].y > 5f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }
        if (positionArray[index].y < -5f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }

        float value = 0f;
        for (int i = 0; i < 1000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}

[BurstCompile]
public struct ReallyToughParallelJobTransforms : IJobParallelForTransform
{
    public NativeArray<float> moveYArray;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        transform.localPosition += new Vector3(0, moveYArray[index] * deltaTime, 0);
        if (transform.localPosition.y > 5f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }
        if (transform.localPosition.y < -5f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }

        float value = 0f;
        for (int i = 0; i < 1000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}