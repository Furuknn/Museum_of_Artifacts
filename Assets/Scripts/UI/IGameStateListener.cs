using UnityEngine;

public interface IGameStateListener
{
  public void GameStateChangedCallBack(EGameState gameState);
}
