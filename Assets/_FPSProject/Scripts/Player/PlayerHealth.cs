using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	private float m_currentHealth;

	public void TakeDamage(float p_damageAmount)
	{
		m_currentHealth -= p_damageAmount;
	}
}
