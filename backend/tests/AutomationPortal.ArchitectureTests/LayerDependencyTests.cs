namespace AutomationPortal.ArchitectureTests;

public class LayerDependencyTests
{
    private const string DomainNamespace         = "AutomationPortal.Domain";
    private const string ApplicationNamespace    = "AutomationPortal.Application";
    private const string InfrastructureNamespace = "AutomationPortal.Infrastructure";
    private const string ApiNamespace            = "AutomationPortal.API";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOn_Application()
    {
        Types.InAssembly(typeof(AutomationPortal.Domain.Abstractions.IRepository<>).Assembly)
             .Should().NotHaveDependencyOn(ApplicationNamespace)
             .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_Not_HaveDependencyOn_Infrastructure()
    {
        Types.InAssembly(typeof(AutomationPortal.Domain.Abstractions.IRepository<>).Assembly)
             .Should().NotHaveDependencyOn(InfrastructureNamespace)
             .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOn_Infrastructure()
    {
        Types.InAssembly(typeof(AutomationPortal.Application.DependencyInjection).Assembly)
             .Should().NotHaveDependencyOn(InfrastructureNamespace)
             .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOn_Api()
    {
        Types.InAssembly(typeof(AutomationPortal.Application.DependencyInjection).Assembly)
             .Should().NotHaveDependencyOn(ApiNamespace)
             .GetResult().IsSuccessful.Should().BeTrue();
    }
}
