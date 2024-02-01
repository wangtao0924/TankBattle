using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube1 : MonoBehaviour
{
    // Start is called before the first frame update
    public void Clicked()
    {
        MsgMove msgMove = new MsgMove();
        msgMove.x = 9;
        msgMove.y = 8;
        msgMove.z = 7;

        string s = JsonUtility.ToJson(msgMove);
        Debug.Log(s);

        string s1 = "{\"desc\":\"127\"}";
       MsgAttack msgAttack =(MsgAttack)  JsonUtility.FromJson(s1,Type.GetType("MsgAttack"));
        Debug.Log(msgAttack.desc);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
