using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;

    public List<TeamLabel> m_redTeam;
    public List<TeamLabel> m_blueTeam;
    private PhotonView m_photonView;

    private bool m_offlineMode;
    private TeamTypes.TeamType m_offlineModeTeam;
    private void Awake()
    {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }

    public void AssignOfflineModeTeam(TeamTypes.TeamType p_team)
    {
        m_offlineMode = true;
        m_offlineModeTeam = p_team;
    }
    public TeamTypes.TeamType AssignTeamType()
    {
        if (m_offlineMode)
        {
            return m_offlineModeTeam;
        }

        if (m_redTeam.Count > m_blueTeam.Count)
        {
            return TeamTypes.TeamType.Blue;
        }
        else
        {
            return TeamTypes.TeamType.Red;
        }
    }


    public void AddPlayerToTeam(TeamLabel p_playerTeamLabel)
    {
        switch (p_playerTeamLabel.m_myTeam)
        {
            case TeamTypes.TeamType.Red:
                m_redTeam.Add(p_playerTeamLabel);
                return;
            case TeamTypes.TeamType.Blue:
                m_blueTeam.Add(p_playerTeamLabel);
                return;
        }
    }

    public void RemovePlayerFromTeam(TeamLabel p_playerTeamLabel)
    {
        switch (p_playerTeamLabel.m_myTeam)
        {
            case TeamTypes.TeamType.Red:
                if (m_redTeam.Contains(p_playerTeamLabel))
                {
                    m_redTeam.Remove(p_playerTeamLabel);
                }
                return;
            case TeamTypes.TeamType.Blue:
                if (m_blueTeam.Contains(p_playerTeamLabel))
                {
                    m_blueTeam.Remove(p_playerTeamLabel);
                }
                return;
        }
    }

    public void SwapPlayerTeam(TeamLabel p_playerTeamLabel)
    {
        if (!m_photonView.IsMine) return;
        m_photonView.RPC("RPC_SwapPlayerTeam", RpcTarget.AllBufferedViaServer, p_playerTeamLabel);
    }

    [PunRPC]
    private void RPC_SwapPlayerTeam(TeamLabel p_playerTeamLabel)
    {
        switch (p_playerTeamLabel.m_myTeam)
        {
            case TeamTypes.TeamType.Blue:
                m_blueTeam.Remove(p_playerTeamLabel);
                m_redTeam.Add(p_playerTeamLabel);
                return;
            case TeamTypes.TeamType.Red:
                m_redTeam.Remove(p_playerTeamLabel);
                m_blueTeam.Add(p_playerTeamLabel);
                return;
        }
    }
}
