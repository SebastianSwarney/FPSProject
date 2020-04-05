using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectAtCollision : MonoBehaviour, IProjectile_Collision
{

    private ObjectPooler m_pooler;
    public GameObject m_spawnedObject;
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }
    public void ActivateCollision(GameObject p_collidedObject, Vector3 p_hitPosition)
    {
        m_pooler.NewObject(m_spawnedObject, p_hitPosition, Quaternion.identity);
    }

}
