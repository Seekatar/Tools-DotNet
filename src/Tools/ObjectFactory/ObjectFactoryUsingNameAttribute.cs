using Microsoft.Extensions.Options;

namespace Seekatar.Tools;

/// <summary>
/// Factory for discovering types from assemblies then creating them on demand 
/// </summary>
/// <remarks>Derived from this to override Predicate or ObjectName</remarks>
/// <typeparam name="T">type of object to serve up</typeparam>
public class ObjectFactoryUsingNameAttribute<T> : ObjectFactory<T> where T : class
{
    public ObjectFactoryUsingNameAttribute(IServiceProvider provider, IOptions<ObjectFactoryOptions> options) : base(provider, options)
    {
    }

    protected override bool Predicate(Type type)
    {
        if (base.Predicate(type))
        {
            System.Diagnostics.Debug.WriteLine($">>> Type {type.Name} passes predication and attrs says {type.GetCustomAttributes(typeof(ObjectNameAttribute), false).Any()} ");
            foreach (var attribute in type.GetCustomAttributes(typeof(ObjectNameAttribute), false))
            {
                System.Diagnostics.Debug.WriteLine($"     {attribute.GetType().Name}");
            }
        }
        return base.Predicate(type) && type.GetCustomAttributes(typeof(ObjectNameAttribute), false).Any();
    }

    protected override string ObjectName(Type type) => (type.GetCustomAttributes(typeof(ObjectNameAttribute), false).FirstOrDefault() as ObjectNameAttribute)!.Name;

}