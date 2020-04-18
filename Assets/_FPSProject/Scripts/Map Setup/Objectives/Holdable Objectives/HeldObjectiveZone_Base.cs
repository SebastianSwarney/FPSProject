using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class HeldObjectiveEvent: UnityEngine.Events.UnityEvent { }

[RequireComponent(typeof(TeamLabel))]
public abstract class HeldObjectiveZone_Base : MonoBehaviour
{
    public LayerMask m_playerMask;
    public Vector3 m_areaSize;
    public DebugTools m_debugTools;

    [HideInInspector]
    public TeamLabel m_teamLabel;

    [HideInInspector]
    public PhotonView m_photonView;

    public ObjectiveEvents m_objectiveEvents;
    [System.Serializable]
    public struct ObjectiveEvents
    {
        public HeldObjectiveEvent m_zoneScored;
    }

    public virtual void Awake()
    {
        m_teamLabel = GetComponent<TeamLabel>();
        m_photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        ObjectInZone();
    }
    public abstract void ObjectInZone();

    public abstract bool InRadius();

    public virtual void ZoneScored()
    {
        m_objectiveEvents.m_zoneScored.Invoke();
    }

    public abstract TeamTypes.TeamType ZoneTeam();

    private void OnDrawGizmos()
    {
        if (!m_debugTools.m_debugTools) return;
        Gizmos.matrix = transform.localToWorldMatrix;

        switch (m_teamLabel.m_myTeam)
        {
            case TeamTypes.TeamType.Neutral:
                Gizmos.color = m_debugTools.m_gizmosColor1;
                break;
            case TeamTypes.TeamType.Red:
                Gizmos.color = Color.red;
                break;
            case TeamTypes.TeamType.Blue:
                Gizmos.color = Color.blue;
                break;
        }
        
        
        switch (m_debugTools.m_gizmosType)
        {
            case DebugTools.GizmosType.Wire:
                Gizmos.DrawWireCube(Vector3.zero, m_areaSize*2);
                break;
            case DebugTools.GizmosType.Shaded:
                Gizmos.DrawCube(Vector3.zero, m_areaSize*2);
                break;
        }
    }
}
