using System;

namespace Emote.Models
{
    public class Videos
    {
        public int id;
        public string file;
        public int emotion;
        public DateTime created_at;
    }

    public class VideoSettings
    {
        public int min_videos = 0;
        public int max_videos = 0;
    }
}