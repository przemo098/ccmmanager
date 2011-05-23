namespace CCMManager.Models
{
    using System;

    [Serializable]
    public enum ComputerStates
    {
        Online, Offline, LoggedOn, Unknown, DNSError, AccessDenied, Broken
    }
}
