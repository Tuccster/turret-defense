using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Resources")]
    public EnemyScriptableObject m_enemyData;
    
    public Action<Guid> onKill;
    public Guid guid = Guid.NewGuid();
    [HideInInspector] public int health; // EnemyManager sets health when new enemy is created.

    public void Damage(int amount)
    {
        health -= amount;
        if (health <= 0) Kill();
    }

    public void Kill()
    {
        //StackTrace stackTrace = new StackTrace(); 
        //UnityEngine.Debug.Log("Kill() called from " + stackTrace.GetFrame(1).GetMethod().Name);
        onKill?.Invoke(guid);
    }
}
