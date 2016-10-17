using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolBot.Common
{
    [Serializable]
    public class State
    {
        public State(string prompt, IOperation operation, List<Tuple<Answer, State>> nextStates)
        {
            Prompt = prompt;
            NextStates = nextStates;
            Operation = operation;
        }

        public string Prompt { get; set; }
        public List<Tuple<Answer, State>> NextStates { get; set; }
        public IOperation Operation { get; set; }

        public bool IsCorrectAnswer(string answer)
        {
            if (NextStates.Count == 1 && NextStates[0].Item1.Options.Count == 1 && NextStates[0].Item1.Options.Contains(""))
            {
                return true;
            }
            return NextStates.Any(a => a.Item1.Equals(new Answer(answer)));
        }

        public State GetNextState(string answer)
        {
            if (!IsCorrectAnswer(answer))
            {
                return this;
            }

            if (NextStates.Count == 1 && NextStates[0].Item1.Options.Count == 1 && NextStates[0].Item1.Options.Contains(""))
            {
                return NextStates[0].Item2;
            }

            return NextStates.First(a => a.Item1.Equals(new Answer(answer))).Item2;
        }

        public bool IsTerminateState()
        {
            return NextStates.Count == 0;
        }
    }
}