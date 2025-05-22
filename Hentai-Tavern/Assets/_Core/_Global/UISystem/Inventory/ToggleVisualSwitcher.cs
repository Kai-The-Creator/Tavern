// Assets/_Core/_Global/UI/Common/ToggleVisualSwitcher.cs
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI
{
    /// <summary>
    /// Кастом-логика для Toggle-кнопок:
    /// • При смене состояния прячет/показывает два набора объектов.<br/>
    /// • Масштабирует графику чек-марка (по умолчанию «чуточку больше», чтобы
    ///   визуально подсветить активное состояние).<br/>
    /// • Работает в Edit- и Play-mode (ExecuteAlways).
    /// </summary>
    [ExecuteAlways, RequireComponent(typeof(Toggle))]
    public sealed class ToggleVisualSwitcher : MonoBehaviour
    {
        [Header("Что показывать, когда Toggle = ON")]
        [SerializeField] private GameObject[] _showOn;

        [Header("Что показывать, когда Toggle = OFF")]
        [SerializeField] private GameObject[] _showOff;

        [Header("Чек-марк (необязательно)")]
        [SerializeField] private Graphic _checkmark;             // Image / TMP etc.
        [SerializeField] private Vector3 _onScale  = Vector3.one * 1.25f;
        [SerializeField] private Vector3 _offScale = Vector3.one;

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(ApplyState);
            ApplyState(_toggle.isOn);
        }

        private void OnDestroy() => _toggle.onValueChanged.RemoveListener(ApplyState);

        private void ApplyState(bool isOn)
        {
            // показать / скрыть массивы
            SetActiveArray(_showOn,  isOn);
            SetActiveArray(_showOff, !isOn);

            if (_checkmark != null)
                _checkmark.rectTransform.localScale = isOn ? _onScale : _offScale;
        }

        private void SetActiveArray(GameObject[] arr, bool state)
        {
            if (arr == null) return;
            foreach (var go in arr)
                if (go) go.SetActive(state);
        }
    }
}
