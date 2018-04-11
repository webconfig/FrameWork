using System;

public class Time
{
    long frameNumber = 1;
    long interval, run_time;
    DateTime lastTickTime;

    public event CallBack update;
    public void Run()
    {
        interval = 50 * 10000;
        lastTickTime = DateTime.UtcNow;
        while (true)
        {
            try
            {
                while (true)
                {
                    //===============================
                    run_time = (DateTime.UtcNow - lastTickTime).Ticks;
                    while (run_time < interval) System.Threading.Thread.Sleep(5);
                    //deltaTime = run_time / (10000 * 1000.00f);
                    lastTickTime = DateTime.UtcNow;
                    //=============================
                    frameNumber++;
                    if (update != null)
                    {
                        update();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + "\nStack Trace:\n" + e.StackTrace);
            }

        }
    }
}
