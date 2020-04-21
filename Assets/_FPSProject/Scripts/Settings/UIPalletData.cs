using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UIPalletData : ScriptableObject
{
	[Header("Text Data")]
	public Font m_font;
	public Color m_textColor;

	[Header("Button Data")]
	public Color m_buttonMainColor;
	public Color m_buttonOutlineColor;
	public Color m_buttonArrowColor;

	[Header("Element Data")]
	public Color m_elementMainColor;
}
