using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticle : MonoBehaviour
{
    public GameObject m_spawnedParticle;
    private ObjectPooler m_pooler;
    public Vector3 m_particleOffset;

    [Header("Match Transform")]
    public Transform m_matchedTransform;
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }

    /// <summary>
    /// Spawns an unparented particle object at the p_position
    /// </summary>
    /// <param name="p_positon"></param>
    public void SpawnParticlePrefab(Vector3 p_positon)
    {
        m_pooler.NewObject(m_spawnedParticle, p_positon, Quaternion.identity);
    }
    /// <summary>
    /// Spawns an unparented particle object at this objects postion.
    /// </summary>
    public void SpawnParticlePrefab()
    {
        m_pooler.NewObject(m_spawnedParticle, transform.position + m_particleOffset, Quaternion.identity);
    }

    /// <summary>
    /// Spawns an unparented particle object at the transform, matching it's rotation
    /// </summary>
    public void SpawnParticleWithRotation()
    {
        m_pooler.NewObject(m_spawnedParticle, m_matchedTransform.position, m_matchedTransform.rotation);
    }

    /// <summary>
    /// Spawns a line renderer, and requires a starting position, and ending position.
    /// </summary>
    /// <param name="p_startingPos"></param>
    /// <param name="p_endingPos"></param>
    public void SpawnParticleLineRenderer(Vector3 p_startingPos, Vector3 p_endingPos)
    {
        LineRenderer newLine = m_pooler.NewObject(m_spawnedParticle, p_startingPos, Quaternion.identity).GetComponent<LineRenderer>();

        newLine.SetPosition(0, p_startingPos);
        newLine.SetPosition(1, p_endingPos);
    }

    [Header("Debugging")]
    public bool m_isDebugging;
    public Color m_gizmosColor1;
    private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawSphere(transform.position + m_particleOffset, .5f);
    }
}