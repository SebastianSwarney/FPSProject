using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour, IProjectile_Collision
{
    private float m_collisionDamage =0;


    public void ActivateCollision(GameObject p_collidedObject, Vector3 p_hitPosition)
    {
        Health newHealth = p_collidedObject.GetComponent<Health>();
        if (newHealth != null)
        {
            if (m_collisionDamage==0)
            {
                m_collisionDamage = GetComponent<Projectiles_Base>().m_projectileDamage;
            }
            newHealth.TakeDamage(m_collisionDamage);
        }
    }
}
