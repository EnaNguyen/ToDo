using System.Reflection;

namespace ToDo.Extensions
{
    public static class EndpointExtensions
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var endpointTypes = typeof(Program).Assembly
                .GetTypes()
                .Where(t => typeof(IApiEndpoint).IsAssignableFrom(t)
                         && !t.IsInterface && !t.IsAbstract);

            foreach (var type in endpointTypes)
            {
                var instance = Activator.CreateInstance(type) as IApiEndpoint;
                instance?.MapEndpoints(app);
            }
        }
    }
}