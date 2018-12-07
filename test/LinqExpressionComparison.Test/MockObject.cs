namespace LinqExpressionComparison.Test {
    
    public class MockObjectA {

        public object TheThing;

        public MockObjectA() { }

        public MockObjectA(int id, string name) => (Id, Name) = (id, name);

        public int Id { get; set; }
        public string Name { get; set; }

        public int DoubleIt(int i) => i * 2;
    }

    public class MockObjectB {

        public object AnotherThing;

        public MockObjectB() { }

        public MockObjectB(int id, string name, string gender) => (Id, Name, Gender) = (id, name, gender);

        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }

        public int TripleIt(int i) => i * 3;
    }
}