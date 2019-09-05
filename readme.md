# nServiceBus Custom Retry Policy
When a message fails processing, it will generally be one of two types of failure; a transient failure, or a permenant failure.  Transient failures (such as a database deadlock) are generally temporary in their nature, and often times, simply retrying the operation a short time after will succeed.  Permenant failures are errors which will always occur when processing a given message (such as processing a message with a mandatory item of data missingitself), and as such, there is no point in retrying them.

Out of the box, nServiceBus implements a default retry policy which applies to all errors, and this policy consists of both immediate and delayed retries.  The default confgigration seems to attempt 6 immediate retries, and then 3 delayed retries (with a gap of 30 seconds between each delayed retry).  Once a retry succeeds, further retries are not undertaken.  If all retries are attempted and the message is still not handled, it is moved to the error queue.

It is interesting to note the relationship between immediate and delayed retries.  Basically, nServiceBus will retry the _immediate count_ of times, each time there is a _delayed retry_.  So, if you have 3 delayed retries, and an immediate retry count of 6, then you will actually get 18 retries.  3 batches of 6, with 30 seconds between each batch.

This behaviour is perfect for transient errors, as it allows our code to be gracefully retried and in most cases the message gets through on retry, so there is no need for any other action.  However, for permenant failures (poison messages) this retry policy can be costly.  As a poisoned message which will have no change of success is tried over and over.  This can be a performance issue, and delay work completing.

In this example, we implement a custom retry policy.  When a handler exists with an exception of type _PoisonMessageException_ then the message is immediately moved into a different error queue, and no further (wasteful) retries are attempted.

## The project

**Client.Core** sends an nServiceBus message to a proessor (Server.Core) for processing.  The message has two elements, an Id and a Value.  In this test, the Value is a random number between 1 and 100.

**Server.Core** processes messages received from Client.Core.  When processing, the message.Value is inspected.  This value dictates how the message is handled, by inspecting which of the following 3 ranges the value falls within...

+  0-24  Message is treated as a transient failure.  The message will be retried using the default immediate and delayed retry counts (default seems to be 3 batches of 6 retries - 18 altogether)
+ 25-75  Message is treated as success, and is processed accordingly.
+ 76-100 Message is trated as a permanent failure.  The message is moved to the _customErrorQueue_.

## The messages

This sample uses the nServiceBus learning transport, which basically persists messges to your hard disk.  When you run your project, you will see a new folder called _.learningtransport_ created.  You will see a sub-folder for each queue inside.