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
            GameObject obj = collision.gameObject;
            bool bFind = false;
            //自分と相手のjointがあったらチェックjointの相手が自分かどうか
            if(gameObject.GetComponent<FixedJoint>())
            {
                if(gameObject.GetComponent<FixedJoint>().connectedBody.gameObject == obj)
                {
                    bFind = true;
                }
            }
            if (obj.GetComponent<FixedJoint>())
            {
                if(obj.GetComponent<FixedJoint>().connectedBody.gameObject == gameObject)
                {
                    bFind = true;
                }
            }

            //違かったら自分か相手にお互いをくっつく
            //相手のジョイントをチェック
            if(!bFind)
            {
                for (int nCnt = 0; nCnt < 5; nCnt++)
                {
                    FixedJoint joint = obj.GetComponent<FixedJoint>();
                    if (!joint)
                    {
                        obj.AddComponent<FixedJoint>();
                        obj.GetComponent<FixedJoint>().connectedBody = m_rb;
                        bFind = true;
                        break;
                    }
                    else
                    {
                        obj = joint.connectedBody.gameObject;
                    }
                }
            }

            //自分のジョイントをチェック
            if (!bFind)
            {
                FixedJoint joint = gameObject.GetComponent<FixedJoint>();
                if(!joint)
                {
                    gameObject.AddComponent<FixedJoint>();
                    gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
                }
                else
                {
                    obj = joint.connectedBody.gameObject;
                    for (int nCnt = 0; nCnt < 5; nCnt++)
                    {
                        joint = obj.GetComponent<FixedJoint>();
                        if (!joint)
                        {
                            obj.AddComponent<FixedJoint>();
                            obj.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
                            bFind = true;
                            break;
                        }
                        else
                        {
                            obj = joint.connectedBody.gameObject;
                        }
                    }
                }
            }

            ////ジョイントをチェック
            //FixedJoint joint = gameObject.GetComponent<FixedJoint>();
            //if (!joint)
            //{
            //    gameObject.AddComponent<FixedJoint>();
            //    gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
            //}
            //else
            //{
            //    //相手のジョイントをチェック
            //    GameObject obj = collision.gameObject;
            //    for (int nCnt = 0;nCnt < 10;nCnt++)
            //    {
            //        FixedJoint jointC = obj.GetComponent<FixedJoint>();
            //        if (!jointC)
            //        {
            //            obj.AddComponent<FixedJoint>();
            //            obj.GetComponent<FixedJoint>().connectedBody = m_rb;
            //            break;
            //        }
            //        else
            //        {
            //            obj = jointC.connectedBody.gameObject;
            //        }
            //    }
            //}

            AkSoundEngine.PostEvent("gam_hit_gam", gameObject);
        }
    }

    protected override void PlayHitSound() { AkSoundEngine.PostEvent("gam_hit_gam", gameObject); }
}
