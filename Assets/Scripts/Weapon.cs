using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform m_firePoint = null;
    [SerializeField] [Range(1.0f, 2000.0f)] float m_force = 10.0f;
    [SerializeField] [Range(0.1f, 60.0f)] float m_lifetime = 10.0f;

    public void FireBullet(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject bullet = BulletPool.Instance.Get();
        bullet.SetActive(true);
        bullet.transform.position = m_firePoint.position;
        bullet.transform.rotation = rotation;

        Vector2 force = direction.normalized * m_force;
        bullet.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(DisableBullet(bullet));
    }

    IEnumerator DisableBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(m_lifetime);

        if (bullet)
        {
            bullet.SetActive(false);
        }
    }
}
