using System.IO;
using UnityEngine;
using ItSeez3D.AvatarSdk.Core;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */

namespace Emote.Utils
{
    public static class AvatarManager
    {
        public static IPersistentStorage storage
        {
            get
            {
                return AvatarSdkMgr.Storage();
            }
        }

        public static Texture2D GetTexture(string avatarCode)
        {
            string srcTextureFile = storage.GetAvatarFilename(avatarCode, AvatarFile.TEXTURE);
            Texture2D texture = null;
            
            if (File.Exists(srcTextureFile))
            {
                byte[] data = File.ReadAllBytes(srcTextureFile);
                texture = new Texture2D(2, 2);
                texture.LoadImage(data);
            }

            return texture;
        }
    }
}
