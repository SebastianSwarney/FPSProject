using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class HillBehaviourEvent : UnityEngine.Events.UnityEvent<TeamTypes.TeamType> { }
public class HillBehaviour : MonoBehaviour
{

    public Vector3 m_hillDimensions;
    public LayerMask m_playerLayer;
    private TeamLabel m_teamLabel;

    public float m_hillCaptureTime;
    private float m_redTimer, m_blueTimer;
    private List<GameObject> m_redTeamInHill = new List<GameObject>(), m_blueTeamInHill = new List<GameObject>();
    private List<GameObject> m_playersInHill = new List<GameObject>();

    private PhotonView m_photonView;

    private TeamTypes.TeamType m_capturingTeam;
    private bool m_startTimer;

    public UnityEngine.UI.Image m_hillBar;


    public HillEvents m_hillEvents;
    [System.Serializable]
    public struct HillEvents
    {
        public HillBehaviourEvent m_capturedEvent;
    }

    public DebugTools m_debuggingTools;

    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        m_teamLabel = GetComponent<TeamLabel>();

    }
    private void Update()
    {
        CheckRadius();
        if (PhotonNetwork.IsMasterClient)
        {
            //If there is someone in the hill
            if (m_redTeamInHill.Count > 0 || m_blueTeamInHill.Count > 0)
            {
                UpdateHill();
            }
            //If no one is in the hill
            else
            {
                //If the hill is neutral, and no one is in it, reset both timers
                if (m_teamLabel.m_myTeam == TeamTypes.TeamType.Neutral)
                {
                    if (m_redTimer > 0 || m_blueTimer > 0)
                    {
                        ResetTimers();
                    }
                }

                if (m_startTimer)
                {
                    m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, 0, false);
                }
            }
        }
        if (m_startTimer)
        {
            PerformHillTimers();
        }
        UpdateUI();
    }

    private void UpdateHill()
    {
        switch (m_teamLabel.m_myTeam)
        {

            #region Hill is neutral
            case TeamTypes.TeamType.Neutral:
                if (m_redTeamInHill.Count == 0)
                {
                    if (!m_startTimer)
                    {
                        m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, 2, true);
                    }
                }
                else if (m_blueTeamInHill.Count == 0)
                {
                    if (!m_startTimer)
                    {
                        m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, 1, true);
                    }
                }
                else
                {
                    if (m_startTimer)
                    {
                        m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, 1, false);
                    }
                }
                break;
            #endregion

            #region Hill Is Red
            case TeamTypes.TeamType.Red:
                if (m_redTeamInHill.Count == 0)
                {
                    if (!m_startTimer)
                    {
                        m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, 2, true);
                    }
                }
                else if (m_blueTeamInHill.Count == 0)
                {
                    if (m_blueTimer > 0)
                    {
                        ResetTimers();
                    }


                }
                break;
            #endregion

            #region Hill Is Blue

            case TeamTypes.TeamType.Blue:
                if (m_redTeamInHill.Count == 0)
                {
                    if (m_redTimer > 0)
                    {
                        ResetTimers();
                    }
                }
                else if (m_blueTeamInHill.Count == 0)
                {
                    if (!m_startTimer)
                    {
                        m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, 1, true);
                    }
                }
                break;
                #endregion
        }
    }

    private void PerformHillTimers()
    {
        switch (m_capturingTeam)
        {
            case TeamTypes.TeamType.Red:
                AddTimeToHill(TeamTypes.TeamType.Red, ref m_redTimer);
                break;
            case TeamTypes.TeamType.Blue:
                AddTimeToHill(TeamTypes.TeamType.Blue, ref m_blueTimer);
                break;
        }
    }

    private void ResetTimers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            m_photonView.RPC("RPC_ResetTimers", RpcTarget.All);
            m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, 0, false);
        }
    }
    [PunRPC]
    private void RPC_ResetTimers()
    {
        m_blueTimer = 0;
        m_redTimer = 0;
    }

    private void AddTimeToHill(TeamTypes.TeamType p_capturingTeam, ref float p_timer)
    {
        p_timer += Time.deltaTime;

        //Only the master client can actually change the hill
        if (PhotonNetwork.IsMasterClient)
        {
            if (p_timer > m_hillCaptureTime)
            {
                SwitchHillTeam(p_capturingTeam);
            }
        }
    }

    private void SwitchHillTeam(TeamTypes.TeamType p_newHillTeam)
    {
        ResetTimers();
        switch (p_newHillTeam)
        {
            case TeamTypes.TeamType.Red:
                m_photonView.RPC("RPC_SwitchHill", RpcTarget.All, 1);
                break;
            case TeamTypes.TeamType.Blue:
                m_photonView.RPC("RPC_SwitchHill", RpcTarget.All, 2);
                break;
        }
    }

    [PunRPC]
    private void RPC_SwitchHill(int p_newTeam)
    {
        if (p_newTeam == 1)
        {
            m_teamLabel.SetTeamType(TeamTypes.TeamType.Red);
            m_hillEvents.m_capturedEvent.Invoke(TeamTypes.TeamType.Red);
        }
        else if (p_newTeam == 2)
        {
            m_teamLabel.SetTeamType(TeamTypes.TeamType.Blue);
            m_hillEvents.m_capturedEvent.Invoke(TeamTypes.TeamType.Blue);
        }

    }

    [PunRPC]
    private void RPC_ToggleTimer(int p_attackingTeam, bool p_startTimer)
    {
        if (p_attackingTeam == 1)
        {
            m_capturingTeam = TeamTypes.TeamType.Red;
        }
        else if (p_attackingTeam == 2)
        {
            m_capturingTeam = TeamTypes.TeamType.Blue;
        }

        m_startTimer = p_startTimer;
    }

    private void UpdateUI()
    {
        m_hillBar.fillAmount = ((m_capturingTeam == TeamTypes.TeamType.Red) ? m_redTimer : m_blueTimer) / m_hillCaptureTime;
    }

    private void CheckRadius()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, m_hillDimensions / 2, transform.rotation, m_playerLayer);
        List<GameObject> playersLeft = new List<GameObject>(), playersCurrentlyInHill = new List<GameObject>();

        RemoveNullsFromList(ref m_playersInHill);
        RemoveNullsFromList(ref m_redTeamInHill);
        RemoveNullsFromList(ref m_blueTeamInHill);

        foreach (Collider col in cols)
        {
            if (col.enabled)
            {
                if (col.GetComponent<Health>().m_isDead) continue;
                playersCurrentlyInHill.Add(col.gameObject);

                if (!m_playersInHill.Contains(col.gameObject))
                {
                    m_playersInHill.Add(col.gameObject);

                    TeamLabel team = col.GetComponent<TeamLabel>();
                    switch (team.m_myTeam)
                    {
                        case TeamTypes.TeamType.Red:
                            m_redTeamInHill.Add(col.gameObject);
                            break;
                        case TeamTypes.TeamType.Blue:
                            m_blueTeamInHill.Add(col.gameObject);
                            break;
                    }
                }
            }
        }
        foreach (GameObject checkPlayer in m_playersInHill)
        {
            if (!playersCurrentlyInHill.Contains(checkPlayer))
            {
                playersLeft.Add(checkPlayer);
            }
        }
        foreach (GameObject removePlayer in playersLeft)
        {
            switch (removePlayer.GetComponent<TeamLabel>().m_myTeam)
            {
                case TeamTypes.TeamType.Red:
                    m_redTeamInHill.Remove(removePlayer);
                    break;
                case TeamTypes.TeamType.Blue:
                    m_blueTeamInHill.Remove(removePlayer);
                    break;
            }
            m_playersInHill.Remove(removePlayer);
        }


    }
    private void RemoveNullsFromList(ref List<GameObject> checkNull)
    {
        List<int> nullList = new List<int>();
        for (int i = 0; i < checkNull.Count; i++)
        {
            if (checkNull[i] == null)
            {
                nullList.Add(i);
            }
        }
        int removedIndex = 0;
        foreach (int newNull in nullList)
        {
            checkNull.RemoveAt(newNull - removedIndex);
            removedIndex += 1;
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_debuggingTools.m_debugTools) return;
        Gizmos.color = m_debuggingTools.m_gizmosColor1;
        Gizmos.matrix = transform.localToWorldMatrix;
        switch (m_debuggingTools.m_gizmosType)
        {
            case DebugTools.GizmosType.Wire:
                Gizmos.DrawWireCube(Vector3.zero, m_hillDimensions);
                break;
            case DebugTools.GizmosType.Shaded:
                Gizmos.DrawCube(Vector3.zero, m_hillDimensions);
                break;
        }
    }
}
