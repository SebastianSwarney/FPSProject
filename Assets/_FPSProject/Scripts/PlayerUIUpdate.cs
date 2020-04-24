using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerUIUpdate : MonoBehaviour
{

    private Health m_health;
    private EquipmentController m_equipmentController;
    private PlayerUI m_playerUI;

    private PhotonView m_photonView;
    private TeamLabel m_teamLabel;

    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();


        if (!m_photonView.IsMine)
        {
            this.enabled = false;
            return;
        }
        m_equipmentController = GetComponent<EquipmentController>();
        m_teamLabel = GetComponent<TeamLabel>();
        m_health = GetComponent<Health>();
        m_playerUI = PlayerUI.Instance;
        UpdateTeamName();
    }

    private void Update()
    {
        m_playerUI.UpdatePlayerHud((int)m_health.m_currentHealth, m_equipmentController.GetAmmoAmount());
        m_playerUI.ChangeReloadingState(m_equipmentController.CheckReloading());
    }

    public void UpdateTeamName()
    {
        m_playerUI.UpdatePlayerNameAndTeam(m_photonView.Owner.NickName, m_teamLabel.m_myTeam);
    }

    public void ShowHitMarker()
    {
        m_playerUI.ShowHitMarker();
    }


    public void PlayerDeathState(bool p_state)
    {
        m_playerUI.PlayerDeathState(p_state);
    }
}
