using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{

	private Coroutine m_aimCoroutine;
	public Equipment_Base m_weapon;
    public Transform m_playerCamera;

	public void OnShootInputDown()
	{
		m_weapon.OnShootInputDown(m_playerCamera);
	}

	public void OnShootInputUp()
	{
		m_weapon.OnShootInputUp(m_playerCamera);
	}
}
