using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeam
{
    TeamTypes.TeamType GetTeamType();
    void SetTeamType(TeamTypes.TeamType p_newTeamType);
}
