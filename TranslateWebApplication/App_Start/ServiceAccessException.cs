namespace TranslateWebApplication
{
    using System;

    using TranslateWebApplication.GoblinServiceReference;

    public class ServiceAccessException : Exception
    {
        public GeneralErrorCode ErrorCode { get; private set; }

        public ServiceAccessException(GeneralErrorCode errorCode, string description) : base(description)
        {
            ErrorCode = errorCode;
        }

        public override string ToString()
        {
            return string.Format("Status: {0}.\n{1}", ErrorCode, Message);
        }
    }
}