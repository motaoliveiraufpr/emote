using System;

namespace Emote.Models
{
    public class Answer
    {
        public int id;
        public int session_id;
        public int pipeline_type_id;
        public string correct;
        public string answer;
        public DateTime created_at;
        public string file;
    }
}
