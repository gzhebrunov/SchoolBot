using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SchoolBot.Common.Operation
{
    [Serializable]
    public class LoadEventListOperation : IOperation
    {
        private const string Url = "/api/v1/calendar_events?type=event&start_date={startDate}&end_date={endDate}";
        private const string NoEventsMessage = "There are no events scheduled";
        private const string EventsResultPrefixTemplate = "{eventsCount} upcoming event(s) in the calendar";
        private const string SingleEventTemplate = "Event {num}\n\n**{title} ({date})**, location: {location}";

        public async Task<List<IMessageActivity>> Execute(IDialogContext context, string userId, string userName, string userMessage)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                        ConfigurationManager.AppSettings["CanvasAuthToken"]);

                    var response = await http.GetAsync(GetUrlForUser(userId, userMessage));
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsAsync<List<object>>();

                        IMessageActivity message;
                        if (content.Count == 0)
                        {
                            message = context.MakeMessage();
                            message.Text = NoEventsMessage;
                            return new List<IMessageActivity> { message };
                        }

                        var eventsResponse = new List<IMessageActivity>();
                        message = context.MakeMessage();
                        message.Text = GetEventsMessagePrefix(content.Count);

                        var counter = 1;
                        foreach (JObject ev in content)
                        {
                            message = context.MakeMessage();
                            message.Text = GetSingleEventMessage(ev, counter++);

                            eventsResponse.Add(message);
                        }
                        

                        return eventsResponse;
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

            var canNotAcceswsMesssage = context.MakeMessage();
            canNotAcceswsMesssage.Text = "Can not access Canvas, please try again later!";
            return new List<IMessageActivity> { canNotAcceswsMesssage };
        }

        private string GetUrlForUser(string userId, string userMessage)
        {
            return ConfigurationManager.AppSettings["CanvasHost"] +
                Url.Replace("{userId}", userId)
                .Replace("{startDate}", DateTime.Now.ToString("yyyy-MM-dd"))
                .Replace("{endDate}", DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
        }

        private string GetEventsMessagePrefix(int numberOfEvents)
        {
            return EventsResultPrefixTemplate.Replace("{eventsCount}", numberOfEvents.ToString("N0"));
        }

        private string GetSingleEventMessage(JObject ev, int position)
        {
            // "Event {num}:\n{Title}\nDate: {date}\nLocation: {location}";
            var res = SingleEventTemplate
                .Replace("{num}", position.ToString("N0"))
                .Replace("{title}", ev.Value<string>("title"))
                .Replace("{location}", ev.Value<string>("location_name"));

            var startDate = ev.Value<DateTime>("start_at").AddHours(-4); //.ToLocalTime();
            var endDate = ev.Value<DateTime>("end_at").AddHours(-4); //.ToLocalTime();

            if (ev.Value<bool>("all_day"))
            {
                res = res.Replace("{date}", startDate.ToShortDateString());
            }
            else
            {
                res = res.Replace("{date}", endDate.ToShortDateString() +
                    " " +
                    startDate.ToString("hh:mm tt") +
                    " - " +
                    endDate.ToString("hh:mm tt"));
            }

            return res;
        }
    }
}