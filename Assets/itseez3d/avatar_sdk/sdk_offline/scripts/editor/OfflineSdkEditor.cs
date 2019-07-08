/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/
using System.Threading;
using ItSeez3D.AvatarSdk.Cloud;

#if UNITY_EDITOR && !UNITY_WEBGL
using System.Collections;
using ItSeez3D.AvatarSdk.Core;
using ItSeez3D.AvatarSdk.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Offline.Editor
{
	[InitializeOnLoad]
	public static class OfflineSdkEditor
	{
		static OfflineSdkEditor ()
		{
			EditorApplication.update += InitializeOnce;
			EditorApplication.update += CheckUpdatedCredentials;
		}

		private static void InitializeOnce ()
		{
			EditorApplication.update -= InitializeOnce;

			if (Utils.IsDesignTime ())
				InitializeOfflineSdk (resetResources: false);
		}

		private static void CheckUpdatedCredentials ()
		{
			if (AuthenticationWindow.justUpdatedCredentials) {
				AuthenticationWindow.justUpdatedCredentials = false;
				Debug.LogFormat ("Just updated API credentials - need to reload the offline SDK resources");

				InitializeOfflineSdk (resetResources: true);
			}
		}

		[MenuItem ("Window/itSeez3D Avatar SDK/Offline SDK/Force reset the SDK license and resources")]
		public static void UpdateLicense ()
		{
			if (Utils.IsDesignTime ())
				InitializeOfflineSdk (resetResources: true);
			else
				Debug.LogFormat ("Please don't use this in play mode. Disable play mode to reset the Offline SDK");
		}

		private static void InitializeOfflineSdk (bool resetResources)
		{
			if (!AvatarSdkMgr.IsInitialized)
				AvatarSdkMgr.Init ();
			var resourcesPath = AvatarSdkMgr.Storage ().GetResourcesDirectory ();
			EditorCoroutineManager.Start (OfflineSdkUtils.EnsureInitialized (resourcesPath, resetResources: resetResources));
		}
	}
}

#endif