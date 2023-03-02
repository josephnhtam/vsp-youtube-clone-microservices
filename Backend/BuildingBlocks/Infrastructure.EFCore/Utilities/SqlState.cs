namespace Infrastructure.EFCore.Utilities {
    public static class SqlState {
        public const string ConstraintViolation = "23";
        public const string InvalidForeignKey = "23503";
        public const string UniqueViolation = "23505";
    }
}
