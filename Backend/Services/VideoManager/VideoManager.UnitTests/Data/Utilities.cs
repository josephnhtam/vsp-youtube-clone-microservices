namespace VideoManager.UnitTests.Data {
    public static class Utilities {
        public static string GenerateString (int length) {
            return new string(Enumerable.Repeat('s', length).ToArray());
        }
    }
}
