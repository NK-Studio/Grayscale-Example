using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Post-processing/Grayscale")]
public class Grayscale : VolumeComponent, IPostProcessComponent
{
    /// <summary>
    /// Grayscale 효과 세기를 조절합니다.
    /// </summary>
    [Tooltip("Grayscale 효과 세기를 조절합니다.")]
    public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0f, 1f);
    
    /// <summary>
    /// Intensity의 값에 따라 활성화 여부를 반환합니다.
    /// </summary>
    public bool IsActive() => Intensity.value > 0f;

#if UNITY_2023
    // IsTileCompatible는 2023.1 버전부터 사용되지 않습니다.
    // public bool IsTileCompatible() => true;
#else
    // True시 효과를 각 타일(화면의 부분을 나타내는 일종의 블록)에 분리하여 적용합니다.
    // 이 방식은 메모리를 절약하고 속도를 높이는 데 도움이 됩니다.
    //
    // False시 효과가 전체 화면에 한 번에 적용되어 동작합니다.
    // 유니티에서는 이 기능을 2023.1 버전부터 제거됩니다. 그러므로 전체 화면에 적용되도록 설정합니다.
    public bool IsTileCompatible() => false;
#endif
}