using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FireBehaviour_Base : ScriptableObject
{
    public abstract void FireBullet(TeamLabel p_teamLabel, GameObject p_bullet, Transform p_fireSpot, float p_bulletSpeed, float p_bulletDamage, Transform p_target);
}
