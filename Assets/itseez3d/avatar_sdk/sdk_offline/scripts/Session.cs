/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/
using System.Collections.Generic;
using AOT;

#if !UNITY_WEBGL

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using ItSeez3D.AvatarSdk.Core;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Offline
{
	/// <summary>
	/// Session encapsulates all the information required to interact with the native plugin.
	/// When avatar generation is no longer needed, the session can be disposed and re-created again later.
	/// Technically, Session is not a singleton, but it is not recommended to have multiple instances of Session.
	/// You should have no more than one Session object at any time. Dispose of existing session before creating a new one.
	/// </summary>
	public class Session : IDisposable
	{
		delegate void ReportProgress(float progressFraction, int avatarIdHash);

		private static Dictionary<int, AsyncRequest> avatarRequests = new Dictionary<int, AsyncRequest>();

		// list of avatars that is being calculated now
		private static List<string> calculatingAvatars = new List<string>();

		private bool initialized = false;

		private IntPtr resourcesHandle = IntPtr.Zero;

		/// <summary>
		/// This counter holds a number of currently active background calculations.
		/// Dispose method will block until this counter is 0, because destroying the session
		/// while background threads are still active may cause a crash.
		/// </summary>
		private int asyncOperationsCounter = 0;

		#region Dll interface

		[DllImport(DllHelper.dll)]
		private static extern int initAvatarSdk(string programName);

		[DllImport(DllHelper.dll)]
		private unsafe static extern void getLastError(byte* buffer, int size);

		[DllImport(DllHelper.dll)]
		private static extern IntPtr initializeResources(string resourcesPath);

		[DllImport(DllHelper.dll)]
		private static extern void deinitializeResources(IntPtr resourceMgr);

		[DllImport(DllHelper.dll)]
		private static extern int initializeAvatarFromRawData(
			IntPtr rawPhotoBytesRGBA,
			int w,
			int h,
			string avatarDirectory
		);

		[DllImport(DllHelper.dll)]
		private static extern int generateAvatar(
			IntPtr resources,
			string avatarDirectory,
			int avatarIdHash,
			ReportProgress reportProgressFunc
		);

		[DllImport(DllHelper.dll)]
		private static extern int generateLODMesh(int levelOfDetails, string meshFilePath, string outMeshFilePath, string blendshapesDir, string outBlendshapesDir);

		[DllImport(DllHelper.dll)]
		private static extern int extractHaircutFromResources(string haircutId, string haircutsDirectory);

		[DllImport(DllHelper.dll)]
		private static extern int extractHaircutPreviewFromResources(string haircutId, string haircutsDirectory);

		#endregion

		#region Atomic counter for current number of async operations

		private int AsyncOperationsCounter { get { return asyncOperationsCounter; } }

		private int IncrementAsyncOperationsCounter()
		{
			return Interlocked.Increment(ref asyncOperationsCounter);
		}

		private int DecrementAsyncOperationsCounter()
		{
			return Interlocked.Decrement(ref asyncOperationsCounter);
		}

		#endregion

		#region Initialization

		private IEnumerator InitializationHelper(AsyncRequest r)
		{
			if (IsInitialized)
			{
				r.IsDone = true;
				yield break;
			}

			var initializationStartTime = Time.realtimeSinceStartup;
			Debug.LogFormat("Initialization...");
			var status = initAvatarSdk("unity_plugin");  // initialization procedure for the native DLL
			bool okay = status == 0;
			if (okay)
			{
				Debug.LogFormat("Library loading successful, status: {0}", status);
			}
			else
			{
				Debug.LogErrorFormat("Library loading failed, status: {0}", status);
				r.SetError("Could not load native library");
				yield break;
			}

			r.Progress = 0.05f;

			var initTime = Time.realtimeSinceStartup;
			var resourcesPath = AvatarSdkMgr.Storage().GetResourcesDirectory();
			yield return OfflineSdkUtils.EnsureInitialized(resourcesPath, showError: true);
			Debug.LogFormat("Took {0} seconds to initialize Offline SDK", Time.realtimeSinceStartup - initTime);

			r.Progress = 0.3f;

			var resourceLoading = Time.realtimeSinceStartup;
			resourcesHandle = initializeResources(AvatarSdkMgr.Storage().GetResourcesDirectory());
			if (resourcesHandle != IntPtr.Zero)
				Debug.LogFormat("Resource manager initialized!");
			else
			{
				Debug.LogFormat("Could not initialize resource manager");
				r.SetError("Could not initialize resource manager");
				yield break;
			}

			Debug.LogFormat("Took {0} seconds to preload resources", Time.realtimeSinceStartup - resourceLoading);

			IsInitialized = okay;
			Debug.LogFormat("Initialization completed! It took {0} seconds", Time.realtimeSinceStartup - initializationStartTime);

			r.IsDone = true;
		}

		public virtual AsyncRequest InitializeAsync()
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.InitializingSession));
			AvatarSdkMgr.SpawnCoroutine(InitializationHelper(request));
			return request;
		}

		public virtual bool IsInitialized
		{
			get { return initialized; }
			private set { initialized = value; }
		}

		#endregion

		#region Error handling

		private unsafe void HandleError(int returnCode, string whoFailed)
		{
			if (returnCode == 0)
				return;

			byte[] errorBuffer = new byte[1024];
			fixed (byte* rawBytes = &errorBuffer[0])
				getLastError(rawBytes, errorBuffer.Length);

			var nativeLibraryError = System.Text.Encoding.ASCII.GetString(errorBuffer);

			var errorMessage = string.Format("{0} failed with code: {1}, {2}", whoFailed, returnCode, nativeLibraryError);
			throw new Exception(errorMessage);
		}

		#endregion

		#region "Session" implementation

		private IEnumerator ReleaseResource(AsyncRequest request, TextAsset resource)
		{
			while (!request.IsDone)
				yield return null;

			Debug.LogFormat("Releasing resource...");
			Resources.UnloadAsset(resource);
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		private unsafe int AvatarInitialization(RawPhoto rawPhoto, string avatarDirectory, AvatarResources resources = null)
		{
			try
			{
				// To provide backward compatibility, use default set of resources if corresponding paremeter is null
				if (resources == null)
					resources = new OfflineResourceManager().GetResources(AvatarResourcesSubset.DEFAULT);

				string json = AvatarSdkMgr.CalculationParametersGenerator().GetAvatarCalculationParamsJson(resources);
				string parametersFilePath = Path.Combine(avatarDirectory, AvatarSdkMgr.Storage().AvatarFilenames[AvatarFile.PARAMETERS_JSON]);
				File.WriteAllText(parametersFilePath, json);
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Exception during creating file with parameters: {0}", exc);
			}

			fixed (Color32* rawBytes = &rawPhoto.rawData[0])
			{
				IntPtr rawBytesPtr = (IntPtr)rawBytes;
				return initializeAvatarFromRawData(rawBytesPtr, rawPhoto.w, rawPhoto.h, avatarDirectory);
			}
		}

		/// <summary>
		/// Creates a unique identifier that we will use to refer to the particular avatar.
		/// </summary>
		/// <returns>The offline avatar identifier.</returns>
		/// <param name="param">User-defined parameter that can optionally be a part of id.</param>
		public virtual string GenerateOfflineAvatarId(string param)
		{
			var avatarId = string.Format("offline_avatar_{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), Guid.NewGuid().ToString("N"));
			return avatarId;
		}

		/// <summary>
		/// Simply loads the list of directories containing avatars generated locally.
		/// </summary>
		public virtual string[] GetAvatarsFromFilesystem()
		{
			var avatarsDir = AvatarSdkMgr.Storage().GetAvatarsDirectory();
			var avatarDirectories = Directory.GetDirectories(avatarsDir, "offline_avatar_*_*", SearchOption.TopDirectoryOnly);  // this should match the GenerateOfflineAvatarId function
			var avatarIds = new List<string>();
			foreach (var dir in avatarDirectories)
				avatarIds.Add(Path.GetFileName(dir));
			return avatarIds.ToArray();
		}

		/// <summary>
		/// Prepare avatar directory for calculations; call this first before calling CalculateAvatarOfflineAsync.
		/// </summary>
		public virtual AsyncRequest<string> InitializeAvatarAsync(RawPhoto rawPhoto, string yourId = "", AvatarResources resources = null)
		{
			var avatarId = GenerateOfflineAvatarId(yourId);
			var avatarDirectory = AvatarSdkMgr.Storage().GetAvatarDirectory(avatarId);
			var request = new AsyncRequestThreaded<string>(() =>
			{
				try
				{
					IncrementAsyncOperationsCounter();
					int returnCode = AvatarInitialization(rawPhoto, avatarDirectory, resources);
					HandleError(returnCode, "Avatar initialization");
				}
				finally
				{
					DecrementAsyncOperationsCounter();
				}
				return avatarId;
			}, AvatarSdkMgr.Str(Strings.InitializingAvatar), startImmediately: false);

			if (!IsInitialized)
			{
				request.SetError("Session is not initialized yet!");
				return request;
			}

			request.StartThread();
			AvatarSdkMgr.SpawnCoroutine(request.Await());
			return request;
		}

		/// <summary>
		/// Must be static for iOS.
		/// </summary>
		[MonoPInvokeCallback(typeof(ReportProgress))]
		private static void ReportProgressForAvatar(float progress, int avatarIdHash)
		{
			if (avatarRequests.ContainsKey(avatarIdHash))
				avatarRequests[avatarIdHash].Progress = progress;
			else
				Debug.LogFormat("There's no active async request for avatar {0}", avatarIdHash);
		}

		/// <summary>
		/// Start avatar generation in the native plugin.
		/// </summary>
		public virtual AsyncRequest<int> CalculateAvatarOfflineAsync(string avatarId)
		{
			int avatarIdHash = avatarId.GetHashCode();
			var avatarDirectory = AvatarSdkMgr.Storage().GetAvatarDirectory(avatarId);
			var request = new AsyncRequestThreaded<int>((r) =>
			{
				try
				{
					// Waiting for an entire calculation on exit can be very annoying during development, so you can try and comment Increment/Decrement lines.
					// Be careful! If you do this, it may crash if you stop the application in a bad moment.
					// We promise to introduce the calculation interrupt mechanism in future versions.
					IncrementAsyncOperationsCounter();

					DateTime startTime = DateTime.Now;
					int returnCode = generateAvatar(resourcesHandle, avatarDirectory, avatarIdHash, ReportProgressForAvatar);
					Debug.LogFormat("Calculation time: {0} sec", (DateTime.Now - startTime).TotalSeconds);

					HandleError(returnCode, "Calculations");
					return returnCode;
				}
				finally
				{
					DecrementAsyncOperationsCounter();
				}
			}, AvatarSdkMgr.Str(Strings.ComputingAvatar), startImmediately: false);

			if (!IsInitialized)
			{
				request.SetError("Session is not initialized yet!");
				return request;
			}

			avatarRequests [avatarIdHash] = request;
			lock (calculatingAvatars)
				calculatingAvatars.Add(avatarId);

			request.SetOnCompleted ((r) => {
				Debug.LogFormat ("Calculations completed!");
				avatarRequests.Remove (avatarIdHash);
				lock (calculatingAvatars)
					calculatingAvatars.Remove(avatarId);
			});

			request.StartThread ();
			AvatarSdkMgr.SpawnCoroutine (request.Await ());
			return request;
		}

		/// <summary>
		/// Generate an avatar mesh with the given level of details.
		/// </summary>
		public virtual AsyncRequest<int> GenerateLODMeshAsync(string avatarId, int levelOfDetails)
		{
			string meshFilePath = AvatarSdkMgr.Storage().GetAvatarFilename(avatarId, AvatarFile.MESH_PLY);
			string lodMeshFilePath = AvatarSdkMgr.Storage().GetAvatarFilename(avatarId, AvatarFile.MESH_PLY, levelOfDetails);
			string blendshapesDirectory = AvatarSdkMgr.Storage().GetAvatarBlendshapesRootDir(avatarId);
			string lodBlendshapesDirectory = AvatarSdkMgr.Storage().GetAvatarBlendshapesRootDir(avatarId, levelOfDetails);

			var request = new AsyncRequestThreaded<int>(() => {
				int code = 0;
				try
				{
					IncrementAsyncOperationsCounter();
					code = generateLODMesh(levelOfDetails, meshFilePath, lodMeshFilePath, blendshapesDirectory, lodBlendshapesDirectory);
				}
				finally
				{
					DecrementAsyncOperationsCounter();
				}
				return code;
			}, AvatarSdkMgr.Str(Strings.GeneratingLODMesh));

			AvatarSdkMgr.SpawnCoroutine(request.Await());
			return request;
		}

		/// <summary>
		/// Unpack haircut mesh and texture from the binary resources. See the samples for details.
		/// </summary>
		public virtual AsyncRequest<string> ExtractHaircutAsync (string haircutId)
		{
			var haircutsDirectory = AvatarSdkMgr.Storage ().GetHaircutsDirectory ();
			var request = new AsyncRequestThreaded<string> (() => {
				try {
					IncrementAsyncOperationsCounter ();
					extractHaircutFromResources (haircutId, haircutsDirectory);
				} finally {
					DecrementAsyncOperationsCounter ();
				}
				return haircutsDirectory;
			}, AvatarSdkMgr.Str (Strings.ExtractingHaircut));

			AvatarSdkMgr.SpawnCoroutine (request.Await ());
			return request;
		}

		/// <summary>
		/// Unpack haircut preview image from the binary resources.
		/// </summary>
		/// <param name="haircutId"></param>
		/// <returns></returns>
		public virtual AsyncRequest<string> ExtractHaircutPreviewAsync(string haircutId)
		{
			var haircutsDirectory = AvatarSdkMgr.Storage().GetHaircutsDirectory();
			var request = new AsyncRequestThreaded<string>(() => {
				try {
					IncrementAsyncOperationsCounter();
					extractHaircutPreviewFromResources(haircutId, haircutsDirectory);
				}
				finally {
					DecrementAsyncOperationsCounter();
				}
				return haircutsDirectory;
			}, AvatarSdkMgr.Str(Strings.ExtractingHaircutPreview));

			AvatarSdkMgr.SpawnCoroutine(request.Await());
			return request;
		}

		/// <summary>
		/// Return True, if avatar is being calculated
		/// </summary>
		public bool IsAvatarCalculating(string avatarId)
		{
			lock (calculatingAvatars)
				return calculatingAvatars.Contains(avatarId);
		}

#endregion

#region IDisposable implementation

		/// <summary>
		/// This is crucial! Dispose must be called when session is no longer needed!
		/// </summary>
		public virtual void Dispose ()
		{
			while (AsyncOperationsCounter > 0) {
				Debug.LogFormat ("Waiting for {0} unfinished async operations", AsyncOperationsCounter);
				Thread.Sleep (50);
			}

			if (resourcesHandle != IntPtr.Zero) {
				deinitializeResources(resourcesHandle);
				resourcesHandle = IntPtr.Zero;
			}

			IsInitialized = false;
			Debug.LogFormat ("Session Dispose completed!");
		}

#endregion
	}
}

#endif