using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_BurstGun : Equipment_Gun
{
    [Header("Burst Fire")]
    public bool m_isAutomatic;
    public int m_burstAmount;
    private int m_currentBurstAmount;
    private bool m_canFireBurst = true, m_playerLetGo = true;

    public float m_timeBetweenBullets;
    private float m_currentTimeBetweenBullets;
    private float m_burstCoroutineLife = 5f, m_burstCorLifeTimer;

    private Coroutine m_burstFireCoroutine;

    public override void ShootInputDown(Transform p_playerCam)
    {
        if (!m_isAutomatic)
        {
            if (!m_playerLetGo) return;
        }
        if (m_canFire)
        {
            if (m_canFireBurst)
            {
                m_playerLetGo = false;
                m_canFire = false;
                m_canFireBurst = false;
                m_burstCorLifeTimer = 0;
                m_currentTimeBetweenBullets = 0;
                m_currentBurstAmount = 0;
                if (m_burstFireCoroutine == null)
                {
                    m_burstFireCoroutine = StartCoroutine(FireBurst(p_playerCam));
                }
            }
        }
    }

    public override void ShootInputUp(Transform p_playerCam)
    {
        m_playerLetGo = true;
    }

    private IEnumerator FireBurst(Transform p_playerCam)
    {
        bool p_startedDelay = false;
        while (m_burstCorLifeTimer < m_burstCoroutineLife)
        {
            while (m_currentBurstAmount < m_burstAmount)
            {
                p_startedDelay = false;
                FireBullet(p_playerCam);
                m_currentTimeBetweenBullets = 0;
                while (m_currentTimeBetweenBullets < m_timeBetweenBullets)
                {
                    m_currentTimeBetweenBullets += Time.deltaTime;
                    yield return null;
                }
                
                m_currentBurstAmount += 1;
                yield return null;
            }
            m_canFireBurst = true;
            m_burstCorLifeTimer += Time.deltaTime;
            if (!p_startedDelay)
            {
                p_startedDelay = true;
                StartFireDelay();
            }
            
            yield return null;
        }
        m_burstFireCoroutine = null;
    }
}
