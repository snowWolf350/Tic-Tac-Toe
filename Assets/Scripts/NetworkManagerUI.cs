using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button _hostButton;
    [SerializeField] Button _clientButton;

    void Awake()
    {
        _hostButton.onClick.AddListener(() =>
        {
           NetworkManager.Singleton.StartHost();
           Hide(); 
        });
        _clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
