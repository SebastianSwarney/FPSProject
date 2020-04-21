using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UIPalletData : ScriptableObject
{
	[Header("Text Data")]
	public Font m_font;
	public Color m_textColor;

	[Header("Base Element Data")]
	public int m_settingDescriptionTextSize;
	public Color m_baseElementMainColor;

	[Header("Base Button Data")]
	public Color m_buttonBackgroundColor;
	public Color m_buttonOutlineColor;

	[Header("Basic Button Data")]
	public int m_basicButtonTextSize;

	[Header("Setting Button Data")]
	public int m_settingButtonTextSize;
	public Color m_settingButtonSelectColor;
	public Color m_settingButtonDeselectColor;
	public Color m_settingButtonArrowColor;

	[Header("Slider Data")]
	public int m_sliderInputTextSize;
	public Color m_sliderHandleColor;
	public Color m_sliderFillColor;
	public Color m_sliderBackgroundColor;
}
