using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonRoom Instance;
    private PhotonView m_photonView;

    public int m_multiplayerScene;
    private int m_currentScene;
    public GameObject m_photonPlayerPrefab;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            if(Instance != null)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room Joined");
        /*photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();*/

        StartGame();
    }

    private void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.LoadLevel(m_multiplayerScene);
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        m_currentScene = scene.buildIndex;
        if (m_currentScene == m_multiplayerScene)
        {
            CreatePlayer();
        }

    }

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", m_photonPlayerPrefab.name), transform.position, Quaternion.identity, 0);
    }
}
