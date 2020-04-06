public class TeamTypes
{
    public enum TeamType { Neutral, Red, Blue };

    public static int GetTeamAsInt(TeamType p_team)
    {
        switch (p_team)
        {
            case TeamType.Neutral:
                return 0;
            case TeamType.Red:
                return 1;
            case TeamType.Blue:
                return 2;
        }
        return 0;
    }

    public static TeamType GetTeamFromInt(int p_intTeam)
    {
        if(p_intTeam == 0)
        {
            return TeamType.Neutral;
        }else if (p_intTeam == 1)
        {
            return TeamType.Red;
        }else if(p_intTeam == 2)
        {
            return TeamType.Blue;
        }
        return TeamType.Neutral;
    }

}
