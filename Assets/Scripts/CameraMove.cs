using System.Collections.Generic;
using UnityEngine;

public class CameraMove : Singleton<CameraMove>
{
    [SerializeField] [Range(1.0f, 10.0f)] float m_attentiveness = 2.0f;
    [SerializeField] GameObject m_primaryTarget = null;
    [SerializeField] [Range(0.0f, 1.0f)] float m_primaryImportance = 1.0f;
    [SerializeField] GameObject m_secondaryTarget = null;
    [SerializeField] [Range(0.0f, 1.0f)] float m_secondaryImportance = 0.0f;
    [SerializeField] [Range(0.1f, 10.0f)] float m_shakeAmplitude = 8.0f;
    [SerializeField] [Range(0.1f, 50.0f)] float m_shakeRate = 20.0f;

    Vector3 m_shake = Vector3.zero;
    float m_shakeAmount = 0.0f;

    static CameraMove ms_camera = null;

    private void Start()
    {
        if (ms_camera == null)
        {
            ms_camera = this;
        }
        else
        {
            Destroy(ms_camera.gameObject);
            ms_camera = this;
        }

        if (!m_secondaryTarget)
        {
            m_secondaryTarget = m_primaryTarget;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        m_shakeAmount -= Time.deltaTime;
        m_shakeAmount = Mathf.Clamp01(m_shakeAmount);

        float time = Time.time * m_shakeRate;

        m_shake.x = m_shakeAmount * m_shakeAmplitude * ((Mathf.PerlinNoise(time, 0.0f) * 2.0f) - 1.0f);
        m_shake.y = m_shakeAmount * m_shakeAmplitude * ((Mathf.PerlinNoise(0.0f, time) * 2.0f) - 1.0f);
        m_shake.z = 0.0f;
    }

    void LateUpdate()
    {
        Vector3 pointOfInterest = Vector3.zero;

        Vector3 distance = Vector3.zero;
        if (m_secondaryTarget)
        {
            distance = m_secondaryTarget.transform.position - m_primaryTarget.transform.position;
        }
        else
        {
            distance = m_primaryTarget.transform.position;
        }
        Vector3 modifier = (-distance * 0.5f) * ((1.0f + m_primaryImportance) / (1.0f + m_secondaryImportance));
        pointOfInterest = (pointOfInterest * 0.5f) + m_primaryTarget.transform.position + modifier;
        //pointOfInterest = pointOfInterest * ((1.0f + m_primaryImportance) / (1.0f + m_secondaryImportance));
        pointOfInterest.z = -10.0f;
        pointOfInterest.y += 1.0f;

        pointOfInterest += m_shake;

        transform.position = Vector3.Lerp(transform.position, pointOfInterest, Time.deltaTime * m_attentiveness);
    }

    public void SetNewPointOfInterest(GameObject pointOfInterest, float importance)
    {
        if (pointOfInterest)
        {
            m_secondaryTarget = pointOfInterest;
            m_secondaryImportance = Mathf.Clamp01(importance);
        }
    }

    public void SetPrimaryImportance(float importance)
    {
        m_primaryImportance = Mathf.Clamp01(importance);
    }

    public void ShakeCamera(float amount, float amplitude = 0.0f, float rate = 0.0f)
    {
        if (amplitude > 0.0f)
        {
            m_shakeAmplitude = Mathf.Clamp(amplitude, 0.0f, 10.0f);
        }
        if (rate > 0.0f)
        {
            m_shakeRate = Mathf.Clamp(rate, 0.0f, 50.0f);
        }

        m_shakeAmount += amount;
    }
}
