// TODO: Code cleanup.
// TODO: Document code

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

namespace MagicPacket {
    class Program {

        // Program Error Codes
        const int ARGUMENT_ERROR = 1;
        const int RUNTIME_ERROR = 2;
        const int NO_ARGUMENT = 3;

        static void Main(string[] args) {

            //Byte Encoder
            System.Text.Encoding enc = System.Text.Encoding.ASCII;

            string macAddress_string;
            byte[] macAddress = new byte[] { };

            // Error Testing
            if (args.Length == 0) {
                error(NO_ARGUMENT);
            }

            if (args[0].Length != 12) {
                error(ARGUMENT_ERROR);
            }

            if (CheckIfHex(args[0]) == false) {
                error(ARGUMENT_ERROR);
            }

            macAddress_string = args[0];
            macAddress = enc.GetBytes(macAddress_string);

            Console.WriteLine("Sending Wake Packet to {0}", args[0]);

            try {
                wake(macAddress);
            } catch {
                error(RUNTIME_ERROR);
            }
            
            Console.WriteLine("Packet Sent");
       } 

        /// <summary>
        /// Method to wake the specified mac address
        /// </summary>
        /// <param name="mac">MAC address of target computer</param>
        private static void wake(byte[] mac) {

            const int WOL_PORT = 9;

            // UDPClient Instance
            UdpClient client = new UdpClient();

            // Byte array for magic packet
            List<byte> magicPacket = new List<byte>();

            // 6 FF trailer
            for (int i = 0; i < 6; i++) {
                magicPacket.Add(0xFF);
            }

            // Add MAC address 16 times
            for (int i = 0; i < 16; i++) {
                magicPacket.AddRange(mac);
            }

            //Send packet to broadcast address
            client.Connect(IPAddress.Broadcast, WOL_PORT);
            client.Send(magicPacket.ToArray(), magicPacket.Count);

        }

        /// <summary>
        /// Checks if input string contains valid hexadecimal digits.
        /// </summary>
        /// <param name="text">String to be checked</param>
        /// <returns>True if string only contains hexadecimal digits</returns>
        private static bool CheckIfHex(string text) {

            for (int i = 0; i < text.Length; i++) {
                bool hex_character = (text[i] >= '0' && text[i] <= '9') ||
                                     (text[i] >= 'a' && text[i] <= 'f') ||
                                     (text[i] >= 'A' && text[i] <= 'F');

                if (!hex_character) {
                    return false;
                }

            }
            return true;
        }

        /// <summary>
        /// Method is called when the application encounters an exception
        /// </summary>
        /// <param name="exitCode">Exit code to indicate which error message to display</param>
        private static void error(int exitCode) {
            switch (exitCode) {
                default:
                    Console.WriteLine("Generic Error.  Exiting");
                    Environment.Exit(3);
                    break;
                case ARGUMENT_ERROR:
                    Console.WriteLine("Incorrect Parameter.  Exiting.");
                    Environment.Exit(2);
                    break;
                case RUNTIME_ERROR:
                    Console.WriteLine("Runtime Error.  Exiting");
                    Environment.Exit(4);
                    break;
                case NO_ARGUMENT:
                    Console.WriteLine("Usage: wake [mac address]");
                    Environment.Exit(1);
                    break;
            }
        }
    }
}
