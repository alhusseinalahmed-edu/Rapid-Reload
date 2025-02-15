using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    public static GameLauncher instance;
    [Header("Input Fields")]
    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] TMP_InputField playerNameInput;

    [Header("Text")]
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text statusText;

    [Header("Transforms")]
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;

    [Header("Game Objects")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject startGameButton;
    public void OnUsernameInputValueChanged()
    {
        if (!string.IsNullOrEmpty(playerNameInput.text) && playerNameInput.text.Length > 1 && !string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            PlayerPrefs.SetString("PlayerUsername", playerNameInput.text);
            PhotonNetwork.NickName = playerNameInput.text;
        }
        else
        {
            statusText.text = "Please enter a valid username!";
        }
    }
    private void Awake()
    {
        instance = this;
        PhotonNetwork.ConnectUsingSettings();
        statusText.text = "Connecting to the server....";
        playerNameInput.characterLimit = 15;
    }
    public void CreateOrJoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        statusText.text = "Connected to the server!";
    }
    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("titleMenu");
        if(PlayerPrefs.HasKey("PlayerUsername"))
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerUsername");
            playerNameInput.text = PhotonNetwork.NickName;
        }
        else
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
            playerNameInput.text = PhotonNetwork.NickName;
        }
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInput.text);
        MenuManager.instance.OpenMenu("loadingMenu");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("loadingMenu");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("loadingMenu");

    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
        startGameButton.GetComponent<Button>().interactable = false;
    }
    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("titleMenu");

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MenuManager.instance.OpenMenu("errorMenu");
        errorText.text = message;
    }
    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("roomMenu");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.instance.OpenMenu("errorMenu");
        errorText.text = message;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
