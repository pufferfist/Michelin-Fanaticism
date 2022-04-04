using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private RawImage rawImage;

    // Use this for initialization
    void Start()
    {
        videoPlayer = this.GetComponent<VideoPlayer>();
        rawImage = this.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (videoPlayer.texture == null)
        {
            return;

        }

        rawImage.texture = videoPlayer.texture;
    }
}
