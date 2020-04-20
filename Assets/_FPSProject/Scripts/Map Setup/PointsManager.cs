using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;
    private GameStateManager m_gameStateManager;
    private KillFeedManager m_killFeedManager;
    private PhotonView m_photonView;
    private int m_currentLeader = 0;

    public int m_pointsToWin;
    public List<TeamPoints> m_points;
    public PointsList m_pointValues;
    [System.Serializable]
    public struct PointsList
    {
        public int[] m_killPoints;
        public int m_initialHillCaptureScore;
        public int m_hillScorePerSecond;
        public int m_bombScore;
    }
    [System.Serializable]
    public class TeamPoints
    {
        public TeamTypes.TeamType m_pointTeam;
        public int m_points;
        public int m_numOfHills;
    }

    [Header("UI")]
    public TMPro.TextMeshProUGUI m_redText;
    public TMPro.TextMeshProUGUI m_blueText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_gameStateManager = GameStateManager.Instance;
        m_killFeedManager = KillFeedManager.Instance;
        m_photonView = GetComponent<PhotonView>();
    }

    private void CheckPointLead(int p_team)
    {
        if (m_currentLeader != p_team)
        {
            if (m_points[p_team].m_points > m_points[m_currentLeader].m_points)
            {
                m_currentLeader = p_team;
                m_gameStateManager.LeaderChange(TeamTypes.GetTeamFromInt(p_team + 1));
            }
        }
    }

    private void AddPoints(int p_currentTeam, int p_pointsToAdd)
    {
        m_photonView.RPC("RPC_AddPointsToClients", RpcTarget.All, p_currentTeam, p_pointsToAdd);
    }
    [PunRPC]

    private void RPC_AddPointsToClients(int p_currentTeam, int p_pointsToAdd)
    {
        m_points[p_currentTeam].m_points += p_pointsToAdd;
        CheckPointLead(p_currentTeam);

        ((p_currentTeam == 0) ? m_redText : m_blueText).text = m_points[p_currentTeam].m_points.ToString();
    }
    public void AddPointsForKill(TeamTypes.TeamType p_team)
    {
        AddPoints(TeamTypes.GetTeamAsInt(p_team) - 1, m_pointValues.m_killPoints[m_points[TeamTypes.GetTeamAsInt(p_team) - 1].m_numOfHills]);
    }

    public void AddPointsForHillCapture(TeamTypes.TeamType p_team)
    {
        AddPoints(TeamTypes.GetTeamAsInt(p_team) - 1, m_pointValues.m_initialHillCaptureScore);
        m_killFeedManager.AddMessage(p_team.ToString() + " team captured a hill!");
    }

    public void AddPointsForKeepingHill(TeamTypes.TeamType p_team)
    {
        AddPoints(TeamTypes.GetTeamAsInt(p_team) - 1, m_pointValues.m_hillScorePerSecond);
    }

    public void AddPointsForBomb(TeamTypes.TeamType p_team)
    {
        AddPoints(TeamTypes.GetTeamAsInt(p_team) - 1, m_pointValues.m_bombScore);
        m_killFeedManager.AddMessage(p_team.ToString() + " bomb detonated!");
    }

    public void FlagCaptured(TeamTypes.TeamType p_team)
    {
        m_killFeedManager.AddMessage(p_team.ToString() + " team captured flag.");
    }

    public void AddHillToTeam(int p_teamNum)
    {
        m_points[p_teamNum - 1].m_numOfHills++;
    }
    public void RemoveHillFromTeam(int p_teamNum)
    {
        if (p_teamNum == 0) return;
        m_points[p_teamNum - 1].m_numOfHills--;
    }
}
