using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HeldObjectiveZone_BombDeliever : HeldObjectiveZone_Base
{
    public float m_bombPlantTime;
    private bool m_inRadius;
    private bool m_startTimer;
    private float m_timer;

    private EquipmentController m_equipmentController;


    public UnityEngine.UI.Image m_bombPlantUI;
    private void OnEnable()
    {
        m_startTimer = false;
        m_inRadius = false;
        m_timer = 0;
    }
    public override void ObjectInZone()
    {
        if (m_startTimer)
        {
            m_timer += Time.deltaTime;
        }
        UpdateUI();
        if (!PhotonNetwork.IsMasterClient) return;
        if (m_startTimer != InRadius())
        {
            m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, !m_startTimer);
        }
        if (m_timer > m_bombPlantTime)
        {
            m_photonView.RPC("RPC_ZoneScored", RpcTarget.All, TeamTypes.GetTeamAsInt(m_equipmentController.GetComponent<TeamLabel>().m_myTeam),m_equipmentController.GetComponent<PhotonView>().ViewID);
        }
    }
    public override bool InRadius()
    {
        if (m_equipmentController == null)
        {
            Collider[] cols = Physics.OverlapBox(transform.position, m_areaSize, transform.rotation, m_playerMask);
            foreach (Collider col in cols)
            {
                EquipmentController equiped = col.GetComponent<EquipmentController>();
                if (equiped.HeldObjectiveValid(gameObject))
                {
                    m_equipmentController = equiped;
                    return true;
                }
            }
            return false;
        }
        else
        {
            Collider[] cols = Physics.OverlapBox(transform.position, m_areaSize , transform.rotation, m_playerMask);
            foreach (Collider col in cols)
            {
                if(col.gameObject == m_equipmentController.gameObject)
                {
                    if (m_equipmentController.GetHeldObjective() != null)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }

    public override TeamTypes.TeamType ZoneTeam()
    {
        return m_teamLabel.m_myTeam;
    }


    public override void ZoneScored()
    {
        base.ZoneScored();
        m_startTimer = false;
        m_timer = 0;
        m_inRadius = false;
    }


    private void DropObjectiveObject(int p_bombHolderID)
    {

        EquipmentController equiped = PhotonView.Find(p_bombHolderID).GetComponent<EquipmentController>();
        equiped.GetHeldObjective().GetComponent<HeldObjective_Base>().ResetObjective();
        equiped.RemoveHeldObjective();
    }


    private void UpdateUI()
    {
        m_bombPlantUI.fillAmount = m_timer / m_bombPlantTime;
    }
    [PunRPC]
    private void RPC_ZoneScored(int p_scoredTeam,int p_bombHolderID)
    {
        ZoneScored();

        KillFeedManager.Instance.AddMessage(PhotonView.Find(p_bombHolderID).GetComponent<TeamLabel>().m_myTeam.ToString() + " bomb detonated!");

        DropObjectiveObject(p_bombHolderID);

        

        if (PhotonNetwork.IsMasterClient)
        {
            GameMode_CaptureTheFlag.Instance.AddPoints(p_scoredTeam);
        }
    }

    [PunRPC]
    private void RPC_ToggleTimer(bool p_state)
    {
        m_startTimer = p_state;
    }



}
