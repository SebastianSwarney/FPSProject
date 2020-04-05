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
    private void Awake()
    {
        Instance = this;
        m_photonView = GetComponent<PhotonView>();
    }
    public TeamTypes.TeamType AssignTeamType()
    {
        if(m_redTeam.Count > m_blueTeam.Count)
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
        if (!m_photonView.IsMine) return;
        //Might want to change this to a json rpc call later, may depend on how efficient this is
        m_photonView.RPC("RPC_AddPlayerToTeam", RpcTarget.AllBufferedViaServer, p_playerTeamLabel.GetPhotonViewID());
        

    }
    [PunRPC]
    private void RPC_AddPlayerToTeam(int p_playerPhotonID)
    {
        TeamLabel playerLabel = PhotonView.Find(p_playerPhotonID).GetComponent<TeamLabel>();

        switch (playerLabel.m_myTeam)
        {
            case TeamTypes.TeamType.Red:
                m_redTeam.Add(playerLabel);
                return;
            case TeamTypes.TeamType.Blue:
                m_blueTeam.Add(playerLabel);
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
