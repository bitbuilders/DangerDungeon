using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 5.0f)] protected float m_jumpResistance = 2.5f;
    [SerializeField] [Range(0.0f, 5.0f)] protected float m_fallMultiplier = 2.5f;

    protected bool m_active = true;
    public void Activate(bool active) { m_active = active; }

    protected Vector3 Flip(Vector2 velocity, Vector3 startScale, Vector3 currentScale)
    {
        if (velocity.x < 0.0f)
        {
            if (startScale.x < 0.0f)
            {
                currentScale.x = startScale.x;
            }
            else if (startScale.x > 0.0f)
            {
                currentScale.x = startScale.x * -1.0f;
            }
        }
        else if (velocity.x > 0.0f)
        {
            if (startScale.x < 0.0f)
            {
                currentScale.x = startScale.x * -1.0f;
            }
            else if (startScale.x > 0.0f)
            {
                currentScale.x = startScale.x;
            }
        }

        return currentScale;
    }

    protected bool IsOnGround(Vector2 position, LayerMask groundMask, LayerMask platformMask, float radius = 0.2f)
    {
        bool onGround = Physics2D.OverlapCircle(position, radius, groundMask.value | platformMask.value);
        return onGround;
    }
}
