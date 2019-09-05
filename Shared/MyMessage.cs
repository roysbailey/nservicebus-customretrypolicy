using System;
using NServiceBus;

public class MyMessage :
    IMessage
{
    public Guid Id { get; set; }
    public int Value { get; set; }
}