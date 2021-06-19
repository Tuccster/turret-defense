using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntEventSO", menuName = "EventSO/Int", order = 1)]
public class IntEventSO : ScriptableObject
{
    public UnityAction<int> OnEventRaised;
	public void RaiseEvent(int value)
	{
		OnEventRaised?.Invoke(value);
	}
}
