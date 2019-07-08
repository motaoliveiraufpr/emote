using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */

namespace Emote.Utils {
    public class AvatarTextureCollection : MonoBehaviour {
        public List<Texture2D> m_HeadTextureList;
        public List<Texture2D> m_FullTextureList;

        public Texture2D GetRandomTexture(AvatarType type)
        {
            int index = 0;
            switch (type)
            {
                case AvatarType.FULL:
                    index = Random.Range(0, m_FullTextureList.Count);
                    return m_FullTextureList.ElementAt(index);
                case AvatarType.HEAD:
                    index = Random.Range(0, m_HeadTextureList.Count);
                    return m_HeadTextureList.ElementAt(index);
                default:
                    return null;
            }
        }
    }
}
