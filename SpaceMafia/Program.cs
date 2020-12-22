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
        }
    }
}
