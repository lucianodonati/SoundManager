// —————————————————————–
// File: SoundManager.cs
// Author: Luciano Donati Date: 09-05-2018 (dd/mm/yyyy)
// 
// me@lucianodonati.com <a href="http://www.lucianodonati.com">www.lucianodonati.com</a>
// 
// Description: Singleton SoundManager in charge of playing Music and SFX as well as handling 
//              and persisting volumes for each type of sound.
// —————————————————————–

namespace LucianoDonati.SoundManager
{
    using System.Collections;
    using UnityEngine;

    ///<summary>
    ///Defines the <see cref="SoundManager" />. Holds AudioClips, Volumes, and has behaviors to play and fade in/out.
    /// </summary>
    public class SoundManager : Singleton<SoundManager>
    {
        #region Fields

        /// <summary>
        /// Defines the PlayerPrefs keys to be used to persist the volumes
        /// </summary>
        private readonly string[] _masterFxMusicKeys = { "masterVolumeKey", "SFXVolumeKey", "musicVolumeKey" };

        /// <summary>
        /// Defines the _masterVolume
        /// </summary>
        private float _masterVolume;

        /// <summary>
        /// Defines the _musicClips
        /// </summary>
        [Header("Music audio clips. Make sure you name the clips exactly how you will call them in code.")]
        [SerializeField] private AudioClip[] _musicClips = null;

        /// <summary>
        /// Defines the _musicSource
        /// </summary>
        [SerializeField] private AudioSource _musicSource = null;

        /// <summary>
        /// Defines the _musicVolume
        /// </summary>
        private float _musicVolume;

        /// <summary>
        /// Defines the _SFXClips
        /// </summary>
        [Header("SFX audio clips. Make sure you name the clips exactly how you will call them in code.")]
        [SerializeField] private AudioClip[] _SFXClips = null;

        /// <summary>
        /// Defines the _sfxSource
        /// </summary>
        [SerializeField] private AudioSource _sfxSource = null;

        /// <summary>
        /// Defines the _sfxVolume
        /// </summary>
        private float _sfxVolume;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the MasterVolume
        /// </summary>
        public float MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                _masterVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(_masterFxMusicKeys[0], _masterVolume);
            }
        }

        /// <summary>
        /// Gets or sets the MusicVolume
        /// </summary>
        public float MusicVolume
        {
            get { return _musicVolume; }
            set
            {
                _musicVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(_masterFxMusicKeys[2], _musicVolume);
            }
        }

        /// <summary>
        /// Gets or sets the SfxVolume
        /// </summary>
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

        #region Methods

        /// <summary>
        /// Play a music clip immediately
        /// </summary>
        /// <param name="musicClipName">The <see cref="string"/> name of the clip</param>
        public void PlayMusic(string musicClipName)
        {
            _musicSource.clip = FindMusicClip(musicClipName);
            _musicSource.Play();
        }

        /// <summary>
        /// Fades the currently playing music track's volume to 0, 
        /// changes the music clip, and fades it's volume back to 100%
        /// </summary>
        /// <param name="musicClipName">The <see cref="string"/> name of the clip</param>
        /// <param name="fadeDuration">The <see cref="float"/> total duration of the fade.
        /// (Half to fade out, half to fade back in)</param>
        public void PlayMusic(string musicClipName, float fadeDuration)
        {
            StartCoroutine(FadeMusicTracks(musicClipName, fadeDuration));
        }

        /// <summary>
        /// Play an SFX clip once
        /// </summary>
        /// <param name="SFX_ClipName">The <see cref="string"/> name of the clip</param>
        public void PlayOneShot(string SFX_ClipName)
        {
            _sfxSource.PlayOneShot(FindSFXClip(SFX_ClipName), _sfxVolume * _masterVolume);
        }

        /// <inheritdoc />
        private void Awake()
        {
            _masterVolume = PlayerPrefs.GetFloat(_masterFxMusicKeys[0], 1);
            _sfxVolume = PlayerPrefs.GetFloat(_masterFxMusicKeys[1], 1);
            _musicVolume = PlayerPrefs.GetFloat(_masterFxMusicKeys[2], 1);

            _musicSource.volume = _musicVolume * _masterVolume;
            _sfxSource.volume = _sfxVolume * _masterVolume;
        }

        /// <summary>
        /// Coroutine that interpolates the given source's volume over a duration
        /// </summary>
        /// <param name="source">The <see cref="AudioSource"/></param>
        /// <param name="fadeFrom">The <see cref="float"/> start volume </param>
        /// <param name="fadeTo">The <see cref="float"/> end volume</param>
        /// <param name="fadeDuration">The <see cref="float"/> total duration of the fade</param>
        private IEnumerator FadeAudioSourceVolumeTo(AudioSource source, float fadeFrom, float fadeTo, float fadeDuration)
        {
            float timeEllapsed = 0;

            source.volume = fadeFrom;
            while (timeEllapsed < fadeDuration)
            {
                yield return null;
                timeEllapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(fadeFrom, fadeTo, timeEllapsed / fadeDuration);
            }
            source.volume = fadeTo;
        }

        /// <summary>
        /// Coroutine that fades the currently playing music track's volume to 0, 
        /// changes the music clip, and fades it's volume back to 100%
        /// </summary>
        /// <param name="musicClipName">The <see cref="string"/> name of the clip</param>
        /// <param name="fadeDuration">The <see cref="float"/> total duration of the fade.
        /// (Half to fade out, half to fade back in)</param>
        private IEnumerator FadeMusicTracks(string musicClipName, float fadeDuration)
        {
            float halfDuration = fadeDuration * .5f;

            if (_musicSource.isPlaying)
                yield return StartCoroutine(FadeAudioSourceVolumeTo(_musicSource, _musicVolume, 0, halfDuration));
            PlayMusic(musicClipName);
            yield return StartCoroutine(FadeAudioSourceVolumeTo(_musicSource, 0, _musicVolume, halfDuration));
        }

        /// <summary>
        /// Finds a clip on an AudioClip array
        /// </summary>
        /// <param name="clipName">The <see cref="string"/> name of the clip</param>
        /// <param name="clipsAray">The <see cref="AudioClip[]"/> array where to look for it</param>
        /// <returns>The <see cref="AudioClip"/> found clip or null if not found</returns>
        private AudioClip FindClip(string clipName, AudioClip[] clipsAray)
        {
            AudioClip foundClip = null;

            for (int i = 0; null == foundClip && i < clipsAray.Length; i++)
            {
                if (_SFXClips[i].name == clipName)
                {
                    foundClip = clipsAray[i];
                }
            }
            return foundClip;
        }

        /// <summary>
        /// Finds a music clip by name
        /// </summary>
        /// <param name="music_ClipName">The <see cref="string"/> name of the clip</param>
        /// <returns>The <see cref="AudioClip"/> found clip or null if not found</returns>
        private AudioClip FindMusicClip(string music_ClipName)
        {
            return FindClip(music_ClipName, _musicClips);
        }

        /// <summary>
        /// Finds a SFX clip by name
        /// </summary>
        /// <param name="SFX_ClipName">The <see cref="string"/> name of the clip</param>
        /// <returns>The <see cref="AudioClip"/> found clip or null if not found</returns>
        private AudioClip FindSFXClip(string SFX_ClipName)
        {
            return FindClip(SFX_ClipName, _SFXClips);
        }

        #endregion
    }
}
