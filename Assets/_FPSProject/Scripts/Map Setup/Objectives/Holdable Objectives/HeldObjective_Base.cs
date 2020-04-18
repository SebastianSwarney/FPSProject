using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[RequireComponent(typeof(TeamLabel))]
[RequireComponent(typeof(PhotonView))]
public abstract class HeldObjective_Base : MonoBehaviour
{
    public float m_resetTime;
    public GameObject m_marker;
    [HideInInspector]
    public TeamLabel m_teamLabel;
    [HideInInspector]
    public TeamLabel m_localPlayerTeamLabel;

    private Vector3 m_startingOffset;
    private Transform m_startingParent;
    private Rigidbody m_rb;
    private PhotonView m_photonView;

    private float m_resetTimer;
    private bool m_startTimer;

    public virtual void Start()
    {
        m_startingParent = transform.parent;
        m_startingOffset = transform.localPosition;
        m_rb = GetComponent<Rigidbody>();
        m_teamLabel = GetComponent<TeamLabel>();
        m_localPlayerTeamLabel = PhotonPlayer.Instance.GetLocalPlayer().GetComponent<TeamLabel>();
        m_photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (m_startTimer)
        {
            m_resetTimer += Time.deltaTime;
            if(m_resetTimer >= m_resetTime)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    CallResetObjective();
                }
            }
        }
    }
    public abstract bool InZone(GameObject p_zone);

    public abstract bool CanPickUp(TeamLabel p_requestingPlayer);

    public virtual void ObjectPickedUp()
    {
        m_rb.isKinematic = true;
        m_startTimer = false;
        m_resetTimer = 0;
    }
    public virtual void ObjectDropped()
    {
        transform.rotation = Quaternion.identity;
        m_rb.isKinematic = false;
        transform.parent = null;
        m_startTimer = true;
    }


    public virtual void ResetObjective()
    {
        m_startTimer = false; 
        transform.parent = m_startingParent;
        transform.localPosition = m_startingOffset;
        transform.localRotation = Quaternion.identity;
    }

    
    public void CallResetObjective()
    {
        m_photonView.RPC("RPC_ResetObjective", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_ResetObjective()
    {
        ResetObjective();
    }
}
