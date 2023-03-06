using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Infrastructure.Caching {
    public class CacheContractResolver : DefaultContractResolver {
        protected override List<MemberInfo> GetSerializableMembers (Type objectType) {
            var members = base.GetSerializableMembers(objectType);

            // Exclude properties without setter
            members.RemoveAll(m => {
                if (m is PropertyInfo pInfo) {
                    if (pInfo.SetMethod == null) {
                        return true;
                    }
                }
                return false;
            });

            // Include non-public fields
            var nonPublicFieldMembers = objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var member in nonPublicFieldMembers) {
                if (!members.Any(m => m.MetadataToken == member.MetadataToken)) {
                    if (!member.IsDefined(typeof(CompilerGeneratedAttribute), true)) {
                        members.Add(member);
                    }
                }
            }

            return members;
        }

        protected override JsonProperty CreateProperty (MemberInfo member, MemberSerialization memberSerialization) {
            var property = base.CreateProperty(member, memberSerialization);

            property.Readable = true;
            property.Writable = true;

            return property;
        }
    }
}
