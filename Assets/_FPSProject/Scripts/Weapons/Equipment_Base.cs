using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EquipmentEvent : UnityEngine.Events.UnityEvent { };
public abstract class Equipment_Base : MonoBehaviour
{
    [HideInInspector]
    public TeamLabel m_teamLabel;

    [HideInInspector]
    public bool m_canUse = false;

    [Header("Equipment Events")]
    [Tooltip("The events that are fired when the player presses the fire button, or stops pressing the fire button.")]
    public EquipmentEvents m_equipmentEvents;
    [System.Serializable]
    public struct EquipmentEvents
    {
        public EquipmentEvent m_equipmentUsed;
        public EquipmentEvent m_equipmentStopUse;
    }

    private void Start()
    {
        
    }

    public virtual void SetUpEquipment(TeamTypes.TeamType p_currentTeam)
    {
        if (m_teamLabel == null)
        {
            m_teamLabel = GetComponent<TeamLabel>();
        }
        m_teamLabel.SetTeamType(p_currentTeam);
        m_canUse = true;
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
