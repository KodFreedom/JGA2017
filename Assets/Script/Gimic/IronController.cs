using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronController : GimicController
{
    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
    protected override void PlayHitSound()
    {
        Debug.Log("Ice Hit");
        AkSoundEngine.PostEvent("iron_hit_wall", gameObject);
    }
}
