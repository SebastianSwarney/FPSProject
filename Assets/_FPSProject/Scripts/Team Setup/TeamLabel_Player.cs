using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class TeamLabelPlayerEvent : UnityEngine.Events.UnityEvent { }
public class TeamLabel_Player : TeamLabel
{
    public static TeamLabel_Player LocalPlayer;
    public MeshRenderer m_renderer;
    public Material m_redMaterial, m_blueMaterial;
    public TeamLabelPlayerEvent m_teamSetup;
    public float m_delayTeamAssignTime = 1;
    public override void Awake()
    {
        base.Awake();
        if (m_photonView.IsMine)
        {
            LocalPlayer = this;
        }
    }
    private void Start()
    {

        if (!m_photonView.IsMine) return;
        
        StartCoroutine(TeamAssignment());

    }
    private IEnumerator TeamAssignment()
    {
        yield return new WaitForSeconds(m_delayTeamAssignTime);
        SetTeamType(TeamManager.Instance.AssignTeamType());
        m_teamSetup.Invoke();

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
        if (p_color == 1)
        {
            m_myTeam = TeamTypes.TeamType.Red;
            m_renderer.material = m_redMaterial;

        }
        else if (p_color == 2)
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
