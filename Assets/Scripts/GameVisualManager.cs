using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float _gridSize = 3.1f;

    [SerializeField] Transform _crossGO;
    [SerializeField] Transform _circleGO;

    void Start()
    {
        GameManager.Instance.OnClickedGridPosition += GameManager_OnClickedGridPosition;
    }

    private void GameManager_OnClickedGridPosition(object sender, GameManager.OnClickedGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.x,e.y);
    }
    [Rpc(SendTo.Server)]
    void SpawnObjectRpc(int x, int y)
    {
        Transform visual = Instantiate(_crossGO);

        visual.GetComponent<NetworkObject>().Spawn(true);

        visual.position = GetWorldGridPosition(x,y);
        
    }

    private Vector2 GetWorldGridPosition(int x,int y)
    {
        return new Vector2(-_gridSize + x*_gridSize,-_gridSize + y*_gridSize);
    }
}
