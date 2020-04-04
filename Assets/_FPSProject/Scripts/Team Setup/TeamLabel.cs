using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeamLabelEvent : UnityEngine.Events.UnityEvent<TeamTypes.TeamType> { }
public class TeamLabel : MonoBehaviour, ITeam
{
    public TeamTypes.TeamType m_myTeam;
    public TeamLabelEvent m_swappedTeams;
    public bool m_disableSwappingTeam;

    public TeamTypes.TeamType GetTeamType()
    {
        return m_myTeam;
    }

    public void SetTeamType(TeamTypes.TeamType p_newTeamType)
    {
        m_swappedTeams.Invoke(p_newTeamType);
        if (!m_disableSwappingTeam) return;
        m_myTeam = p_newTeamType;
        
    }
}
