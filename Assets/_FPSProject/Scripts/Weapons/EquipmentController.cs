using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public TeamLabel m_teamLabel;
	private Coroutine m_aimCoroutine;
	public Equipment_Base m_weapon;
    public Equipment_Base m_holsteredWeapon;
    public Transform m_playerCamera;

    private void Start()
    {
        m_weapon.SetUpEquipment(m_teamLabel.m_myTeam);
        if (m_holsteredWeapon == null)
        {
            m_holsteredWeapon.PutEquipmentAway();
        }
    }
    public void OnShootInputDown()
	{
		m_weapon.OnShootInputDown(m_playerCamera);
	}

	public void OnShootInputUp()
	{
		m_weapon.OnShootInputUp(m_playerCamera);
	}
}
