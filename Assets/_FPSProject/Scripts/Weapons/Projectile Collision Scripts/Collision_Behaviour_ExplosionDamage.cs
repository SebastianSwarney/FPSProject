using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Behaviour_ExplosionDamage : MonoBehaviour, IProjectile_Collision
{
    [Header("Explosion Variables")]

    public float m_explodeRadius;
    public float m_explodeDamage;
    public LayerMask m_explosionHitLayer;

    [Header("Debugging")]
    public bool m_debug;
    public Color m_gizmoColor1;

    private TeamLabel m_myTeamLabel;
    
    private void Start()
    {
        m_myTeamLabel = GetComponent<TeamLabel>();
    }

    public void ActivateCollision(GameObject p_collidedObject, Vector3 p_hitPosition, int p_bulletOwnerPhotonID)
    {
        ExplodeMe(p_bulletOwnerPhotonID);
    }

    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        Gizmos.color = m_gizmoColor1;
        Gizmos.DrawWireSphere(transform.position, m_explodeRadius);
    }

    public void ExplodeMe(int p_bulletOwner)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, m_explodeRadius, m_explosionHitLayer);
        foreach (Collider col in cols)
        {
            Health newHealth = col.GetComponent<Health>();
            if (newHealth.m_teamLabel.m_myTeam == m_myTeamLabel.m_myTeam) return;
            newHealth.TakeDamage(m_explodeDamage, p_bulletOwner);
        }
    }

    public void CreateExplosion(int p_bulletOwner)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, m_explodeRadius, m_explosionHitLayer);
        foreach (Collider col in cols)
        {
            Health newHealth = col.GetComponent<Health>();
            newHealth.TakeDamage(m_explodeDamage, p_bulletOwner);
        }
    }
}
