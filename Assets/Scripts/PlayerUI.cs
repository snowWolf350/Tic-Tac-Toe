using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject _crossText;
    [SerializeField] GameObject _circleText;
    [SerializeField] GameObject _crossArrow;
    [SerializeField] GameObject _circleArrow;

    void Awake()
    {
        _crossArrow.SetActive(false);
        _circleArrow.SetActive(false);
        _crossText.SetActive(false);
        _circleText.SetActive(false);
    }

    void Start()
    {
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnCurrentPlayerTypeChanged += CurrentPlayerTypeChanged;
    }
    void GameManager_OnGameStart(object sender, System.EventArgs e)
    {
        if(GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.cross )
        {
            _crossText.SetActive(true);
            _circleText.SetActive(false);
        }
        else
        {
            _circleText.SetActive(true);
            _crossText.SetActive(false);
        }

        UpdateCurrentArrow();
    }
    void CurrentPlayerTypeChanged(object sender,System.EventArgs e)
    {
         UpdateCurrentArrow();
    }

    void UpdateCurrentArrow()
    {
        if(GameManager.Instance.GetCurrentPlayerType() == GameManager.PlayerType.cross)
        {
            _crossArrow.SetActive(true);
            _circleArrow.SetActive(false);
        }
        else
        {
            _circleArrow.SetActive(true);
            _crossArrow.SetActive(false);
        }
    }
}
