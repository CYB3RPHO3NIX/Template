using ArchUnitNET.Core;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Template.Architecture.Tests
{
    /// <summary>
    /// Enforces CQRS and layered architecture rules.
    /// These tests prevent junior developers from breaking the architecture.
    /// Run: dotnet test Template.Architecture.Tests
    /// </summary>
    public class ArchitectureTests
    {
        private static readonly Architecture Architecture =
            new ArchLoader().LoadAssemblies(
                typeof(Template.API.Program).Assembly,           // API
                typeof(Template.Commands.ICommandBase).Assembly, // Commands (placeholder)
                typeof(Template.CommandHandlers.ServiceCollectionExtensions).Assembly,
                typeof(Template.Queries.IQueryBase).Assembly,    // Queries (placeholder)
                typeof(Template.QueryHandlers.ServiceCollectionExtensions).Assembly,
                typeof(Template.Events.IEventBase).Assembly,     // Events (placeholder)
                typeof(Template.EventHandlers.ServiceCollectionExtensions).Assembly,
                typeof(Template.Contracts.ServiceBus.IServiceBus).Assembly,
                typeof(Template.Database.ServiceCollectionExtensions).Assembly,
                typeof(Template.Consumer.Worker).Assembly
            ).Build();

        [Fact]
        public void CommandsShouldNotDependOnHandlers()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.Commands")
                .Should()
                .NotDependOnAny(
                    Types().That().ResideInNamespace("Template.CommandHandlers")
                )
                .Because("Commands are pure data, handlers contain logic");

            rule.Check(Architecture);
        }

        [Fact]
        public void QueriesShouldNotDependOnHandlers()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.Queries")
                .Should()
                .NotDependOnAny(
                    Types().That().ResideInNamespace("Template.QueryHandlers")
                )
                .Because("Queries are pure data, handlers contain logic");

            rule.Check(Architecture);
        }

        [Fact]
        public void EventsShouldNotDependOnHandlers()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.Events")
                .Should()
                .NotDependOnAny(
                    Types().That().ResideInNamespace("Template.EventHandlers")
                )
                .Because("Events are pure data, handlers contain logic");

            rule.Check(Architecture);
        }

        [Fact]
        public void HandlersShouldNotDependOnControllers()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.CommandHandlers")
                .Or()
                .ResideInNamespace("Template.QueryHandlers")
                .Or()
                .ResideInNamespace("Template.EventHandlers")
                .Should()
                .NotDependOnAny(
                    Types().That().ResideInNamespace("Template.API.Controllers")
                )
                .Because("Handlers are business logic, controllers are presentation");

            rule.Check(Architecture);
        }

        [Fact]
        public void ControllersShouldDependOnContracts()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.API.Controllers")
                .Should()
                .DependOnAny(
                    Types().That().ResideInNamespace("Template.Contracts.ServiceBus")
                )
                .Because("Controllers use IServiceBus for command/query handling");

            rule.Check(Architecture);
        }

        [Fact]
        public void DatabaseShouldNotDependOnAPI()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.Database")
                .Should()
                .NotDependOnAny(
                    Types().That().ResideInNamespace("Template.API")
                )
                .Because("Database layer should not depend on presentation layer");

            rule.Check(Architecture);
        }

        [Fact]
        public void NamespaceShouldFollowPattern()
        {
            var rule = Types()
                .That()
                .AreNotInterfaces()
                .Should()
                .ResideInNamespaceMatching("^Template\\..*")
                .Because("All types should use Template. prefix for consistency");

            rule.Check(Architecture);
        }

        [Fact]
        public void CommandsShouldEndWithCommand()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.Commands")
                .And()
                .AreNotInterfaces()
                .Should()
                .HaveNameEndingWith("Command")
                .Because("Commands should be named consistently");

            rule.Check(Architecture);
        }

        [Fact]
        public void QueriesShouldEndWithQuery()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.Queries")
                .And()
                .AreNotInterfaces()
                .Should()
                .HaveNameEndingWith("Query")
                .Because("Queries should be named consistently");

            rule.Check(Architecture);
        }

        [Fact]
        public void EventsShouldEndWithEvent()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.Events")
                .And()
                .AreNotInterfaces()
                .Should()
                .HaveNameEndingWith("Event")
                .Because("Events should be named consistently");

            rule.Check(Architecture);
        }

        [Fact]
        public void InterfacesShouldStartWithI()
        {
            var rule = Types()
                .That()
                .AreInterfaces()
                .And()
                .ResideInNamespace("Template")
                .Should()
                .HaveNameStartingWith("I")
                .Because("Interfaces should start with 'I' for consistency");

            rule.Check(Architecture);
        }

        [Fact]
        public void ContractsShouldBeInterfaces()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.Contracts")
                .Should()
                .BeInterfaces()
                .Because("Contracts should only contain interfaces");

            rule.Check(Architecture);
        }

        [Fact]
        public void NoPublicFieldsShouldExist()
        {
            var rule = Types()
                .That()
                .AreNotEnums()
                .Should()
                .NotHavePublicFields()
                .Because("Use properties instead of public fields for encapsulation");

            rule.Check(Architecture);
        }

        [Fact]
        public void NoClassesShouldHaveStaticFieldsExcludingConstants()
        {
            var rule = Types()
                .That()
                .AreClasses()
                .Should()
                .NotHaveStaticFields()
                .Because("Avoid static state for testability and thread safety");

            rule.Check(Architecture);
        }

        [Fact]
        public void CommandHandlersShouldImplementCorrectInterface()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.CommandHandlers")
                .And()
                .HaveNameEndingWith("Handler")
                .And()
                .AreNotInterfaces()
                .Should()
                .ImplementInterface(
                    Types().That().ResideInNamespace("Template.Contracts.CommandHandler")
                )
                .Because("All command handlers must implement ICommandHandler");

            rule.Check(Architecture);
        }

        [Fact]
        public void QueryHandlersShouldImplementCorrectInterface()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.QueryHandlers")
                .And()
                .HaveNameEndingWith("Handler")
                .And()
                .AreNotInterfaces()
                .Should()
                .ImplementInterface(
                    Types().That().ResideInNamespace("Template.Contracts.QueryHandler")
                )
                .Because("All query handlers must implement IQueryHandler");

            rule.Check(Architecture);
        }

        [Fact]
        public void EventHandlersShouldImplementCorrectInterface()
        {
            var rule = Types()
                .That()
                .ResideInNamespace("Template.EventHandlers")
                .And()
                .HaveNameEndingWith("Handler")
                .And()
                .AreNotInterfaces()
                .Should()
                .ImplementInterface(
                    Types().That().ResideInNamespace("Template.Contracts.EventHandler")
                )
                .Because("All event handlers must implement IEventHandler");

            rule.Check(Architecture);
        }
    }

    // Placeholder interfaces for architecture loading
    namespace Contracts.CommandHandler { public interface ICommandHandler { } }
    namespace Contracts.QueryHandler { public interface IQueryHandler { } }
    namespace Contracts.EventHandler { public interface IEventHandler { } }
    namespace Commands { public interface ICommandBase { } }
    namespace Queries { public interface IQueryBase { } }
    namespace Events { public interface IEventBase { } }
}
