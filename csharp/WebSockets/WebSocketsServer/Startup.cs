using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;

namespace WebSocketsServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                WriteRequestParam(context);
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("WebSocket connected");

                    await ReceiveMessage(webSocket, async (result, buffer) =>
                    {

                    });
                }
                else
                {
                    Console.WriteLine("This is not WebSocket request");
                    // If it is not web socket, check with next middleware in a request pipeline.
                    await next();
                }
            });

            app.Run(async context =>
            {
                Console.WriteLine("Run delegate");
                await context.Response.WriteAsync("Hi from run delegate");
                
            });
        }

        public void WriteRequestParam(HttpContext context)
        {
            Console.WriteLine($"method: ${context.Request.Method}");
            Console.WriteLine($"protocol: ${context.Request.Protocol}");
            
            if(context.Request.Headers != null)
                foreach (var requestHeader in context.Request.Headers)
                {
                    Console.WriteLine($"--> {requestHeader.Key}: {requestHeader.Value}");
                }
        }

        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(), CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
