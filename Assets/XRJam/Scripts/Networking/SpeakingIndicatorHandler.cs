using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.PUN;
using UnityEngine.UI;

/// <summary>
/// Script that displays an indicator when a player is speaking.
/// </summary>
public class SpeakingIndicatorHandler : MonoBehaviour
{
    /// <summary>
    /// The root speaking indicator object.
    /// </summary>
    [SerializeField]
    [Tooltip("The indicator object.")]
    private GameObject _speakingIndicatorObject;

    /// <summary>
    /// The Photon Voice View on the Player.
    /// </summary>
    private PhotonVoiceView _voiceView;
    
    void Start()
    {
        // Grab the Photon Voice View component.
        _voiceView = this.GetComponent<PhotonVoiceView>();

        // Set the speaking indicator to be inactive.
        _speakingIndicatorObject.SetActive(false);
    }

    void Update()
    {
        // Set the indicator to be active or inactive based on whether a player is speaking or not.
        if (_voiceView.IsSpeaking)
        {
            _speakingIndicatorObject.SetActive(true);
        }
        else
        {
            _speakingIndicatorObject.SetActive(false);
        }
    }



}

