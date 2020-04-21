using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team_Barrier : MonoBehaviour
{

    private TeamLabel m_localTeamLabel, m_teamLabel;
    public Collider[] m_barrierColliders;

    private bool m_onSameTeam;
    private bool m_active = true;
    private void Start()
    {
        
        m_teamLabel = GetComponent<TeamLabel>();
        StartCoroutine(GetLocalPlayer());
    }
    private IEnumerator GetLocalPlayer()
    {
        yield return new WaitForSeconds(.01f);
        m_localTeamLabel = TeamLabel_Player.LocalPlayer.GetComponent<TeamLabel>();
    }
    private void Update()
    {
        if (m_localTeamLabel == null) return;
        if (m_active)
        {
            if (m_onSameTeam)
            {
                if (m_localTeamLabel.m_myTeam != m_teamLabel.m_myTeam)
                {
                    print("enable Colliders");
                    m_onSameTeam = false;
                    ChangeCollidersState(true);
                }
            }
            else
            {
                if (m_localTeamLabel.m_myTeam == m_teamLabel.m_myTeam)
                {
                    print("Disable Colliders");
                    m_onSameTeam = true;
                    ChangeCollidersState(false);
                }
            }
        }
    }
    private void ChangeCollidersState(bool p_activeState)
    {
        foreach(Collider col in m_barrierColliders)
        {
            col.enabled = p_activeState;
        }
    }
    public void ChangeBarrierState(bool p_activeState)
    {
        m_active = p_activeState;
        foreach(Collider col in m_barrierColliders)
        {
            col.gameObject.SetActive(p_activeState);
        }
    }



}
