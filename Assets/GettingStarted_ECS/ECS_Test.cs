using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;

public class ECS_Test : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    private void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(IndexComponent),
            typeof(Translation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(MoveSpeedComponent)
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(1000, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);
        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity, new IndexComponent { index = i });
            entityManager.SetComponentData(entity, new MoveSpeedComponent { moveSpeed = 0.5f + i * 0.005f });
            entityManager.SetComponentData(entity, new Translation { Value = new Unity.Mathematics.float3(Random.Range(-8, 8f), Random.Range(-5, 5f), 0) });
            entityManager.SetComponentData(entity, new Scale { Value = 0.1f + i * 0.001f });

            entityManager.SetSharedComponentData(entity, new RenderMesh { mesh = mesh, material = material });
        }
        entityArray.Dispose();
    }
}
