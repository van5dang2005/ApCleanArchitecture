using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    public class NotFoundException(string message) : Exception(message);
    public class UnauthorizedException(string message) : Exception(message);
    public class ConflictException(string message) : Exception(message);
    public class ValidationException(string message) : Exception(message);
}
