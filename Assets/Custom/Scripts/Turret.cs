using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Settings")]
    public int m_startHealth = 1000;
    public float m_rotationSpeed = 0.025f; // Upgrade 1
    public float m_delayPreShoot = 0.5f;   // Upgrade 2
    public float m_delayPostShoot = 0.25f; // Upgrade 2
    public float m_snapAngle = 2.0f;

    [Header("Debug")]
    public bool m_showOnGUI = false;

    [Header("Resources")]
    public Int32Variable m_maxHealth;
    public Int32Variable m_health;
    public EnemyManager m_enemyManager;
    public Transform m_shootPoint;
    public Transform m_pivot;
    public Camera m_camera;

    private TurretStats m_turretStats;
    private Enemy m_target;
    private bool m_rotationInProgress = false;
    private WaitForFixedUpdate m_waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        m_turretStats = GetComponent<TurretStats>();
    }

    private void Start()
    {
        // ScriptableObjects are saved to disk; reset the values.
        m_health.Value = m_startHealth;
    }

    private void FixedUpdate()
    {
        if (!m_rotationInProgress)
            if (m_target == null)
                m_target = m_enemyManager.GetClosestEnemyToPosition(transform.position);
            else
                StartCoroutine(ShootAtTarget(m_target.transform.position));
    }

    private IEnumerator ShootAtTarget(Vector3 position)
    {
        m_rotationInProgress = true;
        float rotationProgress = 0.0f;
        // TODO : Implement rotation upgrades.
        //float levelAccountedRotationSpeed = m_rotationSpeed + (m_rotationSpeed / (m_rotationSpeed * m_turretStats.m_turnSpeed.m_level.Value));
        Quaternion startRotation = m_pivot.rotation;
        Quaternion targetRotation = Quaternion.LookRotation((position - m_pivot.position).normalized);
        float adjustedRotationSpeed = m_rotationSpeed / Quaternion.Angle(m_pivot.transform.rotation, targetRotation);

        // Rotate incrementally each frame until within snap angle.
        while (Mathf.Abs(Quaternion.Angle(m_pivot.transform.rotation, targetRotation)) >= m_snapAngle)
        {
            rotationProgress += adjustedRotationSpeed;
            m_pivot.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationProgress);
            yield return m_waitForFixedUpdate;
        }
        
        // Snap to and kill target.
        m_pivot.rotation = targetRotation;
        yield return new WaitForSeconds(m_delayPreShoot);
        m_target.Damage(1);
        yield return new WaitForSeconds(m_delayPostShoot);
        m_target = null;
        m_rotationInProgress = false;
    }

    private void OnGUI()
    {   
        if (!m_showOnGUI) return;

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 50;

        Vector3 screenPos = m_camera.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(screenPos.x - 256, Screen.height - screenPos.y, 512, 64), $"m_target:{m_target}", labelStyle);
    }
}
