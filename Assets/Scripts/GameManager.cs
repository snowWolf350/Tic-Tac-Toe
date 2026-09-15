using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public event EventHandler<OnClickedGridPositionEventArgs> OnClickedGridPosition;
    public class OnClickedGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public enum PlayerType
    {
        none,
        cross,
        circle
    }

    [SerializeField]PlayerType _localPlayerType;
    [SerializeField]PlayerType _currentPlayerType;



    void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            _localPlayerType = PlayerType.cross;    
        }
        else
        {
            _localPlayerType = PlayerType.circle;
        }

        if (IsServer)
        {
            _currentPlayerType = PlayerType.cross;
        }

    }
    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y,PlayerType playerType)
    {
        if(playerType != _currentPlayerType)
        {
            //not your turn
            return;
        }

        OnClickedGridPosition?.Invoke(this,new OnClickedGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = playerType
        });

        switch (_currentPlayerType)
        {
            case PlayerType.cross:
                _currentPlayerType = PlayerType.circle;
            break; 
            case PlayerType.circle:
                _currentPlayerType = PlayerType.cross;
            break;
        }
    }

    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }
}
