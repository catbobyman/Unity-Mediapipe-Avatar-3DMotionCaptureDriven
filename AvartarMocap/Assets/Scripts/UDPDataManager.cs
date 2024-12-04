using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class UDPDataManager : MonoBehaviour
{
    
    private UdpClient udpClient;
    private Thread receiveThread;
    public int port = 8888; // Python发送数据的端口

    
    public Pose pose;
    public Hand hand;
    public Face face;
    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("UDPDataManager started");
    }

    private void ReceiveData()
    {
        while (true)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
            byte[] receivedData = udpClient.Receive(ref remoteEndPoint);
            string jsonData = Encoding.UTF8.GetString(receivedData);

            BaseData baseData = JsonUtility.FromJson<BaseData>(jsonData);

            if (baseData != null)
            {
                switch (baseData.result_type)
                {
                    case "pose":
                        
                       pose.poselm = JsonUtility.FromJson<Pose.PoseData>(jsonData);
                     
                        break;
                    
                    case"face_landmarks":
                        
                        face.facelm=JsonUtility.FromJson<Face.FaceData_landmark>(jsonData);
                        
                        break;
                    
                    case"face_blendshape":
                        
                        face.facebs = JsonUtility.FromJson<Face.FaceData_blendshape>(jsonData);
                        
                        break;
                    
                    case"hand":
                        
                        hand.handlm=JsonUtility.FromJson<Hand.HandData>(jsonData);
                        
                        break;
                    default:
                        Debug.Log("invalid result");
                        
                        break;
            
                }
            
            
                // Debug.Log("Received data: " + baseData.result_type.ToString());
            }
            
            
        }
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
        receiveThread.Abort();
    }
    
    [System.Serializable]
    //基本数据，区分收到数据类型
    public class BaseData
    {
        public string result_type;
    }
}