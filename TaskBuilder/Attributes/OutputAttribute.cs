﻿using System;

namespace TaskBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property,
              AllowMultiple = false,
              Inherited = true)]
    public class OutputAttribute : BaseFunctionAttribute
    {
        public OutputAttribute()
        {
        }
    }
}