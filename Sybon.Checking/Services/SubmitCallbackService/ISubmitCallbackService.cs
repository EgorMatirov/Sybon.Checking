using System;
using Bunsan.Broker;

namespace Sybon.Checking.Services.SubmitCallbackService
{
    public interface ISubmitCallbackService : IDisposable
    {
        SubmitCallbackService Listen(ConnectionParameters connectionParameters);
    }
}