using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolBot.Common.Operation
{
    [Serializable]
    public class BookBusOperation : IOperation
    {
        private const string BusPicture = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/ICCE_First_Student_Wallkill_School_Bus.jpg/800px-ICCE_First_Student_Wallkill_School_Bus.jpg";

        public Task<List<IMessageActivity>> Execute(IDialogContext context, string userId, string userName, string userMessage)
        {
            var image = context.MakeMessage();
            image.Attachments = new List<Attachment>
                {
                    new Attachment("image/jpeg", BusPicture)
                };

            return Task.FromResult(new List<IMessageActivity> { image });
        }
    }
}