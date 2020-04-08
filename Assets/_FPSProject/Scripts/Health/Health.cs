﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

[System.Serializable]
public class HealthActivationEvent : UnityEvent { }
[System.Serializable]
public class HealthActivationEventWithID : UnityEvent<int> { }

[RequireComponent(typeof(TeamLabel))]
public class Health : MonoBehaviour
{
    #region Generic Health Values
    public float m_maxHealth;
    public float m_currentHealth;
    [HideInInspector]
    public bool m_isDead;
    public HealthActivationEvent m_onDied = new HealthActivationEvent();
    public HealthActivationEventWithID m_onDiedWithID = new HealthActivationEventWithID();
    #endregion

    #region Shield Values
    [Header("Shields")]
    public bool m_useShields = false;
    public float m_shieldDamageMultiplier = .75f;
    public float m_maxShieldStrength;
    private float m_currentShieldStrength;

    public bool m_shieldRegeneration = true;
    public float m_shieldRegenDelay;
    public float m_shieldRegenTimeToFull;
    private float m_shieldRegenCurrentTime;
    private Coroutine m_shieldRegenerationCoroutine;
    private WaitForSeconds m_shieldRegenDelayTimer;
    #endregion

    private bool m_canLoseHealth = true;

    private ObjectPooler m_pooler;
    [HideInInspector]
    public TeamLabel m_teamLabel;

    [HideInInspector]
    public PhotonView m_photonView;

    private void Start()
    {
        m_teamLabel = GetComponent<TeamLabel>();
        m_pooler = ObjectPooler.instance;
        m_shieldRegenDelayTimer = new WaitForSeconds(m_shieldRegenDelay);
        m_photonView = GetComponent<PhotonView>();
        Respawn();
    }

    public void Respawn()
    {
        StopAllCoroutines();
        m_currentHealth = m_maxHealth;
        m_isDead = false;
        if (m_useShields) m_currentShieldStrength = m_maxShieldStrength;
    }

    public virtual void TakeDamage(float p_takenDamage, int p_attackerID)
    {
        m_photonView.RPC("RPC_TakeDamage", RpcTarget.All, p_takenDamage, p_attackerID);
    }

    [PunRPC]
    public void RPC_TakeDamage(float p_takenDamage, int p_attackerID)
    {
        if (!m_canLoseHealth) return;
        if (!m_isDead)
        {
            StopAllCoroutines();

            if (m_useShields && m_currentShieldStrength > 0)
            {
                m_currentShieldStrength -= p_takenDamage * m_shieldDamageMultiplier;
                if (m_currentShieldStrength < 0)
                {
                    m_currentHealth -= (Mathf.Abs(m_currentShieldStrength * ((1f - m_shieldDamageMultiplier) + 1f)));
                    if ((Mathf.Round(m_currentHealth)) <= 0)
                    {
                        Died(p_attackerID);
                    }
                    m_currentShieldStrength = 0;
                }
                if (!m_isDead)
                {
                    m_shieldRegenerationCoroutine = StartCoroutine(RegenShield());
                }
            }

            else
            {
                m_currentHealth -= p_takenDamage;
                if (m_currentHealth > 0)
                {
                    if (m_useShields)
                    {
                        m_shieldRegenerationCoroutine = StartCoroutine(RegenShield());
                    }
                }
                else
                {
                    Died(p_attackerID);
                }
            }
        }
    }

    private void Died(int p_attackerID)
    {
        m_isDead = true;
        m_onDied.Invoke();
        m_onDiedWithID.Invoke(p_attackerID);
        PhotonView.Find(p_attackerID).GetComponent<PhotonView>().RPC("RPC_PlayerKilledSomeone", RpcTarget.All, m_photonView.ViewID);

    }

    public void RetoggleHealth()
    {
        m_canLoseHealth = false;
    }
    IEnumerator RegenShield()
    {
        yield return m_shieldRegenDelayTimer;
        float regenRate = ((m_maxShieldStrength / m_shieldRegenTimeToFull)) / 60f;
        print(regenRate);

        while (m_currentShieldStrength < m_maxShieldStrength)
        {
            m_currentShieldStrength += regenRate;
            yield return null;
        }

        m_currentShieldStrength = m_maxShieldStrength;
        m_shieldRegenerationCoroutine = null;
    }

    public void ReturnToPool()
    {
        m_pooler.ReturnToPool(this.gameObject);
    }

}
