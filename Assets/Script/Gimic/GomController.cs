using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GomController : GimicController
{
    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
    protected override void PlayHitSound() { AkSoundEngine.PostEvent("gom_hit_gom", gameObject); }
}
