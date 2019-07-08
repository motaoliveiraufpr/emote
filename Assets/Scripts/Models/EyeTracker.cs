using System;

namespace Emote.Models
{
    public class EyeTracker
    {
        public int id;
        public string file;
        public int emotion;
        public DateTime created_at;
    }

    public class EyeTrackerSettings
    {
        public int min_resources = 0;
        public int max_resources = 0;
    }
}