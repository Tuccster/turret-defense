using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "VoidEventSO", menuName = "EventSO/Void", order = 1)]
public class VoidEventSO : ScriptableObject
{
    public UnityAction OnEventRaised;
	public void RaiseEvent()
	{
		OnEventRaised?.Invoke();
	}
}
