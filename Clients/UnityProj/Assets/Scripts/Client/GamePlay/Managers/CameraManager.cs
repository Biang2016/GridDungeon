using BiangLibrary.Singleton;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    public Camera MainCamera;
    public Camera BattleUICamera;
}