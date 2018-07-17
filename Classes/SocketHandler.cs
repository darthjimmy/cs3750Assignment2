using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Conway
{
    public class SocketHandler
    {
        public const int BufferSize = 4096;
        //private Server _servers;
        private Server _server;

        WebSocket _socket;

        SocketHandler(WebSocket socket)
        {
            _socket = socket;
            _server = Server.GetInstance();
            _server.EndOfTick += server_EndOfTick;
            _server.Update += server_Update;
        }

        void NewGame()
        {
            _server.Stop();
            _server.NewGame();
        }

        void ResumeGame()
        {
            _server.Start();
        }

        private async void server_EndOfTick(object sender, EventArgs e)
        {
            var response = JsonConvert.SerializeObject(_server.GetBoard());
            var outgoing = Encoding.ASCII.GetBytes(response);
            if (_socket.State == WebSocketState.Open)
                await this._socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async void server_Update(object sender, EventArgs e)
        {
            var response = JsonConvert.SerializeObject(_server.GetBoard());
            var outgoing = Encoding.ASCII.GetBytes(response);
            if (_socket.State == WebSocketState.Open)
                await this._socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);
            string response = "";

            try
            {
                while (_socket.State == WebSocketState.Open)
                {
                    using (var ms = new MemoryStream())
                    {
                        WebSocketReceiveResult incoming = null;

                        do
                        {
                            incoming = await this._socket.ReceiveAsync(seg, CancellationToken.None);
                            ms.Write(seg.Array, seg.Offset, incoming.Count);
                        } while (!incoming.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        if (incoming.MessageType == WebSocketMessageType.Text)
                        {
                            using (var reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                var buff = reader.ReadToEnd();

                                ClientMessage message = JsonConvert.DeserializeObject<ClientMessage>(buff);
                                
                                switch (message.Command)
                                {
                                    case "reset":
                                    case "newgame":
                                        //response = "Starting a new game!";
                                        NewGame();

                                        //response = "New game created";
                                        break;

                                    case "start":
                                        //response = "Starting Server";
                                        if (_server == null)
                                            NewGame();
                                        else
                                            _server.Start();

                                        //response = "Server has been started";
                                        break;

                                    case "update":

                                        if (message.X >= _server.GetBoard().Size)
                                            continue;
                                        
                                        if (message.Y >= _server.GetBoard().Size)
                                            continue;

                                        Cell cell = _server.GetBoard().Cells[message.X, message.Y];
                                        cell.Color = message.Color;
                                        cell.Alive = !cell.Alive;

                                        response = JsonConvert.SerializeObject(_server.GetBoard());
                                        _server.FireUpdate();
                                        break;

                                    case "stop":
                                        //response = "Stopping Server";
                                        _server.Stop();
                                        break;
                                }
                            }
                        }
                        else
                        {
                            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close received",CancellationToken.None );
                        }

                        var outgoing = Encoding.ASCII.GetBytes(response);
                        if (_socket.State == WebSocketState.Open && response.Length > 0)
                            await this._socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                        
                    }
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new SocketHandler(socket);
            await h.EchoLoop();
        }

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(SocketHandler.Acceptor);
        }
    }
}