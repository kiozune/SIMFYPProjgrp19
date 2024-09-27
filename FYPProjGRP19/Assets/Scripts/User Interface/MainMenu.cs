using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [Header("Model Value")]
    [SerializeField]
    private GameObject robotModel;
    [SerializeField]
    private GameObject robotStageSelect;
    [SerializeField]
    private Animator robotAnimator;
    [SerializeField]
    private Animator robotStageAnimator;

    [Header("Stage select Values")]
    [SerializeField]
    private bool isMelee = false;
    [SerializeField]
    private bool isRanged = false;
    [SerializeField]
    private bool isMultiplayer = false;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject[] playButtonList;

    [SerializeField]
    private GameObject[] mainMenuBtnList;

    [SerializeField]
    private GameObject[] stageSelectList;

    [SerializeField]
    private GameObject settingsButton;

    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioClip[] audioClipList;
    [SerializeField]
    private GameObject singleplayerButton;
    [SerializeField]
    private GameObject multiplayerButton;
    [SerializeField]
    private GameObject backButton;

    [Header("Photon Multiplayer")]
    [SerializeField] 
    private GameObject readyButton;
    [SerializeField] 
    private Text chatDisplay;
    [SerializeField] 
    private InputField chatInputField;
    [SerializeField] 
    private Button sendChatButton;
    [SerializeField]
    private bool isPlayerReady = false;
    [SerializeField]
    private bool otherPlayerReady = false;
    [SerializeField]
    private bool isStageSelected = false;
    [SerializeField]
    private bool isConnectingToRoom = false;
    [SerializeField]
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        robotAnimator = robotModel.GetComponent<Animator>();
        robotStageAnimator = robotStageSelect.GetComponent<Animator>();
        sfxSource.clip = audioClipList[0];
        photonView = GetComponent<PhotonView>();
        sendChatButton.onClick.AddListener(SendChatMessage);
        // Automatically connect to Photon if not already connected
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Connects to the Photon Master Server
            Debug.Log("Connecting to Photon...");
        }
        SetNickname();
    }



    public void onPlayClicked()
    {
        singleplayerButton.SetActive(true);
        multiplayerButton.SetActive(true);
        backButton.SetActive(true);
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
            mainMenuBtnList[j].SetActive(false);
        }
    }
    public void onSinglePlayerClicked()
    {
        robotModel.SetActive(true);
        robotAnimator.SetTrigger("angry");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(true);
        }
        singleplayerButton.SetActive(false);
        multiplayerButton.SetActive(false);
        isMultiplayer = false;
    }
    private void SetNickname()
    {
        int randomNum;
        bool nicknameAssigned = false;

        // Loop until a unique nickname is assigned
        while (!nicknameAssigned)
        {
            randomNum = Random.Range(1, 3); // Generate a random number between 1 and 2

            // Check if the nickname is already taken by other players
            bool isTaken = false;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.NickName == "Player " + randomNum.ToString())
                {
                    isTaken = true;
                    break; // Exit the loop if the nickname is taken
                }
            }

            // If nickname is not taken, assign it
            if (!isTaken)
            {
                PhotonNetwork.NickName = "Player " + randomNum.ToString();
                nicknameAssigned = true; // Exit the loop
            }
        }
    }


    public void onMultiplayerClicked()
    {
        robotModel.SetActive(true);
        robotAnimator.SetTrigger("angry");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(true);
        }
        singleplayerButton.SetActive(false);
        multiplayerButton.SetActive(false);
        isMultiplayer = true;

        JoinOrCreateRoom();
    }
    // Called when connected to the Photon Master Server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server.");

        // If trying to connect to a room after Master connection
        if (isConnectingToRoom)
        {
            JoinOrCreateRoom();
        }
    }
    // Function to join or create a room
    public void JoinOrCreateRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRandomRoom(); // Try to join a random room
        }
        else
        {
            isConnectingToRoom = true;
            Debug.Log("Waiting to connect to Master Server before joining a room...");
        }
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined multiplayer room. Waiting for the second player...");
        SetNickname();
        readyButton.SetActive(true); // Show ready button
        multiplayerButton.SetActive(false);
        singleplayerButton.SetActive(false);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // If failed to join a random room, create a new one
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Show message when a second player joins
        Debug.Log("Player joined. Both players in room.");
        chatDisplay.text += "Player joined the room.\n";
    }
    // Handle the chat message sending
    public void SendChatMessage()
    {
        // Ensure the input field is not null
        if (chatInputField == null)
        {
            Debug.LogError("chatInputField is not assigned!");
            return;
        }

        string message = chatInputField.text.Trim();

        // Ensure the message is not empty
        if (!string.IsNullOrEmpty(message))
        {
            // Ensure photonView is assigned and valid before calling RPC
            if (photonView != null)
            {
                photonView.RPC("UpdateChat", RpcTarget.All, PhotonNetwork.NickName + ": " + message);
            }
            else
            {
                Debug.LogError("photonView is not assigned!");
            }

            chatInputField.text = "";  // Clear the chat input field after sending message
        }
    }
    [PunRPC]
    public void UpdateChat(string message)
    {
        chatDisplay.text += message + "\n";
    }
    [PunRPC]
    public void ShowSelectedStage(string stageName)
    {
        chatDisplay.text += "Stage selected: " + stageName + "\n";
    }

    public void OnReadyButtonClicked()
    {
        isPlayerReady = true;
        readyButton.SetActive(false); // Hide ready button after click
        photonView.RPC("PlayerReady", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void PlayerReady(int playerID)
    {
        if (playerID != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            otherPlayerReady = true;
        }

        if (isPlayerReady && otherPlayerReady && isStageSelected)
        {
            // Both players are ready and stage is selected
            StartCoroutine(StartGameCountdown());
        }
    }

    private IEnumerator StartGameCountdown()
    {
        chatDisplay.text += "Both players are ready. Game starting in 10...\n";
        int countdown = 10;
        while (countdown > 0)
        {
            chatDisplay.text += countdown.ToString() + "\n";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // Load the game scene
        SceneManager.LoadScene("First Level");
    }
    public void onBackclicked()
    {
        robotModel.SetActive(false);
        robotAnimator.SetTrigger("normal");
        robotStageAnimator.SetTrigger("normal");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(false);
        }
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
            mainMenuBtnList[j].SetActive(true);
        }
        singleplayerButton.SetActive(false);
        multiplayerButton.SetActive(false);
    }

    public void onMeleeClicked()
    {
        isMelee = true;
        isRanged = false;
       
        sfxSource.Play();

        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(false);
        }
        for (int j = 0; j < stageSelectList.Length; ++j)
        {
            stageSelectList[j].SetActive(true);
        }
        robotStageSelect.SetActive(true);
        robotModel.SetActive(false);
        robotStageAnimator.SetTrigger("shuffleDance");
    }
    public void onRangeClicked()
    {
        isRanged = true;
        isMelee = false;

        sfxSource.Play();

        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(false);
        }
        for (int j = 0; j < stageSelectList.Length; ++j)
        {
            stageSelectList[j].SetActive(true);
        }
        robotStageSelect.SetActive(true);
        robotModel.SetActive(false);
        robotStageAnimator.SetTrigger("shuffleDance");
    }

    public void onAncientStageSelect()
    {
        if (isMultiplayer)
        {
            // Handle multiplayer stage selection
            if (isMelee)
            {
                PlayerPrefs.DeleteKey("Meleewep");
                PlayerPrefs.DeleteKey("Rangewep");
                PlayerPrefs.SetInt("Meleewep", 1);
                PlayerPrefs.SetInt("Rangewep", 0);
            }
            else if (isRanged)
            {
                PlayerPrefs.DeleteKey("Meleewep");
                PlayerPrefs.DeleteKey("Rangewep");
                PlayerPrefs.SetInt("Meleewep", 0);
                PlayerPrefs.SetInt("Rangewep", 1);
            }

            // Send stage selection to other player
            photonView.RPC("ShowSelectedStage", RpcTarget.All, "Ancient Stage");

            // Show the "Ready" button for both players
            readyButton.SetActive(true);

            Debug.Log("Ancient stage selected in multiplayer.");
        }
        else
        {
            // Single player mode - directly load the scene
            if (isMelee)
            {
                PlayerPrefs.DeleteKey("Meleewep");
                PlayerPrefs.DeleteKey("Rangewep");
                PlayerPrefs.SetInt("Meleewep", 1);
                PlayerPrefs.SetInt("Rangewep", 0);
            }
            else if (isRanged)
            {
                PlayerPrefs.DeleteKey("Meleewep");
                PlayerPrefs.DeleteKey("Rangewep");
                PlayerPrefs.SetInt("Meleewep", 0);
                PlayerPrefs.SetInt("Rangewep", 1);
            }

            SceneManager.LoadScene("First Level");
        }
    }
    public void fromStageSelectBack()
    {
        robotModel.SetActive(true);
        robotStageSelect.SetActive(false);
        robotAnimator.SetTrigger("angry");
        sfxSource.Play();
        for (int i = 0; i < playButtonList.Length; ++i)
        {
            playButtonList[i].SetActive(true);
        }
        for (int j = 0; j < stageSelectList.Length; ++j)
        {
            stageSelectList[j].SetActive(false);
        }
    }

    public void exitGame()
    {
        Application.Quit();
        // for use in editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void rangeHover()
    {
        robotAnimator.SetTrigger("ranged_idle");
    }
    public void meleeHover()
    {
        robotAnimator.SetTrigger("melee_idle");
    }
    public void backHover()
    {
        robotAnimator.SetTrigger("scared");
    }
    public void resetAnimation()
    {
        robotAnimator.SetTrigger("angry");
    }
    public void onSettingsClick()
    {
        settingsButton.SetActive(true);
        sfxSource.Play();
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
            if(j != 1)
            mainMenuBtnList[j].SetActive(false);
        }
    }
    public void onBacksettingsClicked()
    {
        sfxSource.Play();
        for (int j = 0; j < mainMenuBtnList.Length; ++j)
        {
                mainMenuBtnList[j].SetActive(true);
        }
    }
    public void onMuseumClicked()
    {
        SceneManager.LoadScene("Museum");
    }
}
