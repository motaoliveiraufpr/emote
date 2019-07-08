using System.IO;
using UnityEditor;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */

namespace Emote.Utils
{
#if UNITY_EDITOR
    public static class PhotoUtils
    {
        public static string m_TempPath = "";

        public static void SaveTempBytes(byte[] bytes)
        {
            m_TempPath = FileUtil.GetUniqueTempPathInProject();
            File.WriteAllBytes(m_TempPath, bytes);
        }

        public static byte[] ReadTempBytes()
        {
            return File.ReadAllBytes(m_TempPath);
        }
    }
#endif
}
