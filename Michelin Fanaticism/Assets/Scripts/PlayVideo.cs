using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;

using UnityEngine.Networking;
public class PlayVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private RawImage rawImage;
    public string sourcePath;
    // Use this for initialization
    void Start()
    {
        videoPlayer = this.GetComponent<VideoPlayer>();
        rawImage = this.GetComponent<RawImage>();
     
     
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, sourcePath);
        videoPlayer.url = filePath;
    
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
