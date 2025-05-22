// CookingHUD.cs
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts.HUD
{
    public class CookingHUD : MonoBehaviour
    {
        [field: SerializeField] public OrdersPanel        OrdersPanel        { get; private set; }
        [field: SerializeField] public SelectedOrderPanel SelectedOrderPanel { get; private set; }
        [field: SerializeField] public CuttingPanel       CuttingPanel       { get; private set; }
        [field: SerializeField] public CookPanel          CookPanel          { get; private set; }
        [field: SerializeField] public ServingPanel       ServingPanel       { get; private set; }
        [field: SerializeField] public ResultPanel        ResultPanel        { get; private set; }

        public void HideAll()
        {
            OrdersPanel.Hide();
            SelectedOrderPanel.Hide();
            CuttingPanel.Hide();
            CookPanel.Hide();
            ServingPanel.Hide();
            ResultPanel.Hide();
        }
    }
}