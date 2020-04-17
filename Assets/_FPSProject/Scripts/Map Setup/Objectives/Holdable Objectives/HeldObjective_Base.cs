using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[RequireComponent(typeof(TeamLabel))]
[RequireComponent(typeof(PhotonView))]
public abstract class HeldObjective_Base : MonoBehaviour
{
    public GameObject m_marker;
    [HideInInspector]
    public TeamLabel m_teamLabel;
    [HideInInspector]
    public TeamLabel m_localPlayerTeamLabel;

    private Vector3 m_startingOffset;
    private Transform m_startingParent;
    private Rigidbody m_rb;

    public virtual void Start()
    {
        m_startingParent = transform.parent;
        m_startingOffset = transform.localPosition;
        m_rb = GetComponent<Rigidbody>();
        m_teamLabel = GetComponent<TeamLabel>();
        m_localPlayerTeamLabel = PhotonPlayer.Instance.GetLocalPlayer().GetComponent<TeamLabel>();
    }

    public abstract bool InZone(GameObject p_zone);

    public abstract bool CanPickUp(TeamLabel p_requestingPlayer);

    public virtual void ObjectPickedUp()
    {
        m_rb.isKinematic = true;
    }
    public virtual void ObjectDropped()
    {
        transform.rotation = Quaternion.identity;
        m_rb.isKinematic = true;
        transform.parent = null;
    }

    public virtual void ResetObjective()
    {
        transform.parent = m_startingParent;
        transform.localPosition = m_startingOffset;
        transform.localRotation = Quaternion.identity;
    }
}
