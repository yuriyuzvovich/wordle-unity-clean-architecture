using System;

namespace Wordle.Application.Attributes
{
    /// <summary>
    /// Marks a field or property for dependency injection.
    /// Used by the DI container to inject dependencies into MonoBehaviours.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// Optional: Specify if null values are allowed.
        /// Default is false (null values will throw an exception).
        /// </summary>
        public bool Optional { get; set; }

        public InjectAttribute(bool optional = false)
        {
            Optional = optional;
        }
    }
}
