using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class TextManager : Singleton<TextManager>
{
    [System.Serializable]
    public struct ImageDisplay
    {
        public Sprite icon;
        public string id;
        public Color color;
    }

    [SerializeField] ImageDisplay[] m_images = null;
    [SerializeField] Image m_speakerImage = null;
    [SerializeField] TextMeshProUGUI m_messageText = null;
    [SerializeField] TextMeshProUGUI m_fpsText = null;
    [SerializeField] TextMeshProUGUI m_infoText = null;
    [SerializeField] [Range(0.0f, 1.0f)] float m_textSpeed = 0.03f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_textSpeedMultiplier = 2.0f;

    AudioSource m_audioSource;
    float m_currentTextSpeed = 0.0f;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_currentTextSpeed = m_textSpeed;
    }

    public void DisplayText(string message, Color color)
    {
        // Displays generic text (like world events)

        ChangeTextColor(color);

        m_speakerImage.gameObject.SetActive(false);

        ShowText(message);
    }

    public void DisplayTextWithImage(string message, string imageID)
    {
        // Displays text with an icon to show who is talking
        
        m_speakerImage.gameObject.SetActive(true);

        Sprite image = GetImageFromId(imageID);

        if (image)
        {
            m_speakerImage.sprite = image;
            Color c = m_speakerImage.color;
            c.a = 1.0f;
            m_speakerImage.color = c;
        }

        Color newColor = GetColorFromImage(imageID);
        ChangeTextColor(newColor);
        ShowText(message);
    }

    private void ShowText(string message)
    {
        m_messageText.text = "";

        StartCoroutine(DisplayCharactersIndividually(message));
    }

    IEnumerator DisplayCharactersIndividually(string message)
    {
        foreach (char c in message)
        {
            m_messageText.text += c;

            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }
            yield return new WaitForSeconds(m_currentTextSpeed);
        }
    }

    public void SpeedUpTextSpeed()
    {
        m_currentTextSpeed = m_textSpeed / m_textSpeedMultiplier;
    }

    public void SlowDownTextSpeed()
    {
        m_currentTextSpeed = m_textSpeed;
    }

    public float GetDisplayLength(int messageLength)
    {
        float multiplier = Mathf.RoundToInt(messageLength / 20) + 3.0f;
        float time = multiplier;

        return time;
    }

    private void ChangeTextColor(Color color)
    {
        Color c = color;

        m_messageText.color = c;
    }

    public Sprite GetImageFromId(string imageID)
    {
        Sprite image = null;
        for (int i = 0; i < m_images.Length; ++i)
        {
            if (m_images[i].id.Equals(imageID))
            {
                image = m_images[i].icon;
                break;
            }
        }

        return image;
    }

    public Color GetColorFromImage(string imageID)
    {
        Color color = Color.white;

        foreach (ImageDisplay id in m_images)
        {
            if (id.id == imageID)
            {
                color = id.color;
                break;
            }
        }

        return color;
    }
}
