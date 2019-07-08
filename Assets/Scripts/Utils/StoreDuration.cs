using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using Emote.Models;
using Emote.Database;

namespace Emote.Utils
{
    public class StoreDuration : MonoBehaviour
    {
        public string m_PipelineType;
        public PipelineType pipelineType
        {
            get
            {
                if (!string.IsNullOrEmpty(m_PipelineType))
                {
                    List<PipelineType> types = DatabaseManager.m_PipelineType;
                    foreach (var type in types)
                    {
                        if (type.type == m_PipelineType)
                        {
                            return type;
                        }
                    }
                }
                return null;
            }
        }
        private Stopwatch time;
        private int pipelineID;

        void Start()
        {
            time = new Stopwatch();
            time.Start();

            pipelineID = pipelineType.id;
        }

        public void StoreData()
        {
            if (time == null)
            {
                time = new Stopwatch();
                time.Start();
            }
            time.Stop();

            if (!EmoteSession.trainingMode)
            {
                PypelineDuration duration = new PypelineDuration();

                duration.pipeline_type_id = pipelineID;
                duration.session_id = EmoteSession.session.id;
                duration.time = time.Elapsed.TotalSeconds;

                DatabaseManager.m_PipelineDuration = duration;
            }
        }
    }
}