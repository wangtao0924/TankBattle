using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;
using System;

public static class NetManager 
{
    //�����׽���
    static Socket socket;
    //���ܻ�����
    static ByteArray readBuff;
    //д�����
    static Queue<ByteArray> writeQueue;

    //�¼�ί������
    public delegate void EventListener(string err);
    //�¼������б�
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    //��Ӽ����¼�
    public static void AddEventListener(NetEvent netEvent,EventListener eventListener)
    {
        //����¼�
        if (eventListeners.ContainsKey(netEvent)){
            eventListeners[netEvent] += eventListener;
        }
        else
        {
            eventListeners[netEvent] = eventListener;
        }
    }

    //ɾ���¼�����
    public static void RemoveEventListener(NetEvent netEvent,EventListener eventListener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= eventListener;
            //ɾ��
            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }

    //�ַ��¼�
    private static void FireEvent(NetEvent netEvent,string str)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](str);
        }
    }

    //�Ƿ�����
    static bool isConnecting = false;

    //����
    public static void Connect(string ip,int port)
    {
        //״̬�ж�
        if(socket != null && socket.Connected)
        {
            Debug.Log("Connect fail,already Connected!");
            return;
        }
        if (isConnecting)
        {
            Debug.Log("Connect fail,already isConnecting!");
            return;
        }
        //��ʼ����Ա
        InitState();
        //��������
       // socket.NoDelay = true;
        //Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    /// <summary>
    /// Connect�ص�
    /// </summary>
    /// <param name="ar"></param>
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndReceive(ar);
            Debug.Log("Socket Connect Succ!");
            FireEvent(NetEvent.ConnectionSucc, "");
            isConnecting = false;
        }
        catch (Exception e)
        {
            Debug.Log("Socket Connect Fail!"+e.Message.ToString());
            FireEvent(NetEvent.ConnectFail, e.ToString());
            isConnecting = false;
            
        }
    }

    static bool isClosing = false;

    private static void InitState()
    {
        //Socket 
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //���ܻ�����
        readBuff = new ByteArray();
        //д�����
        writeQueue = new Queue<ByteArray>();
        //�Ƿ���������
        isConnecting = false;
        //�Ƿ����ڹر�
        isClosing = false;

    }

    //Connect�ص�����
    private static void ConnectionCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ!");
            FireEvent(NetEvent.ConnectionSucc, "");
            isConnecting = false;
        }
        catch (Exception e)
        {
            Debug.Log("socket fail" + e.Message.ToString());
            FireEvent(NetEvent.ConnectFail, "");
            isConnecting = false;
            throw;
        }
    }

    public static void Close()
    {
        //״̬�ж�
        if(socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }

    }


    public enum NetEvent
    {
        ConnectionSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }

}

internal class ByteArray
{
    //������
    public byte[] bytes;
    //��дλ��
    public int readIdx = 0;
    public int writeIdx = 0;
    //���ݳ���
    public int length { get { return writeIdx - readIdx; } }

    //���캯��
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    public ByteArray()
    {
    }
}

