using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace BassClefStudio.UWP.ApplicationModel.AppServices
{
    public class AppServiceInput
    {
        public string Command { get; }
        public ValueSet Parameters { get; }

        public AppServiceInput(string command, ValueSet parameters)
        {
            Command = command;
            Parameters = parameters;
        }

        public static AppServiceInput Parse(ValueSet input)
        {
            if(input.ContainsKey("Command") && input["Command"] is string)
            {
                var command = input["Command"] as string;
                input.Remove("Command");
                return new AppServiceInput(command, input);
            }
            else
            {
                throw new AppServiceException("A command was not included in the input message.");
            }
        }

        public ValueSet ToValueSet()
        {
            var input = new ValueSet();
            foreach (var parameter in Parameters)
            {
                input.Add(parameter);
            }

            input.Add("Command", Command);
            return input;
        }
    }

    public class AppServiceOutput
    {
        public AppServiceStatus Status { get; }
        public object Content { get; }

        public AppServiceOutput(AppServiceStatus status, object content)
        {
            Status = status;
            Content = content;
        }

        public static AppServiceOutput Parse(ValueSet output)
        {
            if (output.ContainsKey("Status") && output["Status"] is string && output.ContainsKey("Content"))
            {
                var status = (AppServiceStatus)Enum.Parse(typeof(AppServiceStatus), output["Status"] as string);
                return new AppServiceOutput(status, output["Content"]);
            }
            else
            {
                throw new AppServiceException("ValueSet does not contain the correct keys.");
            }
        }

        public ValueSet ToValueSet()
        {
            var output = new ValueSet();
            output.Add("Status", Status.ToString());
            output.Add("Content", Content);
            return output;
        }
    }

    public enum AppServiceStatus
    {
        Success = 0,
        Fail = 1
    }
}
