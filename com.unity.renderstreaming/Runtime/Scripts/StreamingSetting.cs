using System.Threading;
using Unity.RenderStreaming.Signaling;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.RenderStreaming
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    internal static class StreamingSetting
    {
        public static bool IsAutomaticEnabled()
        {
            //todo: detected by settings
            return true;
        }

        public static ISignaling GetSignaling()
        {
            var schema = false ? "wss" : "ws";
            var address = "localhost:80";
            var signalingInterval = 5;
            var signaling = new WebSocketSignaling(
                $"{schema}://{address}", signalingInterval, SynchronizationContext.Current);
            return signaling;
        }

        static StreamingSetting()
        {
            // load settings
#if UNITY_EDITOR
            InitializeInEditor();
#else
            InitializeInPlayer();
#endif
        }

#if UNITY_EDITOR
        private static void InitializeInEditor()
        {
        }

#else
        private static void InitializeInPlayer()
        {
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RunInitialize()
        {
            // common initialization

            if (IsAutomaticEnabled())
            {
                var automaticGo = new GameObject("AutomaticStreaming");
                automaticGo.AddComponent<AutomaticStreaming>();
                Object.DontDestroyOnLoad(automaticGo);
            }
        }
    }
}
