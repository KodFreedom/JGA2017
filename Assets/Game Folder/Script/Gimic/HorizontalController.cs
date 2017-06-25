using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalController : GimicController
{
    //--------------------------------------------------------------------------
    //  Public関数
    //--------------------------------------------------------------------------

    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
    protected override void PlayHitSound() { AkSoundEngine.PostEvent("cube_hit_wall", gameObject); }
}
