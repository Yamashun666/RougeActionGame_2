using System.Collections;
using UnityEngine;

public class SkillVisualHandler
{
    private AudioSource audioSource;
    private Transform effectOrigin;

    public SkillVisualHandler(AudioSource source, Transform origin)
    {
        audioSource = source;
        effectOrigin = origin;
    }

    public IEnumerator PlaySkillEffects(SkillInstance instance)
    {
        SkillData data = instance.Data;

        // SFX
        yield return PlaySFXDelayed(data.UseSkillSFX001, data.DelayUseSkillSFX001);
        yield return PlaySFXDelayed(data.UseSkillSFX002, data.DelayUseSkillSFX002);

        // VFX
        yield return PlayVFXDelayed(data.UseSkillVFX001, data.DelayUseSkillVFX001);
        yield return PlayVFXDelayed(data.UseSkillVFX002, data.DelayUseSkillVFX002);
    }

    private IEnumerator PlaySFXDelayed(string sfxName, float delay)
    {
        if (string.IsNullOrEmpty(sfxName)) yield break;

        yield return new WaitForSeconds(delay);
        AudioClip clip = Resources.Load<AudioClip>(sfxName);
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    private IEnumerator PlayVFXDelayed(string vfxName, float delay)
    {
        if (string.IsNullOrEmpty(vfxName)) yield break;

        yield return new WaitForSeconds(delay);
        GameObject prefab = Resources.Load<GameObject>(vfxName);
        if (prefab != null && effectOrigin != null)
            GameObject.Instantiate(prefab, effectOrigin.position, Quaternion.identity);
    }
}
