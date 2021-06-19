using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntEventListener : MonoBehaviour
{
    [SerializeField] private IntEventSO m_channel = default;

	public IntEvent OnEventRaised;

	private void OnEnable()
	{
		if (m_channel != null)
			m_channel.OnEventRaised += Respond;
	}

	private void OnDisable()
	{
		if (m_channel != null)
			m_channel.OnEventRaised -= Respond;
	}

	private void Respond(int value)
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke(value);
	}
}

[System.Serializable]
public class IntEvent : UnityEvent<int>{}
