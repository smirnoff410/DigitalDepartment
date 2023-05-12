// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;

var ipAddress = "192.168.1.106";
var ipPort = 1234;
var endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), ipPort);

var client = new UdpClient(endPoint);

var result = new List<byte>();
var stopTime = DateTime.UtcNow.AddSeconds(10);
while (DateTime.UtcNow < stopTime)
{
    var bytes = client.Receive(ref endPoint);
    result.AddRange(bytes);
}
File.WriteAllBytes("D:\\Projects\\DigitalDepartment\\Audio\\storage\\audio.mp3", result.ToArray());
