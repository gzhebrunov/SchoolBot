using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolBot.Common.Operation
{
    [Serializable]
    public class VoidOperation : IOperation
    {
        public Task<List<IMessageActivity>> Execute(IDialogContext context, string userId, string userName, string userMessage)
        {
            return Task.FromResult(new List<IMessageActivity>());
        }
    }
}