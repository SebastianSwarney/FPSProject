using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class EquipmentEvent : UnityEngine.Events.UnityEvent { };
public abstract class Equipment_Base : MonoBehaviour
{
    [HideInInspector]
    public TeamLabel m_teamLabel;

    [HideInInspector]
    public bool m_canUse = false;

    [HideInInspector]
    public EquipmentController m_equipController;

    [HideInInspector]
    public PhotonView m_currentPhotonView { get; private set; }

    [Header("Equipment Events")]
    [Tooltip("The events that are fired when the player presses the fire button, or stops pressing the fire button.")]
    public EquipmentEvents m_equipmentEvents;
    [System.Serializable]
    public struct EquipmentEvents
    {
        public EquipmentEvent m_equipmentUsed;
        public EquipmentEvent m_equipmentStopUse;
    }


    public virtual void SetUpEquipment(TeamTypes.TeamType p_currentTeam, EquipmentController p_equipController, PhotonView p_currentPhotonView)
    {
        if (m_teamLabel == null)
        {
            m_teamLabel = GetComponent<TeamLabel>();
        }
        m_teamLabel.SetTeamType(p_currentTeam);
        m_canUse = true;
        m_equipController = p_equipController;
        m_currentPhotonView = p_currentPhotonView;
    }
    public virtual void PutEquipmentAway()
    {
        m_canUse = false;
    }

	public virtual void OnShootInputDown(Transform p_playerCam)
    {
        if (!m_canUse) return;
    }

	public virtual void OnShootInputUp(Transform p_playerCam)
    {
    }
}
