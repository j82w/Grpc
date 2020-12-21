using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Compression;
using GrpcServiceCosmos;

namespace GrpcClient
{
    class Program
    {
        public static readonly Stopwatch stopwatch = Stopwatch.StartNew();
        public static int OperationCount = 0;
        public static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            Greeter.GreeterClient client = new Greeter.GreeterClient(channel);
           
            for(int i = 0; i < 2; i++)
            {
                await ExecuteParallel(client);
                await Task.Delay(1000);
            }

            Console.WriteLine(OperationCount);
        }

        public static async Task Execute(Greeter.GreeterClient greeterClient)
        {
            Task[] responses = new Task[50];
            for (int i = 0; i < responses.Length; i++)
            {
                responses[i] = ExecuteOperation(greeterClient, i);
            }

            await Task.WhenAll(responses);
        }

        public static async Task ExecuteDelay(Greeter.GreeterClient greeterClient)
        {
            Task[] responses = new Task[100];
            for (int i = 0; i < responses.Length; i++)
            {
                responses[i] = ExecuteOperation(greeterClient, i);
                await Task.Delay(1);
            }

            await Task.WhenAll(responses);
        }

        public static async Task ExecuteParallel(Greeter.GreeterClient greeterClient)
        {
            Task[] tasks = new Task[4];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = ExecuteSync(greeterClient);
            }

            await Task.WhenAll(tasks);
        }

        public static async Task ExecuteSync(Greeter.GreeterClient greeterClient)
        {
            for (int i = 0; i < 500; i++)
            {
                await ExecuteOperation(greeterClient, i);
            }
        }

        public static async Task ExecuteOperation(Greeter.GreeterClient greeterClient, int i)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var r = await greeterClient.SayHelloAsync(new HelloRequest()
            {
                Name = "Test" + i
            }).ResponseAsync;
            stopwatch.Stop();

            OperationCount++;
            Console.WriteLine($"Ms: {stopwatch.ElapsedMilliseconds} Elapsed: {stopwatch.Elapsed}");
        }
    }
}
