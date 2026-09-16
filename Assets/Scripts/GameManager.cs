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

    public event EventHandler OnGameStart;
    public event EventHandler OnCurrentPlayerTypeChanged;

    public enum PlayerType
    {
        none,
        cross,
        circle
    }

    PlayerType _localPlayerType;
    NetworkVariable<PlayerType> _currentPlayerType = new NetworkVariable<PlayerType>();
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
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        _currentPlayerType.OnValueChanged += (PlayerType oldPlayerType,PlayerType newPlayerType) =>
        {
            OnCurrentPlayerTypeChanged?.Invoke(this,EventArgs.Empty);
        };
    }
        void NetworkManager_OnClientConnectedCallback(ulong obj)
        {
            if(NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                _currentPlayerType.Value = PlayerType.cross;
                OnGameStartRpc();
            }
        }
    [Rpc(SendTo.ClientsAndHost)]
    void OnGameStartRpc()
    {
        OnGameStart?.Invoke(this,EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y,PlayerType playerType)
    {
        if(playerType != _currentPlayerType.Value)
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

        switch (_currentPlayerType.Value)
        {
            case PlayerType.cross:
                _currentPlayerType.Value = PlayerType.circle;
            break; 
            case PlayerType.circle:
                _currentPlayerType.Value = PlayerType.cross;
            break;
        }
    }
    

    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }

    public PlayerType GetCurrentPlayerType()
    {
        return _currentPlayerType.Value;
    }
}
