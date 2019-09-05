using NServiceBus;
using NServiceBus.Transport;
using Server.Core.Exceptions;
using System.Linq;


// https://docs.particular.net/nservicebus/recoverability/custom-recoverability-policy
// https://discuss.particular.net/t/implementing-custom-retry-policy-breaks-defaultrecoverabilitypolicy-behavior/512

namespace Server.Core
{
    class ConfigureRecoverabilitySettings : INeedInitialization
    {
        public void Customize(EndpointConfiguration configuration)
        {
            var recoverability = configuration.Recoverability();
            recoverability.CustomPolicy(CreateSubjectCustomRetryPolicy);
        }

        private RecoverabilityAction CreateSubjectCustomRetryPolicy(RecoverabilityConfig config, ErrorContext context)
        {
            // this should be populated somewhere else.
            config.Failed.UnrecoverableExceptionTypes.Add(typeof(PoisonedMessageException));

            return config.Failed.UnrecoverableExceptionTypes.Count(exType => exType.IsInstanceOfType(context.Exception)) > 0
                ? RecoverabilityAction.MoveToError("customErrorQueue")
                : DefaultRecoverabilityPolicy.Invoke(config, context);
        }
    }
}
