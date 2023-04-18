using System;
using UnityEngine;
using Zenject;

namespace HayDay_like.Scripts
{
    public class SignalReceiver : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;

        private void Start()
        {
            _signalBus.Subscribe<UIButtonClickSignal_sc>(ChangeTab);
        }

        private void ChangeTab(UIButtonClickSignal_sc u)
        {
            print(u.strClickedButtonName);
            // todo: 依照按鈕名開分頁
        }
    }
}