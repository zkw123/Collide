using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollosionTest : MonoBehaviour
{
    public float force;
    public float friction;
    public float radius = 0.5f;
    public float mass=1.0f;
    public GameObject other;
    public float BoarderX=0;
    public float BoarderZ=0;
    //弹性系数
    public float k = 1.0f;
    //上一帧结束时的速度
    public Vector3 preV;

    void Start()
    {
        preV = Vector3.zero;
        GetBoarder();
    }
    void GetBoarder()
    {
        GameObject wall = GameObject.Find("Wall");
        for(int i=0;i<wall.transform.childCount;i++)
        {
            if(BoarderX<wall.transform.GetChild(i).transform.position.x)
            {
                BoarderX = wall.transform.GetChild(i).transform.position.x;
            }
            if (BoarderZ < wall.transform.GetChild(i).transform.position.z)
            {
                BoarderZ = wall.transform.GetChild(i).transform.position.z;
            }
        }
    }
    void Update()
    {
        //摩擦力
        Vector3 frictionDeltaV = -Time.deltaTime * friction * preV.normalized;
        //防止摩擦力反向运动
        Vector3 finalV = preV + frictionDeltaV;
        if (finalV.x * preV.x <= 0)
            frictionDeltaV.x = -preV.x;
        if (finalV.y * preV.y <= 0)
            frictionDeltaV.y = -preV.y;
        if (finalV.z * preV.z <= 0)
            frictionDeltaV.z = -preV.z;

        //计算用户用力方向
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 fDir = new Vector3(moveHorizontal, 0.0f, moveVertical);
        fDir.Normalize();


        //计算加速度
        Vector3 acceleration = force * fDir;

        Vector3 prePos = transform.position;

        //应用加速度
        Vector3 curV = preV + Time.deltaTime * acceleration + frictionDeltaV;
        transform.Translate((curV + preV) * Time.deltaTime / 2);
        preV = curV;


        //检测是否与其他球相撞
        Vector3 pos = transform.position;
        if (other != null)
        {
            OtherSphere otherSphere = other.GetComponent<OtherSphere>();
            Vector3 otherPos = other.transform.position;

            //球体间碰撞检测，判断球心距离与两球半径之和即可
            if (Vector3.Distance(pos, otherPos) < radius + otherSphere.radius) //简单起见，认为自己的半径为0.5
            {
                Debug.Log("碰撞发生!");
                Vector3 v1 = preV;
                float m1 = mass; // 简单起见，认为自己的质量为1
                Vector3 v2 = otherSphere.preV;
                float m2 = otherSphere.mass;

                preV = ((m1 - m2) * v1 + 2 * m2 * v2) / (m1 + m2);
                otherSphere.preV = ((m2 - m1) * v2 + 2 * m1 * v1) / (m1 + m2);

                //如果有碰撞，位置回退，防止穿透
                transform.position = prePos;
            }
        }
        //检测是否与墙相撞
        if(pos.x>BoarderX-1||pos.x<1-BoarderX)
        {
            Debug.Log("白球撞墙");
            preV.x = -preV.x * k;
            transform.position = prePos;
        }
        if(pos.z > BoarderZ - 1 || pos.z < 1 - BoarderZ)
        {
            Debug.Log("白球撞墙");
            preV.z = -preV.z * k;
            transform.position = prePos;
        }
    }
}
