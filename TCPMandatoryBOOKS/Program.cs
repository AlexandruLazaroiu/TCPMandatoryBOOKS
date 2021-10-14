using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ClassLibraryMandatory2;

namespace TCPMandatoryBOOKS
{
    class Program
    {
        public static Dictionary<string, Book> BooksDictionary = new Dictionary<string, Book>()
        {
        };

        static void Main(string[] args)
        {
            BooksDictionary.Add("1234", new Book("1234", "the best book", "the best author", 200));
            BooksDictionary.Add("2000", new Book("2000", "the best book", "the best author", 200));
            BooksDictionary.Add("3000", new Book("3000", "the best book", "the best author", 200));
            BooksDictionary.Add("4000", new Book("4000", "the best book", "the best author", 200));
            TcpListener listener = new TcpListener(IPAddress.Loopback, 4646);
            listener.Start();
            Console.WriteLine($"Server has started {IPAddress.Loopback}:4646");

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Task.Run(() => { Handler(socket); });
            }
            
        }

        public static void Handler(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);
            Console.WriteLine("Client connected");

            while (true)
            {
                string method = reader.ReadLine();
                string argument = reader.ReadLine();
                switch (method)
                {
                    case "GetAll":
                        writer.WriteLine(GetAll());
                        break;
                    case "Get":
                        writer.WriteLine(GetBook(argument));
                        break;
                    case "SaveBook":
                        writer.WriteLine(SaveBook(argument));
                        break;
                    default:
                        writer.WriteLine("Command doesn't exist");
                        break;
                        
                }
                writer.Flush();
            }

        }

        public static string GetAll()
        {
            string jsonstring = JsonSerializer.Serialize(BooksDictionary.Values);
            return jsonstring;
        }

        public static string GetBook(string ISBN)
        {
            if (BooksDictionary.ContainsKey(ISBN))
            {
                string jsonstring = JsonSerializer.Serialize(BooksDictionary[ISBN]);
                return jsonstring;
            }
            else
            {
                return "Book with this ISBN does not exist";
            }
        }

        public static string SaveBook(string jsontext)
        {
            Book book = JsonSerializer.Deserialize<Book>(jsontext);
            if (!BooksDictionary.ContainsKey(book.ISBN))
            {
                BooksDictionary.Add(book.ISBN,book);
                return "Book added";
            }

            return "Book with this ISBN already exists";
        }

    }
}
