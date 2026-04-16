using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

public interface IGameManager : ISingleton
{
    GameModel Model { get; }
}
public class GameManager : MonoSingleton<GameManager>, IGameManager
{
    public GameModel Model { get; private set; }

    public void OnSingletonInit()
    {
        Model = new GameModel();
    }
}
