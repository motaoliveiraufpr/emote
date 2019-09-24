#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;
using System.Collections.Generic;
using Emote.Models;
using Emote.Utils;
using System.IO;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Database
{
    public class DatabaseManager : MonoBehaviour
    {
        // The path to database
        public static string m_DatabasePath;

        #region Connection
        public static IDbConnection m_Connection;
        public static IDbCommand m_Command;
        public static IDataReader m_Reader;
        #endregion

        #region Avatar type
        [HideInInspector]
        public static int m_AvatarType
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, default_avatar_type FROM settings WHERE id = 0;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();
                    if (m_Reader.Read())
                    {
                        int id = m_Reader.GetInt32(0);
                        int type = m_Reader.GetInt32(1);

                        return type;
                    }
                }
                return 0;
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "UPDATE settings SET default_avatar_type = @defaultAvatar WHERE id = 0;";
                    m_Command.Parameters.Add(new SqliteParameter("@defaultAvatar", value));
                    m_Command.CommandText = query;

                    m_Command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Capture device
        [HideInInspector]
        public static string m_CaptureDevice
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, default_camera_device FROM settings WHERE id = 0;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();
                    if (m_Reader.Read())
                    {
                        int id = m_Reader.GetInt32(0);
                        string device = m_Reader.GetString(1);

                        return device;
                    }
                }
                return null;
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "UPDATE settings SET default_camera_device = @defaultCamera WHERE id = 0;";
                    m_Command.Parameters.Add(new SqliteParameter("@defaultCamera", value));
                    m_Command.CommandText = query;

                    m_Command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Random avatar texture
        [HideInInspector]
        public static bool m_RandomAvatarTexture
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, random_avatar_texture FROM settings WHERE id = 0;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();
                    if (m_Reader.Read())
                    {
                        int id = m_Reader.GetInt32(0);
                        bool random_texure = m_Reader.GetBoolean(1);

                        return random_texure;
                    }
                }
                return true;
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "UPDATE settings SET random_avatar_texture = @randomTexture WHERE id = 0;";
                    m_Command.Parameters.Add(new SqliteParameter("@randomTexture", value));
                    m_Command.CommandText = query;

                    m_Command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Live avatar
        [HideInInspector]
        public static bool m_LiveAvatar
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, live_avatar FROM settings WHERE id = 0;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();
                    if (m_Reader.Read())
                    {
                        int id = m_Reader.GetInt32(0);
                        bool live_avatar = m_Reader.GetBoolean(1);

                        return live_avatar;
                    }
                }
                return false;
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "UPDATE settings SET live_avatar = @liveAvatar WHERE id = 0;";
                    m_Command.Parameters.Add(new SqliteParameter("@liveAvatar", value));
                    m_Command.CommandText = query;

                    m_Command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Quiz enabled
        [HideInInspector]
        public static bool m_QuizEnabled
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, quiz_enabled FROM settings WHERE id = 0;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();
                    if (m_Reader.Read())
                    {
                        int id = m_Reader.GetInt32(0);
                        bool quiz_enabled = m_Reader.GetBoolean(1);

                        return quiz_enabled;
                    }
                }
                return false;
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "UPDATE settings SET quiz_enabled = @quizEnabled WHERE id = 0;";
                    m_Command.Parameters.Add(new SqliteParameter("@quizEnabled", value));
                    m_Command.CommandText = query;

                    m_Command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Random questions
        [HideInInspector]
        public static bool m_RandomQuestions
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, random_questions FROM settings WHERE id = 0;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();
                    if (m_Reader.Read())
                    {
                        int id = m_Reader.GetInt32(0);
                        bool random_questions = m_Reader.GetBoolean(1);

                        return random_questions;
                    }
                }
                return false;
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "UPDATE settings SET random_questions = @randomQuestions WHERE id = 0;";
                    m_Command.Parameters.Add(new SqliteParameter("@randomQuestions", value));
                    m_Command.CommandText = query;

                    m_Command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Max questions
        [HideInInspector]
        public static int m_MaxQuestions
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, max_questions FROM settings WHERE id = 0;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();
                    if (m_Reader.Read())
                    {
                        int id = m_Reader.GetInt32(0);
                        int max_questions = m_Reader.GetInt32(1);

                        return max_questions;
                    }
                }
                return 6;
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "UPDATE settings SET max_questions = @maxQuestions WHERE id = 0;";
                    m_Command.Parameters.Add(new SqliteParameter("@maxQuestions", value));
                    m_Command.CommandText = query;

                    m_Command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Max images question
        [HideInInspector]
        public static ImageSettings m_MinMaxImages
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "SELECT min_image_questions, max_image_questions FROM settings WHERE id = 0;";
                        m_Command.CommandText = query;

                        m_Reader = m_Command.ExecuteReader();
                        if (m_Reader.Read())
                        {
                            ImageSettings imageSettings = new ImageSettings();

                            imageSettings.min_images = m_Reader.GetInt32(0);
                            imageSettings.max_images = m_Reader.GetInt32(1);

                            return imageSettings;
                        }
                    } catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                    }
                }
                return new ImageSettings();
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "UPDATE settings SET min_image_questions = @min, max_image_questions = @max WHERE id = 0;";
                        m_Command.Parameters.Add(new SqliteParameter("@min", value.min_images));
                        m_Command.Parameters.Add(new SqliteParameter("@max", value.max_images));
                        m_Command.CommandText = query;

                        m_Command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Max video questions
        [HideInInspector]
        public static VideoSettings m_MinMaxVideos
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "SELECT min_video_questions, max_video_questions FROM settings WHERE id = 0;";
                        m_Command.CommandText = query;

                        m_Reader = m_Command.ExecuteReader();
                        if (m_Reader.Read())
                        {
                            VideoSettings videoSettings = new VideoSettings();

                            videoSettings.min_videos = m_Reader.GetInt32(0);
                            videoSettings.max_videos = m_Reader.GetInt32(1);

                            return videoSettings;
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                    }
                }
                return new VideoSettings();
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "UPDATE settings SET min_video_questions = @min, max_video_questions = @max WHERE id = 0;";
                        m_Command.Parameters.Add(new SqliteParameter("@min", value.min_videos));
                        m_Command.Parameters.Add(new SqliteParameter("@max", value.max_videos));
                        m_Command.CommandText = query;

                        m_Command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Max audio questions
        [HideInInspector]
        public static Models.AudioSettings m_MinMaxAudios
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "SELECT min_audio_questions, max_audio_questions FROM settings WHERE id = 0;";
                        m_Command.CommandText = query;

                        m_Reader = m_Command.ExecuteReader();
                        if (m_Reader.Read())
                        {
                            Models.AudioSettings audioSettings = new Models.AudioSettings();

                            audioSettings.min_audios = m_Reader.GetInt32(0);
                            audioSettings.max_audios = m_Reader.GetInt32(1);

                            return audioSettings;
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                    }
                }
                return new Models.AudioSettings();
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "UPDATE settings SET min_audio_questions = @min, max_audio_questions = @max WHERE id = 0;";
                        m_Command.Parameters.Add(new SqliteParameter("@min", value.min_audios));
                        m_Command.Parameters.Add(new SqliteParameter("@max", value.max_audios));
                        m_Command.CommandText = query;

                        m_Command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Max resource questions
        [HideInInspector]
        public static EyeTrackerSettings m_MinMaxResources
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "SELECT min_resource_questions, max_resource_questions FROM settings WHERE id = 0;";
                        m_Command.CommandText = query;

                        m_Reader = m_Command.ExecuteReader();
                        if (m_Reader.Read())
                        {
                            EyeTrackerSettings resourceSettings = new EyeTrackerSettings();

                            resourceSettings.min_resources = m_Reader.GetInt32(0);
                            resourceSettings.max_resources = m_Reader.GetInt32(1);

                            return resourceSettings;
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                    }
                }
                return new EyeTrackerSettings();
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "UPDATE settings SET min_resource_questions = @min, max_resource_questions = @max WHERE id = 0;";
                        m_Command.Parameters.Add(new SqliteParameter("@min", value.min_resources));
                        m_Command.Parameters.Add(new SqliteParameter("@max", value.max_resources));
                        m_Command.CommandText = query;

                        m_Command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Max emotion questions
        [HideInInspector]
        public static MEmotionsSettings m_MinMaxEmotions
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "SELECT min_emotion_questions, max_emotion_questions FROM settings WHERE id = 0;";
                        m_Command.CommandText = query;

                        m_Reader = m_Command.ExecuteReader();
                        if (m_Reader.Read())
                        {
                            MEmotionsSettings emotionSettings = new MEmotionsSettings();

                            emotionSettings.min_emotions = m_Reader.GetInt32(0);
                            emotionSettings.max_emotions = m_Reader.GetInt32(1);

                            return emotionSettings;
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                    }
                }
                return new MEmotionsSettings();
            }
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    try
                    {
                        var query = "UPDATE settings SET min_emotion_questions = @min, max_emotion_questions = @max WHERE id = 0;";
                        m_Command.Parameters.Add(new SqliteParameter("@min", value.min_emotions));
                        m_Command.Parameters.Add(new SqliteParameter("@max", value.max_emotions));
                        m_Command.CommandText = query;

                        m_Command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Create new session
        [HideInInspector]
        public static Session m_Session
        {
            get
            {
                if (!Emote.Utils.EmoteSession.trainingMode)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    if (m_Command != null)
                    {
                        var query = "SELECT id, reference, created_at, time FROM session ORDER BY id DESC LIMIT 1;";
                        m_Command.CommandText = query;

                        m_Reader = m_Command.ExecuteReader();
                        if (m_Reader.Read())
                        {
                            Session session = new Session();

                            session.id = m_Reader.GetInt32(0);
                            session.reference = m_Reader.GetString(1);
                            session.created_at = m_Reader.GetDateTime(2);
                            session.time = m_Reader.GetDouble(3);

                            return session;
                        }
                    }
                    return new Session();
                }
                else
                {
                    return Emote.Utils.EmoteSession.session;
                }
            }
            set
            {
                if (!Emote.Utils.EmoteSession.trainingMode)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    if (m_Command != null)
                    {
                        // verify if session exists
                        var query = "SELECT id FROM session WHERE reference = @params;";
                        m_Command.Parameters.Add(new SqliteParameter("@params", value.reference));
                        m_Command.CommandText = query;

                        m_Reader = m_Command.ExecuteReader();
                        if (m_Reader.Read())
                        {
                            m_Reader.Close();

                            query = "UPDATE session SET selfie=@selfie, time=@time WHERE id=@id;";
                            m_Command.Parameters.Add(new SqliteParameter("@selfie", value.selfie));
                            m_Command.Parameters.Add(new SqliteParameter("@time", value.time));

                            m_Command.Parameters.Add(new SqliteParameter("@id", value.id));
                            m_Command.CommandText = query;

                            m_Command.ExecuteNonQuery();
                        }
                        else
                        {
                            m_Reader.Close();
                            query = "INSERT INTO session(reference, selfie, time) values(@params, @selfie, @time);";
                            m_Command.Parameters.Add(new SqliteParameter("@params", value.reference));
                            m_Command.Parameters.Add(new SqliteParameter("@selfie", value.selfie));
                            m_Command.Parameters.Add(new SqliteParameter("@time", value.time));
                            m_Command.CommandText = query;

                            m_Command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        #endregion

        #region Pipeline Type
        [HideInInspector]
        public static List<PipelineType> m_PipelineType
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, type FROM pipeline_type ORDER BY id DESC;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();

                    List<PipelineType> list = new List<PipelineType>();
                    while(m_Reader.Read())
                    {
                        PipelineType type = new PipelineType();
                        type.id = m_Reader.GetInt32(0);
                        type.type = m_Reader.GetString(1);

                        list.Add(type);
                    }
                    return list;
                }
                return new List<PipelineType>();
            }
        }
        #endregion

        #region Pipeline Duration
        [HideInInspector]
        public static PypelineDuration m_PipelineDuration
        {
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "INSERT INTO pipeline_duration(pipeline_type_id, session_id, time) values(@pipeline_type_id, @session_id, @time);";
                    try
                    {
                        m_Command.CommandText = query;
                        m_Command.Parameters.Add(new SqliteParameter("@pipeline_type_id", value.pipeline_type_id));
                        m_Command.Parameters.Add(new SqliteParameter("@session_id", value.session_id));
                        m_Command.Parameters.Add(new SqliteParameter("@time", value.time));

                        m_Command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Answers
        [HideInInspector]
        public static Models.Answer m_LastAnswer
        {
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id FROM answers WHERE id = (SELECT MAX(id) FROM answers);";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();

                    Answer answer = new Answer();
                    if (m_Reader.Read())
                    {
                        answer.id = m_Reader.GetInt32(0);
                    }
                    return answer;
                }
                return new Answer();
            }
        }

        [HideInInspector]
        public static Answer m_Answers
        {
            set
            {
                if (!Emote.Utils.EmoteSession.trainingMode)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    if (m_Command != null)
                    {
                        var query = "INSERT INTO answers(session_id, pipeline_type_id, correct, answer, file) values(@session_id, @pipeline_type_id, @correct, @answer, @file);";
                        m_Command.Parameters.Add(new SqliteParameter("@session_id", value.session_id));
                        m_Command.Parameters.Add(new SqliteParameter("@pipeline_type_id", value.pipeline_type_id));
                        m_Command.Parameters.Add(new SqliteParameter("@correct", value.correct));
                        m_Command.Parameters.Add(new SqliteParameter("@answer", value.answer));
                        m_Command.Parameters.Add(new SqliteParameter("@file", value.file));
                        m_Command.CommandText = query;

                        m_Command.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion

        #region Images
        [HideInInspector]
        public static List<Images> m_Images
        {
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "INSERT INTO images(file, emotion) values(@path, @expression);";
                    try
                    {
                        foreach (Images image in value)
                        {
                            if (m_Reader != null)
                            {
                                m_Reader.Close();
                            }
                            m_Command.CommandText = query;
                            m_Command.Parameters.Add(new SqliteParameter("@path", image.file));
                            m_Command.Parameters.Add(new SqliteParameter("@expression", image.emotion));

                            m_Command.ExecuteNonQuery();
                        }
                    } catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, file, emotion, created_at FROM images ORDER BY id DESC;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();

                    List<Images> list = new List<Images>();
                    while (m_Reader.Read())
                    {
                        Images image = new Images();
                        image.id = m_Reader.GetInt32(0);
                        image.file = m_Reader.GetString(1);
                        image.emotion = m_Reader.GetInt32(2);
                        image.created_at = m_Reader.GetDateTime(3);

                        list.Add(image);
                    }
                    return list;
                }
                return new List<Images>();
            }
        }

        public static void RemoveImages(List<Images> images)
        {
            var query = "DELETE FROM images WHERE id = @id;";
            try
            {
                foreach (var image in images)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    m_Command.CommandText = query;
                    m_Command.Parameters.Add(new SqliteParameter("@id", image.id));

                    m_Command.ExecuteNonQuery();
                }
            } catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                return;
            }
        }
        #endregion

        #region Videos
        [HideInInspector]
        public static List<Videos> m_Videos
        {
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "INSERT INTO videos(file, emotion) values(@path, @expression);";
                    try
                    {
                        foreach (Videos video in value)
                        {
                            if (m_Reader != null)
                            {
                                m_Reader.Close();
                            }
                            m_Command.CommandText = query;
                            m_Command.Parameters.Add(new SqliteParameter("@path", video.file));
                            m_Command.Parameters.Add(new SqliteParameter("@expression", video.emotion));

                            m_Command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, file, emotion, created_at FROM videos ORDER BY id DESC;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();

                    List<Videos> list = new List<Videos>();
                    while (m_Reader.Read())
                    {
                        Videos video = new Videos();
                        video.id = m_Reader.GetInt32(0);
                        video.file = m_Reader.GetString(1);
                        video.emotion = m_Reader.GetInt32(2);
                        video.created_at = m_Reader.GetDateTime(3);

                        list.Add(video);
                    }
                    return list;
                }
                return new List<Videos>();
            }
        }

        public static void RemoveVideos(List<Videos> videos)
        {
            var query = "DELETE FROM videos WHERE id = @id;";
            try
            {
                foreach (var video in videos)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    m_Command.CommandText = query;
                    m_Command.Parameters.Add(new SqliteParameter("@id", video.id));

                    m_Command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                return;
            }
        }
        #endregion

        #region Audios
        [HideInInspector]
        public static List<Audios> m_Audios
        {
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "INSERT INTO audios(file, emotion) values(@path, @expression);";
                    try
                    {
                        foreach (Audios audio in value)
                        {
                            if (m_Reader != null)
                            {
                                m_Reader.Close();
                            }
                            m_Command.CommandText = query;

                            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(audio.file);
                            m_Command.Parameters.Add(new SqliteParameter("@path", System.Text.Encoding.UTF8.GetString(utf8)));
                            m_Command.Parameters.Add(new SqliteParameter("@expression", audio.emotion));

                            m_Command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, file, emotion, created_at FROM audios ORDER BY id DESC;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();

                    List<Audios> list = new List<Audios>();
                    while (m_Reader.Read())
                    {
                        Audios audio = new Audios();
                        audio.id = m_Reader.GetInt32(0);
                        audio.file = m_Reader.GetString(1);
                        audio.emotion = m_Reader.GetInt32(2);
                        audio.created_at = m_Reader.GetDateTime(3);

                        list.Add(audio);
                    }
                    return list;
                }
                return new List<Audios>();
            }
        }

        public static void RemoveAudios(List<Audios> audios)
        {
            var query = "DELETE FROM audios WHERE id = @id;";
            try
            {
                foreach (var audio in audios)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    m_Command.CommandText = query;
                    m_Command.Parameters.Add(new SqliteParameter("@id", audio.id));

                    m_Command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                return;
            }
        }
        #endregion

        #region Resources
        public static void RemoveResources(List<EyeTracker> resources)
        {
            var query = "DELETE FROM resources WHERE id = @id;";
            try
            {
                foreach (var resource in resources)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    m_Command.CommandText = query;
                    m_Command.Parameters.Add(new SqliteParameter("@id", resource.id));

                    m_Command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                return;
            }
        }

        [HideInInspector]
        public static List<EyeTracker> m_Resources
        {
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "INSERT INTO resources(file, emotion) values(@path, @expression);";
                    try
                    {
                        foreach (EyeTracker resource in value)
                        {
                            if (m_Reader != null)
                            {
                                m_Reader.Close();
                            }
                            m_Command.CommandText = query;

                            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(resource.file);
                            m_Command.Parameters.Add(new SqliteParameter("@path", System.Text.Encoding.UTF8.GetString(utf8)));
                            m_Command.Parameters.Add(new SqliteParameter("@expression", resource.emotion));

                            m_Command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, file, emotion, created_at FROM resources ORDER BY id DESC;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();

                    List<EyeTracker> list = new List<EyeTracker>();
                    while (m_Reader.Read())
                    {
                        EyeTracker resource = new EyeTracker();
                        resource.id = m_Reader.GetInt32(0);
                        resource.file = m_Reader.GetString(1);
                        resource.emotion = m_Reader.GetInt32(2);
                        resource.created_at = m_Reader.GetDateTime(3);

                        list.Add(resource);
                    }
                    return list;
                }
                return new List<EyeTracker>();
            }
        }
        #endregion

        #region Emotions
        [HideInInspector]
        public static List<Models.Emotion> m_Emotion
        {
            set
            {
                if (!Emote.Utils.EmoteSession.trainingMode)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    if (m_Command != null)
                    {
                        IDbTransaction transaction = m_Connection.BeginTransaction();
                        m_Command.Transaction = transaction;

                        var query = "INSERT INTO emotion(session_id, joy, fear, disgust, sadness, anger, surprise, contempt, valence, engagement, created_at, emotions_id, answers_id) values(@session_id, @joy, @fear, @disgust, @sadness, @anger, @surprise, @contempt, @valence, @engagement, @created_at, @emotions_id, @answers_id);";
                        try
                        {
                            for (int i=0; i < value.Count; i++)
                            {
                                var emotion = value[i];

                                if (m_Reader != null)
                                {
                                    m_Reader.Close();
                                }
                                m_Command.CommandText = query;

                                m_Command.Parameters.Add(new SqliteParameter("@session_id", emotion.session_id));
                                m_Command.Parameters.Add(new SqliteParameter("@joy", emotion.Joy));
                                m_Command.Parameters.Add(new SqliteParameter("@fear", emotion.Fear));
                                m_Command.Parameters.Add(new SqliteParameter("@disgust", emotion.Disgust));
                                m_Command.Parameters.Add(new SqliteParameter("@sadness", emotion.Sadness));
                                m_Command.Parameters.Add(new SqliteParameter("@anger", emotion.Anger));
                                m_Command.Parameters.Add(new SqliteParameter("@surprise", emotion.Surprise));
                                m_Command.Parameters.Add(new SqliteParameter("@contempt", emotion.Contempt));
                                m_Command.Parameters.Add(new SqliteParameter("@valence", emotion.Valence));
                                m_Command.Parameters.Add(new SqliteParameter("@engagement", emotion.Engagement));
                                m_Command.Parameters.Add(new SqliteParameter("@created_at", emotion.created_at));
                                m_Command.Parameters.Add(new SqliteParameter("@emotions_id", emotion.emotions_id));
                                m_Command.Parameters.Add(new SqliteParameter("@answers_id", emotion.answers_id));

                                m_Command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            m_Command.Transaction = null;
                        }
                        catch (Exception exception)
                        {
                            Debug.LogError(exception.Message);
                            transaction.Rollback();
                            return;
                        }
                    }
                }
            }
        }

        [HideInInspector]
        public static List<MEmotions> m_Emotions
        {
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "INSERT INTO emotions(file, emotion) values(@path, @expression);";
                    try
                    {
                        foreach (MEmotions emotion in value)
                        {
                            if (m_Reader != null)
                            {
                                m_Reader.Close();
                            }
                            m_Command.CommandText = query;

                            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(emotion.file);
                            m_Command.Parameters.Add(new SqliteParameter("@path", System.Text.Encoding.UTF8.GetString(utf8)));
                            m_Command.Parameters.Add(new SqliteParameter("@expression", emotion.emotion));

                            m_Command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception.Message);
                        return;
                    }
                }
            }
            get
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Command != null)
                {
                    var query = "SELECT id, file, emotion, created_at FROM emotions ORDER BY id DESC;";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();

                    List<MEmotions> list = new List<MEmotions>();
                    while (m_Reader.Read())
                    {
                        MEmotions emotion = new MEmotions();
                        emotion.id = m_Reader.GetInt32(0);
                        emotion.file = m_Reader.GetString(1);
                        emotion.emotion = m_Reader.GetInt32(2);
                        emotion.created_at = m_Reader.GetDateTime(3);

                        list.Add(emotion);
                    }
                    return list;
                }
                return new List<MEmotions>();
            }
        }

        public static void RemoveEmotions(List<MEmotions> emotions)
        {
            var query = "DELETE FROM emotions WHERE id = @id;";
            try
            {
                foreach (var emotion in emotions)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    m_Command.CommandText = query;
                    m_Command.Parameters.Add(new SqliteParameter("@id", emotion.id));

                    m_Command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                return;
            }
        }
        #endregion

        #region GazeData
        public static IDbTransaction m_GlobalTransaction = null;

        public static void StartGlobalTransaction()
        {
            m_GlobalTransaction = m_Connection.BeginTransaction();
            m_Command.Transaction = m_GlobalTransaction;
        }

        public static void StopGlobalTransaction()
        {
            m_GlobalTransaction.Commit();
            m_GlobalTransaction = null;
            m_Command.Transaction = null;
        }

        [HideInInspector]
        public static List<GazeRawData> m_GazeRawData
        {
            set
            {
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }

                StartGlobalTransaction();

                foreach (var v in value)
                {
                    if (m_Reader != null)
                    {
                        m_Reader.Close();
                    }
                    if (m_Command != null)
                    {
                        try
                        {
                            var query = "SELECT x, y, value FROM gaze_raw_data WHERE x = @x AND y = @y;";
                            m_Command.CommandText = query;
                            m_Command.Parameters.Add(new SqliteParameter("@x", v.x));
                            m_Command.Parameters.Add(new SqliteParameter("@y", v.y));

                            m_Reader = m_Command.ExecuteReader();
                            GazeRawData hasData = new GazeRawData();
                            if (m_Reader.Read())
                            {
                                hasData.x = m_Reader.GetInt32(0);
                                hasData.y = m_Reader.GetInt32(1);
                                hasData.value = m_Reader.GetInt32(2);

                                if (m_Reader != null)
                                {
                                    m_Reader.Close();
                                }

                                query = "UPDATE gaze_raw_data SET value = @value WHERE x = @x AND y = @y;";
                                m_Command.CommandText = query;

                                m_Command.Parameters.Add(new SqliteParameter("@x", hasData.x));
                                m_Command.Parameters.Add(new SqliteParameter("@y", hasData.y));
                                m_Command.Parameters.Add(new SqliteParameter("@value", hasData.value + v.value));

                                m_Command.ExecuteNonQuery();
                            }
                            else
                            {
                                if (m_Reader != null)
                                {
                                    m_Reader.Close();
                                }

                                query = "INSERT INTO gaze_raw_data(x, y, value, answer_id) values(@x, @y, @value, @answer_id);";
                                m_Command.CommandText = query;

                                m_Command.Parameters.Add(new SqliteParameter("@x", v.x));
                                m_Command.Parameters.Add(new SqliteParameter("@y", v.y));
                                m_Command.Parameters.Add(new SqliteParameter("@value", v.value));
                                m_Command.Parameters.Add(new SqliteParameter("@answer_id", v.answer_id));

                                m_Command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception exception)
                        {
                            Debug.LogError(exception.Message);
                            return;
                        }
                    }
                }

                StopGlobalTransaction();
            }
        }
        #endregion

        #region Data exportation
        public static void CloneFile(string path_from, string path_to, string uuid)
        {
            if (File.Exists(path_from))
            {
                string extension = Path.GetExtension(path_from);
                try
                {
                    File.Copy(path_from, Path.Combine(path_to, uuid + extension), true);
                } catch (Exception e)
                {
                    Debug.LogError("[FILE COPY]: " + e.Message);
                }
            }
        }

        public static void _ExportAnswers(int session_id, string pypeline_type, string path, string separator = ";")
        {
            //IDbConnection _internal_Connection;
            IDbCommand _internal_Command = CreateCommand(); ;
            IDataReader _internal_Reader;

            int lineCounter = 0;
            List<string[]> rowData = new List<string[]>();

            string _path = Path.Combine(path, "answers\\" + pypeline_type.ToLower());

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            string file = Path.Combine(_path, (System.Guid.NewGuid()).ToString() + ".csv");

            try
            {
                string query = "SELECT * FROM get_answers WHERE session_id = @session_id AND type = @type;";
                _internal_Command.CommandText = query;
                _internal_Command.Parameters.Add(new SqliteParameter("@session_id", session_id));
                _internal_Command.Parameters.Add(new SqliteParameter("@type", pypeline_type));

                _internal_Reader = _internal_Command.ExecuteReader();

                string[] tempData = new string[_internal_Reader.FieldCount];
                while (_internal_Reader.Read())
                {
                    // add header
                    if (lineCounter <= 0)
                    {
                        for (int i = 0; i < _internal_Reader.FieldCount; i++)
                        {
                            tempData[i] = _internal_Reader.GetName(i);
                        }
                        rowData.Add(tempData);

                        lineCounter++;
                    }

                    tempData = new string[_internal_Reader.FieldCount];
                    for (int i = 0; i < _internal_Reader.FieldCount; i++)
                    {
                        // fix bug of datetime conversion
                        try
                        {
                            tempData[i] = _internal_Reader.GetValue(i).ToString();
                        }catch(Exception e)
                        {
                            tempData[i] = "";
                        }
                    }
                    rowData.Add(tempData);

                    // export gaze data
                    if (pypeline_type == "EYETRACKING")
                    {
                        _ExportGazeData(_path, _internal_Reader.GetInt32(0), session_id, separator);
                    } else if (pypeline_type == "EMOTION")
                    {
                        _ExportEmotions(_path, _internal_Reader.GetInt32(0), session_id, separator);
                    }
                }

                // create file
                string[][] output = new string[rowData.Count][];

                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = rowData[i];
                }

                int length = output.GetLength(0);
                StringBuilder sb = new StringBuilder();

                for (int index = 0; index < length; index++)
                    sb.AppendLine(string.Join(separator, output[index]));

                if (!File.Exists(file))
                {
                    StreamWriter outStream = System.IO.File.CreateText(file);
                    outStream.WriteLine(sb);
                    outStream.Close();
                }

                _internal_Reader.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static void _ExportGazeData(string path, int answer_id, int session_id, string separator = ";")
        {
            //IDbConnection _internal_Connection;
            IDbCommand _internal_Command = CreateCommand(); ;
            IDataReader _internal_Reader;

            int lineCounter = 0;
            List<string[]> rowData = new List<string[]>();
            List<string[]> rowDataFormated = new List<string[]>();

            string _path = Path.Combine(path, "gaze_data\\");

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            string fileuuid = (System.Guid.NewGuid()).ToString();
            string file = Path.Combine(_path, fileuuid + ".csv");
            string filef = Path.Combine(_path, fileuuid + "_formatted" +  ".csv");

            // copy resource file
            string rspath;

#if UNITY_STANDALONE || UNITY_STANDALONE_WIN
            string _rspath = Directory.GetParent(Application.dataPath).ToString();
            _rspath = Path.Combine(_rspath, "StaticFiles\\ScreenCapture\\EyeTracking\\");
            rspath = _rspath;
#endif

            string rsfile = Path.Combine(rspath, session_id.ToString() + "\\ref" + answer_id.ToString() + ".png");
            CloneFile(rsfile, _path, fileuuid);

            try
            {
                string query = "SELECT * FROM get_gaze_data WHERE answer_id = @answer_id;";
                _internal_Command.CommandText = query;
                _internal_Command.Parameters.Add(new SqliteParameter("@answer_id", answer_id));

                _internal_Reader = _internal_Command.ExecuteReader();

                string[] tempDataExp = new string[3];
                tempDataExp[0] = "X";
                tempDataExp[1] = "Y";
                tempDataExp[2] = "Duration";
                rowDataFormated.Add(tempDataExp);

                string[] tempData = new string[_internal_Reader.FieldCount];
                while (_internal_Reader.Read())
                {
                    // add header
                    if (lineCounter <= 0)
                    {
                        for (int i = 0; i < _internal_Reader.FieldCount; i++)
                        {
                            tempData[i] = _internal_Reader.GetName(i);
                        }
                        rowData.Add(tempData);

                        lineCounter++;
                    }

                    tempDataExp = new string[3];
                    tempDataExp[0] = _internal_Reader.GetValue(1).ToString();
                    tempDataExp[1] = _internal_Reader.GetValue(2).ToString();
                    tempDataExp[2] = _internal_Reader.GetValue(3).ToString();
                    rowDataFormated.Add(tempDataExp);

                    tempData = new string[_internal_Reader.FieldCount];
                    for (int i = 0; i < _internal_Reader.FieldCount; i++)
                    {
                        tempData[i] = _internal_Reader.GetValue(i).ToString();
                    }
                    rowData.Add(tempData);
                }

                // create file
                string[][] outputFormated = new string[rowDataFormated.Count][];
                string[][] output = new string[rowData.Count][];

                for (int i = 0; i < outputFormated.Length; i++)
                {
                    outputFormated[i] = rowDataFormated[i];
                }

                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = rowData[i];
                }

                int length = output.GetLength(0);
                int lengthFormated = outputFormated.GetLength(0);
                StringBuilder sb = new StringBuilder();
                StringBuilder sbf = new StringBuilder();

                for (int index = 0; index < lengthFormated; index++)
                    sbf.AppendLine(string.Join(",", outputFormated[index]));

                for (int index = 0; index < length; index++)
                    sb.AppendLine(string.Join(separator, output[index]));

                if (!File.Exists(filef))
                {
                    StreamWriter outStreamFormated = System.IO.File.CreateText(filef);
                    outStreamFormated.WriteLine(sbf);
                    outStreamFormated.Close();
                }

                if (!File.Exists(file))
                {
                    StreamWriter outStream = System.IO.File.CreateText(file);
                    outStream.WriteLine(sb);
                    outStream.Close();
                }

                _internal_Reader.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static void _ExportEmotions(string path, int answer_id, int session_id, string separator = ";")
        {
            //IDbConnection _internal_Connection;
            IDbCommand _internal_Command = CreateCommand(); ;
            IDataReader _internal_Reader;

            int lineCounter = 0;
            List<string[]> rowData = new List<string[]>();

            string _path = Path.Combine(path, "emotion_data\\");

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            string fileuuid = (System.Guid.NewGuid()).ToString();
            string file = Path.Combine(_path, fileuuid + ".csv");

            try
            {
                string query = "SELECT * FROM get_emotions WHERE answer_id = @answer_id;";
                _internal_Command.CommandText = query;
                _internal_Command.Parameters.Add(new SqliteParameter("@answer_id", answer_id));

                _internal_Reader = _internal_Command.ExecuteReader();

                string[] tempData = new string[_internal_Reader.FieldCount];
                while (_internal_Reader.Read())
                {
                    // add header
                    if (lineCounter <= 0)
                    {
                        for (int i = 0; i < _internal_Reader.FieldCount; i++)
                        {
                            tempData[i] = _internal_Reader.GetName(i);
                        }
                        rowData.Add(tempData);

                        lineCounter++;
                    }

                    tempData = new string[_internal_Reader.FieldCount];
                    for (int i = 0; i < _internal_Reader.FieldCount; i++)
                    {
                        tempData[i] = _internal_Reader.GetValue(i).ToString();
                    }
                    rowData.Add(tempData);
                }

                // create file
                string[][] output = new string[rowData.Count][];

                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = rowData[i];
                }

                int length = output.GetLength(0);
                StringBuilder sb = new StringBuilder();

                for (int index = 0; index < length; index++)
                    sb.AppendLine(string.Join(separator, output[index]));

                if (!File.Exists(file))
                {
                    StreamWriter outStream = System.IO.File.CreateText(file);
                    outStream.WriteLine(sb);
                    outStream.Close();
                }

                _internal_Reader.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static void ExportCSV(string path, string separator = ";")
        {
            if (m_Reader != null)
            {
                m_Reader.Close();
            }
            //StartGlobalTransaction();

            if (m_Command != null)
            {
                try
                {
                    int lineCounter = 0;
                    List<string[]> rowData = new List<string[]>();

                    string query = "SELECT * FROM get_session";
                    m_Command.CommandText = query;

                    m_Reader = m_Command.ExecuteReader();

                    string[] tempData = new string[m_Reader.FieldCount];
                    while (m_Reader.Read())
                    {
                        rowData.Clear();

                        // first column is the identifier of folder
                        string session_id = (System.Guid.NewGuid()).ToString();
                        string _path = Path.Combine(path, session_id);

                        if (!Directory.Exists(_path))
                        {
                            Directory.CreateDirectory(_path);
                        }

                        // export answers
                        _ExportAnswers(m_Reader.GetInt32(0), "IMAGE", _path, separator);
                        _ExportAnswers(m_Reader.GetInt32(0), "VIDEO", _path, separator);
                        _ExportAnswers(m_Reader.GetInt32(0), "AUDIO", _path, separator);
                        _ExportAnswers(m_Reader.GetInt32(0), "EMOTION", _path, separator);
                        _ExportAnswers(m_Reader.GetInt32(0), "AVATAR", _path, separator);
                        _ExportAnswers(m_Reader.GetInt32(0), "EYETRACKING", _path, separator);

                        // add header
                        if (lineCounter <= 0)
                        {
                            for (int i = 0; i < m_Reader.FieldCount; i++)
                            {
                                tempData[i] = m_Reader.GetName(i);
                            }
                            rowData.Add(tempData);

                            lineCounter++;
                        }

                        tempData = new string[m_Reader.FieldCount];
                        for (int i = 0; i < m_Reader.FieldCount; i++)
                        {
                            tempData[i] = m_Reader.GetValue(i).ToString();
                        }
                        rowData.Add(tempData);

                        // create file
                        string[][] output = new string[rowData.Count][];

                        for (int i = 0; i < output.Length; i++)
                        {
                            output[i] = rowData[i];
                        }

                        int length = output.GetLength(0);
                        StringBuilder sb = new StringBuilder();

                        for (int index = 0; index < length; index++)
                            sb.AppendLine(string.Join(separator, output[index]));

                        string file = session_id + ".csv";
                        if (!File.Exists(file))
                        {
                            StreamWriter outStream = System.IO.File.CreateText(Path.Combine(_path, file));
                            outStream.WriteLine(sb);
                            outStream.Close();
                        }

                        // clone selfie
                        string sfile = Path.Combine(EmoteSession.selfie_path, m_Reader.GetValue(2).ToString());
                        CloneFile(sfile, _path, session_id);
                    }
                } catch (Exception e)
                {
                    Debug.LogError("[EXPORT CSV] " + e.Message);
                }
            }

            //StopGlobalTransaction();
        }
        #endregion

        private void Start()
        {
            InitConnection();
        }

        private void Awake()
        {
            m_DatabasePath = GetDatabasePath("Emote");
            InitConnection();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void OnDrawGizmos()
        {
            m_DatabasePath = GetDatabasePath("Emote");
        }

        #region Utils
        public static string GetDatabasePath(string name)
        {
            return "URI=file:Database/" + name + ".s3db,version=3";
        }
        #endregion

        #region Manage Connections
        public static void InitConnection()
        {
            if (m_Connection == null)
            {
                // create a new connection
                m_Connection = CreateConnection(m_DatabasePath);
                if (m_Connection != null)
                {
                    m_Connection.Open();
                    m_Command = CreateCommand();
                    Debug.Log("Sqlite connected!");
                }
                else
                {
                    Debug.LogError("Sqlite connection error!");
                    return;
                }
            }
        }

        public static IDbConnection CreateConnection(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                return (IDbConnection)new SqliteConnection(m_DatabasePath);
            }
            return null;
        }

        public static IDbCommand CreateCommand()
        {
            if (m_Connection != null)
            {
                return m_Connection.CreateCommand();
            }
            return null;
        }

        public static void Dispose()
        {
            if (m_Reader != null && !m_Reader.IsClosed)
            {
                m_Reader.Close();
                m_Reader = null;
            }
            if (m_Command != null)
            {
                m_Command.Dispose();
                m_Command = null;
            }
            if (m_Connection != null)
            {
                m_Connection.Close();
                m_Connection = null;
            }
        }
        #endregion
    }
}