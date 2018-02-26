using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField] [Range(1.0f, 10.0f)] float m_speedIncrease = 7.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IMovable movable = collision.gameObject.GetComponent<IMovable>();
        if (movable != null)
        {
            movable.SpeedMultiplier += m_speedIncrease;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        IMovable movable = collision.gameObject.GetComponent<IMovable>();
        if (movable != null)
        {
            movable.SpeedMultiplier -= m_speedIncrease;
        }
    }
}
