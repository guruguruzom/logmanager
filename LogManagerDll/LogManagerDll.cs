using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

public static class LogManagerDll
{
    static string path;
    static StreamWriter output = null;
    static Queue<Action> msgQueue = new Queue<Action>();
    
    public static void StartLogManager(string exerfitPath)
    {
        string folderPath = LogFolderCreate(exerfitPath);
        string monthPath = MonthFolderCreate(folderPath);
        path = monthPath + "\\Log";
        //30일전 로그파일이 있는지 체크
        for (int i = -2; i > -10; i--)
        {
            if (CheckLogFolder(folderPath + "\\LogMonth_" + DateTime.Now.AddMonths(i).ToString("MM")))
            {
                DeleteLogFolder(folderPath + "\\LogMonth_" + DateTime.Now.AddMonths(i).ToString("MM"));
            }
        }

        path += DateTime.Now.ToString("yyyyMMdd") + ".txt";
        FileInfo file = new FileInfo(path);
        //금일 로그파일이 있는지 체크
        if (!file.Exists)
        {
            FileStream logfile = file.Create();
            logfile.Close();
        }

    }

    public static string LogFolderCreate(string exerfitPath)
    {
        string path = exerfitPath + "\\BlueCloudLog";
        DirectoryInfo di = new DirectoryInfo(path);
        if (!di.Exists)
            di.Create();
        return path;
    }

    public static string MonthFolderCreate(string folderpath)
    {
        string path = folderpath + "\\LogMonth_" + DateTime.Now.ToString("MM");
        DirectoryInfo di = new DirectoryInfo(path);
        if (!di.Exists)
            di.Create();
        return path;
    }

    static void DeleteLogFolder(string path)
    {
        try
        {
            DirectoryInfo diInfo = new DirectoryInfo(path);
            diInfo.Delete(true);
        }
        catch
        {
        }
    }

    static bool CheckLogFolder(string path)
    {
        DirectoryInfo fileInfo = new DirectoryInfo(path);
        if (fileInfo.Exists)
        {
            return true;
        }
        return false;
    }

    static bool CheckLogFile(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists)
        {
            return true;
        }
        return false;
    }

    static Action action;
    static private void TextThread()
    {
        try
        {
            using (output = new StreamWriter(path, true))
            {
                while (msgQueue.Count != 0)
                {
                    action = msgQueue.Dequeue();
                    action?.Invoke();
                }
            }
        }
        finally
        {
          //  output.Dispose();
        }
        //textLogThread.Abort();
        textLogThread = null;
    }

    static Thread textLogThread;
    static public void WriteLog(string logMsg)
    {
        msgQueue.Enqueue(() =>
        {
            output.WriteLine(DateTime.Now.ToString("HH시 mm분 ss초 : ") + logMsg);
        });
        if (textLogThread == null)
        {
            try
            {
                textLogThread = new Thread(new ThreadStart(TextThread));
                textLogThread.Start();
            }
            catch
            {

            }
        }
    }
}
