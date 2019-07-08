using System.Diagnostics;
using Emote.Models;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public static class EmoteSession
    {
        public static bool randomOptions = false;
        public static Session session;
        public static bool trainingMode = true;
        public static Stopwatch time = new Stopwatch();
        public static int awaitSecondsToShowImages = 10;
        public static bool enableNextKey = false;
        public static int current_resource_id;

        public static string selfie_path = "StaticFiles\\Selfies\\";

        public static double current_time
        {
            get
            {
                return time.Elapsed.TotalSeconds;
            }
        }
    }
}
