using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSplatter : MonoBehaviour
{
    [SerializeField] GameObject m_splatter;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y < 0.0f)
        {
            GameObject go = Instantiate(m_splatter, collision.contacts[0].point, Quaternion.identity, collision.transform);
            Destroy(go, 2.0f);
        }
    }
}
