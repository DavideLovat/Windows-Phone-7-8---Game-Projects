#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

#endregion

namespace ZoneGame
{
    public class AudioManager : GameComponent
    {
        #region Fields

        bool isActive;

        static AudioManager audioManager = null;

        public static AudioManager Instance
        {
            get
            {
                return audioManager;
            }
        }

        bool isInitialized;

        public static bool IsInitialized
        {
            get { return audioManager.isInitialized; }
        }

        static readonly string soundAssetLocation = "Sounds/";

        Dictionary<string, SoundEffectInstance> soundBank;
        Dictionary<string, Song> musicBank;

        #endregion

        #region Properties

        public static bool IsActive
        {
            get { return audioManager.isActive; }
            set { audioManager.isActive = value; }
        }

        #endregion

        #region Initialization

        private AudioManager(Game game)
            : base(game) { }

        public static void Initialize(Game game)
        {
            audioManager = new AudioManager(game);
            audioManager.soundBank = new Dictionary<string, SoundEffectInstance>();
            audioManager.musicBank = new Dictionary<string, Song>();
            audioManager.isActive = true;

            game.Components.Add(audioManager);

        }
        
        #endregion

        #region Loading Methodes

        public static void LoadSound(string contentName, string alias)
        {
            SoundEffect soundEffect = audioManager.Game.Content.Load<SoundEffect>(soundAssetLocation + contentName);
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

            if (!audioManager.soundBank.ContainsKey(alias))
            {
                audioManager.soundBank.Add(alias, soundEffectInstance);
            }

            audioManager.isInitialized = true;
        }

        public static void LoadSong(string contentName, string alias)
        {
            Song song = audioManager.Game.Content.Load<Song>(soundAssetLocation + contentName);

            if (!audioManager.musicBank.ContainsKey(alias))
            {
                audioManager.musicBank.Add(alias, song);
            }
        }

        public static void LoadSounds()
        {
            //LoadSound("contentName", "alias");
            LoadSound("bulletAmbientCollision", "bulletAmbientCollision");
            LoadSound("shoot", "shoot");
            LoadSound("enemyAttack", "enemyAttack");
            LoadSound("enemyDead", "enemyDead");
            LoadSound("introSound", "introSound");
        }

        public static void LoadMusic()
        {
            //LoadSong("contentName", "alias");
            LoadSong("gameplayMusic", "gameplayMusic");
            LoadSong("gameplayMusic_2", "gameplayMusic_2");
            LoadSong("mainMenuMusic", "mainMenuMusic");
        }

        #endregion

        #region Sound Methods

        public SoundEffectInstance this[string soundName]
        {
            get
            {
                if (audioManager.soundBank.ContainsKey(soundName))
                {
                    return audioManager.soundBank[soundName];
                }
                else
                {
                    return null;
                }
            }
        }

        public static void PlaySound(string soundName)
        {
            if (IsActive)
            {
                if (IsActive)
                    if (audioManager.soundBank.ContainsKey(soundName))
                    {
                        audioManager.soundBank[soundName].Play();
                    }
            }
        }

        public static void PlaySound(string soundName, bool isLooped)
        {
            if (IsActive)
            {
                // If the sound exists, start it
                if (audioManager.soundBank.ContainsKey(soundName))
                {
                    if (audioManager.soundBank[soundName].IsLooped != isLooped)
                    {
                        audioManager.soundBank[soundName].IsLooped = isLooped;
                    }

                    audioManager.soundBank[soundName].Play();
                }
            }
        }


        /// <summary>
        /// Plays a sound by name.
        /// </summary>
        /// <param name="soundName">The name of the sound to play.</param>
        /// <param name="isLooped">Indicates if the sound should loop.</param>
        /// <param name="volume">Indicates if the volume</param>
        public static void PlaySound(string soundName, bool isLooped, float volume)
        {
            if (IsActive)
            {
                // If the sound exists, start it
                if (audioManager.soundBank.ContainsKey(soundName))
                {
                    if (audioManager.soundBank[soundName].IsLooped != isLooped)
                    {
                        audioManager.soundBank[soundName].IsLooped = isLooped;
                    }

                    audioManager.soundBank[soundName].Volume = volume;
                    audioManager.soundBank[soundName].Play();
                }
            }
        }

        /// <summary>
        /// Stops a sound mid-play. If the sound is not playing, this
        /// method does nothing.
        /// </summary>
        /// <param name="soundName">The name of the sound to stop.</param>
        public static void StopSound(string soundName)
        {
            // If the sound exists, stop it
            if (audioManager.soundBank.ContainsKey(soundName))
            {
                audioManager.soundBank[soundName].Stop();
            }
        }

        /// <summary>
        /// Stops all currently playing sounds.
        /// </summary>
        public static void StopSounds()
        {
            foreach (SoundEffectInstance sound in audioManager.soundBank.Values)
            {
                if (sound.State != SoundState.Stopped)
                {
                    sound.Stop();
                }
            }
        }

        /// <summary>
        /// Pause or resume all sounds.
        /// </summary>
        /// <param name="resumeSounds">True to resume all paused sounds or false
        /// to pause all playing sounds.</param>
        public static void PauseResumeSounds(bool resumeSounds)
        {
            if (IsActive)
            {
                SoundState state = resumeSounds ? SoundState.Paused : SoundState.Playing;

                foreach (SoundEffectInstance sound in audioManager.soundBank.Values)
                {
                    if (sound.State == state)
                    {
                        if (resumeSounds)
                        {
                            sound.Resume();
                        }
                        else
                        {
                            sound.Pause();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Play music by name. This stops the currently playing music first. Music will loop until stopped.
        /// </summary>
        /// <param name="musicSoundName">The name of the music sound.</param>
        /// <remarks>If the desired music is not in the music bank, nothing will happen.</remarks>
        public static void PlayMusic(string musicSoundName)
        {
            if (IsActive)
            {
                // If the music sound exists
                if (audioManager.musicBank.ContainsKey(musicSoundName))
                {
                    // Stop the old music sound
                    if (MediaPlayer.State != MediaState.Stopped)
                    {
                        MediaPlayer.Stop();
                    }

                    MediaPlayer.IsRepeating = true;

                    MediaPlayer.Play(audioManager.musicBank[musicSoundName]);                    
                }
            }
        }

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        public static void StopMusic()
        {
            if (MediaPlayer.State != MediaState.Stopped)
            {
                MediaPlayer.Stop();
            }
        }

        public static void Enable(bool state)
        {
            AudioManager.IsActive = state;
            if (!state)
            {
                StopSounds();
                StopMusic();
            }
        }

        #endregion

        #region Get Active Sound

        public static bool IsActiveSong(string nameSong)
        {
            if (MediaPlayer.Queue != null)
            {
                if (MediaPlayer.Queue.ActiveSong != null)
                {
                    Song song = MediaPlayer.Queue.ActiveSong;

                    if (!string.IsNullOrEmpty(song.Name))
                    {
                        if (song.Name == "Sounds\\" + nameSong &&
                            MediaPlayer.State != MediaState.Stopped)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region Instance Disposal Methods


        /// <summary>
        /// Clean up the component when it is disposing.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    foreach (var item in soundBank)
                    {
                        item.Value.Dispose();
                    }
                    soundBank.Clear();
                    soundBank = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }


        #endregion
    }
}
