namespace SimpleFEM.Domain.Exceptions
{
    public sealed class InvalidStructureException : DomainException
    {
        /// <param name="reason">Specific rule that was violated.</param>
        public InvalidStructureException(string reason)
            : base($"Invalid structure: {reason}") { }
    }
}
