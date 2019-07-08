using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Emote.Utils
{
    public class EnableGazeFilter : MonoBehaviour
    {
        public static bool m_Enabled = true;
        GazePlotter plotter;

        void Start()
        {
            if (EnableGazeFilter.m_Enabled)
            {
                plotter = GetComponent<GazePlotter>();
                if (plotter)
                {
                    plotter.FilterSmoothingFactor = 0.1f;
                    plotter.UseFilter = true;
                }
            }
        }
    }
}
