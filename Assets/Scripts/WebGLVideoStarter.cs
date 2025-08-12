using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class WebGLVideoStarter : MonoBehaviour
{
    [Header("Path inside Assets/StreamingAssets")]
    [Tooltip("Use forward slashes. Example: videos/intro.mp4")]
    [SerializeField] private string relativePath = "videos/intro.mp4";

    [Header("Playback")]
    [Tooltip("Start video muted so browsers allow autoplay. Call Unmute() after a user click.")]
    [SerializeField] private bool autoplayMuted = true;

    [Tooltip("Loop the video once it starts playing.")]
    [SerializeField] private bool loop = false;

    private VideoPlayer vp;

    private void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        vp.source = VideoSource.Url;
        vp.isLooping = loop;

        // Build a correct URL for each platform
#if UNITY_WEBGL && !UNITY_EDITOR
        // In WebGL, streamingAssetsPath is already an HTTP(S) URL.
        // Join with forward slashes; DO NOT URL-encode the '/' characters.
        string url = Application.streamingAssetsPath.TrimEnd('/') + "/" + relativePath.Replace("\\", "/");
        vp.url = url;
#else
        // In Editor/Standalone, streamingAssetsPath is a filesystem path.
        // Use Uri to convert to file:///... and safely encode spaces, etc.
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        var uri = new Uri(fullPath);
        vp.url = uri.AbsoluteUri; // e.g., file:///F:/Project/Assets/StreamingAssets/videos/intro.mp4
#endif

        if (autoplayMuted)
        {
            // Start muted so autoplay works in browsers.
            // (If you want routed audio later, switch to AudioSource or Direct in Unmute().)
            vp.audioOutputMode = VideoAudioOutputMode.None;
        }

        vp.errorReceived += (_, err) =>
        {
            Debug.LogError($"[WebGLVideoStarter] Video error: {err} @ {vp.url}");
        };

        // Play as soon as the stream is ready
        vp.prepareCompleted += _ =>
        {
            try { vp.Play(); }
            catch (Exception e) { Debug.LogError($"[WebGLVideoStarter] Play() failed: {e.Message}"); }
        };
    }

    private void Start()
    {
        // Prepare first; prepareCompleted will invoke Play()
        try { vp.Prepare(); }
        catch (Exception e) { Debug.LogError($"[WebGLVideoStarter] Prepare() failed: {e.Message}"); }
    }

    /// <summary>
    /// Call this after a user click to enable sound (bypasses browser autoplay restrictions).
    /// </summary>
    public void Unmute()
    {
        // Choose one of these based on your setup.
        // AudioSource route (recommended if you need volume/mixer control):
        // 1) Add an AudioSource to this GameObject.
        // 2) Enable audio track 0 below.
        // 3) Route to that AudioSource.

        // If there are audio tracks, enable the first one.
        if (vp.clip != null || vp.source == VideoSource.Url)
        {
            // Ensure track 0 exists; silently ignore if not.
            try { vp.SetDirectAudioMute(0, false); } catch { /* ignore */ }
        }

        // Option A: Output directly (simple)
        vp.audioOutputMode = VideoAudioOutputMode.Direct;

        // Option B: Output via an AudioSource on the same GameObject (uncomment if you add one)
        // var audioSrc = GetComponent<AudioSource>();
        // if (audioSrc != null)
        // {
        //     vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //     vp.SetTargetAudioSource(0, audioSrc);
        // }

        // If the browser blocked the first Play(), try again after unmuting.
        if (!vp.isPlaying)
        {
            try { vp.Play(); } catch (Exception e) { Debug.LogError($"[WebGLVideoStarter] Re-Play() failed after Unmute: {e.Message}"); }
        }
    }

    /// <summary>
    /// Optionally switch videos at runtime (expects path relative to StreamingAssets).
    /// </summary>
    public void SetRelativePathAndPlay(string newRelativePath, bool keepMuted = true)
    {
        if (string.IsNullOrWhiteSpace(newRelativePath)) return;
        relativePath = newRelativePath;

#if UNITY_WEBGL && !UNITY_EDITOR
        string url = Application.streamingAssetsPath.TrimEnd('/') + "/" + relativePath.Replace("\\", "/");
        vp.url = url;
#else
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        var uri = new Uri(fullPath);
        vp.url = uri.AbsoluteUri;
#endif

        vp.audioOutputMode = keepMuted ? VideoAudioOutputMode.None : VideoAudioOutputMode.Direct;
        try
        {
            vp.Stop();
            vp.Prepare();
        }
        catch (Exception e)
        {
            Debug.LogError($"[WebGLVideoStarter] SetRelativePathAndPlay failed: {e.Message}");
        }
    }
}
