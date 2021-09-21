using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;

/// <summary>
/// Deals with the networked player
/// </summary>
public class BasicPlayerHandler : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{

    [Tooltip("Smoothing for player movement and rotation")]
    [SerializeField]
    private float _lerpMultiplier = 80.0f;

    //Used to get new transform vlaues through network
    private Vector3 _newPos;
    private Quaternion _newRot;

    private void Awake()
    {
        // Set the initial transform values.
        _newPos = transform.position;
        _newRot = transform.rotation;
    }

    /// <summary>
    /// Callback function for once the player has been instatiated on the Photon Network.
    /// </summary>
    /// <param name="info"></param>
    void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.IsMine)
        {
            // Move local avatar along with the Nreal.
            transform.parent = BasicNetworkingManager.Instance.NrealCamera.transform;

            // Attach to Photon Voice Network
            BasicNetworkingManager.Instance.VoiceNetwork.PrimaryRecorder = this.gameObject.GetComponent<PhotonVoiceView>().RecorderInUse;
        }
        else
        {
            // Add the player to the OtherPlayers list.
            BasicNetworkingManager.Instance.OtherPlayers.Add(this);
        }
    }


    private void Update()
    {
        // For all non-local players, modify their transforms based on the information received from Photon.
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _newPos, Time.deltaTime * _lerpMultiplier);
            transform.rotation = Quaternion.Lerp(transform.rotation, _newRot, Time.deltaTime * _lerpMultiplier);
        }
    }

    /// <summary>
    /// Sends the position and rotation of a player relative to the reference object.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(BasicNetworkingManager.Instance.ReferenceObject.InverseTransformPoint(transform.position));
            stream.SendNext(Quaternion.Inverse(BasicNetworkingManager.Instance.ReferenceObject.rotation) * transform.rotation);
        }
        else
        {
            _newPos = BasicNetworkingManager.Instance.ReferenceObject.TransformPoint((Vector3)stream.ReceiveNext());
            _newRot = BasicNetworkingManager.Instance.ReferenceObject.rotation * (Quaternion)stream.ReceiveNext();
        }
    }


}
