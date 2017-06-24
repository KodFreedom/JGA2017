using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GomController : GimicController
{
    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gom"))
        {
            AkSoundEngine.PostEvent("gom_hit_gom", gameObject);
        }
    }

    protected override void PlayHitSound() { AkSoundEngine.PostEvent("cube_hit_wall", gameObject); }
}
