using UnityEngine;

namespace Game.Core
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundPool _soundPool;

        public SoundInstance PlaySound(SoundMetaAsset soundAsset, bool autoStart)
        {
            SoundInstance sound = _soundPool.Get();
            sound.ApplySettingsFromAsset(soundAsset);

            if (autoStart)
                sound.Play();

            return sound;
        }

        public SoundInstance PlaySpatialSound(SoundMetaAsset soundAsset, Vector3 position, bool autoStart)
        {
            SoundInstance sound = _soundPool.Get();
            sound.ApplySettingsFromAsset(soundAsset);

            sound.SetSpatial(true);
            sound.SetPosition(position);

            if (autoStart)
                sound.Play();

            return sound;
        }
    }
}
