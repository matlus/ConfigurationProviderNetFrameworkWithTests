using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationProvider.Tests
{
    public static class AssertExtentions
    {
        public static void ExceptionMessageContains(this Assert assertInstance, Exception exception, IEnumerable<string> messageParts)
        {
            var exceptionMessage = new StringBuilder();
            exceptionMessage.AppendLine($"An Exception of Type: {exception.GetType().Name}, was thrown, however the exception message was expected to contain the following Phrases: {string.Join(", ", messageParts)}. However, the exception message did NOT contain the following:\r\n");

            var partNotContained = false;

            foreach (var part in messageParts)
            {
                if (!exception.Message.Contains(part))
                {
                    exceptionMessage.Append($"{part}, ");
                    partNotContained = true;
                }
            }

            if (partNotContained)
            {
                exceptionMessage.AppendLine("\r\nThe actual exception message is: " + exception.Message);
                throw new AssertFailedException(exceptionMessage.ToString());
            }
        }
        public static void ThrowsExceptionWithSpecificWords<T>(this Assert assertInstance, Func<object> funcDelegate, IEnumerable<string> messageParts) where T : Exception
        {
            // Act
            try
            {
                var _ = funcDelegate();
                Assert.Fail("We were expecting a ConfigurationErrorsException to be thrown but no exception was thrown");
            }
            catch (T e)
            {
                // Assert                
                Assert.That.ExceptionMessageContains(e, messageParts);
            }
        }
    }
}
