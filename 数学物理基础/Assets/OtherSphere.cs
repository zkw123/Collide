using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherSphere : MonoBehaviour
{
    public float friction;
    public float mass;
    public float radius;
    //弹性系数
    public GameObject other;
    public float k = 1.0f;
    public float BoarderX = 0;
    public float BoarderZ = 0;
    
    [NonSerialized]
    
    public Vector3 preV;
    // Start is called before the first frame update
    void Start()
    {
        preV = Vector3.zero;
        GetBoarder();
    }
    void GetBoarder()
    {
        GameObject wall = GameObject.Find("Wall");
        for (int i = 0; i < wall.transform.childCount; i++)
        {
            if (BoarderX < wall.transform.GetChild(i).transform.position.x)
            {
                BoarderX = wall.transform.GetChild(i).transform.position.x;
            }
            if (BoarderZ < wall.transform.GetChild(i).transform.position.z)
            {
                BoarderZ = wall.transform.GetChild(i).transform.position.z;
            }
        }
    }
    // Update is called once per frame
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
        
        //应用加速度
        Vector3 curV = preV + frictionDeltaV;
        
       
       
        transform.Translate((curV + preV) * Time.deltaTime / 2);
        preV = curV;
        Vector3 pos = transform.position;
        if (other != null)
        {
            CollosionTest Sphere = other.GetComponent<CollosionTest>();
            Vector3 otherPos = other.transform.position;

            //球体间碰撞检测，判断球心距离与两球半径之和即可
            if (Vector3.Distance(pos, otherPos) < radius + Sphere.radius) //简单起见，认为自己的半径为0.5
            {
                Debug.Log("碰撞发生!");
                Vector3 v1 = preV;
                float m1 = mass; 
                Vector3 v2 = Sphere.preV;
                float m2 = Sphere.mass;

                preV = ((m1 - m2) * v1 + 2 * m2 * v2) / (m1 + m2);
                Sphere.preV = ((m2 - m1) * v2 + 2 * m1 * v1) / (m1 + m2);

                //如果有碰撞，位置回退，防止穿透
                transform.position = pos;
            }
        }
        if (pos.x > BoarderX - (0.5 + radius) || pos.x < (0.5 + radius - BoarderX))
        {
            Debug.Log("红球撞墙");
            preV.x = -preV.x * k;
            transform.position = pos;
        }
        if (pos.z > BoarderZ - (0.5 + radius) || pos.z < (0.5 + radius - BoarderZ))
        {
            Debug.Log("红球撞墙");
            preV.z = -preV.z * k;
            transform.position = pos;
        }
        
    }
}
