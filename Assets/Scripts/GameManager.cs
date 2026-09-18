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

    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs
    {
        public Vector2Int centre;
    }

    public enum PlayerType
    {
        none,
        cross,
        circle
    }

    PlayerType _localPlayerType;
    NetworkVariable<PlayerType> _currentPlayerType = new NetworkVariable<PlayerType>();
    PlayerType[,] _playerTypeArray;
    void Awake()
    {
        Instance = this;
        _playerTypeArray = new PlayerType[3,3];
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

        if(_playerTypeArray[x,y] != PlayerType.none)
        {
            //adready occupied
            return;
        }

        _playerTypeArray[x,y] = playerType;

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

        TestWin();
    }

    bool TestLine(PlayerType playertypeA,PlayerType playertypeB,PlayerType playertypeC)
    {
        if(playertypeA != PlayerType.none && 
        playertypeA == playertypeB &&
        playertypeB == playertypeC)
        {
            return true;
        }
        return false;
    }

    void TestWin()
    {
        if(TestLine(_playerTypeArray[0,0],_playerTypeArray[1,0],_playerTypeArray[2,0]))
        {
            Debug.Log("Win");
            OnGameWin?.Invoke(this,new OnGameWinEventArgs
            {
               centre = new Vector2Int(1,0) 
            });
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
