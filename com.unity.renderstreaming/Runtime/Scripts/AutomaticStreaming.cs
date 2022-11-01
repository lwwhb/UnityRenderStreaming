using System;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.RenderStreaming
{
    public class AutomaticStreaming : MonoBehaviour
    {
        private RenderStreamingHandler renderstreaming;
        private Broadcast broadcast;
        private VideoStreamSender videoStreamSender;
        private AudioStreamSender audioStreamSender;

        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;

            videoStreamSender = gameObject.AddComponent<VideoStreamSender>();
            videoStreamSender.source = VideoStreamSource.Screen;
            videoStreamSender.SetTextureSize(new Vector2Int(Screen.width, Screen.height));

            audioStreamSender = gameObject.AddComponent<AudioStreamSender>();
            audioStreamSender.source = AudioStreamSource.APIOnly;
            var audioListener = FindObjectOfType<AudioListener>();
            var autoFilter = audioListener.gameObject.AddComponent<AutoAudioFilter>();
            autoFilter.SetSender(audioStreamSender);

            broadcast = gameObject.AddComponent<Broadcast>();
            broadcast.AddComponent(videoStreamSender);
            broadcast.AddComponent(audioStreamSender);

            renderstreaming = gameObject.AddComponent<RenderStreamingHandler>();
            renderstreaming.AddSignalingHandler(broadcast);
            renderstreaming.settings = RenderStreaming.SignalingSettings;
            renderstreaming.Run();

            SceneManager.activeSceneChanged += (scene1, scene2) =>
            {
                var audioListener = FindObjectOfType<AudioListener>();
                if (audioListener.gameObject.GetComponent<AutoAudioFilter>() != null)
                {
                    return;
                }
                var autoFilter = audioListener.gameObject.AddComponent<AutoAudioFilter>();
                autoFilter.SetSender(audioStreamSender);
            };
        }

        private void OnDestroy()
        {
            renderstreaming.Stop();
            renderstreaming = null;
            broadcast = null;
            videoStreamSender = null;
            audioStreamSender = null;
        }

        class AutoAudioFilter : MonoBehaviour
        {
            private AudioStreamSender sender;
            private int m_sampleRate;

            public void SetSender(AudioStreamSender sender)
            {
                this.sender = sender;
            }

            private void Awake()
            {
                this.hideFlags = HideFlags.HideInInspector;
            }

            private void OnEnable()
            {
                OnAudioConfigurationChanged(false);
                AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
            }

            private void OnDisable()
            {
                AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigurationChanged;
            }

            private void OnAudioConfigurationChanged(bool deviceWasChanged)
            {
                m_sampleRate = AudioSettings.outputSampleRate;
            }

            private void OnAudioFilterRead(float[] data, int channels)
            {
                if (sender.source != AudioStreamSource.APIOnly)
                {
                    return;
                }

                sender?.SetData(data, channels, m_sampleRate);
            }

            private void OnDestroy()
            {
                sender = null;
            }
        }
    }
}
