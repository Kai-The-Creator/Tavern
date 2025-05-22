using System.Threading;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Global.CameraService
{
    public interface ICameraService : IService
    {
        UniTask MoveCamera(Transform target, float duration, CancellationToken token, string tweenId);
        UniTask MoveCameraToStartPosition();

        void    StartFollow(Transform t);
        // void    StartFollow(PlayerAvatar p);
        void    StopFollow();
        void    SnapTo(Vector3 pos, Quaternion rot);

        void    SetMainCameraActive(bool a);
        void    SetUICameraActive(bool a);
        void    SetMainCameraOrthographic(bool ortho, float sizeOrFov=10f);
        void    SetMainCameraCullingMask(LayerMask mask);

        Camera  GetMainCamera();
        Camera  GetUICamera();
    }
}