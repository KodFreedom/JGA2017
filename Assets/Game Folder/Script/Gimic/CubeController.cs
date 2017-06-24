using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : GimicController
{
    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
    protected override void PlayHitSound() { AkSoundEngine.PostEvent("cube_hit_wall", gameObject); }
}
