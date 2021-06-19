using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyEventListener : MonoBehaviour
{
    [SerializeField] private EnemyEventSO m_channel = default;

	public EnemyEvent OnEventRaised;

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

	private void Respond(Enemy value)
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke(value);
	}
}

[System.Serializable]
public class EnemyEvent : UnityEvent<Enemy> {}
