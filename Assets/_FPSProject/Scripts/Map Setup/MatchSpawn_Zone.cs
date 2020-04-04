using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSpawn_Zone : MonoBehaviour, ITeam
{

    public TeamTypes.TeamType m_zoneTeam;
    public enum ZoneOccupancy { Controlled, Neutral}
    public ZoneOccupancy m_currentZoneStatus;
    public List<Transform> m_spawnPoints;

    [Tooltip("The higher the priority, the most likely the player will spawn in this zone.")]
    public int m_zonePriority;


    private List<Transform> m_randomSpawnPoint;
    public LayerMask m_playerMask;

    public float m_spawnRadiusCheck;
    

    private void Start()
    {
        m_randomSpawnPoint = new List<Transform>(m_spawnPoints);
    }
    public Transform GiveSpawnPoint()
    {
        RandomizeSpawnList();
        foreach(Transform spawn in m_randomSpawnPoint)
        {
            if (!IsPlayerClose(spawn))
            {
                return spawn;
            }
        }
        return m_randomSpawnPoint[Random.Range(0, m_randomSpawnPoint.Count)];
    }

    /// <summary>
    /// Randomizes the spawning list, so that the possibility of spawning in the same spot over and over is reduced
    /// </summary>
    private void RandomizeSpawnList()
    {
        for (int i = 0; i < m_spawnPoints.Count; i++)
        {
            Transform tempTrans = m_randomSpawnPoint[i];
            int randomIndex = Random.Range(0, m_spawnPoints.Count);
            m_randomSpawnPoint[i] = m_randomSpawnPoint[randomIndex];
            m_randomSpawnPoint[randomIndex] = tempTrans;
        }
    }

    private bool IsPlayerClose(Transform p_newPoint)
    {
        return (Physics.SphereCast(new Ray(p_newPoint.position, Vector3.forward), m_spawnRadiusCheck, 0, m_playerMask));
    }

    public TeamTypes.TeamType GetTeamType()
    {
        return m_zoneTeam;
    }

    public void SetTeamType(TeamTypes.TeamType p_newTeamType)
    {
    }
}
