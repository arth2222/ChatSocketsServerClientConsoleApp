using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread thread = new Thread(new ThreadStart(StartServer));
            thread.Start();
            Thread.Sleep(2000);
            Thread threadClient = new Thread(new ThreadStart(StartClient));
            threadClient.Start();

        }

        //endpoint må lytte før en connection opprettes. noe som klient ikke kan vite
        static void StartClient()
        {
            // Create a TCP/IP socket
            Console.WriteLine("Tast ip å koble til. ");
            string ip = Console.ReadLine();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), 8888);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the server
            Console.WriteLine("Connecting to the server...");
            try
            {
                clientSocket.Connect(endPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problemer med tilkobling. Venter litt. " + ex.Message);
                Thread.Sleep(1000);
                StartClient();
            }

            // Send a message to the server
            string message;

            do
            {
                Console.WriteLine("Enter a message to send to the server (or 'q' to quit):");
                message = Console.ReadLine();

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(messageBytes);

                // Receive a response from the server - denne her skapte krøll
                //byte[] buffer = new byte[1024];
                //int bytesReceived = clientSocket.Receive(buffer);
                //string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                //Console.WriteLine("Received response: " + response);

            } while (message != "q");

            // Close the socket
            clientSocket.Close();
        }
        static void StartClient(string ip)
        {
            // Create a TCP/IP socket
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), 8888);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the server
            Console.WriteLine("Connecting to the server...");
            try
            {
                clientSocket.Connect(endPoint);
            }
            catch (Exception ex)
            {
                StartClient(ip);
            }

            // Send a message to the server
            string message;

            do
            {
                Console.WriteLine("Enter a message to send to the server (or 'q' to quit):");
                message = Console.ReadLine();

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(messageBytes);

                // Receive a response from the server
                byte[] buffer = new byte[1024];
                int bytesReceived = clientSocket.Receive(buffer);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("Received response: " + response);

            } while (message != "q");

            // Close the socket
            clientSocket.Close();
        }


        static void StartServer()
        {
            // Create a TCP/IP socket
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 8888);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the endPoint
            serverSocket.Bind(endPoint);

            // Listen for incoming connections
            Console.WriteLine("Server started. Listening for incoming connections...");
            serverSocket.Listen(10);

            // Accept an incoming connection
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("Accepted incoming connection from " + clientSocket.RemoteEndPoint.ToString());

            // Receive data from the client
            byte[] buffer = new byte[1024];
            int bytesReceived;
            string message;

            do
            {
                bytesReceived = clientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine("Received message: " + message);

                // Send a response back to the client
                byte[] response = Encoding.UTF8.GetBytes("");
                clientSocket.Send(response);
            } while (message != "q");

            // Close the socket
            serverSocket.Close();
        }

    }
}
