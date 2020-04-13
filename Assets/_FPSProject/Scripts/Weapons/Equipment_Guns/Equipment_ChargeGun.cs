using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_ChargeGun : Equipment_Gun
{
    [Header("Charge Gun")]
    public float m_maxHoldTime;
    public ChargeStages[] m_chargeStages;
    private float m_currentHeldDownTime;
    private int m_currentChargeState;

    public bool m_isAutiomatic;
    private bool m_playerLetGo = true;
    private bool m_fired;
    private bool m_charging;

    private int m_currentFiredState;

    [System.Serializable]
    public struct ChargeStages
    {
        public GameObject m_stateVisual;
        public bool m_shootProjectile;
        public float m_projectileSpeed;
        public float m_projectileDamage;
        public float m_projectileRecoilY;
        public Vector2 m_bulletSpread;
        public GameObject m_bulletPrefab;
        public float m_endChargeTime;
        public FireBehaviour_Base m_fireBehaviour;
        public float m_fireRecoilTime;
    }
    public override void ShootInputDown(Transform p_playerCam)
    {
        if (m_isReloading) return;
        if (!m_canFire) return;
        if (!m_isAutiomatic)
        {
            if (!m_playerLetGo && !m_charging) return;
        }
        m_playerLetGo = false;
        m_charging = true;
        m_currentHeldDownTime += Time.deltaTime;
        DisplayVisual(m_currentHeldDownTime, true);
        if (m_currentHeldDownTime >= m_maxHoldTime)
        {
            m_fired = true;
            FireGun(p_playerCam);
            m_currentHeldDownTime = 0;
        }
    }

    private void DisplayVisual(float p_chargeTime, bool p_display)
    {
        
        foreach(ChargeStages charge in m_chargeStages)
        {
            if (p_display)
            {
                if (p_chargeTime < charge.m_endChargeTime)
                {
                    charge.m_stateVisual.SetActive(true);
                    return;
                }
                else
                {
                    charge.m_stateVisual.SetActive(false);
                }
            }
            else
            {
                charge.m_stateVisual.SetActive(false);
            }

        }
    }

    public override void ShootInputUp(Transform p_playerCam)
    {

        m_playerLetGo = true;
        if (!m_fired)
        {

            if (m_charging)
            {
                FireGun(p_playerCam);
            }
        }
        m_currentHeldDownTime = 0;
        m_fired = false;

    }

    private void FireGun(Transform p_playerCam)
    {
        m_currentFiredState = CheckChargeState(m_currentHeldDownTime);
        if (m_chargeStages[m_currentFiredState].m_shootProjectile)
        {
            m_charging = false;
            Transform aimedTarget;
            PerformAimAssist(p_playerCam, out aimedTarget);
            m_chargeStages[m_currentFiredState].m_fireBehaviour.FireBullet(m_myPhotonView, m_ownerID, m_teamLabel, m_chargeStages[m_currentFiredState].m_bulletPrefab, m_fireSpot, m_chargeStages[m_currentFiredState].m_projectileSpeed, m_chargeStages[m_currentFiredState].m_projectileDamage, m_chargeStages[m_currentFiredState].m_bulletSpread, aimedTarget);
            m_currentHeldDownTime = 0;
            StartFireDelay();
            StartCoroutine(RecoilDisplay());
        }
        DisplayVisual(m_currentHeldDownTime, false);

        Debug.Break();
    }

    private int CheckChargeState(float p_chargeTime)
    {
        int p_number = 0;
        foreach (ChargeStages currentState in m_chargeStages)
        {
            if (currentState.m_endChargeTime < p_chargeTime)
            {

                if (p_number + 1 == m_chargeStages.Length)
                {
                    return p_number;
                }
                p_number++;
                continue;
            }
            else
            {
                return p_number;
            }
        }
        return p_number;
    }


    private IEnumerator RecoilDisplay()
    {
        yield return new WaitForSeconds(m_chargeStages[m_currentChargeState].m_fireRecoilTime);
    }
}
