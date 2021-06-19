using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidEventListener : MonoBehaviour
{
    [SerializeField] private VoidEventSO m_channel = default;

	public VoidEvent OnEventRaised;

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

	private void Respond()
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke();
	}
}

[System.Serializable]
public class VoidEvent : UnityEvent {}
