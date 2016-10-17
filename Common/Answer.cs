using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolBot.Common
{
    [Serializable]
    public class Answer
    {
        public Answer(params string[] args)
        {
            Options = new HashSet<string>(args);
        }

        public HashSet<string> Options { get; set; }

        public override bool Equals(object obj)
        {
            var otherAnswer = obj as Answer;
            if (otherAnswer == null)
            {
                return false;
            }

            return Options.IsSupersetOf(otherAnswer.Options);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Options.First();
        }
    }
}