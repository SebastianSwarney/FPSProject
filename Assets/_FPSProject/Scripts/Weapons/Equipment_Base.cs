using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EquipmentEvent : UnityEngine.Events.UnityEvent { };
public abstract class Equipment_Base : MonoBehaviour
{
    [Header("Equipment Events")]
    [Tooltip("The events that are fired when the player presses the fire button, or stops pressing the fire button.")]
    public EquipmentEvents m_equipmentEvents;
    [System.Serializable]
    public struct EquipmentEvents
    {
        public EquipmentEvent m_equipmentUsed;
        public EquipmentEvent m_equipmentStopUse;
    }

	public abstract void OnShootInputDown(Transform p_playerCam);

	public abstract void OnShootInputUp(Transform p_playerCam);
}
