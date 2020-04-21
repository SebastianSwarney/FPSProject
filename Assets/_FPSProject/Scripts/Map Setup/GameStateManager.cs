using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class GameStateManagerEvent : UnityEngine.Events.UnityEvent <TeamTypes.TeamType>{ }
public class GameStateManager : MonoBehaviour
{

    public static GameStateManager Instance;
    [System.Serializable]
    public struct StateEvents
    {
        public GameStateManagerEvent m_teamInTheLead;
        public GameStateManagerEvent m_teamWon;
    }

    public StateEvents m_events;

    private PhotonView m_photonView;

    


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
    }
    public void GameWon(TeamTypes.TeamType p_team)
    {
        m_events.m_teamWon.Invoke(p_team);

        if(TeamLabel_Player.LocalPlayer.m_myTeam == p_team)
        {
            KillFeedManager.Instance.AddMessage("Your team won!");
        }
        else
        {
            KillFeedManager.Instance.AddMessage("You guys fucked up.");
        }
    }

    public void LeaderChange(TeamTypes.TeamType p_team)
    {
        m_events.m_teamInTheLead.Invoke(p_team);
    }

    public void ChangeHill(TeamTypes.TeamType p_lastTeam, TeamTypes.TeamType p_newTeam)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        m_photonView.RPC("RPC_ChangeHill", RpcTarget.All, TeamTypes.GetTeamAsInt(p_lastTeam), TeamTypes.GetTeamAsInt(p_newTeam));
    }

    [PunRPC]
    private void RPC_ChangeHill(int p_lastTeam, int p_newTeam)
    {
        PointsManager.Instance.RemoveHillFromTeam(p_lastTeam);
        PointsManager.Instance.AddHillToTeam(p_newTeam);
    }

    
}
