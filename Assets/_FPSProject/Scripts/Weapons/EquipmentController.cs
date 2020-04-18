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

    #region Holdable Objectives
    [Header("Holdable Objective")]
    public Transform m_heldObjectiveParent;
    public KeyCode m_pickupObjectiveKey;
    public LayerMask m_objectiveMask;
    public float m_objectiveRadius;
    public DebugTools m_debuggingTools;

    private bool m_holdingObjective;
    private HeldObjective_Base m_heldObjective;

    #endregion


    private void Awake()
    {
        m_teamLabel = GetComponent<TeamLabel>();
        m_playerController = GetComponent<PlayerController>();
        m_photonView = GetComponent<PhotonView>();
    }
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
        if (m_photonView.IsMine)
        {
            StartCoroutine(PerformControls());
        }
    }

    private IEnumerator PerformControls()
    {
        while (true)
        {
            if (Input.GetKeyDown(m_swapKey))
            {
                SwapWeapons();
            }
            if (Input.GetKeyDown(m_pickupObjectiveKey))
            {
                if (m_heldObjective == null)
                {
                    CheckForObjective();
                }
                else
                {
                    DropHeldObject();
                }
            }
            yield return null;
        }
    }
    public void PlayerDied()
    {
        if (m_currentWeapon != null)
        {
            m_pooler.ReturnToPool(m_currentWeapon.gameObject);
        }
        if (m_currentHolsteredWeapon != null)
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
        if (m_pooler == null)
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



    #region Holdable Objectives

    private void CheckForObjective()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, m_objectiveRadius, m_objectiveMask);
        if (cols.Length > 0)
        {
            if (cols[0].GetComponent<HeldObjective_Base>().CanPickUp(m_teamLabel))
            {
                m_photonView.RPC("RPC_CheckEquipmentOnMaster", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC]
    private void RPC_CheckEquipmentOnMaster()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, m_objectiveRadius, m_objectiveMask);
        if (cols.Length > 0)
        {
            int hitID = cols[0].GetComponent<PhotonView>().ViewID;
            RPC_HoldEquipment(hitID);
            m_photonView.RPC("RPC_HoldEquipment", RpcTarget.Others, hitID);
        }
    }

    [PunRPC]
    private void RPC_HoldEquipment(int p_objectiveID)
    {
        HeldObjective_Base objective = PhotonView.Find(p_objectiveID).GetComponent<HeldObjective_Base>();
        m_heldObjective = objective;
        objective.ObjectPickedUp();
        objective.transform.parent = m_heldObjectiveParent;
        objective.transform.localPosition = Vector3.zero;
        objective.transform.localRotation = Quaternion.identity;
    }

    public GameObject GetHeldObjective()
    {
        if (m_heldObjective != null)
        {
            return m_heldObjective.gameObject;
        }
        return null; 
    }
    public bool HeldObjectiveValid(GameObject p_objectiveZone)
    {
        if (m_heldObjective != null)
        {
            return m_heldObjective.InZone(p_objectiveZone);
        }
        return false;
    }

    public void RemoveHeldObjective()
    {
        if (m_heldObjective != null)
        {
            m_heldObjective.ObjectDropped();
            m_heldObjective = null;
        }
    }

    private void DropHeldObject()
    {
        m_photonView.RPC("RPC_DropObjective", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_DropObjective()
    {
        m_heldObjective.ObjectDropped();
        m_heldObjective = null;
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (!m_debuggingTools.m_debugTools) return;
        Gizmos.color = m_debuggingTools.m_gizmosColor1;

        switch (m_debuggingTools.m_gizmosType)
        {
            case DebugTools.GizmosType.Shaded:
                Gizmos.DrawSphere(transform.position, m_objectiveRadius);
                break;
            case DebugTools.GizmosType.Wire:
                Gizmos.DrawWireSphere(transform.position, m_objectiveRadius);
                break;
        }
    }
}
