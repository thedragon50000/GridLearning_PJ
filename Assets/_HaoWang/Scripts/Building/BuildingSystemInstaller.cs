using UnityEngine;
using Zenject;

public class BuildingSystemInstaller : MonoInstaller
{
    public BuildingSystem_sc buildingSystemSc;

    public override void InstallBindings()
    {
        Container.BindInstance(buildingSystemSc).AsSingle();
    }
}