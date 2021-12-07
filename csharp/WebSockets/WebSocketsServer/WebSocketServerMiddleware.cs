using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebSocketsServer
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketServerConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            WriteRequestParam(context);
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket connected");

                var connectionId = _manager.AddSocket(webSocket);
                await SendConnectionIdAsync(webSocket, connectionId);
                await ReceiveMessage(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine($"Received Text message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Received Close message");
                        return;
                    }
                });
            }
            else
            {
                Console.WriteLine("This is not WebSocket request");
                // If it is not web socket, check with next middleware in a request pipeline.
                await _next(context);
            }
        }

        private async Task SendConnectionIdAsync(WebSocket socket, string connectionId)
        {
            var buffer = Encoding.UTF8.GetBytes($"ConnectionId: {connectionId}");
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                handleMessage(result, buffer);
            }
        }

        public void WriteRequestParam(HttpContext context)
        {
            Console.WriteLine($"method: ${context.Request.Method}");
            Console.WriteLine($"protocol: ${context.Request.Protocol}");

            if (context.Request.Headers != null)
                foreach (var requestHeader in context.Request.Headers)
                {
                    Console.WriteLine($"--> {requestHeader.Key}: {requestHeader.Value}");
                }
        }
    }
}
