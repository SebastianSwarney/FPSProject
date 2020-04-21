using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

[System.Serializable]
public class SettingSliderEvent : UnityEvent<float> { }

public class SettingSlider : MonoBehaviour
{
	public float m_sliderMin;
	public float m_sliderMax;

	public SettingSliderEvent m_onValueUpdatedEvent;

	private Slider m_slider;
	private InputField m_inputField;

	private float m_actualValue;

	private void Start()
	{
		m_slider = GetComponentInChildren<Slider>();
		m_inputField = GetComponentInChildren<InputField>();

		m_slider.minValue = m_sliderMin;
		m_slider.maxValue = m_sliderMax;
	}

	public void OnSliderUpdate(float p_value)
	{
		m_actualValue = Mathf.FloorToInt(p_value);
		m_inputField.text = m_actualValue.ToString();

		m_onValueUpdatedEvent.Invoke(m_actualValue / m_sliderMax);
	}

	public void OnInputFieldUpdate(string p_inputSensitivity)
	{
		if (p_inputSensitivity.Length != 0)
		{
			float parsedNumber = float.Parse(p_inputSensitivity);

			m_actualValue = parsedNumber;

			if (m_actualValue > m_sliderMax)
			{
				m_actualValue = m_sliderMax;
			}

			if (m_actualValue < m_sliderMin)
			{
				m_actualValue = m_sliderMin;
			}

			m_inputField.text = m_actualValue.ToString();

			m_onValueUpdatedEvent.Invoke(m_actualValue / m_sliderMax);
		}
	}

	public void OnInputFieldSubmit(string p_inputSensitivity)
	{
		if (p_inputSensitivity.Length == 0)
		{
			m_inputField.text = "0";
		}
		else
		{
			m_slider.value = m_actualValue;
		}
	}

}
