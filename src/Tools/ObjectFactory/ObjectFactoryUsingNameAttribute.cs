using Microsoft.Extensions.Options;

namespace Seekatar.Tools;

/// <summary>
/// Factory for discovering types from assemblies then creating them on demand 
/// </summary>
/// <remarks>Derived from this to override Predicate or ObjectName</remarks>
/// <typeparam name="T">type of object to serve up</typeparam>
public class ObjectFactoryUsingNameAttribute<T> : ObjectFactory<T> where T : class
{
    public ObjectFactoryUsingNameAttribute(IServiceProvider provider, IOptions<ObjectFactoryOptions> options) : base (provider,options)
    {
    }

    protected override bool Predicate(Type type) => base.Predicate(type) && type.GetCustomAttributes(typeof(ObjectNameAttribute), false).Any();

    protected override string ObjectName(Type type) => (type.GetCustomAttributes(typeof(ObjectNameAttribute), false).FirstOrDefault() as ObjectNameAttribute)!.Name;

}