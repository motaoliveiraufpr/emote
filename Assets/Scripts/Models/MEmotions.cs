using System;

namespace Emote.Models
{
    public class Emotion
    {
        public int id;
        public int session_id;
        public float Joy;
        public float Fear;
        public float Disgust;
        public float Sadness;
        public float Anger;
        public float Surprise;
        public float Contempt;
        public float Valence;
        public float Engagement;
        public DateTime created_at;
        public int emotions_id;
        public int answers_id;
    }

    public class MEmotions
    {
        public int id;
        public string file;
        public int emotion;
        public DateTime created_at;
    }

    public class MEmotionsSettings
    {
        public int min_emotions = 0;
        public int max_emotions = 0;
    }
}