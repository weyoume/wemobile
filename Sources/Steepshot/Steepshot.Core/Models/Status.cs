﻿namespace Steepshot.Core.Models
{
    public struct Status
    {
        public bool IsChanged { get; }

        public string Sender { get; }

        public Status(string sender, bool isChanged)
        {
            Sender = sender;
            IsChanged = isChanged;
        }
    }
}
