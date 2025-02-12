using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [Header("References")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private PhotonView PV;
    [SerializeField] Scoreboard scoreboard;
    [SerializeField] TMP_Text winnerText;


    [Header("Settings")]
    [SerializeField] private int killsToWin = 5;

    [HideInInspector] public int mostKills;

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        Instance = this;
    }

    public void CheckKills(int kills, string playerName)
    {
        if (kills > mostKills)
        {
            mostKills = kills;
        }

        if (kills == killsToWin)
        {
            EndGame(playerName);
        }
    }

    public void EndGame(string playerName)
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;

        PV.RPC("DestroyAll", RpcTarget.MasterClient);
        PV.RPC("ShowGameOverMenu", RpcTarget.All, playerName);
    }

    [PunRPC]
    private void DestroyAll()
    {
        PhotonNetwork.DestroyAll();
    }

    [PunRPC]
    private void ShowGameOverMenu(string playerName)
    {
        scoreboard.UpdateScoreboard();
        gameOverMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        winnerText.text = playerName;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        Destroy(RoomManager.Instance.gameObject);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
