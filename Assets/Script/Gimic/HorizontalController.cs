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
    protected override void FixedUpdate()
    {
        if (Time.timeScale != 0)
        {
            //m_rb.velocity = Vector3.zero;
        }

        base.FixedUpdate();
    }

    protected override void PlayHitSound() { AkSoundEngine.PostEvent("cube_hit_wall", gameObject); }
}
