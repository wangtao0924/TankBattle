using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgMove:MsgBase
{
    public int x = 0;
    public int y = 0;
    public int z = 0;


    void OnMove(MsgBase msgBase)
    {
        MsgMove msgMove = (MsgMove)msgBase;
        msgMove.x = 100;
        msgMove.y = 10;
        msgMove.z = 1;
    }
}
