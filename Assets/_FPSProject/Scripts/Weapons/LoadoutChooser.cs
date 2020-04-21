using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutChooser : MonoBehaviour
{
    public static LoadoutChooser Instance;

    public List<Loadouts> m_loadouts;


    public EquipmentController m_localPlayerEquipmentController;
    public GameObject m_loadoutCanvas;
    private void Awake()
    {
        Instance = this;
    }
    public void AssignEquipmentController(EquipmentController p_controller)
    {
        m_localPlayerEquipmentController = p_controller;
    }

    public void AssignLoadout(int p_loadout)
    {
        m_localPlayerEquipmentController.AssignLoadouts(m_loadouts[p_loadout]);
    }

}

[System.Serializable]
public class Loadouts
{
    public string m_loadoutName;
    public Equipment_Gun m_primaryGun;
    public Equipment_Gun m_secondaryGun;
}
