using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentAssertions.Assertions
{
    /// <summary>
    /// Allows for fluent selection of methods of a type through reflection.
    /// </summary>
    public class MethodInfoSelector
    {
        private IEnumerable<MethodInfo> selectedMethods = new List<MethodInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInfoSelector"/> class.
        /// </summary>
        /// <param name="type">The type from which to select methods.</param>
        public MethodInfoSelector(Type type)
        {
            Subject = type;
            selectedMethods = type
                .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => !IsProperty(method));
        }

        /// <summary>
        /// Gets the <see cref="Type"/> from which to select methods.
        /// </summary>
        public Type Subject { get; private set; }

        /// <summary>
        /// Only select the methods that are public or internal.
        /// </summary>
        public MethodInfoSelector ThatArePublicOrInternal
        {
            get
            {
                selectedMethods = selectedMethods.Where(method => method.IsPublic || method.IsAssembly);
                return this;
            }
        }

        /// <summary>
        /// Only select the methods without a return value
        /// </summary>
        public MethodInfoSelector ThatReturnVoid
        {
            get
            {
                selectedMethods = selectedMethods.Where(method => method.ReturnType == typeof (void));
                return this;
            }
        }

        /// <summary>
        /// Only select the methods that return the specified type 
        /// </summary>
        public MethodInfoSelector ThatReturn<TReturn>()
        {
            selectedMethods = selectedMethods.Where(method => method.ReturnType == typeof(TReturn));
            return this;
        }

        /// <summary>
        /// Only select the methods that are decorated with an attribute of the specified type.
        /// </summary>
        public MethodInfoSelector ThatAreDecoratedWith<TAttribute>()
        {
            selectedMethods = selectedMethods.Where(method => method.GetCustomAttributes(false).OfType<TAttribute>().Any());
            return this;
        }

        /// <summary>
        /// The resulting <see cref="MethodInfo"/> objects.
        /// </summary>
        public MethodInfo[] ToArray()
        {
            return selectedMethods.ToArray();
        }

        private bool IsProperty(MethodInfo method)
        {
            return method.Name.StartsWith("get_") || method.Name.StartsWith("set_");
        }
    }
}