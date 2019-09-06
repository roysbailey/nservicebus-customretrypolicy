using NServiceBus;
using NServiceBus.Transport;
using Server.Core.Exceptions;
using System.Linq;


// https://docs.particular.net/nservicebus/recoverability/custom-recoverability-policy
// https://discuss.particular.net/t/implementing-custom-retry-policy-breaks-defaultrecoverabilitypolicy-behavior/512
// https://docs.particular.net/nservicebus/recoverability/#unrecoverable-exceptions// 

namespace Server.Core
{
    class ConfigureRecoverabilitySettings : INeedInitialization
    {
        public void Customize(EndpointConfiguration configuration)
        {
            var recoverability = configuration.Recoverability();
            recoverability.AddUnrecoverableException<PoisonedMessageException>();

            // The custom policy is only needed if you want finer grained control.
            // By default, all UnrecoverableMessages would be put into the standard error queue - we may not want that!
            recoverability.CustomPolicy(CreateSubjectCustomRetryPolicy);
        }

        private RecoverabilityAction CreateSubjectCustomRetryPolicy(RecoverabilityConfig config, ErrorContext context)
        {
            // In this custom policy, we are electing to move the PoisonedMessages to a different error queue for resolution, so they will use a different error queue to the standard one (so can be monitored easily).
            // You could add whatever logic you liked here to put different messages in different queues.

            return config.Failed.UnrecoverableExceptionTypes.Count(exType => exType.IsInstanceOfType(context.Exception)) > 0
                ? RecoverabilityAction.MoveToError("customErrorQueue")
                : DefaultRecoverabilityPolicy.Invoke(config, context);
        }
    }
}
