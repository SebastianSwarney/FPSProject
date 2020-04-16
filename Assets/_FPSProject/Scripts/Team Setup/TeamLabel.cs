using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class TeamLabelEvent : UnityEngine.Events.UnityEvent<TeamTypes.TeamType> { }

public class TeamLabel : MonoBehaviour
{
    public TeamTypes.TeamType m_myTeam;
    public TeamLabelEvent m_swappedTeams;
    
    [HideInInspector]
    public PhotonView m_photonView;

    private void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
    }
    public TeamTypes.TeamType GetTeamType()
    {
        return m_myTeam;
    }

    public virtual void SetTeamType(TeamTypes.TeamType p_newTeamType)
    {
        m_swappedTeams.Invoke(p_newTeamType);
        m_myTeam = p_newTeamType;
    }
     

    public int GetPhotonViewID()
    {
        return m_photonView.ViewID;
    }
    
}
