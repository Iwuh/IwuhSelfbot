using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IwuhSelfbot.Commands.Entities
{
    public struct EvalResult
    {
        public bool IsSuccessful { get; private set; }
        public string Output { get; private set; }

        public EvalResult(bool success, string output)
        {
            IsSuccessful = success;
            Output = output;
        }
    }
}
