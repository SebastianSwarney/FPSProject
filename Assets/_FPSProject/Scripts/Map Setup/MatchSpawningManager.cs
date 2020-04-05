﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSpawningManager : MonoBehaviour
{
    public static MatchSpawningManager Instance;

    public SpawnZoneTypes[] m_spawnZoneType;
    [System.Serializable]
    public struct SpawnZoneTypes
    {
        public TeamTypes.TeamType m_zoneType;
        public List<MatchSpawn_Zone> m_teamSpawnZones;
    }
    private TeamLabel m_checkingLabel;

    private void Awake()
    {
        Instance = this;
    }
    public Transform SpawnPlayer(TeamTypes.TeamType p_spawnedPlayerTeam)
    {
        

        foreach(SpawnZoneTypes spawnZone in m_spawnZoneType)
        {
            if(spawnZone.m_zoneType == p_spawnedPlayerTeam)
            {
                return GetMostRelevantSpawn(spawnZone);
            }
        }
        Debug.Log("There was no possible spawn whatsoever. If this is the case, contact me.");
        return null;
    }

    public Transform GetMostRelevantSpawn(SpawnZoneTypes p_checkingSpawn)
    {
        MatchSpawn_Zone checkingSpawn = null;
        foreach(MatchSpawn_Zone check in p_checkingSpawn.m_teamSpawnZones)
        {
            if (!check.CanSpawnIn()) continue;
            if (checkingSpawn == null)
            {
                checkingSpawn = check;
            }else if (check.m_zonePriority > checkingSpawn.m_zonePriority)
            {
                checkingSpawn = check;
            }
        }

        return checkingSpawn.GiveSpawnPoint();
    }
    
}
