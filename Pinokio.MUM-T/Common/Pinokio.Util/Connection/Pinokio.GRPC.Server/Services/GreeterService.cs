//using Grpc.Core;
//using Pinokio.GRPC.Server;


//namespace Pinokio.GRPC.Server.Services
//{
//    public class GreeterService : Greeter.GreeterBase
//    {
//        public override Task<Reply> GetAGVStatuses(Request request, ServerCallContext context)
//        {
//            var agvStatuses = AGVManager.Instance.GetAGVStatuses(request.AgvId);
//            return Task.FromResult(new Reply
//            {
//                D5 = agvStatuses["D5"],
//                D100 = agvStatuses["D100"],
//                D101 = agvStatuses["D101"],
//                D102 = agvStatuses["D102"],
//                JobId = agvStatuses["JobId"]
//            });
//        }
//    }
//}