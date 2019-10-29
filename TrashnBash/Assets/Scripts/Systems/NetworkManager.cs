using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public string networkStatus = null;     // are we connected to a network
    public string internetStatus = null;    // is that network connected to the internet

    private const float CONN_CHECK_INTERVAL = 5.0f; // Connection check interval.
    private NetworkReachability _networkStatus = NetworkReachability.NotReachable;
    private bool _hasInternetConnection = false;

    public static event Action<NetworkReachability> NetworkStatusChanged;

    private void Start()
    {
        UpdateStatusText();
        StartCoroutine(ConnectionCheckRoutine());
    }

    private IEnumerator ConnectionCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(CONN_CHECK_INTERVAL);

            // Check to see if the network status has changed.
            var currentStatus = Application.internetReachability;
            if(_networkStatus != currentStatus)
            {
                Debug.Log($"Network status changed to: {currentStatus.ToString()}");
                _networkStatus = currentStatus;
                NetworkStatusChanged?.Invoke(_networkStatus);
            }

            // if a network is reachable, check to see if it is connected to the net.
            if (_networkStatus != NetworkReachability.NotReachable)
            {
                UnityWebRequest wr = new UnityWebRequest("http://www.google.com");
                yield return wr.SendWebRequest();

                if (string.IsNullOrEmpty(wr.error))
                {
                    _hasInternetConnection = true;
                }
                else
                {
                    Debug.Log($"Internet Unreachable: {wr.error}");
                    _hasInternetConnection = false;
                }
            }
            else
            {
                Debug.Log($"Internet Unreachable: {_networkStatus.ToString()}");
                _hasInternetConnection = false;
            }
            UpdateStatusText();
        }
    }

    private void UpdateStatusText()
    {
        networkStatus = _networkStatus.ToString();
        internetStatus = _hasInternetConnection ? "Online" : "Offline";
    }
}
