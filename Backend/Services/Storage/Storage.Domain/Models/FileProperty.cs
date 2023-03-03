using Domain;

namespace Storage.Domain.Models {
    public class FileProperty : ValueObject {

        public string Name { get; private set; }
        public string Value { get; private set; }

        protected FileProperty () { }

        public FileProperty (string name, string value) {
            Name = name;
            Value = value;
        }
    }
}
