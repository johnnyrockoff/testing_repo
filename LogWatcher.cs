namespace LoggerTask;

using System;
using System.IO;

public class LogWatcher
{
    public void log_message(string fileName, string message, string level)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timestamp} [{level}] {message}";
        
        string directory = Directory.GetCurrentDirectory();
        
        try
        {
            Console.WriteLine(directory+"/logs/"+fileName, logEntry + Environment.NewLine);
            File.AppendAllText(directory+"/logs/"+fileName, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"IO ERROR: {ex.Message}");
        }
    }
}