using System;
using UnityEngine;

namespace Game.Core
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundPool _soundPool;

        public SoundInstance PlaySound(SoundMetaAsset soundAsset, bool autoStart)
        {
            SoundInstance sound = _soundPool.Get();
            sound.ApplySettings(soundAsset);
            sound.SetSpatial(false);

            if (autoStart)
            {
                sound.Play();
            }
            else
            {
                sound.Stop();
            }


            return sound;
        }

        public SoundInstance PlaySpatialSound(SoundMetaAsset soundAsset, Vector3 position, bool autoStart)
        {
            if (!soundAsset.IsSpatial)
                throw new ArgumentException($"SoundMetaAsset '{soundAsset.name}' is not spatial.");

            SoundInstance sound = _soundPool.Get();
            sound.ApplySettings(soundAsset);
            sound.SetPosition(position);

            if (autoStart)
                sound.Play();

            return sound;
        }
    }
}
