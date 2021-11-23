namespace Seekatar.Interfaces;


/// <summary>
/// Factory for discovering types from assemblies then creating them on demand
/// </summary>
/// <typeparam name="T">type of object to serve up</typeparam>
public interface IObjectFactory<T> where T : class
{
    /// <summary>
    /// Get the instance of an object type that this factory loaded
    /// </summary>
    /// <param name="name">name that matches with ObjectName</param>
    /// <returns>The instance or null</returns>
    T? GetInstance(string name);

    /// <summary>
    /// Get a list of the loaded types.
    /// </summary>
    IReadOnlyDictionary<string, Type> LoadedTypes { get; }
}