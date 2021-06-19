using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 m_gameOrigin;
    public float m_originProtectionRadius;
    public int m_enemyCap = 512;

    [Header("Debug")]
    public bool m_showOnGUI = false;
    public bool m_stopWaveOnInvokeFail = false;

    [Header("Resources")]
    public EnemyWaveScriptableObject m_startingLevel;
    public Int32Variable m_turretHealth;
    public Int32Variable m_currencyAmount;
    public Camera m_camera;
    public EnemyScriptableObject[] m_enemyTypes;
    public GameObject m_bombPrefab;
    public EnemyEventSO m_enemyEventSO;

    private List<Enemy> m_enemies = new List<Enemy>();
    private List<Guid> m_enemyKillQueue = new List<Guid>();
    private int m_enemyByteSize;
    private int m_frameCount = 0;
    private int m_waveCount = 0;
    private bool m_waveInProgress = false;
    
    private void Awake()
    {
        // Get size of single Enemy (Terribly inacurrate because it doesn't account for the GameObject itself);
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, typeof(Enemy));
        m_enemyByteSize = ms.ToArray().Length;

        // Check for duplicate internalName values
        string[] allInternalNames = new string[m_enemyTypes.Length];
        for (int i = 0; i < m_enemyTypes.Length; i++)
            allInternalNames[i] = m_enemyTypes[i].internalName;
        if (allInternalNames.Distinct().Count() != allInternalNames.Count())
            UnityEngine.Debug.LogError("Duplicate internalName values detected.");
    }

    private void Start()
    {
        SpawnNextWave();
    }

    private void FixedUpdate()
    {
        // Update all enemies
        for (int i = 0; i < m_enemies.Count; i++)
        {
            Enemy curEnemy = m_enemies[i];

            // Check if current enemy has been requested to be destroyed this frame.
            for (int j = 0; j < m_enemyKillQueue.Count; j++)
            {
                if (curEnemy.guid == m_enemyKillQueue[j])
                {
                    curEnemy.onKill -= KillEnemy;
                    Destroy(curEnemy.gameObject);
                    m_currencyAmount.Value += curEnemy.m_enemyData.killReward;
                    m_enemies.RemoveAt(i);
                    m_enemyKillQueue.RemoveAt(j);
                    return; // No need to update an enemy that no longer exits.
                }
            }

            // Move enemy toward game origin if it's outside kill zone, otherwise request death.
            if (Vector3.Distance(curEnemy.transform.position, m_gameOrigin) >= m_originProtectionRadius)
            {
                curEnemy.transform.position += curEnemy.transform.forward * curEnemy.m_enemyData.moveSpeed;
            }
            else
            {
                //  (m_frameCount + i * 4) used for enemies of the same damage rate to deal damage on different ticks
                if ((m_frameCount + i * 4) % curEnemy.m_enemyData.damageInterval == 0)
                    m_turretHealth.Value = (int)Mathf.Clamp(m_turretHealth.Value - 1, 0, int.MaxValue);
                //KillEnemy(curEnemy.guid);
            }
        }

        m_frameCount++;
    }

    // For convenience sake, this method will generate the enemy GameObject from enemy type data.
    public void SpawnEnemy(EnemyScriptableObject type, Vector3 position)
    {
        if (m_enemies.Count >= m_enemyCap) return;
        if (type == null) return;
        position.y = 0;

        // Create new enemy root object
        GameObject newEnemy = new GameObject(type.internalName);

        // Add components to root
        BoxCollider colliderComp = newEnemy.AddComponent<BoxCollider>();
        Enemy enemyComp = newEnemy.AddComponent<Enemy>();

        // Apply correct values to components and subscribe to events
        colliderComp.size = new Vector3(0.1f, 0.35f, 0.1f) * type.scale;
        colliderComp.center = new Vector3(0, 0.175f, 0) * type.scale;
        enemyComp.m_enemyData = type;
        enemyComp.health = type.health;
        enemyComp.onKill += KillEnemy;

        // Create enemy model object
        GameObject newEnemyModel = new GameObject("model");
        MeshFilter filterComp = newEnemyModel.AddComponent<MeshFilter>();
        MeshRenderer rendererComp = newEnemyModel.AddComponent<MeshRenderer>();

        // Apply correct values to components and make child of root
        newEnemyModel.transform.parent = newEnemy.transform;
        newEnemyModel.transform.localScale = new Vector3(0.1f, 0.35f, 0.1f) * type.scale;
        newEnemyModel.transform.position = new Vector3(0, 0.175f, 0) * type.scale;
        filterComp.mesh = type.model;
        rendererComp.material = type.material;

        // Other setup for EnemyManager 
        newEnemy.transform.position = new Vector3(position.x, 0, position.z);
        newEnemy.transform.LookAt(m_gameOrigin);
        m_enemies.Add(enemyComp);
    }

    // Request enemy to be killed on the next frame
    public void KillEnemy(Guid guid)
    {
        m_enemyKillQueue.Add(guid);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.name == "ground")
                {
                    Instantiate(m_bombPrefab, hit.point + new Vector3(0, 10, 0), Quaternion.identity);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.N)) 
            SpawnNextWave();
    }

    public void SpawnNextWave()
    {
        if (m_waveCount < m_startingLevel.waves.Length)
        {
            if (!m_waveInProgress)
            {
                StartCoroutine(SpawnWaveCoroutine(m_startingLevel.waves[m_waveCount]));
                m_waveCount++;
            }
        }
        else 
        {
            UnityEngine.Debug.Log("All waves have been completed!");

            // RAISE SOME SORT OF WIN EVENT
        }
    }

    // Loop through and attempt to execute each instruction in provided Wave
    public IEnumerator SpawnWaveCoroutine(EnemyWaveScriptableObject.Wave wave)
    {
        m_waveInProgress = true;
        for (int i = 0; i < wave.instructions.Length; i++)
        {
            // Extract target method name and args from wave.instructions[i]
            string targetMethodName = Enum.GetName(wave.instructions[i].method.GetType(), wave.instructions[i].method);
            string[] instructionArgs = wave.instructions[i].args.Split(',');

            // Execute special delay operations for the Delay enum and move to next instruction after delay.
            if (targetMethodName == "Delay")
            {
                float delay = 0.0f;
                try { delay = Convert.ToSingle(instructionArgs[0]); } 
                catch { UnityEngine.Debug.LogError($"Failed to parse '{instructionArgs[0]}' to float."); }
                yield return new WaitForSeconds(delay);
                continue;
            }

            // Get method and parameters from instruction specifications.
            MethodInfo targetMethod = typeof(EnemyManager).GetMethod(targetMethodName);
            ParameterInfo[] parameters = targetMethod.GetParameters();
            if (parameters.Length != instructionArgs.Length)
            {
                UnityEngine.Debug.LogError($"WaveInstruction at index {i} from WaveSequence '{wave.name}' provided incorrect arg count. Expected {parameters.Length}, received {instructionArgs.Length}");
                m_waveInProgress = false;
                yield break;
            }

            // Attempt to convert instrucion args to types used in method specified.
            try
            {
                object[] convertedParams = new object[parameters.Length];
                for (int j = 0; j < parameters.Length; j++)
                    convertedParams[j] = Convert.ChangeType(instructionArgs[j], parameters[j].ParameterType);
                targetMethod.Invoke(this, convertedParams);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Failed to invoke method '{targetMethod.Name}':\n{e.Message}");
                if (m_stopWaveOnInvokeFail) 
                {
                    m_waveInProgress = false;
                    yield break;
                }
            }
        }
        m_waveInProgress = false;
    }

    public EnemyScriptableObject InternalNameToType(string refTypeName)
    {
        for (int i = 0; i < m_enemyTypes.Length; i++)
            if (m_enemyTypes[i].internalName == refTypeName)
                return m_enemyTypes[i];
        
        UnityEngine.Debug.LogError($"Could not find enemy with reference name '{refTypeName}'");
        return null;
    }

    // Get the enemy nearest to a given position
    public Enemy GetClosestEnemyToPosition(Vector3 position)
    {
        if (m_enemies.Count == 0) return null;
        if (m_enemies.Count == 1) return m_enemies[0];
        int closestIndex = 0;
        float closest = float.MaxValue;
        for (int i = 0; i < m_enemies.Count; i++)
        {
            float distanceToCurrent = Vector3.Distance(position, m_enemies[i].transform.position);
            if (distanceToCurrent < closest)
            {
                closest = distanceToCurrent;
                closestIndex = i;
            }
        }
        return m_enemies[closestIndex];
    }

    private void OnGUI()
    {
        if (!m_showOnGUI) return;

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 50;

        GUI.Label(new Rect(10, 0, 512, 64), $"m_enemies.Count : {m_enemies.Count} / {m_enemyCap}", labelStyle);
        GUI.Label(new Rect(10, 64, 512, 64), $"m_enemies bytes : {((float)m_enemyByteSize * (float)m_enemies.Count) / 1000}mb", labelStyle);
        GUI.Label(new Rect(10, 128, 512, 64), $"m_enemyKillQueue.Count : {m_enemyKillQueue.Count}", labelStyle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_gameOrigin, m_originProtectionRadius);
    }

    // These methods are specifically meant to be used with EnemyWaveScriptableObjects
#region WaveInstructionMethods

    public void SpawnEnemyAtPosition(string typeRefName, float x, float z)
    {
        SpawnEnemy(InternalNameToType(typeRefName), new Vector3(x + m_gameOrigin.x, 0, z + m_gameOrigin.z));
    }

    public void SpawnEnemyRing(string typeRefName, int amount, float variance)
    {
        for (int i = 0; i < amount; i++)
        {
            // Get random point on circle and remap Vector2 to Vector3
            Vector2 point = UnityEngine.Random.insideUnitCircle.normalized * (5 + UnityEngine.Random.Range(-variance, variance));
            SpawnEnemy(InternalNameToType(typeRefName), new Vector3(m_gameOrigin.x + point.x, 0, m_gameOrigin.y + point.y));
        }
    }

#endregion
}
