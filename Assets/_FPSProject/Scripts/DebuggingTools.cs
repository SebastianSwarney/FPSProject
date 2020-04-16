using UnityEngine;

[System.Serializable]
public struct DebugTools
{
    public bool m_debugTools;
    public Color m_gizmosColor1;
    public enum GizmosType { Wire, Shaded };
    public GizmosType m_gizmosType;
}

