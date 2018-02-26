using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Platform : Singleton<Platform>
{
    [SerializeField] [Range(0.0f, 2.0f)] float m_disableDuration = 1.0f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y < 0.0f)
        {
            if (Input.GetButton("Drop") && collision.gameObject.CompareTag("Player"))
            {
                if (Mathf.Approximately(collision.gameObject.GetComponent<Rigidbody2D>().velocity.y, 0.0f))
                {
                    DropEntity(collision.gameObject);
                }
            }
        }
    }

    public void DropEntity(GameObject entity)
    {
        Collider2D collider = entity.GetComponent<Collider2D>();
        collider.enabled = false;
        StartCoroutine(EnableDroppedCollider(collider));
    }

    IEnumerator EnableDroppedCollider(Collider2D collider)
    {
        yield return new WaitForSeconds(m_disableDuration);
        collider.enabled = true;
    }
}
