/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ItSeez3D.AvatarSdk.Core;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ItSeez3D.AvatarSdk.Offline
{
	public static class OfflineSdkUtils
	{
		private static readonly string offlineResourcesDir = "avatar_sdk";
		private static readonly string resourceExtension = ".bytes";

		private static void RemoveExistingUnpackedResources (string unpackedResourcesPath)
		{
			if (Directory.Exists (unpackedResourcesPath)) {
				// remove all the existing data
				Directory.Delete (unpackedResourcesPath, true);
			}
			Directory.CreateDirectory (unpackedResourcesPath);
		}

		private static string GetResourcePathInAssets(string resource)
		{
			string path = offlineResourcesDir + "/" + Path.Combine(Path.GetDirectoryName(resource), Path.GetFileNameWithoutExtension(resource));
			return path;
		}

		private static IEnumerator UnpackResources (string[] resourceList, string unpackedResourcesPath, string testFile)
		{
			RemoveExistingUnpackedResources (unpackedResourcesPath);

			if (Utils.IsDesignTime ()) {
				// Resources.LoadAsync does not work in Editor in Unity 5.6, hence the special non-async version
				foreach (var resource in resourceList) {
					var resourceObject = Resources.Load (GetResourcePathInAssets(resource));
					var asset = resourceObject as TextAsset;
					var filename = asset.name + resourceExtension;
					Debug.LogFormat ("Unpacking {0}...", filename);
					File.WriteAllBytes (Path.Combine (unpackedResourcesPath, filename), asset.bytes);
					yield return null;  // avoid blocking the main thread
				}
			} else {
				// load several resources at a time to reduce loading time
				int nSimultaneously = 20;
				for (int i = 0; i < resourceList.Length; i += nSimultaneously) {
					var resourceRequestsAsync = new List<ResourceRequest> ();
					for (int j = 0; j < nSimultaneously && i + j < resourceList.Length; ++j) {
						var resource = resourceList [i + j];
						resourceRequestsAsync.Add (Resources.LoadAsync (GetResourcePathInAssets(resource)));
					}

					bool allDone;
					do {
						allDone = true;
						foreach (var request in resourceRequestsAsync)
							if (!request.isDone)
								allDone = false;
						yield return null;
					} while (!allDone);

					var copyRequests = new List<AsyncRequestThreaded<string>> ();
					foreach (var request in resourceRequestsAsync) {
						var asset = request.asset as TextAsset;
						if (asset == null) {
							Debug.LogWarning ("Asset is null! Could not unpack one of the resources!");
						} else {
							var filename = asset.name + resourceExtension;
							Debug.LogFormat ("Unpacking {0}...", filename);
							var assetBytes = asset.bytes;
							copyRequests.Add (new AsyncRequestThreaded<string> (() => {
								File.WriteAllBytes (Path.Combine (unpackedResourcesPath, filename), assetBytes);
								return filename;
							}));
						}
					}

					yield return AsyncUtils.AwaitAll (copyRequests.ToArray ());

					foreach (var request in resourceRequestsAsync) {
						var asset = request.asset as TextAsset;
						if (asset != null)
							Resources.UnloadAsset (asset);
					}
				}
			}

			Debug.LogFormat ("Unpacked all resources!");
			File.WriteAllText (testFile, "unpacked!");

			Resources.UnloadUnusedAssets ();
			GC.Collect ();
		}

		/// <summary>
		/// Finds files in directory and adds their names to the list.
		/// </summary>
		/// <param name="fileNames">Output list with filenames</param>
		/// <param name="dir">Directory where files will be searched</param>
		/// <param name="relativeDir">Relative directory to concatenate with filename</param>
		/// <param name="includeSubDir">True if need to look up files in subdirs</param>
		/// <param name="extension">Include only files with the given extension</param>
		private static void GetFileNamesInDirectory(ref List<string> fileNames, string dir, string relativeDir, bool includeSubDir = true, string extension = "")
		{
			foreach (var filePath in Directory.GetFiles(dir))
			{
				var filename = Path.GetFileName(filePath);
				if (filename.EndsWith(extension))
					fileNames.Add(Path.Combine(relativeDir, filename));
			}

			if (includeSubDir)
			{
				foreach (var subdir in Directory.GetDirectories(dir))
					GetFileNamesInDirectory(ref fileNames, subdir, Path.Combine(relativeDir, Path.GetFileName(subdir)), true, extension);
			}
		}

		public static IEnumerator EnsureSdkResourcesUnpacked (string unpackedResourcesPath)
		{
			var resourceListFilename = "resource_list.txt";

			#if UNITY_EDITOR
			// getting list of offline sdk resources and saving it to file
			AssetDatabase.Refresh ();

			string resourcesDir = PluginStructure.GetPluginDirectoryPath(PluginStructure.OFFLINE_RESOURCES_DIR, PathOriginOptions.FullPath);
			string miscResourcesDir = PluginStructure.GetPluginDirectoryPath(PluginStructure.MISC_OFFLINE_RESOURCES_DIR, PathOriginOptions.FullPath);
			var offlineResourcesList = new List<string> { resourcesDir, miscResourcesDir };
			var resourceFiles = new List<string> ();
			foreach (var resDir in offlineResourcesList)
			{
				PluginStructure.CreatePluginDirectory(resDir);
				GetFileNamesInDirectory(ref resourceFiles, resDir, "", true, resourceExtension);
			}

			File.WriteAllLines (Path.Combine (miscResourcesDir, resourceListFilename), resourceFiles.ToArray ());
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			#endif

			var loadingStartTime = Time.realtimeSinceStartup;

			// read list of all resources files
			UnityEngine.Object resourceListObject = null;
			var resourceListPath = GetResourcePathInAssets(resourceListFilename);
			if (Utils.IsDesignTime ())
				resourceListObject = Resources.Load (resourceListPath);
			else {
				var resourceListRequest = Resources.LoadAsync (resourceListPath);
				yield return resourceListRequest;
				resourceListObject = resourceListRequest.asset;
			}

			var resourceListAsset = resourceListObject as TextAsset;
			if (resourceListAsset == null) {
				Debug.LogErrorFormat ("Could not read the list of resources from {0}", resourceListFilename);
				yield break;
			}

			// names of all the resources that need to be unpacked
			var resourceList = resourceListAsset.text.Split (new char[]{ '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			// verify the integrity of the unpacked resource folder
			bool shouldUnpackResources = false;
			var testFile = Utils.CombinePaths (unpackedResourcesPath, CoreTools.SdkVersion.ToString () + "_copied.test");

			// first of all, check if the indicator file is present in the directory
			if (!File.Exists (testFile))
				shouldUnpackResources = true;

			// get actual list of the unpacked resources
			var unpackedResources = new HashSet<string> ();
			foreach (var unpackedResourcePath in Directory.GetFiles(unpackedResourcesPath)) {
				var unpackedResource = Path.GetFileName (unpackedResourcePath);
				unpackedResources.Add (unpackedResource);
			}

			bool unpackedAllResources = resourceList.All (resource => unpackedResources.Contains (Path.GetFileName(resource)));
			if (!unpackedAllResources)
				shouldUnpackResources = true;

			if (shouldUnpackResources) {
				Debug.LogFormat ("Should unpack all resources");
				yield return UnpackResources (resourceList, unpackedResourcesPath, testFile);
				Debug.LogFormat ("Took {0} seconds to load resources", Time.realtimeSinceStartup - loadingStartTime);
			}
		}

		public static IEnumerator EnsureInitialized (string unpackedResourcesPath, bool showError = false, bool resetResources = false)
		{
			if (resetResources) {
				RemoveExistingUnpackedResources (unpackedResourcesPath);
				#if UNITY_EDITOR
				AssetDatabase.DeleteAsset (PluginStructure.GetPluginDirectoryPath(PluginStructure.MISC_OFFLINE_RESOURCES_DIR, PathOriginOptions.RelativeToAssetsFolder));
				AssetDatabase.Refresh ();
				#endif
			}

			string clientId = null, clientSecret = null;
			var accessCredentials = AuthUtils.LoadCredentials ();
			if (accessCredentials != null) {
				clientId = accessCredentials.clientId;
				clientSecret = accessCredentials.clientSecret;
			}

			var offlineSdkInitializer = new OfflineSdkInitializer ();
			string miscResourcesPath = PluginStructure.GetPluginDirectoryPath(PluginStructure.MISC_OFFLINE_RESOURCES_DIR, PathOriginOptions.FullPath);
			yield return offlineSdkInitializer.Run (miscResourcesPath, CoreTools.SdkVersion.ToString (), NetworkUtils.rootUrl, clientId, clientSecret);
			if (!offlineSdkInitializer.Success && showError)
				Utils.DisplayWarning ("Could not initialize offline SDK!", "Error message: \n" + offlineSdkInitializer.LastError);

			yield return EnsureSdkResourcesUnpacked (unpackedResourcesPath);
		}
	}
}
