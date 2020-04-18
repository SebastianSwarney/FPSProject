using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class KillZone : MonoBehaviour
{

    public LayerMask m_objectsWithHealth;
    public LayerMask m_objectiveObjects;
    public Vector3 m_size;

    public DebugTools m_debugging;
    Collider[] cols;

    public float m_killzoneTickTime = .25f;

    private void Start()
    {
        StartCoroutine(KillZoneCheck());
    }

    private Collider[] CheckRadius(LayerMask p_detectingLayer)
    {
        return Physics.OverlapBox(transform.position, m_size, transform.rotation, p_detectingLayer);
    }

    private IEnumerator KillZoneCheck()
    {
        float timer = 0;
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (timer >= m_killzoneTickTime)
                {
                    cols = CheckRadius(m_objectsWithHealth);
                    foreach (Collider col in cols)
                    {
                        col.GetComponent<Health>().KillZoneCollision();
                    }
                    cols = CheckRadius(m_objectiveObjects);
                    foreach (Collider col in cols)
                    {
                        col.GetComponent<HeldObjective_Base>().CallResetObjective();
                    }
                    timer = 0;
                }
                timer += Time.deltaTime;
            }
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_debugging.m_debugTools) return;
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, m_size*2);
    }
}
