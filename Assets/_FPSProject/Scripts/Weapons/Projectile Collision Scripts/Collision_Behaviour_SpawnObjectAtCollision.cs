using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Behaviour_SpawnObjectAtCollision : MonoBehaviour, IProjectile_Collision
{

    private ObjectPooler m_pooler;
    public GameObject m_spawnedObject;
    public GameObject m_spawnedObject2;
    private TeamLabel m_teamLabel;
    private void Start()
    {
        m_teamLabel = GetComponent<TeamLabel>();
        m_pooler = ObjectPooler.instance;
    }
    public void ActivateCollision(GameObject p_collidedObject, Vector3 p_hitPosition, int p_bulletOwnerPhotonID)
    {
        m_pooler.NewObject((m_teamLabel.m_myTeam == TeamTypes.TeamType.Blue)? m_spawnedObject:m_spawnedObject2, p_hitPosition, Quaternion.identity);
    }


}
