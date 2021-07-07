using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ClientConfig", menuName = "Proxy/ClientConfig", order = 1)]
public class ClientConfig: ScriptableObject
{
    public string Host = "127.0.0.1";
    public int Port = 6688;
    public int MaxConn = 1;
}