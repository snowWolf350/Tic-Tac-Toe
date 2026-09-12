using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event EventHandler<OnClickedGridPositionEventArgs> OnClickedGridPosition;
    public class OnClickedGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    void Awake()
    {
        Instance = this;
    }

    public void ClickedOnGridPosition(int x, int y)
    {
        Debug.Log("Clicked on " + x + y);
        OnClickedGridPosition?.Invoke(this,new OnClickedGridPositionEventArgs
        {
            x = x,
            y = y
        });
    }
}
