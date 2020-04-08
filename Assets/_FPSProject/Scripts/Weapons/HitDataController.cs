using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HitDataController : MonoBehaviour
{
    private PhotonView m_myPhotonView;

    public float m_hitMarkerAppearTime;
    private float m_currentHitTime;
    public GameObject m_hitMarker;
    private void Start()
    {
        m_myPhotonView = GetComponent<PhotonView>();
        if (m_myPhotonView.IsMine)
        {
            StartCoroutine(HitMarker());
        }
    }
    [PunRPC]
    public void RPC_PlayerHitSomeone(int p_hitObjectID)
    {
        if (!m_myPhotonView.IsMine) return;

        m_currentHitTime = 0;

    }

    [PunRPC]
    public void RPC_PlayerKilledSomeone(int p_killedObjectID)
    {
        if (!m_myPhotonView.IsMine) return;
        print("I Killed : " + PhotonView.Find(p_killedObjectID).GetComponent<PhotonView>().Owner.NickName);

    }

    private void KilledPlayer(int p_hitID)
    {

    }

    private IEnumerator HitMarker()
    {
        while (true)
        {
            if (m_currentHitTime < m_hitMarkerAppearTime)
            {
                m_hitMarker.SetActive(true);
                m_currentHitTime += Time.deltaTime;

            }
            else
            {
                m_hitMarker.SetActive(false);
            }
            yield return null;
        }
    }

}
