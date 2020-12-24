using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpaceMafia.Client;
using SpaceMafia.Exceptions;
using SpaceMafia.Networking;

namespace SpaceMafia.Player
{
    public static class Player
    {
        public static AmongUsClient Client;
        public static async Task Connect(string ipAddress, string Lobby)
        {
            Client = new AmongUsClient();

            Client.OnConnect += () => WriteMessage(new { type = "connect" });
            Client.OnDisconnect += () =>
            {
                WriteMessage(new { type = "disconnect" });
                Environment.Exit(0);
            };
            Client.OnTalkingEnd += () => WriteMessage(new { type = "talkingEnd" });
            Client.OnTalkingStart += () => WriteMessage(new { type = "talkingStart" });
            Client.OnGameEnd += () => WriteMessage(new { type = "gameEnd" });
            Client.OnPlayerDataUpdate += data => WriteMessage(new { type = "gameData", data });

            try
            {
                await Client.Connect(IPAddress.Parse(ipAddress), Lobby);
            }
            catch (AUException ex)
            {
                WriteMessage(new { type = "error", message = ex.Message });
                return;
            }

            // Trap ctrlc (SIGINT) to disconnect before terminating.
            Console.CancelKeyPress += (sender, ev) =>
            {
                ev.Cancel = true; // cancel direct shutdown, so we can disconnect and then kill ourselves
                Client.DisconnectAndExit();
            };

            // Idle endlessly.
           // while (true)
            //{
              //  await Task.Delay(30000);
            //}
        }

        private static void WriteMessage(object obj)
        {
            Console.Out.WriteLine(JsonConvert.SerializeObject(obj));
            Console.Out.Flush();
        }
    }
}