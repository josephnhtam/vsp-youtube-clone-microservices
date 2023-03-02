using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.EFCore.Utilities {
    public static class AlwaysTrueValueComparer {

        public static ValueComparer<T> Create<T> () {
            return new ValueComparer<T>((x, y) => true, v => v.GetHashCode(), v => v);
        }

    }
}
