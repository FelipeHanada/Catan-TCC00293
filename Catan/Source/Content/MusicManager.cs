using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Catan.Source.Content
{
    public enum MusicId
    {
        MenuPrincipal,
        Partida,
        FimDeJogo,
    }

    public class MusicManager
    {
        private static MusicManager _instance;

        private readonly Dictionary<MusicId, Song> _songs = new();
        private float _globalVolume = 1f;
        private MusicId? _currentMusic;

        private static readonly Dictionary<MusicId, string> _musicPaths = new()
        {
            [MusicId.MenuPrincipal] = "Musicas/Loop_The_Old_Tower_Inn",
            [MusicId.Partida] = "Musicas/Woodland Fantasy",
            [MusicId.FimDeJogo] = "Musicas/Loop_The_Old_Tower_Inn",
        };

        public static MusicManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MusicManager();
                }

                return _instance;
            }
        }

        public float GlobalVolume
        {
            get => _globalVolume;
            set
            {
                _globalVolume = MathHelper.Clamp(value, 0f, 1f);
                MediaPlayer.Volume = _globalVolume;
            }
        }

        public bool IsRepeating
        {
            get => MediaPlayer.IsRepeating;
            set => MediaPlayer.IsRepeating = value;
        }

        public MusicId? CurrentMusic => _currentMusic;

        private MusicManager()
        {
        }

        public void LoadContent(ContentManager content)
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = _globalVolume;

            foreach (var music in _musicPaths)
            {
                if (_songs.ContainsKey(music.Key))
                {
                    continue;
                }

                try
                {
                    _songs[music.Key] = content.Load<Song>(music.Value);
                }
                catch (ContentLoadException exception)
                {
                    Debug.WriteLine($"Musica {music.Key} nao encontrada em '{music.Value}': {exception.Message}");
                }
            }
        }

        public bool Play(MusicId? musicId)
        {
            if (musicId == null)
            {
                Stop();
                return true;
            }

            return Play(musicId.Value);
        }

        public bool Play(MusicId musicId)
        {
            if (!_songs.TryGetValue(musicId, out var song))
            {
                Debug.WriteLine($"Musica {musicId} nao encontrada.");
                Stop();
                return false;
            }

            if (_currentMusic == musicId && MediaPlayer.State == MediaState.Playing)
            {
                return true;
            }

            MediaPlayer.Play(song);
            _currentMusic = musicId;
            return true;
        }

        public void Stop()
        {
            if (_currentMusic == null)
            {
                return;
            }

            MediaPlayer.Stop();
            _currentMusic = null;
        }
    }
}
