using System;
using System.Net;
using System.Threading.Tasks;
using SpaceMafia.Networking;

namespace SpaceMafia
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("ABCDEF");
            Console.WriteLine(Lobbies.GameNameToIntV2("ABCDEF"));
            GameListRequest request = new GameListRequest(0x01, 0x03, 256);
            var bytes = request.GenerateOptionRequest();
            SpaceMafia.Networking.Client player = new SpaceMafia.Networking.Client();
            await player.Connect(IPAddress.Parse("45.79.40.75"), "EJBRCF");
        }
    }
}
