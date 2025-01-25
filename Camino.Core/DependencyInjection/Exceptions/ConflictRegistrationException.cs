using System;

namespace Camino.Core.DependencyInjection.Exceptions
{
    public class ConflictRegistrationException : Exception
    {
        public ConflictRegistrationException(string message) : base(message)
        {
        }
    }
}