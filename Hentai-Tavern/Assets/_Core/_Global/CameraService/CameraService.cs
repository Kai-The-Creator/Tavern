using System.Threading;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Core._Global.CameraService
{
    public class CameraService : AService, ICameraService
    {
        [Header("Camera Prefabs")] [SerializeField]
        private Camera mainCameraPrefab;

        [SerializeField] private Camera uiCameraPrefab;

        private Camera mainCamera;
        private Camera uiCamera;
        private CameraFollow follow;

        private Vector3 mainCameraStartPosition;
        private Quaternion mainCameraStartRotation;

        private UniversalAdditionalCameraData mainCameraData;
        private bool isUIInStack = false;

        public override async UniTask OnStart()
        {
            await UniTask.Yield();

            if (!mainCameraPrefab)
            {
                Debug.LogError("CameraService: mainCameraPrefab is not assigned!");
                return;
            }

            mainCamera = Instantiate(mainCameraPrefab, transform);
            mainCameraData = mainCamera.GetComponent<UniversalAdditionalCameraData>()
                             ?? mainCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            mainCameraData.renderType = CameraRenderType.Base;

            follow = mainCamera.gameObject.AddComponent<CameraFollow>();

            DontDestroyOnLoad(mainCamera.gameObject);

            if (!uiCameraPrefab)
            {
                Debug.LogError("CameraService: uiCameraPrefab is not assigned!");
                return;
            }

            uiCamera = Instantiate(uiCameraPrefab, transform);
            var uiData = uiCamera.GetComponent<UniversalAdditionalCameraData>()
                         ?? uiCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            uiData.renderType = CameraRenderType.Overlay;
            DontDestroyOnLoad(uiCamera.gameObject);

            AddUICameraToStack();

            mainCameraStartPosition = mainCamera.transform.position;
            mainCameraStartRotation = mainCamera.transform.rotation;

            Debug.Log("CameraService: Successfully initialized main & UI cameras.");
        }

        private void AddUICameraToStack()
        {
            if (!mainCameraData.cameraStack.Contains(uiCamera))
            {
                mainCameraData.cameraStack.Add(uiCamera);
                isUIInStack = true;
                Debug.Log("CameraService: UI Camera added to Base Camera stack.");
            }
        }

        public void UpdateCameraStack()
        {
            if (!isUIInStack) AddUICameraToStack();
        }

        public void StartFollow(Transform target) => follow.SetTarget(target);
        // public void StartFollow(PlayerAvatar player) => StartFollow(player.CameraAnchor);
        public void StopFollow() => follow.ClearTarget();

        public void SnapTo(Vector3 pos, Quaternion rot)
        {
            DOTween.Kill(mainCamera.transform);
            mainCamera.transform.SetPositionAndRotation(pos, rot);
        }

        public Camera GetMainCamera() => mainCamera;
        public Camera GetUICamera() => uiCamera;

        public void SetMainCameraActive(bool isActive)
        {
            if (mainCamera) mainCamera.gameObject.SetActive(isActive);
        }

        public void SetUICameraActive(bool isActive)
        {
            if (uiCamera) uiCamera.gameObject.SetActive(isActive);
        }

        public void SetMainCameraOrthographic(bool orthographic, float sizeOrFov = 10f)
        {
            if (!mainCamera) return;
            mainCamera.orthographic = orthographic;

            if (orthographic) mainCamera.orthographicSize = sizeOrFov;
            else mainCamera.fieldOfView = sizeOrFov;
        }

        public void SetMainCameraCullingMask(LayerMask mask)
        {
            if (mainCamera) mainCamera.cullingMask = mask;
        }

        public async UniTask MoveCameraToStartPosition()
        {
            DOTween.Kill(mainCamera.transform);

            var move = mainCamera.transform.DOMove(mainCameraStartPosition, .5f).SetEase(Ease.InOutQuad);
            var rot = mainCamera.transform.DORotateQuaternion(mainCameraStartRotation, .5f).SetEase(Ease.InOutQuad);

            await UniTask.WhenAll(move.ToUniTask(), rot.ToUniTask());
        }

        public async UniTask MoveCamera(Transform target, float duration, CancellationToken token, string tweenId)
        {
            DOTween.Kill(mainCamera.transform);

            var move = mainCamera.transform.DOMove(target.position, duration)
                .SetEase(Ease.InOutQuad).SetId(tweenId);
            var rot = mainCamera.transform.DORotateQuaternion(target.rotation, duration)
                .SetEase(Ease.InOutQuad).SetId(tweenId);

            await UniTask.WhenAll(
                move.ToUniTask(cancellationToken: token),
                rot.ToUniTask(cancellationToken: token)
            );
        }
    }
}