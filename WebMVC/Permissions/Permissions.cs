namespace WebMVC.PermissionsRoot
{
    public class Permissions
    {
        public static class Stock
        {
            public const string Read = "Permissions.Stock.Read";
            public const string Creat = "Permissions.Stock.Creat";
            public const string Update = "Permissions.Stock.Update";
            public const string Delete = "Permissions.Stock.Delete";
        }

        public static class Order
        {
            public const string Read = "Permissions.Order.Read";
            public const string Creat = "Permissions.Order.Creat";
            public const string Update = "Permissions.Order.Update";
            public const string Delete = "Permissions.Order.Delete";
        }

        public static class Catalog
        {
            public const string Read = "Permissions.Catalog.Read";
            public const string Creat = "Permissions.Catalog.Creat";
            public const string Update = "Permissions.Catalog.Update";
            public const string Delete = "Permissions.Catalog.Delete";
        }
    }
}
