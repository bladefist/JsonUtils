
namespace testNamespace
{

        public class Employee
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
        }

        public class JsonInput
        {
            public IList<Employee> employees { get; set; }
        }

}
