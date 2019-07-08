using System;

namespace Emote.Models
{
    public class Audios
    {
        public int id;
        public string file;
        public int emotion;
        public DateTime created_at;
    }

    public class AudioSettings
    {
        public int min_audios = 0;
        public int max_audios = 0;
    }
}