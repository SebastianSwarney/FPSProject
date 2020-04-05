using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby Instance;

    public GameObject m_joinButton;
    public GameObject m_cancelButton;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState == ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Player Connected");
        PhotonNetwork.AutomaticallySyncScene = true;
        m_joinButton.SetActive(true);
    }

    public void JoinRandomRoom()
    {
        m_joinButton.SetActive(false);
        m_cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Failed");
        CreateRoom();
    }

    private void CreateRoom()
    {
        int randomRoomNum = Random.Range(0, 1000);
        RoomOptions newRoomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.CreateRoom("Room" + randomRoomNum, newRoomOps);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed");
        CreateRoom();
    }



    public void OnCancelSearch()
    {
        m_cancelButton.SetActive(false);
        m_joinButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnect");
    }
}
