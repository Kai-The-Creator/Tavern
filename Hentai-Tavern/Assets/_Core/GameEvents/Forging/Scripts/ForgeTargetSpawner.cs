using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using _Core.GameEvents.Forging.Scripts;
using _Core.GameEvents.Forging.Data;
using _Core._Global.CameraService;
using _Core._Global.Services;

namespace _Core.GameEvents.Forging.UI
{
    public sealed class ForgeTargetSpawner : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform targetParent;
        [SerializeField] private ForgeTargetView prefab;

        [Header("Config")]
        [SerializeField] private ForgeGameConfig cfg;

        private readonly List<TargetInfo> active = new();
        private readonly Queue<ForgeTargetView> pool = new();

        private Camera uiCam;
        private Camera gameCam;
        private Renderer model;
        private Rect screenRect;
        private int frame = -1;

        void Awake()
        {
            var camSvc = GService.GetService<ICameraService>();
            uiCam   = camSvc.GetUICamera();
            gameCam = camSvc.GetMainCamera();
        }

        #region API -------------------------------------------------------
        public void SetModel(Renderer rend) => model = rend;

        public bool TrySpawn(out ForgeTargetView view, out Vector2 localPos)
        {
            view = null; localPos = default;
            if (!model) return false;

            CacheRect();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetParent, screenRect.min, uiCam, out var min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetParent, screenRect.max, uiCam, out var max);

            for (int i = 0; i < cfg.MaxSpawnTry; i++)
            {
                var p = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
                if (IsOverlap(p)) continue;

                view = GetFromPool();
                view.RectT.anchoredPosition = p;
                active.Add(new TargetInfo(view, p));
                localPos = p;
                return true;
            }
            return false;
        }

        public void Despawn(ForgeTargetView v)
        {
            for (int i = active.Count - 1; i >= 0; i--)
                if (active[i].View == v)
                    active.RemoveAt(i);

            v.gameObject.SetActive(false);
            pool.Enqueue(v);
        }

        public void ClearAll()
        {
            for (int i = active.Count - 1; i >= 0; i--)
            {
                var view = active[i].View;
                view.gameObject.SetActive(false);
                pool.Enqueue(view);
            }
            active.Clear();
        }

        public int ActiveCount => active.Count;
        #endregion

        #region Helpers ---------------------------------------------------

        private ForgeTargetView GetFromPool()
        {
            var v = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, targetParent);
            v.gameObject.SetActive(true);
            return v;
        }

        private bool IsOverlap(Vector2 pos)
        {
            float rr = cfg.TargetRadius * 2f;
            for (int i = 0; i < active.Count; i++)
                if (Vector2.Distance(pos, active[i].Pos) < rr)
                    return true;
            return false;
        }

        private void CacheRect()
        {
            if (frame == Time.frameCount) return;
            frame = Time.frameCount;

            var c = model.bounds.center;
            var e = model.bounds.extents;
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);

            for (int ix = -1; ix <= 1; ix += 2)
            for (int iy = -1; iy <= 1; iy += 2)
            for (int iz = -1; iz <= 1; iz += 2)
            {
                var p = new Vector3(c.x + ix*e.x, c.y + iy*e.y, c.z + iz*e.z);
                var s = (Vector2)gameCam.WorldToScreenPoint(p);
                if (s.x < min.x) min.x = s.x; if (s.y < min.y) min.y = s.y;
                if (s.x > max.x) max.x = s.x; if (s.y > max.y) max.y = s.y;
            }
            float r = cfg.TargetRadius;
            screenRect = Rect.MinMaxRect(min.x + r, min.y + r, max.x - r, max.y - r);
        }

        private readonly struct TargetInfo
        {
            public readonly ForgeTargetView View;
            public readonly Vector2 Pos;
            public TargetInfo(ForgeTargetView v, Vector2 p){ View=v; Pos=p;}
        }
        #endregion
    }
}
