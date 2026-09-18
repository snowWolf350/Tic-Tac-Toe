using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float _gridSize = 3.1f;

    [SerializeField] Transform _crossGO;
    [SerializeField] Transform _circleGO;
    [SerializeField] Transform _winLine;

    void Start()
    {
        GameManager.Instance.OnClickedGridPosition += GameManager_OnClickedGridPosition;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
    }
    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        Transform spawnedLine =Instantiate(_winLine,GetWorldGridPosition(e.centre.x,e.centre.y),Quaternion.identity);
        spawnedLine.GetComponent<NetworkObject>().Spawn();
    }

    private void GameManager_OnClickedGridPosition(object sender, GameManager.OnClickedGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.x,e.y,e.playerType);
    }
    [Rpc(SendTo.Server)]
    void SpawnObjectRpc(int x, int y,GameManager.PlayerType playerType)
    {
        Transform visualGO = null;

        switch(playerType)
        {
            case GameManager.PlayerType.cross:
            visualGO = _crossGO;
            break;

            case GameManager.PlayerType.circle:
            visualGO = _circleGO;
            break;
        }

        Transform visual = Instantiate(visualGO,GetWorldGridPosition(x,y),Quaternion.identity);

        visual.GetComponent<NetworkObject>().Spawn(true);        
    }

    private Vector2 GetWorldGridPosition(int x,int y)
    {
        return new Vector2(-_gridSize + x*_gridSize,-_gridSize + y*_gridSize);
    }
}
