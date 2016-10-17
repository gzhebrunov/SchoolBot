using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SchoolBot.Common;
using SchoolBot.Common.Operation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolBot.Dialog
{
    [Serializable]
    public class StateMachineDialog: IDialog<object>
    {
        private static State InitialState = 
            new State("Please choose one of the options:\n" +
            "1. Subscribe/unsubscribe to notifications\n" +
            "2. List upcoming school events\n" +
            "3. Book a bus for Your kid\n" +
            "4. Send family alert",
            new VoidOperation(), 
            new List<Tuple<Answer, State>> { 
                new Tuple<Answer, State>(new Answer("1", "1."), 
                    new State("Do You want to receive all school notifications using this channel?\n" +
                        "1. Yes (subscribe me)\n" +
                        "2. No (unsubscribe me)", new VoidOperation(), 
                        new List<Tuple<Answer, State>> {
                            new Tuple<Answer, State>(new Answer("1", "yes", "Yes"), new State("", new SubscribeOperation(), new List<Tuple<Answer, State>>())),
                            new Tuple<Answer, State>(new Answer("2", "no", "No"), new State("", new UnsubscribeOperation(), new List<Tuple<Answer, State>>()))
                        })),
                new Tuple<Answer, State>(new Answer("2", "2."), 
                    new State("Loading scheduled events...", new LoadEventListOperation(), new List<Tuple<Answer, State>>())),
                new Tuple<Answer, State>(new Answer("3", "3."), new State("Sorry, this operation would be available soon...", new BookBusOperation(), new List<Tuple<Answer, State>>())),
                new Tuple<Answer, State>(new Answer("4", "4."), 
                    new State("Do you have problem?", new VoidOperation(), 
                        new List<Tuple<Answer, State>> {
                            new Tuple<Answer, State>(new Answer("yes", "Yes"), 
                                new State("Please describe Your problem", new VoidOperation(), 
                                    new List<Tuple<Answer, State>> {
                                        new Tuple<Answer, State>(new Answer(""), new State("", new SendAlertOperation(), new List<Tuple<Answer, State>>()))
                                    })),
                            new Tuple<Answer, State>(new Answer("no", "No"), new State("Thank you!", new VoidOperation(), new List<Tuple<Answer, State>>()))
                        }))
            });

        private State CurrentState = StateMachineDialog.InitialState;
        private const string HelloMessage = "Hello, {0}!";
        private const string WrongMessage = "Sorry I did not understand. Options are: ";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            
            if (message.Text.ToLowerInvariant().Contains("thank"))
            {
                var response = context.MakeMessage();
                response.Type = "message";
                response.TextFormat = "xml";
                response.Text = "<ss type =\"wink\">;)</ss>";

                await context.PostAsync(response);
            }
            else if (!CurrentState.IsCorrectAnswer(message.Text))
            {
                //await context.PostAsync(string.Format(HelloMessage, message.From.Name));
                await context.PostAsync(GetWrongMessage(CurrentState));
                await context.PostAsync(CurrentState.Prompt);
            }
            else
            {
                var nextStep = CurrentState.GetNextState(message.Text);
                await context.PostAsync("You choose: " + message.Text);

                if(!string.IsNullOrEmpty(nextStep.Prompt))
                {
                    await context.PostAsync(nextStep.Prompt);
                }
                
                //await context.PostAsync(new Activity(ActivityTypes.Typing));

                var result = await nextStep.Operation.Execute(context, message.From.Id, message.From.Name, message.Text);
                foreach (var mes in result)
                {
                    await context.PostAsync(mes);
                }

                CurrentState = nextStep.IsTerminateState() ? StateMachineDialog.InitialState : nextStep;
            }

            context.Wait(MessageReceivedAsync);
        }

        private string GetWrongMessage(State state)
        {
            return WrongMessage + string.Join(", ", state.NextStates.Select(a => a.Item1.ToString()));
        }
    }
}