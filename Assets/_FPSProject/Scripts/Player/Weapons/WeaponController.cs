using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	public Transform m_aimPoint;

	private float m_aimProgress;
	private Coroutine m_aimCoroutine;

	public Weapon_Base m_weapon;

	public Weapon_Base m_currentWeapon;

	public void OnShootInputDown()
	{
		m_weapon.OnShootInputDown(this);
	}

	public void OnShootInputUp()
	{
		m_weapon.OnShootInputUp(this);
	}
}
