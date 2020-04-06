using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PhotonOfflineToggle : MonoBehaviourPunCallbacks
{
    public bool m_toggleOfflineMode;
    public GameObject m_playerPrefab;
    public Transform m_playerSpawn;

    private void Awake()
    {
        PhotonNetwork.OfflineMode = m_toggleOfflineMode;
        if (m_toggleOfflineMode)
        {
            int randomRoomNum = Random.Range(0, 1000);
            RoomOptions newRoomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
            PhotonNetwork.CreateRoom("Room" + randomRoomNum, newRoomOps);

            print("Oi");
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", m_playerPrefab.name), m_playerSpawn.position, m_playerSpawn.rotation, 0);
        }
    }


}
