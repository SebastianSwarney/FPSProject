using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon_Base : ScriptableObject
{
	public GameObject m_bulletPrefab;

	public float m_bulletSpeed;

	public abstract void OnShootInputDown(WeaponController p_playerWeaponController);

	public abstract void OnShootInputUp(WeaponController p_playerWeaponController);

	public void ShootBullet(Quaternion p_aimDirection, Vector3 p_spawnPosition)
	{
		GameObject newBullet = Instantiate(m_bulletPrefab, p_spawnPosition, p_aimDirection);

		Bullet_Base bullet = newBullet.GetComponent<Bullet_Base>();

		bullet.m_speed = m_bulletSpeed;
	}
}
