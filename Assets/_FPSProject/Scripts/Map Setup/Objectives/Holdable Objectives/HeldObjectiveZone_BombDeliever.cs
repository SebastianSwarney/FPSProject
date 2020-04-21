using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HeldObjectiveZone_BombDeliever : HeldObjectiveZone_Base
{
    public float m_bombPlantTime;
    private float m_plantingTimer;

    private bool m_destroyed;
    public float m_resetTime;
    private float m_resetTimer;

    private bool m_canBeTaken;
    private bool m_startTimer;

    public HeldObjectiveEvent m_defensesRestored;
    private EquipmentController m_equipmentController;


    public UnityEngine.UI.Image m_bombPlantUI;
    private void OnEnable()
    {
        m_startTimer = false;
        m_plantingTimer = 0;
    }

    public override void Update()
    {
        if (!m_destroyed)
        {
            base.Update();
        }
        else
        {
            if (m_resetTimer > m_resetTime)
            {
                ZoneReset();
            }
            m_resetTimer += Time.deltaTime;
        }
    }

    public void ZoneReset()
    {
        m_destroyed = false;
        m_defensesRestored.Invoke();
        m_resetTimer = 0;
    }

    public override void ObjectInZone()
    {
        if (m_startTimer)
        {
            m_plantingTimer += Time.deltaTime;
        }
        UpdateUI();
        if (!PhotonNetwork.IsMasterClient) return;
        if (m_startTimer != InRadius())
        {
            m_photonView.RPC("RPC_ToggleTimer", RpcTarget.All, !m_startTimer);
        }
        if (m_plantingTimer > m_bombPlantTime)
        {
            m_photonView.RPC("RPC_ZoneScored", RpcTarget.All, TeamTypes.GetTeamAsInt(m_equipmentController.GetComponent<TeamLabel>().m_myTeam), m_equipmentController.GetComponent<PhotonView>().ViewID);
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
            Collider[] cols = Physics.OverlapBox(transform.position, m_areaSize, transform.rotation, m_playerMask);
            foreach (Collider col in cols)
            {
                if (col.gameObject == m_equipmentController.gameObject)
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
        m_destroyed = true;
        m_plantingTimer = 0;
    }


    private void DropObjectiveObject(int p_bombHolderID)
    {

        EquipmentController equiped = PhotonView.Find(p_bombHolderID).GetComponent<EquipmentController>();
        HeldObjective_Base heldObject = equiped.GetHeldObjective().GetComponent<HeldObjective_Base>();
        equiped.RemoveHeldObjective();
        heldObject.ResetObjective();
    }


    private void UpdateUI()
    {
        m_bombPlantUI.fillAmount = m_plantingTimer / m_bombPlantTime;
    }
    [PunRPC]
    private void RPC_ZoneScored(int p_scoredTeam, int p_bombHolderID)
    {
        DropObjectiveObject(p_bombHolderID);
        ZoneScored();
        



        if (PhotonNetwork.IsMasterClient)
        {
            PointsManager.Instance.AddPointsForBomb(TeamTypes.GetTeamFromInt(p_scoredTeam));
        }
    }

    [PunRPC]
    private void RPC_ToggleTimer(bool p_state)
    {
        m_startTimer = p_state;
        if (!p_state)
        {
            m_plantingTimer = 0;
        }
    }


    public void ChangeZoneState(bool p_newState)
    {
        m_canBeTaken = p_newState;
    }
}
