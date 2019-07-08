/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using ItSeez3D.AvatarSdk.Core;
using ItSeez3D.AvatarSdk.Core.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace ItSeez3D.AvatarSdk.Offline.Editor
{
	public class AvatarSdk : AssetPostprocessor
	{
		readonly static string avatarSdkResourceSuffix = "res_";
		readonly static string avatarSdkResourcePostfix = ".bytes";

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			bool isResourcesUpdated = false;

			foreach (string asset in importedAssets)
			{
				string resourceName = Path.GetFileName(asset);
				if (resourceName.StartsWith(avatarSdkResourceSuffix) && resourceName.EndsWith(avatarSdkResourcePostfix))
				{
					isResourcesUpdated = true;
					break;
				}
			}

			if (isResourcesUpdated && AvatarSdkMgr.IsInitialized)
			{
				EditorCoroutineManager.Start(OfflineSdkUtils.EnsureSdkResourcesUnpacked(AvatarSdkMgr.Storage().GetResourcesDirectory()));
			}
		}
	}
}
