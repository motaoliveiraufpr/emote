using System;

namespace Emote.Models
{
    public class Images
    {
        public int id;
        public string file;
        public int emotion;
        public DateTime created_at;
    }

    public class ImageSettings
    {
        public int min_images = 0;
        public int max_images = 0;
    }
}
