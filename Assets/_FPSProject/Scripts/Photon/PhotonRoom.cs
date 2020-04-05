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

    public int m_menuScene, m_multiplayerScene;
    private int m_currentScene;
    public GameObject m_photonPlayerPrefab;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {

            Destroy(gameObject);

        }

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

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Room Left");
        
        Instance = null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Destroy(gameObject);
        PhotonNetwork.Disconnect();
        if (m_applicationQuitting) return;
        SceneManager.LoadScene(m_menuScene);
        //PhotonNetwork.LoadLevel(m_menuScene);
    }

    private bool m_applicationQuitting;

    private void OnApplicationQuit()
    {
        m_applicationQuitting = true;
    }
}
