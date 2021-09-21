using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Android;


/// <summary>
/// A basic script to manage the connection to Photon Servers, as well as the creation/joining of a default room.
/// </summary>
public class BasicNetworkingManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// This script is meant to be a singleton.
    /// </summary>
    public static BasicNetworkingManager Instance;

    /// <summary>
    /// A list of all other players in the room.
    /// </summary>
    public ObservedList<BasicPlayerHandler> OtherPlayers = new ObservedList<BasicPlayerHandler>();

    /// <summary>
    /// The main camera object that represents the Nreal glasses.
    /// </summary>
    [SerializeField]
    [Tooltip("The Nreal Camera Rig.")]
    public GameObject NrealCamera;

    [SerializeField]
    [Tooltip("The reference point for spatial interactions.")]
    public Transform ReferenceObject;

    /// <summary>
    /// The game's voice network.
    /// </summary>
    [SerializeField]
    [Tooltip("The Photon Voice network holder.")]
    public PhotonVoiceNetwork VoiceNetwork;

    // Flag to know if the local player has been instantiated or not.
    public bool IsPlayerInstantiated { get; private set; }

    // Reference to the local player.
    public GameObject LocalPlayer { get; private set; }

    // Room Options control the room size and the time for which rooms should be kept active if empty, among other properties.
    RoomOptions _roomOptions;

    private void Awake()
    {
        // Set the instatiation flag to false on awake.
        IsPlayerInstantiated = false;

        // Setting singleton.
        if (Instance is null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        // Checking for Camera and Microphone permissions on Android.
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif

        Debug.Log("Trying to connect to server");

        // Initializing room options.
        _roomOptions = new RoomOptions();
        _roomOptions.CleanupCacheOnLeave = true;

        // Setting the room timeout durations.
        if (!Application.isEditor)
        {
            _roomOptions.EmptyRoomTtl = 3600;
            _roomOptions.PlayerTtl = 3600;
        }

        // Attempt to connect to Photon servers.
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    #region PunCallbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        StartCoroutine(CreateOrJoinRoomCoroutine());
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarningFormat("Create Room failed with reason {0}", message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        StartCoroutine(SpawnPlayer());
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarningFormat("Join room failed with reason {0}", message);
    }

    #endregion

    IEnumerator CreateOrJoinRoomCoroutine()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinOrCreateRoom("test", _roomOptions, TypedLobby.Default);
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(1);
        // Spawn player here.

        if(!IsPlayerInstantiated)
        {
            LocalPlayer = PhotonNetwork.Instantiate("BasicPlayer", NrealCamera.transform.position, NrealCamera.transform.rotation);
            IsPlayerInstantiated = true;
        }
    }

    /// <summary>
    /// Singleton closure.
    /// </summary>
    private void OnDestroy()
    {
        Instance = null;
    }
}
