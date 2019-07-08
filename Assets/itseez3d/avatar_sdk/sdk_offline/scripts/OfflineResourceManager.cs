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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ItSeez3D.AvatarSdk.Offline
{
	/// <summary>
	/// Resource manager for Offline SDK
	/// </summary>
	public class OfflineResourceManager : ResourceManager
	{
		#region Dll interface
		[DllImport(DllHelper.dll)]
		private unsafe static extern int getBaseResourcesJson(byte* resourcesJson, int resourcesBufferSize);

		[DllImport(DllHelper.dll)]
		private unsafe static extern int getDefaultResourcesJson(byte* resourcesJson, int resourcesBufferSize);

		[DllImport(DllHelper.dll)]
		private unsafe static extern int getCustomResourcesJson(byte* resourcesJson, int resourcesBufferSize);
		#endregion

		/// <summary>
		/// There are three sets of the offline resources
		/// </summary>
		private enum OfflineResourcesSubset
		{
			// resources available for all users
			Base, 
			// default resources
			Default,
			// individual resources
			Custom
		}


		private Dictionary<AvatarResourcesSubset, AvatarResources> avatarResourcesDictionary = new Dictionary<AvatarResourcesSubset, AvatarResources>();

		/// <summary>
		/// Returns lists of resources asynchronous
		/// </summary>
		public override AsyncRequest<AvatarResources> GetResourcesAsync(AvatarResourcesSubset resourcesSubset, PipelineType pipelineType)
		{
			var request = new AsyncRequestThreaded<AvatarResources>(() => { return GetResources(resourcesSubset); }, AvatarSdkMgr.Str(Strings.GettingResourcesList));
			AvatarSdkMgr.SpawnCoroutine(request.Await());
			return request;
		}

		/// <summary>
		/// Returns lists of resources for a given subset that can be generated for avatar
		/// </summary>
		public unsafe AvatarResources GetResources(AvatarResourcesSubset resourcesSubset)
		{
			if (avatarResourcesDictionary.ContainsKey(resourcesSubset))
			{
				return avatarResourcesDictionary[resourcesSubset];
			}
			else
			{
				AvatarResources avatarResources = AvatarResources.Empty;

				if (resourcesSubset == AvatarResourcesSubset.DEFAULT)
					avatarResources = GetResourcesForOfflineSubset(OfflineResourcesSubset.Default);
				else
				{
					// Merge Base resources with Custom to get the list of all available resources
					AvatarResources baseResources = GetResourcesForOfflineSubset(OfflineResourcesSubset.Base);
					AvatarResources customResources = GetResourcesForOfflineSubset(OfflineResourcesSubset.Custom);
					avatarResources.Merge(baseResources);
					avatarResources.Merge(customResources);
				}

				avatarResourcesDictionary.Add(resourcesSubset, avatarResources);
				return avatarResources;
			}
		}

		private unsafe AvatarResources GetResourcesForOfflineSubset(OfflineResourcesSubset offlineSubset)
		{
			byte[] buffer = new byte[32768];
			fixed (byte* rawBytes = &buffer[0])
			{
				switch (offlineSubset)
				{
					case OfflineResourcesSubset.Base:
						getBaseResourcesJson(rawBytes, buffer.Length);
						break;

					case OfflineResourcesSubset.Default:
						getDefaultResourcesJson(rawBytes, buffer.Length);
						break;

					case OfflineResourcesSubset.Custom:
						getCustomResourcesJson(rawBytes, buffer.Length);
						break;
				}
			}

			return GetResourcesFromJson(System.Text.Encoding.ASCII.GetString(buffer));
		}
	}
}
