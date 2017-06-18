using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamController : GimicController
{
    //--------------------------------------------------------------------------
    //  Protected関数
    //--------------------------------------------------------------------------
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gimic"))
        {
            //ジョイントをチェック
            FixedJoint joint = gameObject.GetComponent<FixedJoint>();
            if (!joint)
            {
                gameObject.AddComponent<FixedJoint>();
                gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
            }
            else
            {
                //相手のジョイントをチェック
                GameObject obj = collision.gameObject;
                for (int nCnt = 0;nCnt < 10;nCnt++)
                {
                    FixedJoint jointC = obj.GetComponent<FixedJoint>();
                    if (!jointC)
                    {
                        obj.AddComponent<FixedJoint>();
                        obj.GetComponent<FixedJoint>().connectedBody = m_rb;
                        break;
                    }
                    else
                    {
                        obj = jointC.connectedBody.gameObject;
                    }
                }
            }

            AkSoundEngine.PostEvent("gam_hit_gam", gameObject);
        }
    }

    protected override void PlayHitSound() { AkSoundEngine.PostEvent("gam_hit_gam", gameObject); }
}
