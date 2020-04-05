using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[System.Serializable]
public class RespawnObjectEvent : UnityEngine.Events.UnityEvent { }
public class RespawnObject_Player : RespawnObject
{
    public GameObject m_visual;
    public GameObject m_camera;
    private MatchSpawningManager m_matchSpawning;
    private Transform m_respawnPoint;
    private TeamLabel m_myTeam;
    private PlayerController m_controller;
    private PlayerInput m_playerInput;
    private PhotonTransformView m_transformView;
    private PhotonView m_photonView;



    public RespawnObjectEvent m_respawned;
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
            
            DisablePlayerControl();
            StartCoroutine(RespawnCoroutine());
            if (!m_photonView.IsMine)
            {
                m_camera.SetActive(false);
            }
        }
    }

    private void DisablePlayerControl()
    {
        m_controller.enabled = false;
        m_playerInput.enabled = false;
    }
    public void PlayerWasKilled()
    {
        DisablePlayerControl();
        m_visual.SetActive(false);

        if (m_photonView.IsMine)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }


    public override void RespawnMe()
    {
        if (m_photonView.IsMine)
        {
            m_photonView.RPC("RPC_Respawn", RpcTarget.All);

        }
    }

    [PunRPC]
    private void RPC_Respawn()
    {
        if (m_photonView.IsMine)
        {

            m_respawnPoint = m_matchSpawning.SpawnPlayer(m_myTeam.m_myTeam);
            Debug.DrawLine(m_respawnPoint.position, transform.position, Color.yellow, 2);
            transform.position = m_respawnPoint.position;
            transform.rotation = m_respawnPoint.rotation;

            StartCoroutine(DelayControllerStart());
        }

        m_visual.SetActive(true);
        m_respawned.Invoke();
    }

    IEnumerator DelayControllerStart()
    {
        yield return new WaitForSeconds(.1f);
        m_controller.enabled = true;
        m_playerInput.enabled = true;
        
    }
}
