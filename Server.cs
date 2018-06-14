using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Conway;

class Server {
    public static void Main() 
    {
        TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);

        server.Start();

        Console.WriteLine($"Server has started. {Environment.NewLine} Waiting for connection...");

        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("A client connected.");

        NetworkStream stream = client.GetStream();

        while (true)
        {
            while (!stream.DataAvailable);

            if (client.Available < 3)
                continue;

            Byte[] bytes = new Byte[client.Available];

            stream.Read(bytes, 0, bytes.Length);

            String data = Encoding.UTF8.GetString(bytes);

            if (Regex.IsMatch(data, "^GET"))
            {
                const string eol = "\r\n";

                Byte[] response = Encoding.UTF8.GetBytes($"HTTP/1.1 101 Switching Protocols{eol}"
                    + $"Connection: Upgrade {eol}"
                    + $"Upgrade: websocket {eol}"
                    + $"Sec-WebSocket-Accept: " + Convert.ToBase64String(
                        System.Security.Cryptography.SHA1.Create().ComputeHash(
                            Encoding.UTF8.GetBytes(
                                new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim()
                            )
                        )
                    ) + eol
                + eol);
                stream.Write(response, 0, response.Length);
            }
            else
            {

            }
        }
    }

    public static void Run()
    {
        GameBoard lastBoard = Utils.FillBoard(16, 25);
        GameBoard curBoard;

        while (true) // only break when killed...
        {
            // start of our tick
            curBoard = new GameBoard(lastBoard);

            // check each of our cells
            for (int i = 0; i < curBoard.Size; i++)
            {
                for (int j = 0; j < curBoard.Size; j++)
                {
                    // Meat and potatoes of our game logic right here...
                    int neighbors = 0;
                    // RULES!

                    if (lastBoard.Cells[i,j].Alive)
                    {
                        neighbors = lastBoard.GetLiveNeighbors(i, j);
                        // Any live cell with fewer than two live neighbors dies
                        if (neighbors < 2)
                        {
                            curBoard.Cells[i,j].Alive = false;
                        }
                        else if (neighbors == 2 || neighbors == 3)
                        {
                            // Any live cell with two or three live neighbors lives
                            curBoard.Cells[i,j].Alive = true;
                        }
                        else if (neighbors > 3)
                        {
                            // Any live cell with more than three live neighbors dies
                            curBoard.Cells[i,j].Alive = false;
                        }
                    }
                    else
                    {
                        neighbors = lastBoard.GetLiveNeighbors(i, j);

                        // Any dead cell with exactly three live neighbors becomes a live cell
                        if (neighbors == 3)
                        {
                            curBoard.Cells[i,j].Alive = true;
                        }
                    }
                }
            }

            // send the results of our tick to the client (curBoard)

            lastBoard = new GameBoard(curBoard);
            // end of our tick
        }
    }
}