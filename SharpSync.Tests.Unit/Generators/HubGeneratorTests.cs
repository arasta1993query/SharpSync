using System;
using System.Collections.Generic;
using FluentAssertions;
using SharpSync.Core.Generators;
using SharpSync.Core.Attributes;
using Xunit;

namespace SharpSync.Tests.Unit.Generators
{
    public class HubGeneratorTests
    {
        private readonly HashSet<Type> _zodEnabledTypes;
        private readonly DependencyResolver _resolver;
        private readonly HubGenerator _generator;

        // Mock Classes to simulate SignalR Hub
        public class Hub<T> { }
        
        public interface IChatClient
        {
            void ReceiveMessage(string user, string message);
        }

        [SharpSyncHub]
        public class ChatHub : Hub<IChatClient>
        {
            public void SendMessage(string user, string message) { }
        }

        public HubGeneratorTests()
        {
            _zodEnabledTypes = new HashSet<Type>();
            _resolver = new DependencyResolver(_zodEnabledTypes);
            _generator = new HubGenerator(_zodEnabledTypes);
        }

        [Fact]
        public void GenerateHubFile_ValidHub_GeneratesTypedClientAndHook()
        {
            // Act
            var result = _generator.GenerateHubFile(typeof(ChatHub), _resolver, FrameworkType.React);

            // Assert
            result.Should().Contain("export class ChatHubClient {");
            result.Should().Contain("constructor(public connection: HubConnection) {}");
            
            // Server Method
            result.Should().Contain("async sendMessage(user: string, message: string): Promise<void>");
            result.Should().Contain("this.connection.invoke('SendMessage', user, message)");

            // Client Method
            result.Should().Contain("onReceiveMessage(callback: (user: string, message: string) => void)");
            result.Should().Contain("this.connection.on('ReceiveMessage', (user, message) => {");

            // React Hook
            result.Should().Contain("export const useChatHub = (url: string) => {");
            result.Should().Contain("new HubConnectionBuilder()");
        }

        [Fact]
        public void DependencyResolver_GetHubDependencies_IdentifiesClientInterface()
        {
            // Act
            var deps = _resolver.GetHubDependencies(typeof(ChatHub));

            // Assert
            deps.Should().Contain(typeof(IChatClient));
        }

        [Fact]
        public void DependencyResolver_CollectTypesRecursive_IncludesHubAndClient()
        {
            // Act
            var types = _resolver.CollectTypesRecursive(new[] { typeof(ChatHub) });

            // Assert
            types.Should().Contain(typeof(ChatHub));
            types.Should().Contain(typeof(IChatClient));
        }
    }
}
