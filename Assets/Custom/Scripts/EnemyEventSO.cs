using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EnemyEventSO", menuName = "EventSO/Enemy", order = 1)]
public class EnemyEventSO : ScriptableObject
{
    public UnityAction<Enemy> OnEventRaised;
	public void RaiseEvent(Enemy value)
	{
		OnEventRaised?.Invoke(value);
	}
}
