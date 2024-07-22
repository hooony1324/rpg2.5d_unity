using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimatorEventReceiver : InitBase
{
    // Running Sounds
    [SerializeField]
    private List<string> _runningSounds = new List<string>();
    private AudioSource _audioSource;
    private int _runningIdx = 0;

    float _minDistance = 0;
    float _maxDistance = 4;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _audioSource = gameObject.GetOrAddComponent<AudioSource>();
        _audioSource.spatialBlend = 1.0f;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.minDistance = _minDistance;
        _audioSource.maxDistance = _maxDistance;

        return true;
    }

    public void OnPlaySound(string soundName)
    {
        Managers.Sound.Play(Define.ESound.Effect, soundName);
    }

    public void OnPlayRunningSound()
    {
        if (_runningSounds.Count == 0)
            return;

        Managers.Sound.Play(Define.ESound.Effect, _runningSounds[_runningIdx++]);
        if (_runningIdx == _runningSounds.Count)
            _runningIdx = 0;
    }
}
