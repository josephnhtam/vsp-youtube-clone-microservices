using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using System.Reflection;

namespace Infrastructure.MongoDb {
    public static class MongoClassMapHelper {

        private class MongoClassMapRegister { }

        public static void RegisterMongoClassMaps (this IHost app, params Assembly[] assemblies) {
            assemblies = assemblies.Union(new List<Assembly> { typeof(MongoClassMapHelper).Assembly }).ToArray();

            foreach (var assembly in assemblies) {
                foreach (var classMap in assembly.GetExportedTypes()) {
                    if (classMap.IsClass && !classMap.IsAbstract) {
                        var documentType = GetDocumentType(classMap);

                        if (documentType != null) {
                            using var scope = app.Services.CreateScope();
                            var services = scope.ServiceProvider;

                            var logger = services.GetRequiredService<ILogger<MongoClassMapRegister>>();

                            try {
                                var classMapInstance = Activator.CreateInstance(classMap);
                                var registerClassMapMethod = classMap.GetMethod("RegisterClassMap", BindingFlags.Instance | BindingFlags.Public);

                                var bsonClassMapType = typeof(BsonClassMap<>).MakeGenericType(documentType);
                                var bsonClassMap = Activator.CreateInstance(bsonClassMapType) as BsonClassMap;

                                registerClassMapMethod!.Invoke(classMapInstance, new object[] { bsonClassMap! });

                                BsonClassMap.RegisterClassMap(bsonClassMap);
                            } catch (Exception ex) {
                                logger.LogError(ex, "Failed to register class map of {DocumentType}", documentType.Name);
                            }
                        }
                    }
                }
            }
        }

        private static Type? GetDocumentType (Type classMapType) {
            while (!classMapType.IsGenericType || classMapType.GetGenericTypeDefinition() != typeof(MongoClassMap<>)) {
                if (classMapType.BaseType != null) {
                    classMapType = classMapType.BaseType;
                } else {
                    return null;
                }
            }

            return classMapType.GetGenericArguments()[0];
        }

    }
}
