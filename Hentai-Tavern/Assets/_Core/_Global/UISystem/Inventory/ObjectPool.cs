﻿// Assets/_Core/_Util/ObjectPool.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Core._Util
{
    /// <summary>Простой, но гибкий generic-пул компонентов Unity.</summary>
    /// <remarks>
    /// • Поддерживает <c>constructor</c>-API <b>и</b> <c>Init()</c>-API.<br/>
    /// • <c>Expand(n)</c> гарантирует, что всего создано ≥ n объектов.<br/>
    /// • Индексатор <c>[i]</c> даёт доступ к <b>любой</b> созданной копии (активной или нет).<br/>
    /// • Объекты не уничтожаются — только скрываются и переиспользуются.
    /// </remarks>
    public sealed class ObjectPool<T> where T : Component
    {
        /* ─── runtime storages ───────────────────────────────────────── */
        private readonly Stack<T> _inactive = new();
        private readonly List<T>  _active   = new();   // удобно для ReleaseAll()
        private readonly List<T>  _all      = new();   // нужно для индексатора

        /* ─── settings (допускаем ленивую Init) ──────────────────────── */
        private T         _prefab;
        private Transform _parent;

        #region Ctors / Init

        /// <summary>Классический конструктор. Prewarm выполняется сразу.</summary>
        public ObjectPool(T prefab, Transform parent, int initialSize = 0)
        {
            Init(prefab, parent, initialSize);
        }

        /// <summary>Безаргументный Ctor + поздний <see cref="Init"/>.</summary>
        public ObjectPool() { /* поля будут заданы позднее */ }

        /// <summary>Поздняя инициализация: prefab, родитель, начальный размер.</summary>
        public void Init(T prefab, Transform parent, int initialSize = 0)
        {
            if (_prefab) throw new InvalidOperationException("ObjectPool already initialised.");

            _prefab = prefab ? prefab : throw new ArgumentNullException(nameof(prefab));
            _parent = parent;

            Prewarm(initialSize);
        }

        #endregion

        /* ─── public API ─────────────────────────────────────────────── */

        /// <summary>Получить (или создать) активный экземпляр.</summary>
        public T Get()
        {
            EnsureReady();

            T item = _inactive.Count > 0
                     ? _inactive.Pop()
                     : CreateInstance();

            item.gameObject.SetActive(true);
            _active.Add(item);
            return item;
        }

        /// <summary>Вернуть объект в пул.</summary>
        public void Release(T item)
        {
            if (!item) return;

            item.gameObject.SetActive(false);
            item.transform.SetParent(_parent, false);

            _active.Remove(item);
            _inactive.Push(item);
        }

        /// <summary>Вернуть <b>все</b> активные объекты.</summary>
        public void ReleaseAll()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                Release(_active[i]);
        }

        /// <summary>Гарантировать, что всего создано как минимум <paramref name="total"/> объектов.</summary>
        public void Expand(int total)
        {
            EnsureReady();

            int have = _all.Count;
            for (int i = have; i < total; i++)
            {
                var inst = CreateInstance();
                inst.gameObject.SetActive(false);
                _inactive.Push(inst);
            }
        }

        /* ─── helpers / props ────────────────────────────────────────── */

        public int Count        => _all.Count;
        public int ActiveCount  => _active.Count;
        public int InactiveCount=> _inactive.Count;

        public T this[int index] => _all[index];

        private void EnsureReady()
        {
            if (!_prefab)
                throw new InvalidOperationException("ObjectPool not initialised. Call Init().");
        }

        private T CreateInstance()
        {
            var inst = UnityEngine.Object.Instantiate(_prefab, _parent);
            _all.Add(inst);
            return inst;
        }

        private void Prewarm(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                var inst = CreateInstance();
                inst.gameObject.SetActive(false);
                _inactive.Push(inst);
            }
        }
    }
}
