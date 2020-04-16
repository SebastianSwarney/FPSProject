using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLabel_Hill : TeamLabel
{
    public GameObject m_neutralTeamVisual, m_redTeamVisul, m_blueTeamVisual;
    public override void SetTeamType(TeamTypes.TeamType p_newTeamType)
    {
        m_neutralTeamVisual.SetActive(false);
        m_redTeamVisul.SetActive(false);
        m_blueTeamVisual.SetActive(false);

        base.SetTeamType(p_newTeamType);

        switch (m_myTeam)
        {
            case TeamTypes.TeamType.Red:
                m_redTeamVisul.SetActive(true);
                break;
            case TeamTypes.TeamType.Blue:
                m_blueTeamVisual.SetActive(true);
                break;
        }
        
    }
}
