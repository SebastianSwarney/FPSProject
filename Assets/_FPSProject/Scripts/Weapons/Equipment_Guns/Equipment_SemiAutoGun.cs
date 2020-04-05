using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_SemiAutoGun : Equipment_Gun
{
    private bool m_playerLetGo = true;

    public override void ShootInputDown(Transform p_playerCam)
    {
        if (m_playerLetGo)
        {
            m_playerLetGo = false;
            base.ShootInputDown(p_playerCam);
        }
    }
    public override void ShootInputUp(Transform p_playerCam)
    {
        m_playerLetGo = true;
        base.ShootInputUp(p_playerCam);
    }

}
