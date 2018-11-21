using System;

namespace InfiniBot
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ExampleAttribute : Attribute
    {
        private string example;
        public ExampleAttribute(string example)
        {
            this.example = example;
        }

        public virtual string Example
        {
            get { return example; }
        }
    }
}