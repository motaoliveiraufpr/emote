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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Offline
{
	public class OfflineMeshConverter : CoreMeshConverter
	{
#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN && !UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
		[DllImport(DllHelper.dll)]
		private static extern int convertPlyModelToObj(string plyModelFile, string templateModelFile, string objModelFile, string textureFile);

		public override int ConvertPlyModelToObj(string plyModelFile, string templateModelFile, string objModelFile, string textureFile)
		{
			return convertPlyModelToObj(plyModelFile, templateModelFile, objModelFile, textureFile);
		}

		public override bool IsObjConvertEnabled { get { return true; } }
#endif

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
		[DllImport(DllHelper.dll)]
		private static extern int convertPlyModelToFbx(string plyModelFile, string templateModelFile, string fbxModelFile, string textureFile);

		[DllImport(DllHelper.dll)]
		private static extern int exportFbxWithBlendshapes(string plyModelFile, string texturePath, string binaryBlendshapesDir, string outputFbxPath);

		public override int СonvertPlyModelToFbx(string plyModelFile, string templateModelFile, string fbxModelFile, string textureFile)
		{
			return convertPlyModelToFbx(plyModelFile, templateModelFile, fbxModelFile, textureFile);
		}

		public override int ExportFbxWithBlendshapes(string plyModelFile, string texturePath, string binaryBlendshapesDir, string outputFbxPath)
		{
			return exportFbxWithBlendshapes(plyModelFile, texturePath, binaryBlendshapesDir, outputFbxPath);
		}

		public override bool IsFBXExportEnabled { get { return true; } }
#endif
	}
}
