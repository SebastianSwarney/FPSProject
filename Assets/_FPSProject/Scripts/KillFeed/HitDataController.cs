using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HitDataController : MonoBehaviour
{
    private PhotonView m_myPhotonView;
    private KillFeedManager m_killFeedManager;
    private TeamLabel m_teamLabel;
    private PlayerUIUpdate m_playerUI;
    private void Start()
    {
        m_myPhotonView = GetComponent<PhotonView>();
        m_teamLabel = GetComponent<TeamLabel>();
        m_playerUI = GetComponent<PlayerUIUpdate>();
        m_killFeedManager = KillFeedManager.Instance;
    }
    [PunRPC]
    public void RPC_PlayerHitSomeone(int p_hitObjectID)
    {
        if (!m_myPhotonView.IsMine) return;

        m_playerUI.ShowHitMarker();

    }

    [PunRPC]
    public void RPC_PlayerKilledSomeone(int p_killedObjectID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PointsManager.Instance != null)
            {
                PointsManager.Instance.AddPointsForKill(m_teamLabel.m_myTeam);
            }
        }

        if (!m_myPhotonView.IsMine) return;
        if (p_killedObjectID != m_myPhotonView.ViewID)
        {
            PhotonView hitPhoton = PhotonView.Find(p_killedObjectID).GetComponent<PhotonView>();
            m_killFeedManager.AddMessage("You Killed : " + hitPhoton.Owner.NickName);
            hitPhoton.RPC("RPC_AddToEveryoneKillFeed", RpcTarget.All, m_myPhotonView.Owner.NickName, m_myPhotonView.ViewID);
        }
        else
        {
            m_myPhotonView.RPC("RPC_PlayerKilledThemselves", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_AddToEveryoneKillFeed(string p_message, int p_killerID)
    {
        if (m_myPhotonView.IsMine)
        {
            m_killFeedManager.AddMessage(p_message + " killed you!");
        }
        else
        {
            if (!PhotonView.Find(p_killerID).GetComponent<PhotonView>().IsMine)
            {
                m_killFeedManager.AddMessage(p_message + " killed " + m_myPhotonView.Owner.NickName);
            }
        }
    }



    [PunRPC]
    public void RPC_PlayerKilledThemselves()
    {
        if (m_myPhotonView.IsMine)
        {
            m_killFeedManager.AddMessage("You killed yourself, you fool!");
        }
        else
        {
            m_killFeedManager.AddMessage(m_myPhotonView.Owner.NickName + " commited suicide");
        }
    }
}
