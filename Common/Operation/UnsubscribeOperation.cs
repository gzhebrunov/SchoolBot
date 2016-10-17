using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolBot.Common.Operation
{
    [Serializable]
    public class UnsubscribeOperation : IOperation
    {
        public Task<List<IMessageActivity>> Execute(IDialogContext context, string userId, string userName, string userMessage)
        {
            var message = context.MakeMessage();
            message.Text = "You was sucessfully **unsubscribed** from the all notifications!";

            return Task.FromResult(new List<IMessageActivity> { message });
        }
    }
}