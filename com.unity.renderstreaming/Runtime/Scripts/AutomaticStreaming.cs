using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.RenderStreaming
{
    public class AutomaticStreaming : MonoBehaviour
    {
        private RenderStreaming renderstreaming;
        private Broadcast broadcast;
        private VideoStreamSender videoStreamSender;
        private AudioStreamSender audioStreamSender;

        private void Awake()
        {
            videoStreamSender = gameObject.AddComponent<VideoStreamSender>();
            videoStreamSender.source = VideoStreamSource.Screen;
            videoStreamSender.SetTextureSize(new Vector2Int(Screen.width, Screen.height));

            var audioListener = FindObjectOfType<AudioListener>();
            audioStreamSender = audioListener.gameObject.AddComponent<AudioStreamSender>();
            audioStreamSender.source = AudioStreamSource.AudioListener;
            audioStreamSender.audioListener = audioListener;

            broadcast = gameObject.AddComponent<Broadcast>();
            broadcast.AddComponent(videoStreamSender);
            broadcast.AddComponent(audioStreamSender);

            renderstreaming = gameObject.AddComponent<RenderStreaming>();
            renderstreaming.Run(StreamingSetting.GetSignaling(), new SignalingHandlerBase[] {broadcast});

            SceneManager.activeSceneChanged += (prev, current) =>
            {
                var audioListener = FindObjectOfType<AudioListener>();
                if (audioListener != audioStreamSender.audioListener)
                {
                }
            };
        }

        private void OnDestroy()
        {
            renderstreaming = null;
            broadcast = null;
            videoStreamSender = null;
            DestroyImmediate(audioStreamSender);
            audioStreamSender = null;
        }
    }
}
