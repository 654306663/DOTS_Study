using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Systems;

public class TestRaycast : MonoBehaviour
{
    private Entity Raycast(float3 fromPosition, float3 toPosition)
    {
        PhysicsWorld physicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

        RaycastInput raycastInput = new RaycastInput()
        {
            Start = fromPosition,
            End = toPosition,
            Filter = CollisionFilter.Default
        };

        Unity.Physics.RaycastHit raycastHit;
        if (physicsWorld.CastRay(raycastInput, out raycastHit))
        {
            Entity hitEntity = raycastHit.Entity;
            return hitEntity;
        }
        else
        {
            return Entity.Null;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance = 100f;
            Debug.Log(Raycast(ray.origin, ray.direction * rayDistance));
        }
    }
}
