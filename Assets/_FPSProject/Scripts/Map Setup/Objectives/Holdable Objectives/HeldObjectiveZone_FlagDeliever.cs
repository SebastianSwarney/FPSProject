using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class HeldObjectiveZone_FlagDeliever : HeldObjectiveZone_Base
{
    public override void ObjectInZone()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (InRadius())
        {
            m_photonView.RPC("RPC_ZoneScored", RpcTarget.All, TeamTypes.GetTeamAsInt(m_teamLabel.m_myTeam));
        }
    }

    public override bool InRadius()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, m_areaSize, transform.rotation, m_playerMask);
        foreach (Collider col in cols)
        {
            EquipmentController equiped = col.GetComponent<EquipmentController>();
            if (equiped.HeldObjectiveValid(gameObject))
            {
                m_photonView.RPC("RPC_DropObjectiveObject", RpcTarget.All, equiped.GetComponent<PhotonView>().ViewID);
                return true;
            }
        }
        return false;
    }

    [PunRPC]
    private void RPC_DropObjectiveObject(int p_photonID)
    {
        EquipmentController equiped = PhotonView.Find(p_photonID).GetComponent<EquipmentController>();
        equiped.GetHeldObjective().GetComponent<HeldObjective_Base>().ResetObjective();
        equiped.RemoveHeldObjective();
    }

    [PunRPC]
    private void RPC_ZoneScored(int p_teamNum)
    {
        ZoneScored();
        if (PhotonNetwork.IsMasterClient)
        {
            PointsManager.Instance.FlagCaptured(TeamTypes.GetTeamFromInt(p_teamNum));
        }
    }


    public override TeamTypes.TeamType ZoneTeam()
    {
        return m_teamLabel.m_myTeam;
    }
}
