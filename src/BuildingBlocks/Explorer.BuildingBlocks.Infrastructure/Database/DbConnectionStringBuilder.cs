public static class DbConnectionStringBuilder
{
    public static string Build(string schemaName)
    {
        var server = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";

        // Ovde sad koristimo DATABASE_NAME za ime baze
        var database = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "explorer";

        // A ovo koristimo za schema (SearchPath)
        var schema = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? schemaName;

        var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "super";
        var integratedSecurity = Environment.GetEnvironmentVariable("DATABASE_INTEGRATED_SECURITY") ?? "false";
        var pooling = Environment.GetEnvironmentVariable("DATABASE_POOLING") ?? "true";

        return
            $"Server={server};Port={port};Database={database};SearchPath={schema};User ID={user};Password={password};Integrated Security={integratedSecurity};Pooling={pooling};";
    }
}
