using System;

namespace InfiniBot
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HideAttribute : Attribute
    {
    }

}