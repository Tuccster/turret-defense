using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Settings")]
    public float m_explosionRadius = 1.0f;

    private void OnCollisionEnter(Collision col)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRadius);
        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i].tag == "Enemy")
                colliders[i].GetComponent<Enemy>().Kill();

        Destroy(gameObject);
    }
}
