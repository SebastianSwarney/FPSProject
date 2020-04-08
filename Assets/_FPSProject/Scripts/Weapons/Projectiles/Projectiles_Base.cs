using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletEvent : UnityEngine.Events.UnityEvent { }

public class Projectiles_Base : MonoBehaviour
{
    [HideInInspector]
    public TeamLabel m_teamLabel;
    [HideInInspector]
    public int m_bulletOwnerID;
    [Header("Bullet Physics")]
    public Vector3 m_velocity;
    public float m_gravity;
    public bool m_armorPiercing;
    public float m_collisionRadius;
    public LayerMask m_collisionDetectionMask;
    public LayerMask m_boundsLayer;
    public BulletEvent m_bulletSpawnedEvent;
    public BulletEvent m_bulletHitEvent;

    [HideInInspector]
    public float m_projectileDamage;


    [HideInInspector]
    public ObjectPooler m_pooler;


    
    [Header("Visuals")]
    public Transform m_visualsTransform;

    [Header("Debugging")]
    public bool m_debugGizmos;
    public Color m_gizmosColor1, m_gizmosColor2;

    private IProjectile_Collision[] m_projectileCollisionScripts;

    private void Awake()
    {
        m_projectileCollisionScripts = GetComponents<IProjectile_Collision>();
        m_teamLabel = GetComponent<TeamLabel>();
    }
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
        
    }
    public virtual void SetVariables(TeamTypes.TeamType p_myNewTeam, Vector3 p_newVelocity, int p_ownerID, Transform p_target = null, float p_projectileDamage = 0)
    {

        m_teamLabel.SetTeamType(p_myNewTeam);
        m_velocity = p_newVelocity;
        m_bulletSpawnedEvent.Invoke();
        m_projectileDamage = p_projectileDamage;
        m_bulletOwnerID = p_ownerID;
    }


    public virtual void FixedUpdate()
    {
        m_velocity = new Vector3(m_velocity.x, m_velocity.y - (Mathf.Abs(m_gravity) / 50), m_velocity.z);

        m_visualsTransform.LookAt(transform.position + m_velocity);

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, m_collisionRadius, m_velocity.normalized, out hit, m_velocity.magnitude * Time.fixedDeltaTime, m_collisionDetectionMask))
        {
            Vector3 hitPos = transform.position + m_velocity.normalized * hit.distance;

            bool destroyObject = true;


            if (m_armorPiercing)
            {
                if (Physics.SphereCast(new Ray(transform.position, m_velocity.normalized), m_collisionRadius, m_velocity.magnitude * Time.fixedDeltaTime, m_boundsLayer))
                {
                    transform.position = hitPos;
                }
                else
                {
                    transform.position += m_velocity * Time.fixedDeltaTime;
                    destroyObject = false;
                }
            }
            else
            {
                transform.position = hitPos;
            }


            HitObject(hit.transform.gameObject, hitPos, destroyObject);

        }
        else
        {
            transform.position += m_velocity * Time.fixedDeltaTime;
        }
    }


    public void HitObject(GameObject p_objectHit,Vector3 p_hitPoint,bool p_objectPool)
    {
        PerformAllCollisionScripts(p_objectHit, p_hitPoint);
        if (p_objectPool)
        {
            DestroyBullet();
        }
    }

    /// <summary>
    /// Performs all the possible collision events. IE Explosions, collision damage.
    /// </summary>
    /// <param name="p_hitObject"></param>
    /// <param name="p_hitPoint"></param>
    private void PerformAllCollisionScripts(GameObject p_hitObject, Vector3 p_hitPoint)
    {
        foreach(IProjectile_Collision newScript in m_projectileCollisionScripts)
        {
            newScript.ActivateCollision(p_hitObject, p_hitPoint, m_bulletOwnerID);
        }
    }



    /// <summary>
    /// Returns the object to the pooler
    /// </summary>
    public void DestroyBullet()
    {
        m_bulletHitEvent.Invoke();
        m_pooler.ReturnToPool(this.gameObject);

    }

    public virtual void OnDrawGizmos()
    {
        if (!m_debugGizmos) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawWireSphere(transform.position, m_collisionRadius);
        Gizmos.color = m_gizmosColor2;
        Gizmos.DrawWireSphere(transform.position + m_velocity * Time.fixedDeltaTime, m_collisionRadius);
    }
}
