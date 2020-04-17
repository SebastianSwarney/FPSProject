using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HeldObject_Flag : HeldObjective_Base
{
    public override bool CanPickUp(TeamLabel p_requestingPlayer)
    {
        if (p_requestingPlayer.m_myTeam != m_teamLabel.m_myTeam)
        {
            return true;
        }
        return false;
    }

    public override bool InZone(GameObject p_zone)
    {
        HeldObjectiveZone_FlagDeliever zone = p_zone.GetComponent<HeldObjectiveZone_FlagDeliever>();
        if (zone != null)
        {
            if (zone.ZoneTeam() != m_teamLabel.m_myTeam)
            {
                return true;
            }
        }
        return false;
    }

    public override void ObjectPickedUp()
    {
        base.ObjectPickedUp();
        if (m_teamLabel.m_myTeam == m_localPlayerTeamLabel.m_myTeam)
        {
            m_marker.SetActive(false);
        }
    }

    public override void ObjectDropped()
    {
        base.ObjectDropped();
        m_marker.SetActive(true);
    }
}
