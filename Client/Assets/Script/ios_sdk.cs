using System.Runtime.InteropServices;


public class ios_sdk
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void getPlayer1();

    [DllImport("__Internal")]
    private static extern void getPlayer2();

    [DllImport("__Internal")]
    private static extern void getPlayer1Pic();

    [DllImport("__Internal")]
    private static extern void getPlayer2Pic();

    [DllImport("__Internal")]
    private static extern void getRoomId();

    [DllImport("__Internal")]
    private static extern void getToken();

    [DllImport("__Internal")]
    private static extern void getServer();

    [DllImport("__Internal")]
    private static extern void getGameId();

    [DllImport("__Internal")]
    private static extern void end(string result);

     [DllImport("__Internal")]
    private static extern void commonFunction(string data);
#endif
    public void GetPlayer1()
    {
#if UNITY_IOS
        getPlayer1();
#endif
    }

    public void GetPlayer2()
    {
#if UNITY_IOS
         getPlayer2();
#endif
    }

    public void GetRoomId()
    {
#if UNITY_IOS
         getRoomId();
#endif
    }

    public void GetToken()
    {
#if UNITY_IOS
         getToken();
#endif

    }

    public void GetServer()
    {
#if UNITY_IOS
        getServer();
#endif
    }


    public void GetPlayer1Img()
    {
#if UNITY_IOS
         getPlayer1Pic();
#endif
    }
    public void GetPlayer2Img()
    {
#if UNITY_IOS
         getPlayer2Pic();
#endif
    }

    public void GetGameId()
    {
#if UNITY_IOS
         getGameId();
#endif
    }

    public void End(string str)
    {
#if UNITY_IOS
        end(str);
#endif
    }

    public void CommonFunction(string str)
    {
#if UNITY_IOS
        commonFunction(str);
#endif
    }
}

