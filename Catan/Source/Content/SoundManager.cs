using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Catan.Source.Content
{
    public enum SfxId
    {
        ConstrucaoEstrada,
        ConstrucaoCasa,
        LadraoDado7,
        Ovelha,
        Pedra,
        Planta,
        Tijolo,
        TijoloCaindo
    }

    public class SoundManager
    {
        private static SoundManager _instance;

        private readonly Dictionary<SfxId, SoundEffect> _soundEffects = new();
        private float _globalVolume = 0.1f;

        private static readonly Dictionary<SfxId, string> _soundPaths = new()
        {
            [SfxId.ConstrucaoEstrada] = "Sons/construcao_estrada",
            [SfxId.ConstrucaoCasa] = "Sons/construcao_casa",
            [SfxId.LadraoDado7] = "Sons/ladrao",
            [SfxId.Ovelha] = "Sons/ovelhaBee",
            [SfxId.Pedra] = "Sons/pedra",
            [SfxId.Planta] = "Sons/planta",
            [SfxId.Tijolo] = "Sons/tijolo",
            [SfxId.TijoloCaindo] = "Sons/tijolo_caindo",
        };

        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundManager();
                }

                return _instance;
            }
        }

        public float GlobalVolume
        {
            get => _globalVolume;
            set => _globalVolume = MathHelper.Clamp(value, 0f, 1f);
        }

        private SoundManager()
        {
        }

        public void LoadContent(ContentManager content)
        {
            foreach (var sound in _soundPaths)
            {
                _soundEffects[sound.Key] = content.Load<SoundEffect>(sound.Value);
            }
        }

        public bool Play(SfxId soundId)
        {
            return Play(soundId, 1f, 0f, 0f);
        }

        public bool Play(SfxId soundId, float volume, float pitch = 0f, float pan = 0f)
        {
            if (!_soundEffects.TryGetValue(soundId, out var soundEffect))
            {
                throw new ArgumentException($"Som {soundId} nao encontrado.", nameof(soundId));
            }

            volume = MathHelper.Clamp(volume, 0f, 1f) * GlobalVolume;
            pitch = MathHelper.Clamp(pitch, -1f, 1f);
            pan = MathHelper.Clamp(pan, -1f, 1f);

            return soundEffect.Play(volume, pitch, pan);
        }
    }
}
