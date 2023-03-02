namespace SharedKernel.Utilities {
    public static class EnumExtensions {
        public static TEnum Verify<TEnum> (this TEnum value, TEnum defaultValue) where TEnum : struct, Enum {
            if (Enum.IsDefined(value)) {
                return value;
            }
            return defaultValue;
        }
    }
}
