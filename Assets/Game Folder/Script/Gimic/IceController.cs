using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceController : GimicController
{
    //--------------------------------------------------------------------------
    //  定数
    //--------------------------------------------------------------------------
    public float m_fDamageSpeedMin;
    public int m_nLife = 2;
	public Material m_hibi;


    //--------------------------------------------------------------------------
    //  変数
    //--------------------------------------------------------------------------
	private Vector3 m_vVelosityLast;

    //--------------------------------------------------------------------------
    //  Public関数
    //--------------------------------------------------------------------------

    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
	protected override void FixedUpdate ( )
	{
        if (!GameManager.m_bPlay) { return; }

        m_vVelosityLast = m_rb.velocity;
		base.FixedUpdate ();
	} 

    protected override void OnCollisionEnter(Collision collision)
    {
        //Check Speed
		if(m_status == STATUS.NORMAL && IsGrounded() && m_vVelosityLast.magnitude >= m_fDamageSpeedMin)
        {
            m_nLife--;

			GetComponent<MeshRenderer>().material = m_hibi;

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
