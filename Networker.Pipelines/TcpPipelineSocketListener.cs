using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server;
using Networker.Server.Abstractions;

namespace Networker.Pipelines
{
    public class TcpPipelineSocketListener : ITcpSocketListener
    {
        private readonly ILogger logger;
        private readonly ServerBuilderOptions options;
        private readonly IServerPacketProcessor serverPacketProcessor;
        private Socket listenSocket;
        private readonly ObjectPool<bool[]> objectPool;

        public TcpPipelineSocketListener(ServerBuilderOptions options,
            ILogger logger,
            IServerPacketProcessor serverPacketProcessor)
        {
            this.options = options;
            this.logger = logger;
            this.serverPacketProcessor = serverPacketProcessor;
            this.objectPool = new ObjectPool<bool[]>(options.TcpMaxConnections);

            for(var i = 0; i < options.TcpMaxConnections; i++)
            {
                this.objectPool.Push(new bool[options.PacketSizeBuffer]);
            }
        }

        public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

        public Socket GetSocket()
        {
            return this.listenSocket;
        }

        public void Listen()
        {
            this.listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.Bind(new IPEndPoint(IPAddress.Any, this.options.TcpPort));

            this.listenSocket.Listen(this.options.TcpMaxConnections);
            _ = this.Run();
        }

        private async Task FillPipeAsync(Socket socket, PipeWriter writer)
        {
            int minimumBufferSize = this.options.PacketSizeBuffer;

            while(true)
            {
                try
                {
                    Memory<byte> memory = writer.GetMemory(minimumBufferSize);

                    int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None);
                    if(bytesRead == 0)
                    {
                        break;
                    }

                    writer.Advance(bytesRead);
                }
                catch
                {
                    break;
                }

                FlushResult result = await writer.FlushAsync();

                if(result.IsCompleted)
                {
                    break;
                }
            }

            writer.Complete();
        }

        private async Task ProcessLinesAsync(Socket socket)
        {
            this.logger.Debug($"[{socket.RemoteEndPoint}]: connected");

            var pipe = new Pipe();
            Task writing = this.FillPipeAsync(socket, pipe.Writer);
            Task reading = this.ReadPipeAsync(socket, pipe.Reader);

            await Task.WhenAll(reading, writing);

            this.logger.Debug($"[{socket.RemoteEndPoint}]: disconnected");
        }

        private async Task ReadPipeAsync(Socket socket, PipeReader reader)
        {
            while(true)
            {
                ReadResult result = await reader.ReadAsync();

                ReadOnlySequence<byte> buffer = result.Buffer;
                SequencePosition? position = null;

                //this.serverPacketProcessor.ProcessFromBuffer(result.Buffer.ToArray());

                reader.AdvanceTo(result.Buffer.End);

                if(result.IsCompleted)
                {
                    break;
                }
            }

            reader.Complete();
        }

        private async Task Run()
        {
            while(true)
            {
                var socket = await this.listenSocket.AcceptAsync();
                _ = this.ProcessLinesAsync(socket);
            }
        }
    }
}