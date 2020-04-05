using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class RespawnObject_Player : RespawnObject
{
    public GameObject m_visual;
    public GameObject m_camera;
    private MatchSpawningManager m_matchSpawning;
    private Transform m_respawnPoint;
    private TeamLabel m_myTeam;
    private PlayerController m_controller;
    private PlayerInput m_playerInput;
    private PhotonView m_photonView;


    [Header("Debugging")]
    public bool m_isDebugging;
    public override void Start()
    {
        m_matchSpawning = MatchSpawningManager.Instance;
        m_myTeam = GetComponent<TeamLabel>();
        m_controller = GetComponent<PlayerController>();
        m_playerInput = GetComponent<PlayerInput>();
        m_photonView = GetComponent<PhotonView>();
        
        if (!m_isDebugging)
        {
            PlayerWasKilled();
            StartCoroutine(RespawnCoroutine());
            if (!m_photonView.IsMine)
            {
                m_camera.SetActive(false);
            }
        }
    }

    public void PlayerWasKilled()
    {
        
        m_controller.enabled = false;
        m_playerInput.enabled = false;
        if (m_photonView.IsMine)
        {
            m_visual.SetActive(false);
        }
    }

    
    public override void RespawnMe()
    {
        if (m_photonView.IsMine)
        {
            RPC_Respawn();
            m_controller.enabled = true;
            m_playerInput.enabled = true;
        }
    }
    [PunRPC]
    private void RPC_Respawn()
    {
        m_respawnPoint = m_matchSpawning.SpawnPlayer(m_myTeam.m_myTeam);
        transform.position = m_respawnPoint.position;
        transform.rotation = m_respawnPoint.rotation;
        m_visual.SetActive(true);
    }
}
