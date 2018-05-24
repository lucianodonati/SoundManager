#region Copyright

// —————————————————————–
// File: SoundManager.cs
// Author: Luciano Donati Date: 09-05-2018 (dd/mm/yyyy)
// 
// me@lucianodonati.com www.lucianodonati.com
// 
// Description: 
// —————————————————————–

#endregion

#region Using Directives

using UnityEngine;

#endregion

public class SoundManager : Singleton<SoundManager>
{
    private readonly string[] _masterFxMusicKeys = { "masterVolumeKey", "SFXVolumeKey", "musicVolumeKey" };

    private float _masterVolume;
    public float MasterVolume
    {
        get { return _masterVolume; }
        set
        {
            _masterVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(_masterFxMusicKeys[0], _masterVolume);
        }
    }

    #region Music
    [SerializeField] private AudioClip[] _music = null;

    [SerializeField] private AudioSource _musicSource = null;

    private float _musicVolume;
    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(_masterFxMusicKeys[2], _musicVolume);
        }
    }
    #endregion

    #region SFX
    [SerializeField] private AudioClip[] _sfx = null;

    [SerializeField] private AudioSource _sfxSource = null;

    private float _sfxVolume;
    public float SfxVolume
    {
        get { return _sfxVolume; }
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(_masterFxMusicKeys[1], _sfxVolume);
        }
    }
    #endregion

    private void Awake()
    {
        _masterVolume = PlayerPrefs.GetFloat(_masterFxMusicKeys[0], 1);
        _sfxVolume = PlayerPrefs.GetFloat(_masterFxMusicKeys[1], 1);
        _musicVolume = PlayerPrefs.GetFloat(_masterFxMusicKeys[2], 1);
    }

    /// <summary>
    /// Plays the given SFX clip once.
    /// </summary>
    /// <param name="SFX_Name">The name of the Clip in the SoundManager clip list.</param>
    public void PlayOnShot(string SFX_ClipName)
    {
        _sfxSource.PlayOneShot(FindClip(SFX_ClipName), _sfxVolume * _masterVolume);
    }

    private AudioClip FindClip(string SFX_ClipName)
    {
        AudioClip foundClip = null;

        for (int i = 0; null == foundClip && i < _sfx.Length; i++)
        {
            if (_sfx[i].name == SFX_ClipName)
            {
                foundClip = _sfx[i];
            }
        }
        return foundClip;
    }
}