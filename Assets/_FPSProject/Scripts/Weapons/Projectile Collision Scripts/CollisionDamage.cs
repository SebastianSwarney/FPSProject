using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour, IProjectile_Collision
{
    private Projectiles_Base m_projectileDamage;
    private TeamLabel m_myTeamLabel;
    private void Start()
    {
        m_myTeamLabel = GetComponent<TeamLabel>();
        m_projectileDamage = GetComponent<Projectiles_Base>();
    }


    public void ActivateCollision(GameObject p_collidedObject, Vector3 p_hitPosition)
    {
        Health newHealth = p_collidedObject.GetComponent<Health>();
        if (newHealth != null)
        {
            if (newHealth.m_teamLabel.m_myTeam == m_myTeamLabel.m_myTeam) return;
            newHealth.TakeDamage(m_projectileDamage.m_projectileDamage);
        }
    }
}
