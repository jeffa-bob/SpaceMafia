using System;
using System.Net;
using System.Threading.Tasks;
using SpaceMafia.Player;

namespace SpaceMafia
{
    class Program
    {
        static async Task Main(string[] args)
        {


           // SpaceMafia.Networking.AmongUsClient player = new SpaceMafia.Networking.AmongUsClient();
            //await player.Connect(IPAddress.Parse("45.79.40.75"), "BIMMVF");
            await Player.Player.Connect("104.237.135.186", "YLJCGF");
            while (true)
            {
               System.Threading.Thread.Sleep(100);
               Player.Player.Client.PingPacket(null);
                //Player.Player.Client.SendMovement(0,0,5,5);
                //Player.Player.Client.SendChat("TREE");
            }
        }
    }
}
