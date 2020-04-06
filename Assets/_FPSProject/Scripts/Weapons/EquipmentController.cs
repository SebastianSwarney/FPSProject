using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EquipmentController : MonoBehaviour
{
    private TeamLabel m_teamLabel;
	private Coroutine m_aimCoroutine;
	public Equipment_Base m_weapon;
    public Equipment_Base m_holsteredWeapon;
    public Transform m_playerCamera;

    private PhotonView m_photonView;
    private PlayerController m_playerController;

    private void Awake()
    {
        m_teamLabel = GetComponent<TeamLabel>();
        m_playerController = GetComponent<PlayerController>();
        m_photonView = GetComponent<PhotonView>();
    }

    public void SetupEquipment()
    {
        m_weapon.SetUpEquipment(m_teamLabel.m_myTeam, this, m_photonView);

        if (m_holsteredWeapon != null)
        {
            m_holsteredWeapon.PutEquipmentAway();
        }
    }
    public void OnShootInputDown()
	{
        if (!m_photonView.IsMine) return;
        m_weapon.OnShootInputDown(m_playerCamera);
    }


	public void OnShootInputUp()
	{
        if (!m_photonView.IsMine) return;
        m_weapon.OnShootInputUp(m_playerCamera);
        
    }




    public void ApplyRecoilCameraRotation(float p_recoilAmount)
    {
        m_playerController.RotateCameraX(p_recoilAmount);
    }
}
