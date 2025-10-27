using UnityEngine;
using UnityEngine.Video;

public class SkillEffectPlayer : MonoBehaviour
{
    public static SkillEffectPlayer Instance;
    public AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[SkillEffectPlayer] Awake() 実行");


        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySkillEffects(SkillData skill, Transform origin)
    {
        if (skill == null) return;
        Debug.Log($"[SkillEffectPlayer] 再生要求: {skill.SkillName}");


        // 🔊 SFX
        if (!string.IsNullOrEmpty(skill.UseSkillSFX001))
            PlaySFX(skill.UseSkillSFX001);

        // 🎬 VFX (MP4再生対応)
        if (!string.IsNullOrEmpty(skill.UseSkillVFX001))
            PlayVFXOnSurface(skill.UseSkillVFX001, origin);
    }

    private void PlaySFX(string name)
    {
        var clip = Resources.Load<AudioClip>("SFX/" + name);
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    private void PlayVFXOnSurface(string videoName, Transform origin)
    {
        Debug.Log($"[SkillEffectPlayer] VFX再生要求: {videoName}");

        string fullPath = "VFX/" + videoName;
        VideoClip clip = Resources.Load<VideoClip>(fullPath);

        if (clip == null)
        {
            Debug.LogError($"[SkillEffectPlayer] VideoClipが見つかりません: {fullPath}");
            return;
        }

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = origin.position + Vector3.down * 0.1f;
        plane.transform.localScale = new Vector3(0.3f, 1, 0.3f);
        plane.transform.rotation = Quaternion.Euler(90, 0, 0);
        plane.name = "VFX_VideoSurface_" + videoName;

        Renderer renderer = plane.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Unlit/Texture"));

        var vp = plane.AddComponent<UnityEngine.Video.VideoPlayer>();
        vp.clip = clip;
        vp.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        vp.targetMaterialRenderer = renderer;
        vp.targetMaterialProperty = "_MainTex";
        vp.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.None;

        vp.Prepare();
        vp.prepareCompleted += (source) =>
        {
            Debug.Log("[SkillEffectPlayer] Video準備完了 → 再生開始");
            source.Play();
        };

        Destroy(plane, (float)clip.length + 0.5f);
    }

}
