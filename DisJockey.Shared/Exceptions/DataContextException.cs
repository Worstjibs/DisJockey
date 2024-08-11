using System.Data.Common;

namespace DisJockey.Shared.Exceptions;

public class DataContextException : DbException
{
    public DataContextException(string message) : base(message)
    {
    }
}