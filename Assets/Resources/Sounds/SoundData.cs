using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnGame
{
    [CreateAssetMenu(fileName = "NewSoundData", menuName = "Sound/SoundData")]
    public class SoundData : ScriptableObject
    {
        public SoundType soundType;
        public AudioClip clip;
        
        public static List<SoundData> LoadAllSoundData()
        {
            return Resources.LoadAll<SoundData>("Sounds/SoundData").ToList();
        }
    }
    
    public enum SoundType
    {
        None,
        MainBGM,
        BossBGM,
        Attack,
        Guard,
        Hit,
    }
}
