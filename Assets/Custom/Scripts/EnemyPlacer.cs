using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LEGACY

public class EnemyPlacer : MonoBehaviour
{
    [Header("Resources")]
    public Camera m_camera;
    public Transform m_enemyManager;
    public GameObject m_enemyPrefab;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit)) 
            {
                if (hit.transform.name == "ground")
                {
                    Instantiate(m_enemyPrefab, hit.point, Quaternion.identity, m_enemyManager);
                }
            }
        }
    }
}
