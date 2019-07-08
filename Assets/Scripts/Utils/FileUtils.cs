using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Emote.Utils
{
    public static class FileUtils
    {
        public static string[] GetFileNames(string path)
        {
            return Directory.GetFiles(path);
        }
    }
}