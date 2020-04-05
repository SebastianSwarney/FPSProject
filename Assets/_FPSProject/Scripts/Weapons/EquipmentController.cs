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
    private void Start()
    {
        m_teamLabel = GetComponent<TeamLabel>();
        m_weapon.SetUpEquipment(m_teamLabel.m_myTeam);
        m_photonView = GetComponent<PhotonView>();
        if (m_holsteredWeapon != null)
        {
            m_holsteredWeapon.PutEquipmentAway();
        }
    }
    public void OnShootInputDown()
	{
        if (!m_photonView.IsMine) return;
        m_photonView.RPC("RPC_FireInputDown", RpcTarget.All);
	}
    [PunRPC]
    private void RPC_FireInputDown()
    {
        m_weapon.OnShootInputDown(m_playerCamera);
    }

	public void OnShootInputUp()
	{
        if (!m_photonView.IsMine) return;
        m_photonView.RPC("RPC_InputUp", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_InputUp()
    {
        m_weapon.OnShootInputUp(m_playerCamera);
    }
}
