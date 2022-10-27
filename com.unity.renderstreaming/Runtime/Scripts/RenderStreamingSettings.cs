using System;
using UnityEngine;

namespace Unity.RenderStreaming
{
    public class RenderStreamingSettings : ScriptableObject
    {
        public bool automaticStreaming;
        public SignalingSettings signalingSettings;
        public CodecSettings codecSettings;
    }

    [Serializable]
    public class SignalingSettings
    {

    }

    [Serializable]
    public class CodecSettings
    {

    }
}
