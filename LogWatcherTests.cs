using Microsoft.Playwright;
using System.Text.Json;

namespace LoggerUtils.Tests
{
    /***
    * To test the function, I assumed it would be executed as a REST API, so I created a simple web server to run the function as an API.
    * That's why the folder's structure depends on the project Structure
    * NEED UPDATE
    */
    
    [TestClass]
    public class LogWatcherTests
    {
        private const string ApiUrl = "http://localhost:5000/log/";

        [TestMethod]
        public async Task Should_LogMessage_Successfully()
        {
            using var playwright = await Playwright.CreateAsync();
            var requestContext = await playwright.APIRequest.NewContextAsync();
            
            var logData = new
            {
                FileName = "application.log",
                Message = "Testing log_message with Playwright in C# using MSTest",
                Level = "INFO"
            };
            
            var jsonData = JsonSerializer.Serialize(logData);

            var response = await requestContext.PostAsync(ApiUrl, new APIRequestContextOptions()
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                Data = jsonData
            });
            
            Console.WriteLine(response);
            
            Assert.AreEqual(200, response.Status, "The API did not return the expected status.");
            
            var responseBody = JsonSerializer.Deserialize<LogResponse>(await response.TextAsync());
            Assert.IsNotNull(responseBody, "The API response could not be deserialized.");
            Assert.AreEqual("Success", responseBody.Status, "The response status was not as expected.");
            
            string currentDirectory = Directory.GetCurrentDirectory();
            string parentDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            string prev = Directory.GetParent(parentDirectory).Parent.FullName;

            var logFilePath = Path.Combine(prev+"/LoggerUtils/logs", logData.FileName);
            
            Assert.IsTrue(File.Exists(logFilePath), "The log file was not created.");

            var logContent = await File.ReadAllTextAsync(logFilePath);
            StringAssert.Contains(logContent, logData.Message, "The log message was not found in the file.");
        }

        private class LogResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
        }
    }
}
