using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon_Automatic : Weapon_Base
{
	public float m_fireRate;

	private bool m_isShooting;

	private Coroutine m_shootingCoroutine;

	private void OnEnable()
	{
		m_isShooting = false;
	}

	public override void OnShootInputDown(WeaponController p_playerWeaponController)
	{
		if (!m_isShooting)
		{
			m_shootingCoroutine = p_playerWeaponController.StartCoroutine(RunShoot(p_playerWeaponController));
		}
	}

	public override void OnShootInputUp(WeaponController p_playerWeaponController)
	{
		m_isShooting = false;
		p_playerWeaponController.StopCoroutine(m_shootingCoroutine);
	}

	private IEnumerator RunShoot(WeaponController p_playerWeaponController)
	{
		m_isShooting = true;

		while (true)
		{
			ShootBullet(p_playerWeaponController.m_aimPoint.rotation, p_playerWeaponController.m_aimPoint.position);

			yield return new WaitForSeconds(m_fireRate);
		}
	}
}
