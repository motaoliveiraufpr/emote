using System;
using Affdex;
using UnityEngine;
using Emote.Avatar;

namespace Emote.Utils
{
    public class AvatarEmotions : MonoBehaviour
    {
        public const int EXTRA_PRESETS = 5;

        [HideInInspector]
        public static float[] expressions = new float[System.Enum.GetValues(typeof(Expressions)).Length + EXTRA_PRESETS + System.Enum.GetValues(typeof(ExtraPresets)).Length];

        public float[] Joy
        {
            get
            {
                var temp = ResetVector(expressions);

                temp[0] = 1.0f;  // Smile
                temp[11] = 0.2f; // MouthOpen

                return temp;
            }
        }

        public float[] Fear
        {
            get
            {
                var temp = ResetVector(expressions);
                temp[22] = 1.0f; // BrowsU_C
                temp[23] = 0.5f; // BrowsU_L
                temp[24] = 0.5f; // BrowsU_R
                temp[25] = 1.0f; // CheeckSquint_L
                temp[26] = 1.0f; // CheeckSquint_R
                temp[27] = 0.4f; // ChinLowerRaise
                temp[28] = 0.7f; // ChinUpperRaise
                temp[31] = 1.0f; // EyeOpen_L
                temp[32] = 1.0f; // EyeOpen_R
                temp[42] = 0.5f; // JawOpen
                return temp;
            }
        }

        public float[] Disgust
        {
            get
            {
                var temp = ResetVector(expressions);
                temp[20] = 1.0f; // BrownsD_L
                temp[21] = 1.0f; // BrownsD_R
                temp[25] = 1.0f; // CheekSquint_L
                temp[26] = 1.0f; // CheekSquint_R
                temp[27] = 0.6f; // ChinLowerRaise
                temp[28] = 0.6f; // ChinUpperRaise
                temp[42] = 0.2f; // JawOpen
                temp[44] = 0.1f; // LipsFunnel
                return temp;
            }
        }

        public float[] Sadness
        {
            get
            {
                var temp = ResetVector(expressions);
                temp[25] = 0.5f; // CheekSquint_L
                temp[26] = 0.5f; // CheekSquint_R
                temp[29] = 0.2f; // EyeBlink_L
                temp[30] = 0.2f; // EyeBlink_R
                temp[33] = 0.7f; // EyeSquint_L
                temp[34] = 0.7f; // EyeSquint_R
                temp[44] = 0.1f; // LipsFunnel
                temp[45] = 0.4f; // LipsLowerClose
                return temp;
            }
        }

        public float[] Anger
        {
            get
            {
                var temp = ResetVector(expressions);
                temp[20] = 0.7f; // BrownsD_L
                temp[21] = 0.7f; // BrownsD_R
                temp[34] = 1.0f; // EyeSquint_R
                temp[43] = 0.4f; // JawRight
                temp[45] = 0.2f; // LipsLowerClose
                temp[46] = 0.2f; // LipsUpperClose
                temp[63] = 0.8f; // Sneer
                return temp;
            }
        }

        public float[] Surprise
        {
            get
            {
                var temp = ResetVector(expressions);
                temp[22] = 1.0f; // BrownsU_C
                temp[23] = 0.5f; // BrownsU_L
                temp[24] = 0.5f; // BrownsU_R
                temp[39] = 0.9f; // JawChew
                temp[42] = 0.5f; // JawOpen
                return temp;
            }
        }

        public float[] Neutral
        {
            get
            {
                var temp = ResetVector(expressions);
                return temp;
            }
        }

        public static Emotions[] GetStaticEmotions()
        {
            Emotions[] emotions =
            {
                Emotions.Joy,
                Emotions.Fear,
                Emotions.Disgust,
                Emotions.Sadness,
                Emotions.Anger,
                Emotions.Surprise,
                Emotions.Neutral
            };
            return emotions;
        }

        public static Emotions GetRandomEmotion()
        {
            int length = Enum.GetValues(typeof(Emotions)).Length;
            int index = UnityEngine.Random.Range(0, length);

            var emotion = (Emotions)index;

            if (emotion == Emotions.Valence)
            {
                return GetRandomEmotion();
            } else if (emotion == Emotions.Contempt)
            {
                return GetRandomEmotion();
            } else if (emotion == Emotions.Engagement)
            {
                return GetRandomEmotion();
            }

            return emotion;
        }

        public static Emotions[] GetRandomEmotions(int number = 1)
        {
            if (number >= 0)
            {
                Emotions[] emotions = new Emotions[number];
                for (int i = 0; i < number; i++)
                {
                    emotions[i] = GetRandomEmotion();
                }

                return emotions;
            }
            return new Emotions[0];
        }

        private static float[] ResetVector(float[] vector)
        {
            for (int i = 0; i < vector.Length; i++)
                vector[i] = 0.0f;
            return vector;
        }
    }
}