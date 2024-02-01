using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectionSucc, onConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, onConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, onConnectClose);

    }

    public void clicked()
    {
        NetManager.Connect("127.0.0.1", 8888);
        //TODO:��ʼתȦ����ʾ������

    }

    //���ӶϿ��ص�
    private void onConnectClose(string err)
    {
       
    }

    public void OnCloseClick()
    {
        NetManager.Close();
    }

    /// <summary>
    /// ����ʧ�ܻص�
    /// </summary>
    /// <param name="err"></param>
    private void onConnectFail(string err)
    {
        
    }

    /// <summary>
    /// ���ӳɹ��ص�
    /// </summary>
    /// <param name="err"></param>
    private void onConnectSucc(string err)
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
