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
        }

        async Task<GameBoard> NewGame()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }

            _server = new Server(16, 0);
            return await Task.Run( () => _server.GetBoard());
        }

        async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);
            string response = "";

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
                        switch (buff)
                        {
                            case "NewGame":
                                //response = "Starting a new game!";
                                response = JsonConvert.SerializeObject(await NewGame());
                                break;
                            case "Start":
                                //response = "Starting Server";
                                response = JsonConvert.SerializeObject(_server.GetBoard());
                                break;
                            case "Stop":
                                response = "Stopping Server";
                                _server.Stop();
                                break;
                        }
                    }
                }

                var outgoing = Encoding.ASCII.GetBytes(response);
                await this._socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                
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