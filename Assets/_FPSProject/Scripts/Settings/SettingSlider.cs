using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
	public float m_sliderMin;
	public float m_sliderMax;

	private Slider m_slider;
	private InputField m_inputField;

	private void Start()
	{
		m_slider = GetComponentInChildren<Slider>();
		m_inputField = GetComponentInChildren<InputField>();
	}

	public void OnInputFieldUpdate(string p_inputSensitivity)
	{
		if (p_inputSensitivity.Length != 0)
		{
			//float parsedNumber = float.Parse(p_inputSensitivity);


		}
	}

}
