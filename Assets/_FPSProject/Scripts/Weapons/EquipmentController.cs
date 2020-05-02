using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EquipmentController : MonoBehaviour
{
    private TeamLabel m_teamLabel;
    private Coroutine m_aimCoroutine;
    public Transform m_weaponParent;
    public Equipment_Gun m_startingWeapon;
    public Equipment_Gun m_startingHolsteredWeapon;

    private Equipment_Gun m_currentWeapon, m_currentHolsteredWeapon;

    public PlayerCamera m_camera;
    private PhotonView m_photonView;
    private PlayerController m_playerController;
    private ObjectPooler m_pooler;


    #region Holdable Objectives
    [Header("Holdable Objective")]
    public Transform m_heldObjectiveParent;
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
    }

    public void AssignLoadouts(Loadouts p_loadout)
    {
        m_startingWeapon = p_loadout.m_primaryGun;
        m_startingHolsteredWeapon = p_loadout.m_secondaryGun;
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
        if (m_currentWeapon != null)
        {
            m_currentWeapon.transform.parent = null;
            if(m_currentHolsteredWeapon != null)
            {
                m_currentHolsteredWeapon.transform.parent = null;
            }
        } 
        
        Equipment_Gun newWeapon = m_pooler.NewObject(Resources.Load("Guns/" + p_primaryWeapon) as GameObject, m_weaponParent.position, m_weaponParent.rotation).GetComponent<Equipment_Gun>();
        newWeapon.transform.parent = m_weaponParent;
        newWeapon.ResetEquipment();
        m_currentWeapon = newWeapon;
        
        newWeapon = m_pooler.NewObject(Resources.Load("Guns/" + p_secondaryWeapon) as GameObject, m_weaponParent.position, m_weaponParent.rotation).GetComponent<Equipment_Gun>();
        newWeapon.transform.parent = m_weaponParent;
        newWeapon.ResetEquipment();
        m_currentHolsteredWeapon = newWeapon;

        SetupEquipment();
    }


    [PunRPC]
    private void RPC_SwapWeapons()
    {
        Equipment_Gun temp = m_currentWeapon;
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
        m_currentWeapon.OnShootInputDown(m_weaponParent);
    }
    public void OnShootInputUp()
    {
        if (!m_photonView.IsMine) return;
        m_currentWeapon.OnShootInputUp(m_weaponParent);
    }

    public void OnReloadDown()
    {
        m_currentWeapon.ReloadDown();
    }
    public void OnEquipDown()
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

    public void OnSwapDown()
    {
        m_photonView.RPC("RPC_SwapWeapons", RpcTarget.All);
    }

    public void OnAimDown()
    {
        m_currentWeapon.ToggleZoom();
    }
    public void OnAimUp()
    {
        m_currentWeapon.ToggleZoom();
    }

    public void OnDoubleAimDown()
    {
        m_currentWeapon.ToggleDoubleZoom();
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


    #region UI Functions
    public bool CheckReloading()
    {
        if (m_currentWeapon != null)
        {

            return m_currentWeapon.m_isReloading;
        }
        return false;
    }
    public Vector3Int GetAmmoAmount()
    {
        if (m_currentWeapon == null)
        {
            return Vector3Int.zero;
        }
        return m_currentWeapon.GetAmmoAmount();
    }
    #endregion

    #region Camera Functions
    public void ShakeCamera(float p_shakeTime, float p_kickbackAmount, Vector2 p_shakeAmount, bool p_shakeRandom)
    {
        m_camera.StartShakeCamera(p_shakeTime, p_kickbackAmount, p_shakeAmount, p_shakeRandom);
    }
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

    public void ZoomCamera(bool p_isZoom, float p_newFov = 90, float p_sensitivtyMultiplier = 1)
    {
        if (!p_isZoom)
        {
            m_camera.ResetFOV();
            return;
        }
        else
        {
            m_camera.ChangeFOV(p_newFov, p_sensitivtyMultiplier);
        }
    }
    #endregion
}
