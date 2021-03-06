using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServiceCosmos
{
    public class GreeterService : Greeter.GreeterBase
    {
        private static string OneKbDocument = "";
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
            while(Encoding.UTF8.GetByteCount(OneKbDocument) < 1000)
            {
                OneKbDocument += " abcdefhijklmnopqrstuvxyz0123456789";
            }
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name + OneKbDocument
            });
        }
    }
}
