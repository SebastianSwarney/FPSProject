using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile_Collision
{
    void ActivateCollision(GameObject p_collidedObject, Vector3 p_hitPosition);
}
