
namespace Wrapperizer.Repository.EntityFrameworkCore.Extensions
{
    // public static class ModelBuilderExtensions
    // {
    //     public static void AddSoftDeleteQueryFilter(this ModelBuilder modelBuilder, Assembly migrationAssembly)
    //     {
    //         var softDeletedEntityTypes = migrationAssembly.WithReferencedAssemblies().SelectMany(assembly =>
    //             assembly.GetTypes().Where(type =>
    //                     typeof(ICanBeAudited).IsAssignableFrom(type)
    //                     && type.IsClass && !type.IsAbstract
    //                 )
    //                 .Select(type => type));
    //
    //         softDeletedEntityTypes.ForEach(entityType =>
    //         {
    //             modelBuilder.Entity(entityType, builder =>
    //             {
    //                 builder.Property<bool>("SoftDeleted");
    //                 builder.HasQueryFilter(GenerateQueryFilterExpression(entityType));
    //             });
    //         });
    //     }
    //
    //     public static void AddAuditProperties(this ModelBuilder modelBuilder, Assembly migrationAssembly)
    //     {
    //         var auditTypes = migrationAssembly.WithReferencedAssemblies().SelectMany(assembly =>
    //             assembly.GetTypes().Where(type =>
    //                     typeof(ICanBeAudited).IsAssignableFrom(type)
    //                     && type.IsClass && !type.IsAbstract
    //                 )
    //                 .Select(type => type));
    //
    //         foreach (var auditEntity in auditTypes)
    //         {
    //             modelBuilder.Entity(auditEntity, builder =>
    //             {
    //                 builder.Property<DateTimeOffset>("CreatedOn")
    //                     .IsRequired()
    //                     .HasDefaultValueSql("GETUTCDATE()");
    //
    //                 builder.Property<DateTimeOffset?>("UpdatedOn").IsRequired(false);
    //             });
    //         }
    //     }
    //     private static LambdaExpression GenerateQueryFilterExpression(Type entityType)
    //     {
    //         // // e => e.SoftDeleted == false
    //         // var value = Expression.Constant(false);
    //         // var parameter = Expression.Parameter(entityType, "e");
    //         // var property = Expression.Property(parameter, $"{nameof(ICanBeSoftDeleted.SoftDeleted)}");
    //         // var equal = Expression.Equal(property, value);
    //         // var lambda = Expression.Lambda(equal, parameter);
    //         
    //         // // e => !e.SoftDeleted
    //         // var parameter = Expression.Parameter(entityType, "e");
    //         // var property = Expression.Property(parameter, $"SoftDeleted");
    //         // var notUnaryExpression = Expression.Not(property);
    //         // var lambda = Expression.Lambda(notUnaryExpression, parameter);
    //         
    //         // e => !EF.Property<bool>(e, "SoftDeleted"));
    //         
    //         var parameter = Expression.Parameter(entityType, "e"); // e =>
    //         
    //         var fieldName = Expression.Constant("SoftDeleted", typeof(string)); // "SoftDeleted"
    //         
    //         // EF.Property<bool>(e, "SoftDeleted")
    //         var genericMethodCall = Expression.Call(typeof(EF), "Property", new[] {typeof(bool)}, parameter, fieldName);
    //         
    //         // !EF.Property<bool>(e, "SoftDeleted"))
    //         var not = Expression.Not(genericMethodCall);
    //         
    //         // e => !EF.Property<bool>(e, "SoftDeleted"));
    //         var lambda = Expression.Lambda(not, parameter);
    //
    //         return lambda;
    //     }
    // }
}
