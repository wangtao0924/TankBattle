using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;
using System;

public static class NetManager 
{
    //定义套接字
    static Socket socket;
    //接受缓冲区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;

    //事件委托类型
    public delegate void EventListener(string err);
    //事件监听列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    //添加监听事件
    public static void AddEventListener(NetEvent netEvent,EventListener eventListener)
    {
        //添加事件
        if (eventListeners.ContainsKey(netEvent)){
            eventListeners[netEvent] += eventListener;
        }
        else
        {
            eventListeners[netEvent] = eventListener;
        }
    }

    //删除事件监听
    public static void RemoveEventListener(NetEvent netEvent,EventListener eventListener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= eventListener;
            //删除
            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }

    //分发事件
    private static void FireEvent(NetEvent netEvent,string str)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](str);
        }
    }

    //是否链接
    static bool isConnecting = false;

    //连接
    public static void Connect(string ip,int port)
    {
        //状态判断
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
        //初始化成员
        InitState();
        //参数设置
       // socket.NoDelay = true;
        //Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    /// <summary>
    /// Connect回调
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
        //接受缓冲区
        readBuff = new ByteArray();
        //写入队列
        writeQueue = new Queue<ByteArray>();
        //是否正在连接
        isConnecting = false;
        //是否正在关闭
        isClosing = false;

    }

    //Connect回调函数
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
        //状态判断
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
    //缓冲区
    public byte[] bytes;
    //读写位置
    public int readIdx = 0;
    public int writeIdx = 0;
    //数据长度
    public int length { get { return writeIdx - readIdx; } }

    //构造函数
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

