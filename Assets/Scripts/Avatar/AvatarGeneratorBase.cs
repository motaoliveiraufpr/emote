using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ItSeez3D.AvatarSdk.Core;
using ItSeez3D.AvatarSdkSamples.Core;

#if !UNITY_WEBGL

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */

namespace Emote.Avatar
{
    /// <summary>
    /// Avatar states for a simple "state machine".
    /// </summary>
    public enum AvatarState
    {
        UNKNOWN,
        GENERATING,
        COMPLETED,
        FAILED
    }

    public static class CurrentAvatar
    {
        public static AvatarState state;
        public static string code;
    }

    public abstract class AvatarGeneratorBase : MonoBehaviour
    {
        //Avatar provider - initialized once per application runtime.
        protected IAvatarProvider avatarProvider = null;

        // type of used SDK
        protected SdkType sdkType;

        // Pipeline that will be used to generate avatar
        protected static PipelineType pipelineType = PipelineType.FACE;

        // instance of string manager
        protected IStringManager stringManager = new CustomStringMgr();

        // instance of persistent storage
        protected IPersistentStorage persistentStorage = new CustomPersistentStorage();

        #region UI
        // displayed status and progress of requests
        public GameObject progressText;
        public GameObject statusText;
        public GameObject codeText;

        // wrapper to support Unity common text and Text mesh pro
        private string _code
        {
            get
            {
                if (codeText)
                {
                    if (codeText.GetComponent<Text>())
                    {
                        return codeText.GetComponent<Text>().text;
                    }
                    else if (codeText.GetComponent<TextMeshProUGUI>())
                    {
                        return codeText.GetComponent<TextMeshProUGUI>().text;
                    }
                }
                return "";
            }
            set
            {
                if (codeText)
                {
                    if (codeText.GetComponent<Text>())
                    {
                        codeText.GetComponent<Text>().text = value;
                    }
                    else if (codeText.GetComponent<TextMeshProUGUI>())
                    {
                        codeText.GetComponent<TextMeshProUGUI>().text = value;
                    }
                }
            }
        }

        private string _status
        {
            get
            {
                if (statusText)
                {
                    if (statusText.GetComponent<Text>())
                    {
                        return statusText.GetComponent<Text>().text;
                    }
                    else if (statusText.GetComponent<TextMeshProUGUI>())
                    {
                        return statusText.GetComponent<TextMeshProUGUI>().text;
                    }
                }
                return "";
            }
            set
            {
                if (statusText)
                {
                    if (statusText.GetComponent<Text>())
                    {
                        statusText.GetComponent<Text>().text = value;
                    }
                    else if (statusText.GetComponent<TextMeshProUGUI>())
                    {
                        statusText.GetComponent<TextMeshProUGUI>().text = value;
                    }
                }
            }
        }

        private string _progress
        {
            get
            {
                if (progressText)
                {
                    if (progressText.GetComponent<Text>())
                    {
                        return progressText.GetComponent<Text>().text;
                    }
                    else if (progressText.GetComponent<TextMeshProUGUI>())
                    {
                        return progressText.GetComponent<TextMeshProUGUI>().text;
                    }
                }
                return "";
            }
            set
            {
                if (progressText)
                {
                    if (progressText.GetComponent<Text>())
                    {
                        progressText.GetComponent<Text>().text = value;
                    }
                    else if (progressText.GetComponent<TextMeshProUGUI>())
                    {
                        progressText.GetComponent<TextMeshProUGUI>().text = value;
                    }
                }
            }
        }

        // scripts that allows to open image from the file system
        public FileBrowser fileBrowser = null;

        #endregion

        #region Lifecycle
        void Start()
        {
            StartCoroutine(Initialize());

            // start the scene with file handler if desire automatic generation
            if (fileBrowser != null)
                fileBrowser.fileHandler = CreateNewAvatar;
        }
        #endregion

        #region Initialization
        private IEnumerator Initialize()
        {
            // First of all, initialize the SDK. This sample shows how to provide user-defined implementations for
            // the interfaces if needed. If you don't need to override the default behavior, just pass null instead.
            if (!AvatarSdkMgr.IsInitialized)
            {
                AvatarSdkMgr.Init(
                    stringMgr: stringManager,
                    storage: persistentStorage,
                    sdkType: sdkType
                );
            }

            GameObject providerContainerGameObject = GameObject.Find("AvatarProviderContainer");
            if (providerContainerGameObject != null)
            {
                avatarProvider = providerContainerGameObject.GetComponent<AvatarProviderContainer>().avatarProvider;
            }
            else
            {
                // Initialization of the IAvatarProvider may take some time. 
                // We don't want to initialize it each time when the Gallery scene is loaded.
                // So we store IAvatarProvider instance in the object that will not destroyed during navigation between the scenes (Gallery -> ModelViewer -> Gallery).
                providerContainerGameObject = new GameObject("AvatarProviderContainer");
                DontDestroyOnLoad(providerContainerGameObject);
                AvatarProviderContainer providerContainer = providerContainerGameObject.AddComponent<AvatarProviderContainer>();
                avatarProvider = AvatarSdkMgr.IoCContainer.Create<IAvatarProvider>();
                providerContainer.avatarProvider = avatarProvider;

                var initializeRequest = InitializeAvatarProviderAsync();
                yield return Await(initializeRequest, null);
                if (initializeRequest.IsError)
                {
                    Debug.LogError("Avatar provider isn't initialized!");
                    yield break;
                }
            }
        }

        protected virtual AsyncRequest InitializeAvatarProviderAsync()
        {
            return avatarProvider.InitializeAsync();
        }
        #endregion

        #region Async utils

        /// <summary>
        /// Helper function that waits until async request finishes and keeps track of progress on request and it's
        /// subrequests. Note it does "yield return null" every time, which means that code inside the loop
        /// is executed on each frame, but after progress is updated the function does not block the main thread anymore.
        /// </summary>
        /// <param name="r">Async request to await.</param>
        /// <param name="avatarCode">If null the request does not correspond to the particular avatar, and the progress
        /// will be printed at the bottom of the screen below the "gallery". If not null then progress will
        /// be updated inside particular avatar preview item.</param>
        protected IEnumerator Await(AsyncRequest r, string avatarCode)
        {
            while (!r.IsDone)
            {
                yield return null;

                if (r.IsError)
                {
                    Debug.LogError(r.ErrorMessage);
                    yield break;
                }

                // Iterate over subrequests to obtain overall progress, as well as progress of the current stage.
                // E.g. main request: "Downloading avatar", overall progress 20%;
                // current stage: "Downloading mesh", progress 40%.
                // Level of nesting can be arbitrary, but generally less than three.
                var progress = new List<string>();
                AsyncRequest request = r;
                while (request != null)
                {
                    progress.Add(string.Format("{0}: {1}%", request.State, request.ProgressPercent.ToString("0.0")));
                    request = request.CurrentSubrequest;
                }

                if (string.IsNullOrEmpty(avatarCode))
                {
                    // update progress at the top of the screen
                    _progress = string.Join("  -->  ", progress.ToArray());
                }
                else
                {
                    // update progress inside small gallery preview item
                    UpdateAvatarProgress(avatarCode, string.Join("\n", progress.ToArray()));
                }
            }

            _progress = string.Empty;
        }

        #endregion

        #region "Custom" implementations of SDK interfaces

        private class CustomStringMgr : DefaultStringManager
        {
            // your implementation...
        }

        private class CustomPersistentStorage : DefaultPersistentStorage
        {
            // your implementation...
        }

        #endregion

        #region Avatar creation and processing
        /// <summary>
        /// Create avatar and save photo to disk.
        /// </summary>
        public IEnumerator CreateNewAvatar(byte[] photoBytes)
        {
            PipelineType pipeline = pipelineType;

            // Choose default set of resources to generate
            var resourcesRequest = avatarProvider.ResourceManager.GetResourcesAsync(AvatarResourcesSubset.DEFAULT, pipelineType);
            yield return resourcesRequest;
            if (resourcesRequest.IsError)
                yield break;

            var initializeAvatar = avatarProvider.InitializeAvatarAsync(photoBytes, "name", "description", pipeline, resourcesRequest.Result);
            yield return Await(initializeAvatar, null);

            string avatarCode = initializeAvatar.Result;
            if (initializeAvatar.IsError)
            {
                UpdateAvatarState(avatarCode, AvatarState.FAILED);
                yield break;
            }

            UpdateAvatarState(avatarCode, AvatarState.GENERATING);

            var calculateAvatar = avatarProvider.StartAndAwaitAvatarCalculationAsync(avatarCode);
            yield return Await(calculateAvatar, avatarCode);
            if (calculateAvatar.IsError)
            {
                UpdateAvatarState(avatarCode, AvatarState.FAILED);
                yield break;
            }

            var downloadAvatar = avatarProvider.MoveAvatarModelToLocalStorageAsync(avatarCode, pipeline == PipelineType.FACE, true);
            yield return Await(downloadAvatar, avatarCode);
            if (downloadAvatar.IsError)
            {
                UpdateAvatarState(avatarCode, AvatarState.FAILED);
                yield break;
            }

            UpdateAvatarState(avatarCode, AvatarState.COMPLETED);
        }
        #endregion

        #region Update information of individuals uploaded avatars
        private void UpdateAvatarProgress(string avatarCode, string progressStr)
        {
            _progress = progressStr;
        }

        private void UpdateAvatarState(string avatarCode, AvatarState state)
        {
            _code = string.Format("Code: {0}...", avatarCode.Substring(0, 24));
            _status = string.Format("State: {0}", state);

            // set current avatar
            CurrentAvatar.code = avatarCode;
            CurrentAvatar.state = state;

            if (state != AvatarState.GENERATING)
            {
                _progress = string.Empty;
            }
        }
        #endregion
    }
}

#endif