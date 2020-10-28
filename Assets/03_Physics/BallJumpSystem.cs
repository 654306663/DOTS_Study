using Unity.Entities;
using UnityEngine;
using Unity.Physics;

public class BallJumpSystem : ComponentSystem
{
    protected override void OnUpdate() {
        Entities.ForEach((ref PhysicsVelocity physicsVelocity) =>{
            if (Input.GetKeyDown(KeyCode.Space))
            {
                physicsVelocity.Linear.y = 5f;
            }
        });
    }
}
