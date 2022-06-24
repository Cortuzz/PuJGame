using UnityEngine;
using UnityEngine.Audio;

namespace UIInteractions.Settings
{
    public class VolumeSettings: MonoBehaviour
    {
        private const int Difference = 80;
        
        public AudioMixer audioMixer;

        private static float FixVolume(float volume)
        {
            return volume - Difference;
        }
        
        public void SetGeneralVolume(float volume)
        {
            audioMixer.SetFloat("GeneralMixer", FixVolume(volume));
        }

        public void SetGameVolume(float volume)
        {
            audioMixer.SetFloat("SoundsMixer", FixVolume(volume));
        }
        
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicMixer", FixVolume(volume));
        }
    }
}