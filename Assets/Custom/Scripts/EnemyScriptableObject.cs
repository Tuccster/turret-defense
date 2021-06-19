using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Settings")]
    public string displayName = "New Enemy";
    public string internalName = "new_enemy";
    [Range(0.001f, 0.01f)]
    public float moveSpeed = 0.01f;
    public int damageInterval = 60;
    public int killReward = 1;
    public float scale = 1.0f;
    public int health = 1;

    [Header("Resources")]
    public Mesh model;
    public Material material;
}
