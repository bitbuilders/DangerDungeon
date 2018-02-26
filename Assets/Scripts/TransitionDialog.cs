using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/DialogInfo", order = 1)]
public class TransitionDialog : ScriptableObject
{
    [System.Serializable]
    public struct DialogInfo
    {
        public string imageID;
        public string message;
        public float cameraAmplitude;
        public float cameraRate;
    }

    [SerializeField] public DialogInfo[] m_dialogInfo;
    [SerializeField] public string m_nextLevel;
}
