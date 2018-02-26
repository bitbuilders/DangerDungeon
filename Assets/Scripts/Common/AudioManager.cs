using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [System.Serializable]
    public struct ClipInfo
    {
        public AudioClip audioClip;
        public float pitch;
        public float volume;
        public bool music;
        public bool playOnAwake;
        public AudioMixerGroup output;
        public string clipID;
    }

    [SerializeField] [Range(1.0f, 10.0f)] int m_numOfChannels = 5;
    [SerializeField] ClipInfo[] m_clips = null;

    AudioSource[] m_SFXChannels;
    AudioSource[] m_musicChannels;

    void Awake()
    {
        m_SFXChannels = new AudioSource[m_numOfChannels];
        m_musicChannels = new AudioSource[m_numOfChannels];

        for (int i = 0; i < m_numOfChannels; ++i)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            m_SFXChannels[i] = audioSource;

            AudioSource music = gameObject.AddComponent<AudioSource>();
            music.loop = true;
            m_musicChannels[i] = music;
        }

        for (int i = 0; i < m_clips.Length; ++i)
        {
            if (m_clips[i].playOnAwake)
            {
                PlayClip(m_clips[i].clipID);
            }
        }
    }

    public void StopAllClips(bool fade, float time = 1.0f)
    {
        if (fade)
        {
            StartCoroutine(FadeAudioVolume(time));
        }
        else
        {
            StopEverything();
        }
    }

    IEnumerator FadeAudioVolume(float time)
    {
        for (float a = 1.0f; a >= 0.0f; a -= Time.deltaTime / time)
        {
            for (int i = 0; i < m_numOfChannels; ++i)
            {
                AudioSource sfx = m_SFXChannels[i];
                AudioSource music = m_musicChannels[i];

                if (sfx.volume > a)
                {
                    sfx.volume = a;
                }
                if (music.volume > a)
                {
                    music.volume = a;
                }
            }

            yield return null;
        }

        StopEverything();
    }

    private void StopEverything()
    {
        for (int i = 0; i < m_numOfChannels; ++i)
        {
            AudioSource sfx = m_SFXChannels[i];
            AudioSource music = m_musicChannels[i];

            sfx.Stop();
            music.Stop();
        }
    }

    public void StopClip(string clipID)
    {
        ClipInfo clip = GetClip(clipID);

        for (int i = 0; i < m_numOfChannels; ++i)
        {
            AudioSource sfx = m_SFXChannels[i];
            AudioSource music = m_musicChannels[i];

            if (sfx.clip == clip.audioClip)
            {
                sfx.Stop();
            }
            if (music.clip == clip.audioClip)
            {
                music.Stop();
            }
        }
    }

    public void PlayClip(string clipID)
    {
        ClipInfo clip = GetClip(clipID);

        if (clip.music)
        {
            PlayMusic(clip);
        }
        else
        {
            PlaySFX(clip);
        }
    }

    private void PlaySFX(ClipInfo clip)
    {
        foreach (AudioSource audioSource in m_SFXChannels)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.clip = clip.audioClip;
                audioSource.pitch = clip.pitch;
                audioSource.volume = clip.volume;
                audioSource.outputAudioMixerGroup = clip.output;
                audioSource.Play();
                break;
            }
        }
    }

    private void PlayMusic(ClipInfo clip)
    {
        foreach (AudioSource audioSource in m_musicChannels)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.clip = clip.audioClip;
                audioSource.pitch = clip.pitch;
                audioSource.volume = clip.volume;
                audioSource.outputAudioMixerGroup = clip.output;
                audioSource.Play();
                break;
            }
        }
    }

    private ClipInfo GetClip(string clipID)
    {
        ClipInfo clip = new ClipInfo();

        foreach (ClipInfo ci in m_clips)
        {
            if (ci.clipID.Equals(clipID))
            {
                clip = ci;
                break;
            }
        }

        return clip;
    }
}
