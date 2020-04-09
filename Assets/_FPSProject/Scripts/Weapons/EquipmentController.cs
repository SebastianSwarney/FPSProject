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

    public KeyCode m_swapKey;


    private void Awake()
    {
        m_teamLabel = GetComponent<TeamLabel>();
        m_playerController = GetComponent<PlayerController>();
        m_photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(m_swapKey))
        {
            SwapWeapons();
        }
    }

    private void SwapWeapons()
    {
        Equipment_Base temp = m_weapon;
        m_weapon.gameObject.SetActive(false);
        m_weapon.PutEquipmentAway();
        m_weapon = m_holsteredWeapon;
        m_weapon.gameObject.SetActive(true);
        m_weapon.SetUpEquipment(m_teamLabel.m_myTeam, this, m_photonView);
        m_holsteredWeapon = temp;
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

    public void ApplyRecoilCameraRotation(float p_recoilAmountX, float p_recoilAmountY, float p_fireRate)
    {
        m_playerController.AddRecoil(p_recoilAmountX, p_recoilAmountY, p_fireRate);
    }
}
