using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles_Tracking : Projectiles_Base
{
    [Header("Missile Properties")]
    public bool m_isTypeTracking;
    public float m_rotateToTargetSpeed;
    public float m_lifespan;

    private float m_missileSpeed;
    private Transform m_targetUnit;
    private WaitForSeconds m_lifeDelay;
    private Coroutine m_lifeCoroutine;

    public override void SetVariables(TeamTypes.TeamType p_myNewTeam, Vector3 p_newVelocity,int p_ownerID, Transform p_target = null, float p_projectileDamage = 0)
    {
        m_teamLabel.SetTeamType(p_myNewTeam);
        if(m_lifeDelay == null)
        {
            m_lifeDelay = new WaitForSeconds(m_lifespan);
        }
        m_missileSpeed = p_newVelocity.magnitude;
        if (p_target != null)
        {
            m_targetUnit = p_target;
        }
        m_projectileDamage = p_projectileDamage;
        m_lifeCoroutine = StartCoroutine(LifeTime());

    }

    public void RotateMissile(Vector3 p_targetPos)
    {
        Quaternion lookAt = Quaternion.LookRotation(p_targetPos - transform.position, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, m_rotateToTargetSpeed * Time.deltaTime);
    }

    public override void FixedUpdate()
    {

        if (m_isTypeTracking)
        {
            if (m_targetUnit!= null)
            {
                
                RotateMissile(m_targetUnit.position);
                if (!m_targetUnit.gameObject.activeSelf)
                {
                    m_targetUnit = null;
                }
            }
            
        }
        Vector3 velocity = transform.forward * m_missileSpeed * Time.fixedDeltaTime;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, m_collisionRadius, velocity, out hit, velocity.magnitude, m_collisionDetectionMask))
        {
            HitObject(hit.transform.gameObject,hit.point, true);
            StopAllCoroutines();
        }
        else
        {
            transform.position += velocity;
        }

    }

    private IEnumerator LifeTime()
    {
        yield return m_lifeDelay;
        DestroyBullet();
        
    }
}
