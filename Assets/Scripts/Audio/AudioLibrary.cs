using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.Audio
{

    [CreateAssetMenu(fileName = "AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        public List<AudioItem> Items;

        public AudioClip GetSound(string name)
        {
            AudioItem item = Items.FirstOrDefault(i => i.Name == name);
            if (item == null) return null;
            return item.Clip;
        }
    }

}