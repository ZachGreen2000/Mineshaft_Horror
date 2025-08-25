using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MineEntrance : MonoBehaviour
{
    public Volume globalVol;
    private Vignette vig;

    private void OnTriggerEnter(Collider col)
    {
        if (globalVol.profile.TryGet(out vig))
        {
            vig.intensity.overrideState = true;
            vig.intensity.value = 0.551f;
            vig.smoothness.overrideState = true;
            vig.smoothness.value = 0.543f;
        }
    }
}
