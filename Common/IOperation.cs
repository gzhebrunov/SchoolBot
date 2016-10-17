using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolBot.Common
{
    public interface IOperation
    {
        Task<List<IMessageActivity>> Execute(IDialogContext context, string userId, string userName, string userMessage);
    }
}