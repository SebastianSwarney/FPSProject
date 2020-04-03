using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Base : MonoBehaviour
{
	public float m_speed;

	public LayerMask m_wallMask;

	private Rigidbody m_rigidbody;

	private void Start()
	{
		m_rigidbody = GetComponent<Rigidbody>();

		//m_rigidbody.velocity = transform.forward * m_speed;

		m_rigidbody.AddForce(transform.forward * m_speed, ForceMode.Impulse);
	}

	private void Update()
	{
		transform.rotation = Quaternion.LookRotation(m_rigidbody.velocity);

		//transform.Translate(transform.forward * m_speed * Time.deltaTime, Space.World);

		Reflect();
	}

	private void Reflect()
	{
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, m_wallMask))
		{
			m_rigidbody.velocity = Vector3.Reflect(m_rigidbody.velocity, hit.normal);
		}
	}
}
