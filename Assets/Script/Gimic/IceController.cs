using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceController : GimicController
{
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public float m_fDamageSpeedMin;
    public int m_nLife = 2;

    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------

    //--------------------------------------------------------------------------
    //  Public関数
    //--------------------------------------------------------------------------

    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
    protected override void OnCollisionEnter(Collision collision)
    {
        //Check Speed
        if(m_status == STATUS.NORMAL && m_rb.velocity.magnitude >= m_fDamageSpeedMin)
        {
            m_nLife--;

            if(m_nLife == 0)
            {
                gameObject.SetActive(false);

                //Sound
                AkSoundEngine.PostEvent("ice_break", gameObject);
            }
            else
            {//Sound
                AkSoundEngine.PostEvent("ice_hibi", gameObject);
            }
        }
    }

    protected override void PlayHitSound()
    {
        AkSoundEngine.PostEvent("ice_hit_wall", gameObject);
    }
}
