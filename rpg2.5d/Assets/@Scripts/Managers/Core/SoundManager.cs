using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] _audioSources = new AudioSource[(int)Define.ESound.Max];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
}
