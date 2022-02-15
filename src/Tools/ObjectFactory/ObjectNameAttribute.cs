using System;

namespace Seekatar.Tools
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ObjectNameAttribute : Attribute
    {
        public string Name { get; set; } = "";
    }

}