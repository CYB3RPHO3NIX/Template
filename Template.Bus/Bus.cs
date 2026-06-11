using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Template.CommandHandlers;
using Template.Commands;
using Template.EventHandlers;
using Template.Events;
using Template.Queries;
using Template.QueryHandlers;

namespace Template.Bus
{
    public class Bus : IBus
    {
        private readonly IServiceProvider _serviceProvider;
        public Bus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<TResult> Send<TResult>(ICommand<TResult> command)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            return await handler.Handle((dynamic)command);
        }

        public async Task<TResult> Send<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            return await handler.Handle((dynamic)query);
        }

        public async Task Publish(IEvent @event)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());

            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (dynamic? handler in handlers)
            {
                if (handler != null)
                {
                    await handler.Handle((dynamic)@event);
                }
            }
        }
    }
}
