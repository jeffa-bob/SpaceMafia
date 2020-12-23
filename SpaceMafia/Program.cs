using System;
using System.Net;
using SpaceMafia.Networking;

namespace SpaceMafia
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ABCDEF");
            Console.WriteLine(Lobbies.GameNameToIntV2("ABCDEF"));
            GameListRequest request = new GameListRequest(0x01, 0x03, 256);
            var bytes = request.GenerateOptionRequest();
            Console.WriteLine(BitConverter.ToString(bytes).Replace('-',' '));
            bytes = request.SendOptionRequest(IPAddress.Parse("45.79.40.75"));
            Console.WriteLine(BitConverter.ToString(bytes).Replace('-',' '));
        }
    }
}
