using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EquipmentController : MonoBehaviour
{
    private TeamLabel m_teamLabel;
	private Coroutine m_aimCoroutine;
    public Transform m_weaponParent;
	public Equipment_Base m_startingWeapon;
    public Equipment_Base m_startingHolsteredWeapon;

    private Equipment_Base m_currentWeapon, m_currentHolsteredWeapon;

    public Transform m_playerCamera;

    private PhotonView m_photonView;
    private PlayerController m_playerController;

    public KeyCode m_swapKey;

    private ObjectPooler m_pooler;


    private void Awake()
    {
        m_teamLabel = GetComponent<TeamLabel>();
        m_playerController = GetComponent<PlayerController>();
        m_photonView = GetComponent<PhotonView>();
    }
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }
    public void PlayerDied()
    {
        if(m_currentWeapon != null)
        {
            m_pooler.ReturnToPool(m_currentWeapon.gameObject);
        }
        if(m_currentHolsteredWeapon != null)
        {
            m_pooler.ReturnToPool(m_currentHolsteredWeapon.gameObject);
        }
    }

    public void PlayerRespawn()
    {
        if (!m_photonView.IsMine) return;
        m_photonView.RPC("RPC_SpawnWeapon", RpcTarget.AllBuffered, m_startingWeapon.name, m_startingHolsteredWeapon.name);

    }
    [PunRPC]
    private void RPC_SpawnWeapon(string p_primaryWeapon, string p_secondaryWeapon)
    {
        if(m_pooler == null)
        {
            m_pooler = ObjectPooler.instance;
        }
        Equipment_Base newWeapon = m_pooler.NewObject(Resources.Load("Guns/" + p_primaryWeapon) as GameObject, m_weaponParent.position, m_weaponParent.rotation).GetComponent<Equipment_Base>();
        newWeapon.transform.parent = m_weaponParent;
        newWeapon.ResetEquipment();
        m_currentWeapon = newWeapon;

        newWeapon = m_pooler.NewObject(Resources.Load("Guns/" + p_secondaryWeapon) as GameObject, m_weaponParent.position, m_weaponParent.rotation).GetComponent<Equipment_Base>();
        newWeapon.transform.parent = m_weaponParent;
        newWeapon.ResetEquipment();
        m_currentHolsteredWeapon = newWeapon;

        SetupEquipment();
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
        Equipment_Base temp = m_currentWeapon;
        m_currentWeapon.PutEquipmentAway();
        m_currentWeapon.gameObject.SetActive(false);
        m_currentWeapon = m_currentHolsteredWeapon;
        m_currentWeapon.gameObject.SetActive(true);
        m_currentWeapon.SetUpEquipment(m_teamLabel.m_myTeam, this, m_photonView);
        m_currentHolsteredWeapon = temp;
    }
    public void SetupEquipment()
    {
        m_currentWeapon.SetUpEquipment(m_teamLabel.m_myTeam, this, m_photonView);

        if (m_currentHolsteredWeapon != null)
        {
            m_currentHolsteredWeapon.PutEquipmentAway();
            m_currentHolsteredWeapon.gameObject.SetActive(false);
        }
    }
    public void OnShootInputDown()
	{
        if (!m_photonView.IsMine) return;
        m_currentWeapon.OnShootInputDown(m_playerCamera);
    }
	public void OnShootInputUp()
	{
        if (!m_photonView.IsMine) return;
        m_currentWeapon.OnShootInputUp(m_playerCamera);
    }
    public void ApplyRecoilCameraRotation(float p_recoilAmountX, float p_recoilAmountY, float p_fireRate)
    {
        m_playerController.AddRecoil(p_recoilAmountX, p_recoilAmountY, p_fireRate);
    }
}
