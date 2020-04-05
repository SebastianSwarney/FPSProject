using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeamLabel_Player : TeamLabel
{
    public MeshRenderer m_renderer;
    public Material m_redMaterial, m_blueMaterial;

    private void Start()
    {
        if (!m_photonView.IsMine) return;
        SetTeamType(TeamManager.Instance.AssignTeamType());
        Transform newSpawn = MatchSpawningManager.Instance.SpawnPlayer(m_myTeam);
        transform.position = newSpawn.position;
        transform.rotation = newSpawn.rotation;
        
    }

    public override void SetTeamType(TeamTypes.TeamType p_newTeamType)
    {
        base.SetTeamType(p_newTeamType);

        if (!m_photonView.IsMine) return;
        int colorType = 0;
        switch (p_newTeamType)
        {
            case TeamTypes.TeamType.Red:
                colorType = 1;
                break;
            case TeamTypes.TeamType.Blue:
                colorType = 2;
                break;
        }
        m_photonView.RPC("RPC_ChangeColor", RpcTarget.AllBuffered, colorType);
    }

    [PunRPC]
    private void RPC_ChangeColor(int p_color)
    {
        if(p_color == 1)
        {
            m_myTeam = TeamTypes.TeamType.Red;
            m_renderer.material = m_redMaterial;
            
        }else if(p_color == 2)
        {
            m_myTeam = TeamTypes.TeamType.Blue;
            m_renderer.material = m_blueMaterial;
        }
        TeamManager.Instance.AddPlayerToTeam(this);
    }

    private void OnDestroy()
    {
        TeamManager.Instance.RemovePlayerFromTeam(this);
    }
}
