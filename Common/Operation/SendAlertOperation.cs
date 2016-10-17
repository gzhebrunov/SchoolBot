using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SchoolBot.Common.Operation
{
    [Serializable]
    public class SendAlertOperation : IOperation
    {
        private const string Url = "/api/v1/courses/2/assignments";

        public async Task<List<IMessageActivity>> Execute(IDialogContext context, string userId, string userName, string userMessage)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                        ConfigurationManager.AppSettings["CanvasAuthToken"]);
                    
                    var response = await http.PostAsync(GetUrlForUser(userId, userMessage), 
                        new FormUrlEncodedContent(new List<KeyValuePair<string, string>> {
                            new KeyValuePair<string, string>("assignment[description]", userMessage),
                            //"{ alerts : [ {id : 5, description: \"" + userMessage  + "\" } ] }"),
                            new KeyValuePair<string, string>("assignment[name]", "Alert from " + userName)
                        }));

                    if (response.IsSuccessStatusCode)
                    {
                        IMessageActivity message;
                        message = context.MakeMessage();
                        message.Text = "Thank you!";
                        return new List<IMessageActivity> { message };
                    }
                    else // TODO: Log error and try again
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine(content);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = context.MakeMessage();
                message.Text = ex.Message;

                return new List<IMessageActivity> { message };
            }

            return new List<IMessageActivity>();
        }

        private string GetUrlForUser(string userId, string userMessage)
        {
            return ConfigurationManager.AppSettings["CanvasHost"] +
                Url.Replace("{userId}", userId);
        }
    }
}