using System.Net;


HttpListener? listener = new HttpListener();

listener.Prefixes.Add("http://localhost:12345/");

listener.Start();
Console.WriteLine("Listening");

string directoryPath = System.IO.Directory.GetCurrentDirectory();
directoryPath = Path.Combine(directoryPath,"Pages");
Console.WriteLine("Current Directory: " + directoryPath);

while (true)
{
    HttpListenerContext? context = listener.GetContext();

    HttpListenerRequest? request = context.Request;
    HttpListenerResponse? response = context.Response;
    string url = request?.RawUrl!;
    var splitUrl = url.Split('/').ToList().Where(x => !string.IsNullOrWhiteSpace(x));

    response.ContentType = "text/html";
    response.StatusCode = 200;
    using var writer = new StreamWriter(response.OutputStream);

    string filename = string.Join("/", splitUrl);
    if(filename == "")
    {
        var text = File.ReadAllText("Pages/index.html");
        response.StatusCode = 200;
        writer.Write(text);
        continue;
    }

    if (!filename.EndsWith("html"))
        filename+=".html";

    string filePath = Path.Combine(directoryPath,filename);

    bool fileExists = File.Exists(filePath);

    if (fileExists)
    {
        var text = File.ReadAllText(filePath);
        writer.WriteLine(text);
    }
    else
    {
        var text = File.ReadAllText("Pages/404.html");
        response.StatusCode = 404;
        writer.Write(text);
    }
}