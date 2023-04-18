using System;
using HayDay_like.Scripts;
using UnityEngine;
using Zenject;

public class HaydayInstaller : MonoInstaller
{
    public SignalReceiver SignalReceiver;
    public override void InstallBindings()
    {
        Container.DeclareSignal<UIButtonClickSignal_sc>();
        Container.BindSignal<UIButtonClickSignal_sc>().ToMethod(UIButtonOnClicked);

        Container.BindInstance(SignalReceiver).AsSingle().NonLazy();
    }

    private void UIButtonOnClicked(UIButtonClickSignal_sc u)
    {
        print(u.strClickedButtonName);
        // todo: 依照按鈕名開分頁
    }
}