using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

// Name for this class is a bit confusing since now it acts as instructions for a whole level:
// Contains x amount of waves, with each wave containing x amount of instructions.

[CreateAssetMenu(fileName = "EnemyWave", menuName = "ScriptableObjects/EnemyWave", order = 1)]
public class EnemyWaveScriptableObject : ScriptableObject
{
    public Wave[] waves;

    [System.Serializable]
    public class Wave
    {
        public string name;
        [Space(10)]
        public WaveInstruction[] instructions;
    }

    [System.Serializable]
    public class WaveInstruction
    {
        public Method method;
        public string args;
        
        // This enum allows for a dropdown in the inspector for which method call is desired
        public enum Method
        {
            Delay,
            SpawnEnemyAtPosition,
            SpawnEnemyRing,
        }
    }
}

[UnityEditor.CustomEditor(typeof(EnemyWaveScriptableObject))]
public class InspectorCustomizer : Editor
{
    // Should change "element x" to name of Method enum used...

    /*
    public void ShowArrayProperty(UnityEditor.SerializedProperty list)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            UnityEditor.EditorGUILayout.PropertyField(
                list.GetArrayElementAtIndex(i), 
                new UnityEngine.GUIContent(Enum.GetName(list.GetArrayElementAtIndex(i).propertyType.GetType(), i)), 
                true);
        }
    }

    public override void OnInspectorGUI()
    {
        ShowArrayProperty(serializedObject.FindProperty("m_instructions"));
    }
    */
}
