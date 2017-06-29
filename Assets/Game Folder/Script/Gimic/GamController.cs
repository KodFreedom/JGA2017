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
            FixedJoint jointSelf;
            FixedJoint jointObj;

            //自分と相手のjointがあったらチェックjointの相手が自分かどうか
            jointSelf = gameObject.GetComponent<FixedJoint>();
            if (jointSelf)
            {
                if(jointSelf.connectedBody.gameObject == obj)
                {
                    bFind = true;
                }
            }

            jointObj = obj.GetComponent<FixedJoint>();
            if (jointObj)
            {
                if(jointObj.connectedBody.gameObject == gameObject)
                {
                    bFind = true;
                }
            }

            //違かったら自分か相手にお互いをくっつく
            
            if(!bFind)
            {//相手のジョイントをチェック
                for (int nCnt = 0; nCnt < 5; nCnt++)
                {
                    FixedJoint joint = obj.GetComponent<FixedJoint>();
                    if (!joint && obj != gameObject)
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

            
            if (!bFind)
            {//自分のジョイントをチェック
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
                        if (!joint && obj != collision.gameObject)
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

            AkSoundEngine.PostEvent("gam_hit_gam", gameObject);
        }
    }

    protected override void PlayHitSound() { AkSoundEngine.PostEvent("gam_hit_gam", gameObject); }
}
