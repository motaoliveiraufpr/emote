using System.IO;
using Emote.Avatar;
using ItSeez3D.AvatarSdk.Core;
using ItSeez3D.AvatarSdk.Offline;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */

public class AvatarGenerator : AvatarGeneratorBase {
    public AvatarGenerator()
    {
        sdkType = SdkType.Offline;
    }

    /// <summary>
    /// Determinates state of the avatar. It simply checks existence of the mesh and texture files. 
    /// </summary>
    private AvatarState GetAvatarState(string avatarCode)
    {
        OfflineAvatarProvider offlineAvatarProvider = avatarProvider as OfflineAvatarProvider;

        AvatarState avatarState = AvatarState.UNKNOWN;

        if (offlineAvatarProvider.Session.IsAvatarCalculating(avatarCode))
            avatarState = AvatarState.GENERATING;
        else
        {
            string meshFilePath = AvatarSdkMgr.Storage().GetAvatarFilename(avatarCode, AvatarFile.MESH_PLY);
            string textureFilePath = AvatarSdkMgr.Storage().GetAvatarFilename(avatarCode, AvatarFile.TEXTURE);
            if (File.Exists(meshFilePath) && File.Exists(textureFilePath))
                avatarState = AvatarState.COMPLETED;
            else
                avatarState = AvatarState.FAILED;
        }

        return avatarState;
    }
}
