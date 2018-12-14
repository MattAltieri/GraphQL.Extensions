using System;

namespace GraphQL.Extensions.Test {
    public class MockEntity : IEquatable<MockEntity> {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }

        public bool Equals(MockEntity other) {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;
            return this.Id.Equals(other.Id) &&
                   this.Name.Equals(other.Name) &&
                   this.DOB.Equals(other.DOB);
        }

        public override bool Equals(object obj)
            => Equals(obj as MockEntity);

        public override int GetHashCode()
            => unchecked((193 * 101) +
                         Id.GetHashCode() +
                         Name.GetHashCode() +
                         DOB.GetHashCode());

        public static bool Equals(MockEntity operand1, MockEntity operand2) {
            if (ReferenceEquals(operand1, operand2)) return true;
            return operand1?.Equals(operand2) ?? false;
        }

        public static bool operator ==(MockEntity operand1, MockEntity operand2)
            => Equals(operand1, operand2);

        public static bool operator !=(MockEntity operand1, MockEntity operand2)
            => !(operand1 == operand2);
    }
}